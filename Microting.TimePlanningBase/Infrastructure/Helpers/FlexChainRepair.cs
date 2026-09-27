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
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// Repairs one worker's flex chain from a restore plan, inside one database
/// transaction.
///
/// Per worker, under a row lock on the worker's rows: (1) snapshot every live
/// row into a before-image; (2) guard — every restore line must name a live
/// row of THIS worker whose Version and NettoHours still equal what the plan
/// was built from, and no Id may appear twice, otherwise nothing is written;
/// (3) restore NettoHours / NettoHoursInSeconds on those rows (Version + 1,
/// one version-history row each); (4) walk the whole chain forward from the
/// worker's first row with <see cref="FlexChainRecompute.RunForwardAsync"/>;
/// (5) check every row that has an expected balance, and the continuity of
/// every consecutive pair (start = previous end), within the tolerance, and
/// that every expected row is still live.
///
/// The transaction commits only when <c>apply</c> is true and every check
/// passed. A dry run does exactly the same work and then rolls back, so it
/// reports the same counts and mismatches as the real run would, and leaves
/// rows, versions and version history untouched. A worker with a reconciled
/// (locked) day is refused before anything is loaded for writing.
/// </summary>
public static class FlexChainRepair
{
    public const double DefaultTolerance = 0.01;
    private const int MaxMismatches = 20;
    private const double HoursGuardTolerance = 1e-6;

