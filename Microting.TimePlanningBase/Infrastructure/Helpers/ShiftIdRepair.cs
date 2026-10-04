/*
The MIT License (MIT)

Copyright (c) 2007 - 2026 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// Applies a shift-id repair manifest: puts back the Start1Id / Stop1Id (and,
/// when the manifest gives them, Start1StartedAt / Stop1StoppedAt) that a
/// read-path bug corrupted, inside one database transaction.
///
/// Under a row lock on every manifest row, each line is judged on its own:
/// <see cref="ShiftIdRepairOutcome.Missing"/> (no such row, or another
/// worker's or another date's), <see cref="ShiftIdRepairOutcome.Locked"/>
/// (reconciled, or on/before the worker's reconciled-day lock),
/// <see cref="ShiftIdRepairOutcome.Changed"/> (Version / Start1Id / Stop1Id no
/// longer what the manifest was built from), else
/// <see cref="ShiftIdRepairOutcome.Repaired"/>: the new values are set and
/// saved through <see cref="PnBase.Update"/>, so the row's Version moves on and
/// a version-history row is written exactly as for any other edit. Nothing else
/// on the row (NettoHours, flex, ...) is touched. A line that is not repaired
/// never blocks the others.
///
/// The transaction commits only when <c>apply</c> is true. A dry run does the
/// same work and rolls back, so it reports what the real run would do.
/// </summary>
public static class ShiftIdRepair
{
    /// <summary>Largest IN list sent in one locking statement.</summary>
    private const int LockChunk = 1000;

    /// <exception cref="ArgumentException"><paramref name="lines"/> names the same Id twice.</exception>
    public static async Task<ShiftIdRepairResult> ApplyAsync(TimePlanningPnDbContext db,
        IReadOnlyList<ShiftIdRepairLine> lines, bool apply)
    {
        var duplicateIds = lines.GroupBy(x => x.Id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicateIds.Count > 0)
        {
            throw new ArgumentException(
                $"manifest names Id(s) {string.Join(", ", duplicateIds)} more than once.", nameof(lines));
        }

        // Lower bound for the UpdatedAt PnBase.Update writes: lets the
        // lost-commit-ack check tell our write apart from an earlier one. The
        // second of slack covers clock/precision differences.
        var callStartedUtc = DateTime.UtcNow.AddSeconds(-1);

        var result = new ShiftIdRepairResult();

        // Before-image entries by Id, first capture wins: if an attempt committed
        // without us hearing back and the check below did not recognise it, a
        // retry would read the repaired values, and the originals must survive.
        var beforeImage = new Dictionary<int, ShiftIdBeforeImageRow>();

        // The lines the attempt being retried counted as Repaired — kept outside
        // `result`, which every attempt resets.
        var repaired = new List<ShiftIdRepairLine>();

        db.ChangeTracker.Clear();

        // EnableRetryOnFailure: a user transaction must run inside the execution
        // strategy, which retries the whole unit on a transient failure.
        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            result,
            async (_, res, _) =>
            {
                res.Applied = false;
                res.Lines.Clear();
                repaired.Clear();
                db.ChangeTracker.Clear();

                await using var tx = await db.Database.BeginTransactionAsync();
                try
                {
                    await RunAsync(db, lines, res, beforeImage, repaired);
                    if (apply)
                    {
                        await tx.CommitAsync();
                    }
                    else
                    {
                        await tx.RollbackAsync();
                    }
                    res.Applied = apply;
                    return apply;
                }
                finally
                {
                    db.ChangeTracker.Clear();
                }
            },
            async (_, res, _) =>
            {
                db.ChangeTracker.Clear();
                if (await RepairsLandedAsync(db, repaired, apply, callStartedUtc))
                {
                    // the commit went through but its acknowledgement was lost
                    res.Applied = true;
                    return new ExecutionResult<bool>(true, true);
                }
                return new ExecutionResult<bool>(false, false);
            });

        db.ChangeTracker.Clear();
        result.BeforeImage.AddRange(beforeImage.Values.OrderBy(x => x.Id));
        return result;
    }

    /// <summary>
    /// The manifest line that undoes the repair recorded in
    /// <paramref name="before"/>: it expects the repaired state (Version
    /// <see cref="ShiftIdBeforeImageRow.RepairedVersion"/> and the repaired ids)
    /// and puts the before-image ids and stamps back. A stamp the repair did not
    /// change is "restored" to the value it still has, which writes nothing. A
    /// stamp that was null before the repair is left as it is now (an empty
    /// stamp means "leave unchanged"), so a repair that set a stamp over a null
    /// one is not fully reversible this way.
    /// </summary>
    public static ShiftIdRepairLine ReverseLine(ShiftIdBeforeImageRow before) =>
        new(before.Id, before.SdkSitId, before.Date, before.RepairedVersion,
            before.RepairedStart1Id, before.RepairedStop1Id, before.Start1Id, before.Stop1Id,
            before.Start1StartedAt, before.Stop1StoppedAt, "revert");

    /// <summary>
    /// Called by the execution strategy when an attempt threw, before it
    /// retries: did that attempt commit anyway (a lost commit acknowledgement)?
    /// True iff this was an apply that repaired at least one row and every one
    /// of those rows now holds its new values at a Version past the one the
    /// manifest expected, updated no earlier than this call started. With
    /// nothing repaired there is nothing to recognise the commit by, so the
    /// answer is "not succeeded" and the strategy re-runs — harmless, since a
    /// re-run of a landed repair reports its rows Changed instead of writing.
    /// </summary>
    private static async Task<bool> RepairsLandedAsync(TimePlanningPnDbContext db,
        IReadOnlyList<ShiftIdRepairLine> repaired, bool apply, DateTime callStartedUtc)
    {
        if (!apply || repaired.Count == 0)
        {
            return false;
        }

        var now = new Dictionary<int, PlanRegistration>();
        foreach (var chunk in repaired.Select(x => x.Id).Chunk(LockChunk))
        {
            foreach (var row in await db.PlanRegistrations.AsNoTracking()
                         .Where(x => chunk.Contains(x.Id)).ToListAsync())
            {
                now[row.Id] = row;
            }
        }

        return repaired.All(line => now.TryGetValue(line.Id, out var r)
                                    && r.Start1Id == line.NewStart1Id
                                    && r.Stop1Id == line.NewStop1Id
                                    && (line.NewStart1StartedAt is not { } s || r.Start1StartedAt == s)
                                    && (line.NewStop1StoppedAt is not { } e || r.Stop1StoppedAt == e)
                                    && r.Version > line.ExpectVersion
                                    && r.UpdatedAt >= callStartedUtc);
    }

    private static async Task RunAsync(TimePlanningPnDbContext db, IReadOnlyList<ShiftIdRepairLine> lines,
        ShiftIdRepairResult result, Dictionary<int, ShiftIdBeforeImageRow> beforeImage,
        List<ShiftIdRepairLine> repaired)
    {
        var ids = lines.Select(x => x.Id).ToList();

        // PlanRegistration has no concurrency token and this runs against a live
        // tenant, so the guard below is enforced under a row lock held to commit.
        // The ids are ints (never free text), so joining them into the IN list is
        // safe from injection, and keeps one round trip per chunk.
        var rows = new Dictionary<int, PlanRegistration>();
        foreach (var chunk in ids.Chunk(LockChunk))
        {
            var lockSql = "SELECT Id FROM PlanRegistrations WHERE Id IN (" + string.Join(",", chunk) + ") FOR UPDATE";
            await db.Database.ExecuteSqlRawAsync(lockSql);
            foreach (var row in await db.PlanRegistrations.Where(x => chunk.Contains(x.Id)).ToListAsync())
            {
                rows[row.Id] = row;
            }
        }

        // Each worker's reconciled-day boundary, read once, after the row lock:
        // a reconciliation that committed before it is seen, and one that tries
        // to reconcile a manifest row waits for our commit. Only the manifest's
        // own rows are locked, so a later day of the same worker reconciled in
        // the few milliseconds before our commit is not seen; the reconciled
        // flag itself is set on that later row, never on ours.
        var lockedThrough = new Dictionary<int, DateTime?>();

        foreach (var line in lines)
        {
            var outcome = await JudgeAsync(db, line, rows, lockedThrough);
            result.Lines.Add(new ShiftIdRepairLineResult(line.Id, line.SdkSitId, outcome));
            if (outcome != ShiftIdRepairOutcome.Repaired)
            {
                continue;
            }

            var row = rows[line.Id];
            var before = new ShiftIdBeforeImageRow(row.Id, row.SdkSitId, row.Date, row.Version, row.Version,
                row.Start1Id, row.Stop1Id, row.Start1StartedAt, row.Stop1StoppedAt,
                line.NewStart1Id, line.NewStop1Id);

            row.Start1Id = line.NewStart1Id;
            row.Stop1Id = line.NewStop1Id;
            if (line.NewStart1StartedAt is { } startedAt)
            {
                row.Start1StartedAt = startedAt;
            }
            if (line.NewStop1StoppedAt is { } stoppedAt)
            {
                row.Stop1StoppedAt = stoppedAt;
            }

            // Version + 1, UpdatedAt, one version-history row — like any other
            // edit. (No-op when the new values equal the current ones.)
            await row.Update(db);

            beforeImage.TryAdd(row.Id, before with { RepairedVersion = row.Version });
            repaired.Add(line);
        }
    }

    private static async Task<ShiftIdRepairOutcome> JudgeAsync(TimePlanningPnDbContext db, ShiftIdRepairLine line,
        IReadOnlyDictionary<int, PlanRegistration> rows, Dictionary<int, DateTime?> lockedThrough)
    {
        if (!rows.TryGetValue(line.Id, out var row) || row.SdkSitId != line.SdkSitId
                                                    || row.Date.Date != line.Date.Date)
        {
            return ShiftIdRepairOutcome.Missing;
        }

        if (!lockedThrough.TryGetValue(row.SdkSitId, out var boundary))
        {
            boundary = await DayLock.LockedThroughAsync(db, row.SdkSitId);
            lockedThrough[row.SdkSitId] = boundary;
        }

        if (row.Reconciled || DayLock.IsLocked(boundary, row.Date))
        {
            return ShiftIdRepairOutcome.Locked;
        }

        if (row.Version != line.ExpectVersion || row.Start1Id != line.ExpectStart1Id
                                              || row.Stop1Id != line.ExpectStop1Id)
        {
            return ShiftIdRepairOutcome.Changed;
        }

        return ShiftIdRepairOutcome.Repaired;
    }
}
