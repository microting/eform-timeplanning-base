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
using Microting.TimePlanningBase.Infrastructure.Data;
using Microting.TimePlanningBase.Infrastructure.Data.Factories;
using Microting.TimePlanningBase.Infrastructure.Helpers;

const string Usage =
    """
    Usage:
      ShiftIdRepair --connection <cs> --manifest <manifest.csv> --out <dir>
                    [--apply --confirm-database <db name>]

    Without --apply the run is a dry run (every chunk is rolled back).
    --apply requires --confirm-database to equal the connection string's database name.

    Exit codes: 0 every line Repaired; 1 a line ended Changed / Missing / Locked,
    or a chunk threw; 2 the invocation was wrong (bad/missing/unknown flags, a
    value flag followed by another --flag, --apply without a matching
    --confirm-database, an unreadable or invalid manifest, a non-empty --out).
    """;

// Lines per transaction; each chunk is its own ApplyAsync call with a fresh context.
const int ChunkSize = 200;

try
{
    return await Run(args);
}
catch (Exception ex)
{
    // Never a raw stack trace: anything unexpected is a plain error, exit 1.
    Console.Error.WriteLine($"error: {ex.Message}");
    return 1;
}

static async Task<int> Run(string[] args)
{
    var allowedFlags = new[] { "--connection", "--manifest", "--out", "--apply", "--confirm-database" };
    if (!TryParseFlags(args, allowedFlags, out var flags, out var apply, out var parseError))
    {
        return InvocationError(parseError, showUsage: true);
    }

    var missing = new[] { "--connection", "--manifest", "--out" }.Where(f => !flags.ContainsKey(f)).ToList();
    if (missing.Count > 0)
    {
        return InvocationError($"missing required flag(s) {string.Join(", ", missing)}", showUsage: true);
    }

    var connectionString = flags["--connection"];
    var outDir = flags["--out"];

    // Required with --apply; checked whenever given, so a dry run with the
    // apply command line minus --apply still proves it names the right tenant.
    if (flags.TryGetValue("--confirm-database", out var confirmDatabase))
    {
        // GetDbConnection().Database only parses the connection string; it
        // opens no connection, so this check runs before any query.
        using var checkDb = CreateDb(connectionString);
        var actualDatabase = checkDb.Database.GetDbConnection().Database;
        if (actualDatabase != confirmDatabase)
        {
            return InvocationError(
                $"--confirm-database '{confirmDatabase}' does not match the connection's database '{actualDatabase}'");
        }
    }
    else if (apply)
    {
        return InvocationError("--apply requires --confirm-database");
    }

    IReadOnlyList<ShiftIdRepairLine> lines;
    try
    {
        using var reader = new StreamReader(flags["--manifest"]);
        lines = ShiftIdRepairCsv.ReadManifest(reader);
    }
    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException)
    {
        return InvocationError($"manifest: {ex.Message}");
    }

    // Last exit-2 gate, so nothing is written to disk before the invocation is
    // known to be valid. A used --out is refused outright: report.csv and
    // before-image.csv must never be appended to or overwritten.
    if (Directory.Exists(outDir) && Directory.EnumerateFileSystemEntries(outDir).Any())
    {
        return InvocationError($"--out '{outDir}' is not empty; use a fresh directory per run");
    }
    Directory.CreateDirectory(outDir);

    // Both opened with CreateNew: a file that appeared since the check above is
    // refused rather than overwritten.
    await using var reportWriter = new StreamWriter(
        new FileStream(Path.Combine(outDir, "report.csv"), FileMode.CreateNew, FileAccess.Write));
    await reportWriter.WriteLineAsync("Id,SdkSitId,Outcome");

    FileStream? beforeImageStream = null;
    StreamWriter? beforeImageWriter = null;
    if (apply)
    {
        beforeImageStream = new FileStream(Path.Combine(outDir, "before-image.csv"), FileMode.CreateNew,
            FileAccess.Write);
        beforeImageWriter = new StreamWriter(beforeImageStream);
        ShiftIdRepairCsv.WriteBeforeImage(beforeImageWriter, [], header: true);
        await FlushToDiskAsync(beforeImageWriter, beforeImageStream);
    }

    var counts = Enum.GetValues<ShiftIdRepairOutcome>().ToDictionary(o => o.ToString(), _ => 0);
    counts["Error"] = 0;
    counts["NotRun"] = 0;
    var exitCode = 0;

    try
    {
        var chunks = lines.Chunk(ChunkSize).ToList();
        for (var c = 0; c < chunks.Count; c++)
        {
            var chunk = chunks[c];
            var label = $"chunk {c + 1} (Ids {Int(chunk[0].Id)}..{Int(chunk[^1].Id)})";

            ShiftIdRepairResult? result = null;
            try
            {
                using var db = CreateDb(connectionString);
                result = await ShiftIdRepair.ApplyAsync(db, chunk, apply);
            }
            catch (Exception ex)
            {
                // ApplyAsync threw: its execution-strategy retry may have
                // committed without us hearing back, so the chunk's true state
                // is unknown.
                Console.Error.WriteLine(apply
                    ? $"{label}: state UNKNOWN — the commit may have landed; before-image.csv covers only the " +
                      $"chunks before it; re-run a dry run on these lines to see which are still unrepaired ({ex.Message})"
                    : $"{label}: dry run failed ({ex.Message})");
                foreach (var line in chunk)
                {
                    await WriteCsvRowAsync(reportWriter, Int(line.Id), Int(line.SdkSitId), "Error");
                }
                counts["Error"] += chunk.Length;
                exitCode = 1;
            }

            var stop = result is null && apply;
            if (result is not null)
            {
                // The before-image first, and fsynced, before anything else
                // about this chunk: it is the only undo record for rows that
                // just committed. If this write fails, the chunk is known to
                // have committed but its undo record is not durable.
                if (beforeImageWriter is not null && beforeImageStream is not null)
                {
                    try
                    {
                        ShiftIdRepairCsv.WriteBeforeImage(beforeImageWriter, result.BeforeImage, header: false);
                        await FlushToDiskAsync(beforeImageWriter, beforeImageStream);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(
                            $"{label}: APPLIED but its before-image is NOT durable ({ex.Message}); stopping");
                        exitCode = 1;
                        stop = true;
                    }
                }

                foreach (var line in result.Lines)
                {
                    await WriteCsvRowAsync(reportWriter, Int(line.Id), Int(line.SdkSitId), line.Outcome.ToString());
                    counts[line.Outcome.ToString()]++;
                    if (line.Outcome != ShiftIdRepairOutcome.Repaired)
                    {
                        exitCode = 1;
                    }
                }
            }
            await reportWriter.FlushAsync();

            if (stop)
            {
                // --apply never proceeds past a chunk whose outcome can't be
                // fully trusted; a dry run keeps going to show every result.
                var notRun = chunks.Skip(c + 1).SelectMany(x => x).ToList();
                foreach (var line in notRun)
                {
                    await WriteCsvRowAsync(reportWriter, Int(line.Id), Int(line.SdkSitId), "NotRun");
                }
                counts["NotRun"] += notRun.Count;
                await reportWriter.FlushAsync();
                break;
            }
        }
    }
    finally
    {
        if (beforeImageWriter is not null)
        {
            await beforeImageWriter.DisposeAsync();
        }
    }

    Console.WriteLine(apply ? "mode: APPLY" : "mode: dry run (nothing written)");
    Console.WriteLine($"lines: {lines.Count}");
    // The four outcomes always; Error / NotRun only when they happened.
    foreach (var (outcome, count) in counts.Where(x => x.Value > 0 || x.Key is not ("Error" or "NotRun")))
    {
        Console.WriteLine($"{outcome}: {count}");
    }

    return exitCode;
}

