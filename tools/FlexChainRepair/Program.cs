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

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Factories;
using Microting.TimePlanningBase.Infrastructure.Helpers;

const string Usage =
    """
    Usage:
      FlexChainRepair repair --connection <cs> --restore <restore.csv> --expected <expected.csv>
                              --out <dir> [--apply --confirm-database <db name>] [--worker <sdkSitId>]
      FlexChainRepair revert --connection <cs> --before-image <file> --out <dir>
                              [--apply --confirm-database <db name>]

    Without --apply, both commands run as a dry run (every step is rolled back).
    --apply requires --confirm-database to equal the connection string's database name.

    Exit codes: 0 every worker OK; 1 a worker/setup ran into a real problem
    (mismatch, guard failure, locked worker, a thrown exception, a database
    query error, malformed CSV content, a duplicate Id in expected.csv, or a
    revert with changed/missing/errored rows); 2 the invocation itself was
    wrong (bad/missing/unknown flags, --apply without a matching
    --confirm-database, --out already used, an unreadable input path, --worker
    naming a worker that is not active, or a restore/expected line naming a
    worker outside the tenant's active set).

    With --apply, the first worker whose call throws stops the run right
    there (a dry run keeps going past errors, to show every worker's result
    in one pass).
    """;

// A setup-time failure that isn't one of the specific argument-validation
// checks below (a database error, a file that vanished between the earlier
// existence check and opening it, and so on) is still reported as `error: `
// on stderr with exit 1, never a raw stack trace.
try
{
    return await Run(args);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"error: {ex.Message}");
    return 1;
}

static async Task<int> Run(string[] args)
{
    if (args.Length == 0 || (args[0] != "repair" && args[0] != "revert"))
    {
        Console.Error.WriteLine(Usage);
        return 2;
    }

    var command = args[0];
    var allowedFlags = command == "repair"
        ? new[] { "--connection", "--restore", "--expected", "--out", "--worker", "--apply", "--confirm-database" }
        : new[] { "--connection", "--before-image", "--out", "--apply", "--confirm-database" };

    if (!TryParseFlags(args, 1, allowedFlags, out var flags, out var apply, out var parseError))
    {
        Console.Error.WriteLine($"error: {parseError}");
        Console.Error.WriteLine(Usage);
        return 2;
    }

    var required = command == "repair"
        ? new[] { "--connection", "--restore", "--expected", "--out" }
        : new[] { "--connection", "--before-image", "--out" };
    var missing = required.Where(flag => !flags.ContainsKey(flag)).ToList();
    if (missing.Count > 0)
    {
        Console.Error.WriteLine($"error: missing required flag(s) {string.Join(", ", missing)}");
        Console.Error.WriteLine(Usage);
        return 2;
    }

    var connectionString = flags["--connection"];
    var outDir = flags["--out"];

    if (apply)
    {
        if (!flags.TryGetValue("--confirm-database", out var confirmDatabase))
        {
            Console.Error.WriteLine("error: --apply requires --confirm-database");
            return 2;
        }

        // GetDbConnection().Database only parses the connection string; it does
        // not open a connection, so this check runs before any query.
        using var checkDb = CreateDb(connectionString);
        var actualDatabase = checkDb.Database.GetDbConnection().Database;
        if (actualDatabase != confirmDatabase)
        {
            Console.Error.WriteLine(
                $"error: --confirm-database '{confirmDatabase}' does not match the connection's database '{actualDatabase}'");
            return 2;
        }
    }

    // --out is created, and checked for a previous run's files, only after
    // every other exit-2 check (including the command-specific ones inside
    // RunRepair/RunRevert below) — so nothing is written to disk before the
    // invocation itself is known to be valid.
    return command == "repair"
        ? await RunRepair(connectionString, flags, outDir, apply)
        : await RunRevert(connectionString, flags, outDir, apply);
}

/// <summary>
/// Creates <paramref name="outDir"/> if missing and refuses (exit 2) if it
/// already holds a previous run's output files. Called only after every
/// other exit-2 check for the command has passed, so this is the last gate
/// before any file is written.
/// </summary>
static int? CreateOutDirOrConflictExitCode(string outDir)
{
    Directory.CreateDirectory(outDir);
    var conflicts = FindConflictingOutputFiles(outDir);
    if (conflicts.Count == 0)
    {
        return null;
    }

    Console.Error.WriteLine(
        $"error: --out already contains {string.Join(", ", conflicts)} from a previous run; use a fresh --out directory");
    return 2;
}