    public static async Task<WorkerRepairResult> RepairWorkerAsync(TimePlanningPnDbContext db, int sdkSitId,
        IReadOnlyList<RestoreLine> restores, IReadOnlyDictionary<int, ExpectedBalance> expected,
        bool apply, double tolerance = DefaultTolerance)
    {
        // NaN compares false against everything, so a NaN tolerance would pass
        // every balance check; a negative one would fail them all.
        if (double.IsNaN(tolerance) || double.IsInfinity(tolerance) || tolerance < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), tolerance,
                "tolerance must be a finite, non-negative number.");
        }

        // Every comparison below is `Math.Abs(a - b) > limit`, which a NaN or an
        // infinity makes false — so a non-finite input would pass every check.
        var nonFinite = restores
            .Where(r => !double.IsFinite(r.ExpectNettoHours) || !double.IsFinite(r.RestoreNettoHours))
            .Select(r => r.Id)
            .Concat(expected.Values
                .Where(e => !double.IsFinite(e.SumFlexStart) || !double.IsFinite(e.SumFlexEnd))
                .Select(e => e.Id))
            .ToList();
        if (nonFinite.Count > 0)
        {
            throw new ArgumentException(
                $"restore/expected values must be finite numbers; Id(s) {string.Join(", ", nonFinite)}.");
        }

        // Lower bound for the UpdatedAt our own restore step writes: lets the
        // lost-commit-ack check tell our write apart from an earlier one that
        // happened to leave the same hours. The second of slack covers
        // datetime precision on the column.
        var callStartedUtc = DateTime.UtcNow.AddSeconds(-1);

        var result = new WorkerRepairResult { SdkSitId = sdkSitId };
        db.ChangeTracker.Clear();

        if (await DayLock.LockedThroughAsync(db, sdkSitId) is not null)
        {
            result.Outcome = RepairOutcome.Locked;
            return result;
        }

        // EnableRetryOnFailure: a user transaction must run inside the execution
        // strategy, which retries the whole unit on a transient failure.
        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            result,
            async (_, res, _) =>
            {
                // A retried attempt starts from a clean slate, except the
                // before-image: it is captured once (see RunAsync), because a
                // failed attempt may have committed without us hearing about it.
                res.GuardFailedIds.Clear();
                res.Mismatches.Clear();
                res.RowsRestored = 0;
                res.RowsWalked = 0;
                res.EndBalanceBefore = null;
                res.EndBalanceAfter = null;
                db.ChangeTracker.Clear();

                await using var tx = await db.Database.BeginTransactionAsync();
                try
                {
                    var commit = await RunAsync(db, sdkSitId, restores, expected, apply, tolerance, res);
                    if (commit)
                    {
                        await tx.CommitAsync();
                    }
                    else
                    {
                        await tx.RollbackAsync();
                    }
                    return commit;
                }
                finally
                {
                    db.ChangeTracker.Clear();
                }
            },
            async (_, res, _) =>
            {
                db.ChangeTracker.Clear();
                if (await RestoresLandedAsync(db, sdkSitId, restores, apply, callStartedUtc))
                {
                    // the commit went through but its acknowledgement was lost
                    res.Outcome = RepairOutcome.Applied;
                    return new ExecutionResult<bool>(true, true);
                }
                return new ExecutionResult<bool>(false, false);
            });
        return result;
    }

    /// <summary>
    /// Called by the execution strategy when an attempt threw, before it retries:
    /// did that attempt commit anyway (a lost commit acknowledgement)? True iff
    /// this was an apply with restore lines and every restore row is still this
    /// worker's, carries the restored hours at a Version past the one the plan
    /// expected, and was updated no earlier than this call started (UpdatedAt is
    /// written by the restore step itself). With no restore
    /// lines there is nothing to recognise the commit by, so the answer is "not
    /// succeeded" and the strategy re-runs — harmless, since a walk over an
    /// already-walked chain writes nothing.
    /// </summary>
    private static async Task<bool> RestoresLandedAsync(TimePlanningPnDbContext db, int sdkSitId,
        IReadOnlyList<RestoreLine> restores, bool apply, DateTime callStartedUtc)
    {
        if (!apply || restores.Count == 0)
        {
            return false;
        }

        var ids = restores.Select(x => x.Id).ToList();
        var now = await db.PlanRegistrations.AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);
        return restores.All(line => now.TryGetValue(line.Id, out var r)
                                    && r.SdkSitId == sdkSitId
                                    && r.Version > line.ExpectVersion
                                    && Math.Abs(r.NettoHours - line.RestoreNettoHours) <= HoursGuardTolerance
                                    && r.UpdatedAt >= callStartedUtc);
    }

    /// <returns>true only when <paramref name="apply"/> is set and every check passed.</returns>
    private static async Task<bool> RunAsync(TimePlanningPnDbContext db, int sdkSitId,
        IReadOnlyList<RestoreLine> restores, IReadOnlyDictionary<int, ExpectedBalance> expected,
        bool apply, double tolerance, WorkerRepairResult result)
    {
        // PlanRegistration has no concurrency token and this runs against a live
        // tenant, so the guard below is enforced under a row lock held to commit.
        await db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT Id FROM PlanRegistrations WHERE SdkSitId = {sdkSitId} FOR UPDATE");

        // A reconciliation committed between the pre-check and the lock is caught
        // here; once the rows are locked, a reconcile of this worker waits for us.
        if (await DayLock.LockedThroughAsync(db, sdkSitId) is not null)
        {
            result.Outcome = RepairOutcome.Locked;
            return false;
        }

        var live = db.PlanRegistrations
            .Where(x => x.SdkSitId == sdkSitId)
            .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

        // 1. before-image — captured by the first attempt that reads rows and
        // kept on a retry: if an earlier attempt committed despite failing, a
        // re-read would snapshot the repaired values instead of the originals.
        // (An empty image is safe to re-capture: an attempt without rows writes nothing.)
        var rows = await live.OrderBy(x => x.Date).ThenBy(x => x.Id).ToListAsync();
        result.EndBalanceBefore = rows.LastOrDefault()?.SumFlexEnd;
        if (result.BeforeImage.Count == 0)
        {
            result.BeforeImage.AddRange(rows.Select(ToBeforeImage));
        }

        if (rows.Count == 0)
        {
            // No live rows: a restore line cannot name one of them, and an
            // expected entry cannot still be live, so either is a failed plan.
            // Only an empty plan is a no-op success.
            if (restores.Count > 0)
            {
                result.GuardFailedIds.AddRange(restores.Select(x => x.Id).Distinct());
                result.Outcome = RepairOutcome.GuardFailed;
                return false;
            }
            if (expected.Count > 0)
            {
                result.Mismatches.AddRange(expected.OrderBy(x => x.Key).Take(MaxMismatches)
                    .Select(x => new RowMismatch(x.Key, default, "Missing", x.Value.SumFlexEnd, double.NaN)));
                result.Outcome = RepairOutcome.Mismatch;
                return false;
            }
            result.Outcome = apply ? RepairOutcome.Applied : RepairOutcome.DryRunOk;
            return apply;
        }

        // 2. guard — the query is scoped to this worker's live rows, so a missing,
        // removed or other worker's row is simply absent from byId. A duplicated
        // Id is a malformed plan: each is reported once.
        var byId = rows.ToDictionary(x => x.Id);
        var seen = new HashSet<int>();
        foreach (var line in restores)
        {
            var failed = !seen.Add(line.Id)
                         || !byId.TryGetValue(line.Id, out var r)
                         || line.SdkSitId != sdkSitId
                         || r.Version != line.ExpectVersion
                         || Math.Abs(r.NettoHours - line.ExpectNettoHours) > HoursGuardTolerance;
            if (failed && !result.GuardFailedIds.Contains(line.Id))
            {
                result.GuardFailedIds.Add(line.Id);
            }
        }

        if (result.GuardFailedIds.Count > 0)
        {
            result.Outcome = RepairOutcome.GuardFailed;
            return false;
        }

        // 3. restore
        if (restores.Count > 0)
        {
            var now = DateTime.UtcNow;
            foreach (var line in restores)
            {
                var r = byId[line.Id];
                r.NettoHours = line.RestoreNettoHours;
                r.NettoHoursInSeconds = line.RestoreNettoHoursInSeconds;
                r.Version += 1;
                r.UpdatedAt = now;
            }
            await db.SaveChangesAsync();

            await SaveVersionSnapshotsAsync(db, restores.Select(x => x.Id), byId);
        }
        result.RowsRestored = restores.Count;

        // 4. walk the whole chain from the worker's first row
        var site = await db.AssignedSites.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SiteId == sdkSitId && x.WorkflowState != Constants.WorkflowStates.Removed);
        result.RowsWalked = await FlexChainRecompute.RunForwardAsync(db, site, sdkSitId, rows[0].Date);

        // 5. check — both ways: every live row against the oracle and its
        // predecessor, and every oracle entry must still be a live row
        var after = await live.AsNoTracking().OrderBy(x => x.Date).ThenBy(x => x.Id).ToListAsync();
        var mismatches = new List<RowMismatch>();
        // The oracle covers every live row that existed at dump time; only rows
        // created since (higher Ids — Ids are auto-increment) may lack an entry.
        // This catches a gap inside the worker's expected range; a file missing
        // the worker's newest rows is caught by the caller against the whole
        // file's highest Id (the console's pre-flight does this).
        var lastExpectedId = expected.Count > 0 ? expected.Keys.Max() : 0;
        for (var i = 0; i < after.Count; i++)
        {
            var row = after[i];
            if (expected.TryGetValue(row.Id, out var exp))
            {
                if (Math.Abs(row.SumFlexStart - exp.SumFlexStart) > tolerance)
                {
                    mismatches.Add(new RowMismatch(row.Id, row.Date, nameof(PlanRegistration.SumFlexStart),
                        exp.SumFlexStart, row.SumFlexStart));
                }
                if (Math.Abs(row.SumFlexEnd - exp.SumFlexEnd) > tolerance)
                {
                    mismatches.Add(new RowMismatch(row.Id, row.Date, nameof(PlanRegistration.SumFlexEnd),
                        exp.SumFlexEnd, row.SumFlexEnd));
                }
            }
            else if (row.Id <= lastExpectedId)
            {
                mismatches.Add(new RowMismatch(row.Id, row.Date, "Uncovered", double.NaN, row.SumFlexEnd));
            }

            if (i > 0 && Math.Abs(row.SumFlexStart - after[i - 1].SumFlexEnd) > tolerance)
            {
                mismatches.Add(new RowMismatch(row.Id, row.Date, "Continuity",
                    after[i - 1].SumFlexEnd, row.SumFlexStart));
            }
        }

        var versionAfter = after.ToDictionary(x => x.Id, x => x.Version);
        foreach (var (id, exp) in expected.OrderBy(x => x.Key))
        {
            if (!versionAfter.ContainsKey(id))
            {
                mismatches.Add(new RowMismatch(id, default, "Missing", exp.SumFlexEnd, double.NaN));
            }
        }

        result.Mismatches.AddRange(mismatches.Take(MaxMismatches));
        result.EndBalanceAfter = after.Count > 0 ? after[^1].SumFlexEnd : null;

        for (var i = 0; i < result.BeforeImage.Count; i++)
        {
            var before = result.BeforeImage[i];
            if (versionAfter.TryGetValue(before.Id, out var v))
            {
                result.BeforeImage[i] = before with { RepairedVersion = v };
            }
        }

        if (result.Mismatches.Count > 0)
        {
            result.Outcome = RepairOutcome.Mismatch;
            return false;
        }

        // 6. done
        result.Outcome = apply ? RepairOutcome.Applied : RepairOutcome.DryRunOk;
        return apply;
    }

    private static BeforeImageRow ToBeforeImage(PlanRegistration r) => new(r.Id, r.SdkSitId, r.Version, r.Version,
        r.NettoHours, r.NettoHoursInSeconds, r.Flex, r.FlexInSeconds,
        r.SumFlexStart, r.SumFlexStartInSeconds, r.SumFlexEnd, r.SumFlexEndInSeconds);

    /// <summary>
    /// Reverts one worker's previously applied repair from its before-image,
    /// inside one database transaction (same shape as
    /// <see cref="RepairWorkerAsync"/>). One worker per call — callers group a
    /// multi-worker before-image by SdkSitId themselves — keeps the Galera
    /// writeset small and matches the repair's own per-worker atomicity.
    /// A row that no longer exists is reported as missing; a soft-deleted row IS
    /// reverted (it is still writable, just no longer live). A row whose Version
    /// has moved past <see cref="BeforeImageRow.RepairedVersion"/> was touched by
    /// something else since the repair, and a row that now belongs to another
    /// SdkSitId than the before-image says; both are reported as changed — neither
    /// blocks the other rows in the list. The transaction commits only when
    /// <paramref name="apply"/> is true.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <paramref name="rows"/> names more than one SdkSitId, or the same Id twice.
    /// </exception>
    public static async Task<RevertResult> RevertAsync(TimePlanningPnDbContext db,
        IReadOnlyList<BeforeImageRow> rows, bool apply)
    {
        var sdkSitIds = rows.Select(x => x.SdkSitId).Distinct().ToList();
        if (sdkSitIds.Count > 1)
        {
            throw new ArgumentException(
                $"RevertAsync takes one worker's before-image; got SdkSitIds {string.Join(", ", sdkSitIds)}.",
                nameof(rows));
        }

        var duplicateIds = rows.GroupBy(x => x.Id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicateIds.Count > 0)
        {
            throw new ArgumentException(
                $"RevertAsync got duplicate before-image Ids: {string.Join(", ", duplicateIds)}.",
                nameof(rows));
        }

        var result = new RevertResult();
        db.ChangeTracker.Clear();

        // The ids counted as Reverted by the attempt the execution strategy is
        // about to retry: captured outside `result` because `result` is reset to
        // a clean slate at the top of every attempt (see below), but the retry's
        // verifySucceeded check needs to know what the FAILED attempt reverted.
        var revertedIds = new List<int>();

        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            result,
            async (_, res, _) =>
            {
                // A retried attempt starts from a clean result: Reverted/ChangedIds/
                // MissingIds are all re-derived from the current row state, unlike
                // RepairWorkerAsync's before-image, there is nothing here that must
                // survive a retry.
                res.Reverted = 0;
                res.ChangedIds.Clear();
                res.MissingIds.Clear();
                revertedIds.Clear();
                db.ChangeTracker.Clear();

                await using var tx = await db.Database.BeginTransactionAsync();
                try
                {
                    var commit = await RunRevertAsync(db, rows, apply, res, revertedIds);
                    if (commit)
                    {
                        await tx.CommitAsync();
                    }
                    else
                    {
                        await tx.RollbackAsync();
                    }
                    return commit;
                }
                finally
                {
                    db.ChangeTracker.Clear();
                }
            },
            async (_, res, _) =>
            {
                db.ChangeTracker.Clear();
                if (await RevertLandedAsync(db, rows, revertedIds, apply))
                {
                    // the commit went through but its acknowledgement was lost
                    res.Applied = true;
                    return new ExecutionResult<bool>(true, true);
                }
                return new ExecutionResult<bool>(false, false);
            });
        return result;
    }

    /// <summary>
    /// Called by the execution strategy when an attempt threw, before it retries:
    /// did that attempt commit anyway (a lost commit acknowledgement)? True iff
    /// this was an apply with rows this attempt counted as reverted, and every one
    /// of those rows now carries the captured columns at Version ==
    /// RepairedVersion + 1. With nothing reverted there is nothing to recognise
    /// the commit by, so the answer is "not succeeded" and the strategy re-runs —
    /// harmless, since re-reverting an already-reverted row is a no-op read that
    /// then reports it as changed instead (its Version has moved on).
    /// </summary>
    private static async Task<bool> RevertLandedAsync(TimePlanningPnDbContext db,
        IReadOnlyList<BeforeImageRow> rows, IReadOnlyList<int> revertedIds, bool apply)
    {
        if (!apply || revertedIds.Count == 0)
        {
            return false;
        }

        var byId = rows.ToDictionary(x => x.Id);
        var now = await db.PlanRegistrations.AsNoTracking()
            .Where(x => revertedIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        return revertedIds.All(id => now.TryGetValue(id, out var r)
                                      && byId.TryGetValue(id, out var before)
                                      && r.Version == before.RepairedVersion + 1
                                      && Math.Abs(r.NettoHours - before.NettoHours) <= HoursGuardTolerance
                                      && r.NettoHoursInSeconds == before.NettoHoursInSeconds
                                      && Math.Abs(r.Flex - before.Flex) <= HoursGuardTolerance
                                      && r.FlexInSeconds == before.FlexInSeconds
                                      && Math.Abs(r.SumFlexStart - before.SumFlexStart) <= HoursGuardTolerance
                                      && r.SumFlexStartInSeconds == before.SumFlexStartInSeconds
                                      && Math.Abs(r.SumFlexEnd - before.SumFlexEnd) <= HoursGuardTolerance
                                      && r.SumFlexEndInSeconds == before.SumFlexEndInSeconds);
    }

    /// <returns>true only when <paramref name="apply"/> is set.</returns>
    private static async Task<bool> RunRevertAsync(TimePlanningPnDbContext db, IReadOnlyList<BeforeImageRow> rows,
        bool apply, RevertResult result, List<int> revertedIds)
    {
        if (rows.Count == 0)
        {
            result.Applied = apply;
            return apply;
        }

        var ids = rows.Select(x => x.Id).ToList();

        // PlanRegistration has no concurrency token and this runs against a live
        // tenant, so the before-image rows are locked to commit before they are
        // read. The ids are the Ids captured in a prior repair's before-image —
        // ints produced by this same codebase, never free text — so joining them
        // straight into the IN list is safe from injection; ExecuteSqlRaw keeps
        // the lock to one round trip instead of a parameterised statement per id
        // (MySQL's placeholder limit would otherwise cap how many rows a single
        // revert could lock).
        await db.Database.ExecuteSqlRawAsync(
            $"SELECT Id FROM PlanRegistrations WHERE Id IN ({string.Join(",", ids)}) FOR UPDATE");

        var byId = await db.PlanRegistrations
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var now = DateTime.UtcNow;
        foreach (var before in rows)
        {
            if (!byId.TryGetValue(before.Id, out var r))
            {
                result.MissingIds.Add(before.Id);
                continue;
            }

            // a row of another worker (a hand-edited before-image) is treated like
            // a version mismatch: never written, reported as changed
            if (r.SdkSitId != before.SdkSitId || r.Version != before.RepairedVersion)
            {
                result.ChangedIds.Add(before.Id);
                continue;
            }

            r.NettoHours = before.NettoHours;
            r.NettoHoursInSeconds = before.NettoHoursInSeconds;
            r.Flex = before.Flex;
            r.FlexInSeconds = before.FlexInSeconds;
            r.SumFlexStart = before.SumFlexStart;
            r.SumFlexStartInSeconds = before.SumFlexStartInSeconds;
            r.SumFlexEnd = before.SumFlexEnd;
            r.SumFlexEndInSeconds = before.SumFlexEndInSeconds;
            r.Version += 1;
            r.UpdatedAt = now;
            result.Reverted++;
            revertedIds.Add(before.Id);
        }
        await db.SaveChangesAsync();

        await SaveVersionSnapshotsAsync(db, revertedIds, byId);

        result.Applied = apply;
        return apply;
    }

    /// <summary>
    /// Adds one version-history snapshot per id, for writers that batch several
    /// row updates into one <c>SaveChangesAsync</c> instead of calling
    /// <see cref="PnBase.Update"/> per row (the restore step of
    /// <see cref="RunAsync"/> and the write step of <see cref="RunRevertAsync"/>).
    /// </summary>
    private static async Task SaveVersionSnapshotsAsync(TimePlanningPnDbContext db, IEnumerable<int> ids,
        IReadOnlyDictionary<int, PlanRegistration> byId)
    {
        foreach (var id in ids)
        {
            await db.PlanRegistrationVersions.AddAsync(
                (PlanRegistrationVersion)byId[id].CreateVersionSnapshot());
        }
        await db.SaveChangesAsync();
    }
}