static int InvocationError(string message, bool showUsage = false)
{
    Console.Error.WriteLine($"error: {message}");
    if (showUsage)
    {
        Console.Error.WriteLine(Usage);
    }
    return 2;
}

static bool TryParseFlags(string[] args, IReadOnlyCollection<string> allowedFlags,
    out Dictionary<string, string> flags, out bool apply, out string error)
{
    flags = new Dictionary<string, string>();
    apply = false;
    error = "";

    for (var i = 0; i < args.Length; i++)
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

        // A missing value, or a next token that is itself a flag, is an
        // invocation error: `--out --apply` must never parse as a dry run
        // writing to a directory called "--apply".
        if (i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
        {
            error = $"flag '{token}' requires a value";
            return false;
        }

        if (flags.ContainsKey(token))
        {
            error = $"flag '{token}' given more than once";
            return false;
        }

        flags[token] = args[++i];
    }

    return true;
}

// Through the writer's buffer and the OS cache onto the disk.
static async Task FlushToDiskAsync(StreamWriter writer, FileStream stream)
{
    await writer.FlushAsync();
    stream.Flush(flushToDisk: true);
}

static TimePlanningPnDbContext CreateDb(string connectionString) =>
    new TimePlanningPnContextFactory().CreateDbContext(new[] { connectionString });

static Task WriteCsvRowAsync(StreamWriter writer, params string[] fields) =>
    writer.WriteLineAsync(string.Join(",", fields));

static string Int(int value) => value.ToString(CultureInfo.InvariantCulture);