static async Task<int> RunRepair(string connectionString, Dictionary<string, string> flags, string outDir, bool apply)
{
    int? workerFilter = null;
    if (flags.TryGetValue("--worker", out var workerText))
    {
        if (!int.TryParse(workerText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedWorker))
        {
            Console.Error.WriteLine($"error: --worker '{workerText}' is not an integer");
            return 2;
        }
        workerFilter = parsedWorker;
    }

    IReadOnlyList<RestoreLine> restores;
    IReadOnlyList<ExpectedBalance> expectedList;
    try
    {
        using var restoreReader = new StreamReader(flags["--restore"]);
        restores = FlexChainRepairCsv.ReadRestore(restoreReader);
        using var expectedReader = new StreamReader(flags["--expected"]);
        expectedList = FlexChainRepairCsv.ReadExpected(expectedReader);
    }
    catch (IOException ex)
    {
        // An unreadable path (missing file, bad permissions, ...) is treated as
        // a bad invocation, the same as any other malformed argument.
        Console.Error.WriteLine($"error: {ex.Message}");
        return 2;
    }
    catch (FormatException ex)
    {
        // The file opened fine but its content is malformed — that is a setup
        // failure in the data being fed in, not a bad command line.
        Console.Error.WriteLine($"error: {ex.Message}");
        return 1;
    }

    var duplicateExpectedIds = expectedList.GroupBy(e => e.Id).Where(g => g.Count() > 1)
        .Select(g => g.Key).ToList();
    if (duplicateExpectedIds.Count > 0)
    {
        Console.Error.WriteLine(
            $"error: expected.csv contains duplicate Id(s): {string.Join(", ", duplicateExpectedIds)}");
        return 1;
    }

    List<int> workers;
    List<int> restoreOutsiders;
    List<int> expectedOutsiders;
    bool workerFilterInvalid;
    try
    {
        (workers, restoreOutsiders, expectedOutsiders, workerFilterInvalid) =
            await ResolveWorkersAsync(connectionString, workerFilter, restores, expectedList);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"error: {ex.Message}");
        return 1;
    }

    if (workerFilterInvalid)
    {
        Console.Error.WriteLine($"error: --worker {workerFilter} is not an active worker");
        return 2;
    }

    if (restoreOutsiders.Count > 0)
    {
        Console.Error.WriteLine(
            $"error: restore manifest names SdkSitId(s) {string.Join(", ", restoreOutsiders)} outside the tenant's active worker set");
        return 2;
    }

    if (expectedOutsiders.Count > 0)
    {
        Console.Error.WriteLine(
            $"error: expected balances name SdkSitId(s) {string.Join(", ", expectedOutsiders)} outside the tenant's active worker set");
        return 2;
    }

    if (CreateOutDirOrConflictExitCode(outDir) is { } conflictExitCode)
    {
        return conflictExitCode;
    }

    var restoresByWorker = restores.GroupBy(r => r.SdkSitId)
        .ToDictionary(g => g.Key, g => (IReadOnlyList<RestoreLine>)g.ToList());
    var expectedByWorker = expectedList.GroupBy(e => e.SdkSitId)
        .ToDictionary(g => g.Key, g => (IReadOnlyDictionary<int, ExpectedBalance>)g.ToDictionary(e => e.Id));

    await using var reportWriter = new StreamWriter(Path.Combine(outDir, "report.csv"));
    await reportWriter.WriteLineAsync("SdkSitId,Outcome,RowsRestored,RowsWalked,EndBalanceBefore,EndBalanceAfter");
    await using var mismatchesWriter = new StreamWriter(Path.Combine(outDir, "mismatches.csv"));
    await mismatchesWriter.WriteLineAsync("SdkSitId,Id,Date,Field,Expected,Actual");
    await using var guardFailedWriter = new StreamWriter(Path.Combine(outDir, "guard-failed.csv"));
    await guardFailedWriter.WriteLineAsync("SdkSitId,Id");

    // Opened with CreateNew (refuses to silently overwrite a race-created
    // file) so it can be flushed straight to disk after every Applied
    // worker: this file is the only way to undo a committed repair, so it
    // must survive a crash immediately after that worker's transaction
    // commits, not just at process exit.
    await using var beforeImageStream =
        new FileStream(Path.Combine(outDir, "before-image.csv"), FileMode.CreateNew, FileAccess.Write);
    await using var beforeImageWriter = new StreamWriter(beforeImageStream);
    var beforeImageHeaderWritten = false;

    var exitCode = 0;
    foreach (var sdkSitId in workers)
    {
        var lines = restoresByWorker.TryGetValue(sdkSitId, out var l) ? l : Array.Empty<RestoreLine>();
        var expected = expectedByWorker.TryGetValue(sdkSitId, out var e)
            ? e
            : new Dictionary<int, ExpectedBalance>();

        var stop = false;
        try
        {
            using var db = CreateDb(connectionString);
            var result = await FlexChainRepair.RepairWorkerAsync(db, sdkSitId, lines, expected, apply);

            // Before-image first, and fsynced, before anything else this
            // worker touches: if the process dies right after, the undo
            // record for a committed repair must already be durable. If THIS
            // write/flush itself throws, the repair is known to have
            // committed (RepairWorkerAsync already returned) but its undo
            // record is not — a different, more specific failure than "we
            // don't know what happened" below.
            var beforeImageDurable = true;
            if (result.Outcome == RepairOutcome.Applied)
            {
                try
                {
                    FlexChainRepairCsv.WriteBeforeImage(
                        beforeImageWriter, result.BeforeImage, !beforeImageHeaderWritten);
                    beforeImageHeaderWritten = true;
                    await beforeImageWriter.FlushAsync();
                    beforeImageStream.Flush(flushToDisk: true);
                }
                catch (Exception beforeImageEx)
                {
                    beforeImageDurable = false;
                    Console.Error.WriteLine(
                        $"worker {sdkSitId}: APPLIED but its before-image is NOT durable ({beforeImageEx.Message})");
                }
            }

            if (beforeImageDurable)
            {
                Console.WriteLine(
                    $"{sdkSitId} {result.Outcome} restored={result.RowsRestored} walked={result.RowsWalked} " +
                    $"before={FormatDouble(result.EndBalanceBefore)}->after={FormatDouble(result.EndBalanceAfter)}");
            }

            try
            {
                // The repair itself succeeded even when its before-image did
                // not persist, so the report row is still worth writing.
                await WriteCsvRowAsync(reportWriter,
                    FormatInt(sdkSitId), result.Outcome.ToString(), FormatInt(result.RowsRestored),
                    FormatInt(result.RowsWalked), FormatDouble(result.EndBalanceBefore),
                    FormatDouble(result.EndBalanceAfter));

                if (beforeImageDurable)
                {
                    foreach (var m in result.Mismatches)
                    {
                        await WriteCsvRowAsync(mismatchesWriter,
                            FormatInt(sdkSitId), FormatInt(m.Id), FormatDate(m.Date), m.Field,
                            FormatDouble(m.Expected), FormatDouble(m.Actual));
                    }

                    foreach (var id in result.GuardFailedIds)
                    {
                        await WriteCsvRowAsync(guardFailedWriter, FormatInt(sdkSitId), FormatInt(id));
                    }
                }
            }
            catch when (!beforeImageDurable)
            {
                // Best-effort report row for a worker whose before-image
                // already failed to persist; nothing more to do if this
                // write fails too.
            }

            if (!beforeImageDurable || result.Outcome is not (RepairOutcome.DryRunOk or RepairOutcome.Applied))
            {
                exitCode = 1;
            }

            stop = apply && !beforeImageDurable;
        }
        catch (Exception ex)
        {
            // RepairWorkerAsync itself threw: its own execution-strategy retry
            // may have committed the transaction without us hearing back, so
            // the worker's true state is unknown — unlike the before-image
            // failure above, where we know the repair committed.
            Console.Error.WriteLine(
                $"worker {sdkSitId}: state UNKNOWN — the commit may have landed; before-image.csv covers only " +
                $"the workers reported Applied ({ex.Message})");
            await WriteCsvRowAsync(reportWriter, FormatInt(sdkSitId), "Error", "", "", "", "");
            exitCode = 1;
            stop = apply;
        }
        finally
        {
            // Every report file stays consistent with the before-image after
            // each worker, not just at process exit — including the worker
            // this run is about to stop on.
            await reportWriter.FlushAsync();
            await mismatchesWriter.FlushAsync();
            await guardFailedWriter.FlushAsync();
            await beforeImageWriter.FlushAsync();
        }

        if (stop)
        {
            // --apply: never proceed past a worker whose outcome we can't
            // fully trust. A dry run keeps going, same as before.
            break;
        }
    }

    return exitCode;
}

static async Task<int> RunRevert(string connectionString, Dictionary<string, string> flags, string outDir, bool apply)
{
    IReadOnlyList<BeforeImageRow> rows;
    try
    {
        using var reader = new StreamReader(flags["--before-image"]);
        rows = FlexChainRepairCsv.ReadBeforeImage(reader);
    }
    catch (IOException ex)
    {
        Console.Error.WriteLine($"error: {ex.Message}");
        return 2;
    }
    catch (FormatException ex)
    {
        Console.Error.WriteLine($"error: {ex.Message}");
        return 1;
    }

    if (CreateOutDirOrConflictExitCode(outDir) is { } conflictExitCode)
    {
        return conflictExitCode;
    }

    await using var changedWriter = new StreamWriter(Path.Combine(outDir, "revert-changed.csv"));
    await changedWriter.WriteLineAsync("Id");
    await using var missingWriter = new StreamWriter(Path.Combine(outDir, "revert-missing.csv"));
    await missingWriter.WriteLineAsync("Id");
    await using var breaksWriter = new StreamWriter(Path.Combine(outDir, "revert-breaks.csv"));
    await breaksWriter.WriteLineAsync("SdkSitId,Id,Date,Expected,Actual");

    var totalChanged = 0;
    var totalMissing = 0;
    var anyError = false;

    var groups = rows.GroupBy(r => r.SdkSitId).OrderBy(g => g.Key);
    foreach (var group in groups)
    {
        var sdkSitId = group.Key;
        var workerRows = group.ToList();

        var stop = false;
        try
        {
            using var db = CreateDb(connectionString);
            var result = await FlexChainRepair.RevertAsync(db, workerRows, apply);

            Console.WriteLine(
                $"{sdkSitId} reverted={result.Reverted} changed={result.ChangedIds.Count} missing={result.MissingIds.Count}");

            foreach (var id in result.ChangedIds)
            {
                await WriteCsvRowAsync(changedWriter, FormatInt(id));
            }
            foreach (var id in result.MissingIds)
            {
                await WriteCsvRowAsync(missingWriter, FormatInt(id));
            }

            totalChanged += result.ChangedIds.Count;
            totalMissing += result.MissingIds.Count;

            // Rows created after the repair are not in the before-image, so a
            // continuity break is possible even after every reverted row lands
            // cleanly. Only meaningful once the revert actually wrote something.
            if (apply && result.Reverted > 0)
            {
                var live = await db.PlanRegistrations.AsNoTracking()
                    .Where(x => x.SdkSitId == sdkSitId && x.WorkflowState != Constants.WorkflowStates.Removed)
                    .OrderBy(x => x.Date).ThenBy(x => x.Id)
                    .ToListAsync();

                for (var i = 1; i < live.Count; i++)
                {
                    if (Math.Abs(live[i].SumFlexStart - live[i - 1].SumFlexEnd) > FlexChainRepair.DefaultTolerance)
                    {
                        await WriteCsvRowAsync(breaksWriter,
                            FormatInt(sdkSitId), FormatInt(live[i].Id), FormatDate(live[i].Date),
                            FormatDouble(live[i - 1].SumFlexEnd), FormatDouble(live[i].SumFlexStart));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // RevertAsync itself threw: its own execution-strategy retry may
            // have committed the transaction without us hearing back, so this
            // worker's true state is unknown.
            Console.Error.WriteLine(
                $"worker {sdkSitId}: state UNKNOWN — the revert may have committed; re-run a dry-run revert " +
                $"for this worker to see which rows are still at their repaired version ({ex.Message})");
            anyError = true;
            stop = apply;
        }
        finally
        {
            await changedWriter.FlushAsync();
            await missingWriter.FlushAsync();
            await breaksWriter.FlushAsync();
        }

        if (stop)
        {
            // --apply: never proceed past a worker whose outcome we can't
            // fully trust. A dry run keeps going, same as before.
            break;
        }
    }

    return totalChanged == 0 && totalMissing == 0 && !anyError ? 0 : 1;
}

/// <summary>
/// The tenant's full active worker set (non-removed, non-resigned
/// AssignedSites, ascending), narrowed to <paramref name="workerFilter"/>
/// when given, plus every restore/expected SdkSitId not in the FULL active
/// set and whether <paramref name="workerFilter"/> itself names a worker
/// outside it — all checked against the full set, not the narrowed one, so
/// a full-tenant manifest still works unmodified with --worker for a
/// one-worker canary run.
/// </summary>
static async Task<(List<int> Workers, List<int> RestoreOutsiders, List<int> ExpectedOutsiders,
    bool WorkerFilterInvalid)> ResolveWorkersAsync(string connectionString, int? workerFilter,
    IReadOnlyList<RestoreLine> restores, IReadOnlyList<ExpectedBalance> expected)
{
    using var db = CreateDb(connectionString);
    var activeWorkers = await db.AssignedSites
        .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && !x.Resigned)
        .Select(x => x.SiteId)
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync();

    var activeSet = activeWorkers.ToHashSet();
    var restoreOutsiders = restores.Select(r => r.SdkSitId).Distinct().Where(id => !activeSet.Contains(id)).ToList();
    var expectedOutsiders = expected.Select(e => e.SdkSitId).Distinct().Where(id => !activeSet.Contains(id)).ToList();
    var workerFilterInvalid = workerFilter is { } f && !activeSet.Contains(f);

    var workers = workerFilter is { } filter ? activeWorkers.Where(x => x == filter).ToList() : activeWorkers;
    return (workers, restoreOutsiders, expectedOutsiders, workerFilterInvalid);
}

static bool TryParseFlags(string[] args, int startIndex, IReadOnlyCollection<string> allowedFlags,
    out Dictionary<string, string> flags, out bool apply, out string error)
{
    flags = new Dictionary<string, string>();
    apply = false;
    error = "";

    for (var i = startIndex; i < args.Length; i++)
    {
        var token = args[i];
        if (!token.StartsWith("--", StringComparison.Ordinal))
        {
            error = $"unexpected argument '{token}'";
            return false;
        }

        if (!allowedFlags.Contains(token))
        {
            error = $"unknown flag '{token}'";
            return false;
        }

        if (token == "--apply")
        {
            apply = true;
            continue;
        }

        if (i + 1 >= args.Length)
        {
            error = $"flag '{token}' requires a value";
            return false;
        }

        flags[token] = args[++i];
    }

    return true;
}

static TimePlanningPnDbContext CreateDb(string connectionString) =>
    new TimePlanningPnContextFactory().CreateDbContext(new[] { connectionString });

/// <summary>
/// Report/before-image files from an earlier run in <paramref name="outDir"/>
/// that this run must never append to or silently overwrite.
/// </summary>
static List<string> FindConflictingOutputFiles(string outDir)
{
    var found = new List<string>();
    foreach (var name in new[] { "report.csv", "before-image.csv" })
    {
        if (File.Exists(Path.Combine(outDir, name)))
        {
            found.Add(name);
        }
    }
    found.AddRange(Directory.EnumerateFiles(outDir, "revert-*.csv").Select(Path.GetFileName)!);
    return found;
}

static Task WriteCsvRowAsync(StreamWriter writer, params string[] fields) =>
    writer.WriteLineAsync(string.Join(",", fields));

static string FormatDouble(double? value) =>
    value.HasValue ? value.Value.ToString("R", CultureInfo.InvariantCulture) : "";

static string FormatInt(int value) => value.ToString(CultureInfo.InvariantCulture);

static string FormatDate(DateTime date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
