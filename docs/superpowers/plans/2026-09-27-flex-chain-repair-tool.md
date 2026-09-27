# Flex Chain Repair Tool Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** A guarded, dry-run-by-default tool that restores worked hours on listed rows and re-carries each worker's flex chain with the product's own `RunForwardAsync`, verifying against offline expected balances before committing.

**Architecture:** Library class `FlexChainRepair` (+ `FlexChainRepairCsv`, record types) in `Microting.TimePlanningBase/Infrastructure/Helpers`, tested in the existing test project; a thin console in `tools/FlexChainRepair` that parses arguments, loops workers with a fresh context each, and writes report files.

**Tech Stack:** C# / .NET 10, EF Core 10 + Pomelo MariaDB, NUnit 4, MariaDB service in CI.

**Spec:** `docs/superpowers/specs/2026-09-27-flex-chain-repair-tool-design.md`

## Global Constraints

- Balances are carried ONLY by `FlexChainRecompute.RunForwardAsync` — no second chain implementation.
- Every write goes through `db.Database.CreateExecutionStrategy().ExecuteAsync` wrapping an explicit transaction (the context factory enables retry-on-failure).
- Dry run is the default everywhere; a dry run leaves rows, versions and `PlanRegistrationVersions` untouched.
- After any rollback, call `db.ChangeTracker.Clear()` so the context never holds rolled-back values.
- Check tolerance: `0.01` hours. Guard tolerance on `ExpectNettoHours`: `1e-6`.
- CSV: header row, comma separator, `CultureInfo.InvariantCulture` for every number.
- No real names, customer names or tenant ids in code, tests, comments, commits or PR text.
- Do not change `RunForwardAsync`, `CarryChain`, `DayLock` or any entity. Do not modify the `.sln` or existing `.csproj` references.
- Tests run only in CI (`dotnet test` locally is blocked); verify locally with `dotnet build`.
- Each new file carries the MIT license header used by the other files in the project.

## Review Focus

1. A worker with pre-created future rows (dated after the expected file's last row) — continuity check must still pass and those rows must not count as mismatches. Test: Task 2 `RowWithoutExpectedEntry_IsStillCheckedForContinuity`.
2. A manifest line whose row was soft-deleted since the dump — must be `GuardFailed`, not silently skipped. Test: Task 2 `Guard_RemovedRow_Fails`.
3. Dry run followed by apply on the same context — the second call must see database state, not the rolled-back tracked values. Test: Task 2 `DryRunThenApply_OnSameContext_Applies`.
4. A worker with no live rows (restore list empty) — returns `DryRunOk`/`Applied` with zero counts, does not throw. Test: Task 2 `WorkerWithNoRows_IsANoOp`.
5. Decimal-comma process culture (the servers run da-DK) — CSV numbers still parse/write with `.`. Test: Task 1 `Parsing_IgnoresCurrentCulture`.

---

### Task 1: Record types and CSV

**Files:**
- Create: `Microting.TimePlanningBase/Infrastructure/Helpers/FlexChainRepairTypes.cs`
- Create: `Microting.TimePlanningBase/Infrastructure/Helpers/FlexChainRepairCsv.cs`
- Test: `Microting.TimePlanningBase.Tests/FlexChainRepairCsvTests.cs`

**Interfaces:**
- Produces (namespace `Microting.TimePlanningBase.Infrastructure.Helpers`):

```csharp
public sealed record RestoreLine(int Id, int SdkSitId, int ExpectVersion, double ExpectNettoHours,
    double RestoreNettoHours, int RestoreNettoHoursInSeconds, string Reason);

public sealed record ExpectedBalance(int SdkSitId, int Id, double SumFlexStart, double SumFlexEnd);

public sealed record BeforeImageRow(int Id, int SdkSitId, int Version, int RepairedVersion,
    double NettoHours, int NettoHoursInSeconds, double Flex, int FlexInSeconds,
    double SumFlexStart, int SumFlexStartInSeconds, double SumFlexEnd, int SumFlexEndInSeconds);

public enum RepairOutcome { DryRunOk, Applied, GuardFailed, Mismatch, Locked }

public sealed record RowMismatch(int Id, DateTime Date, string Field, double Expected, double Actual);

public sealed class WorkerRepairResult
{
    public int SdkSitId { get; init; }
    public RepairOutcome Outcome { get; set; }
    public int RowsRestored { get; set; }
    public int RowsWalked { get; set; }
    public double? EndBalanceBefore { get; set; }
    public double? EndBalanceAfter { get; set; }
    public List<BeforeImageRow> BeforeImage { get; } = new();
    public List<int> GuardFailedIds { get; } = new();
    public List<RowMismatch> Mismatches { get; } = new();
}

public sealed class RevertResult
{
    public bool Applied { get; set; }
    public int Reverted { get; set; }
    public List<int> ChangedIds { get; } = new();
    public List<int> MissingIds { get; } = new();
}

public static class FlexChainRepairCsv
{
    public static IReadOnlyList<RestoreLine> ReadRestore(TextReader reader);
    public static IReadOnlyList<ExpectedBalance> ReadExpected(TextReader reader);
    public static IReadOnlyList<BeforeImageRow> ReadBeforeImage(TextReader reader);
    public static void WriteBeforeImage(TextWriter writer, IEnumerable<BeforeImageRow> rows, bool header);
}
```

CSV headers (exact, column order fixed):
- restore: `Id,SdkSitId,ExpectVersion,ExpectNettoHours,RestoreNettoHours,RestoreNettoHoursInSeconds,Reason`
- expected: `SdkSitId,Id,SumFlexStart,SumFlexEnd`
- before-image: `Id,SdkSitId,Version,RepairedVersion,NettoHours,NettoHoursInSeconds,Flex,FlexInSeconds,SumFlexStart,SumFlexStartInSeconds,SumFlexEnd,SumFlexEndInSeconds`

Readers: first line must equal the header exactly, otherwise `throw new FormatException($"expected header '{Header}'")`. Blank lines are skipped. A line with the wrong column count throws `FormatException` naming the 1-based line number. Numbers use `double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture)` / `int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture)`. Writer uses `ToString("R", CultureInfo.InvariantCulture)` for doubles.

- [ ] **Step 1: Write the failing tests** (`FlexChainRepairCsvTests`, plain `[TestFixture]`, NOT `DbTestFixture`)

```csharp
[TestFixture]
public class FlexChainRepairCsvTests
{
    [Test]
    public void ReadRestore_ParsesEveryColumn()
    {
        var csv = "Id,SdkSitId,ExpectVersion,ExpectNettoHours,RestoreNettoHours,RestoreNettoHoursInSeconds,Reason\n"
                + "10,7,3,6.25,7.5,27000,recompute\n\n"
                + "11,7,1,-9.58,0,0,open-shift\n";
        var lines = FlexChainRepairCsv.ReadRestore(new StringReader(csv));
        Assert.That(lines, Is.EqualTo(new[]
        {
            new RestoreLine(10, 7, 3, 6.25, 7.5, 27000, "recompute"),
            new RestoreLine(11, 7, 1, -9.58, 0, 0, "open-shift"),
        }));
    }

    [Test]
    public void ReadExpected_ParsesEveryColumn()
    {
        var csv = "SdkSitId,Id,SumFlexStart,SumFlexEnd\n7,10,1.5,2.25\n";
        Assert.That(FlexChainRepairCsv.ReadExpected(new StringReader(csv)),
            Is.EqualTo(new[] { new ExpectedBalance(7, 10, 1.5, 2.25) }));
    }

    [Test]
    public void BeforeImage_RoundTrips()
    {
        var row = new BeforeImageRow(10, 7, 3, 5, 7.123456789, 25644, -0.376543211, -1356,
            12.5, 45000, 12.123456789, 43644);
        var sw = new StringWriter();
        FlexChainRepairCsv.WriteBeforeImage(sw, new[] { row }, header: true);
        Assert.That(FlexChainRepairCsv.ReadBeforeImage(new StringReader(sw.ToString())),
            Is.EqualTo(new[] { row }));
    }

    [Test]
    public void WrongHeader_Throws()
    {
        Assert.Throws<FormatException>(() =>
            FlexChainRepairCsv.ReadExpected(new StringReader("Id,SdkSitId,SumFlexStart,SumFlexEnd\n")));
    }

    [Test]
    public void WrongColumnCount_ThrowsWithLineNumber()
    {
        var ex = Assert.Throws<FormatException>(() =>
            FlexChainRepairCsv.ReadExpected(new StringReader("SdkSitId,Id,SumFlexStart,SumFlexEnd\n7,10,1.5\n")));
        Assert.That(ex!.Message, Does.Contain("2"));
    }

    [Test]
    public void Parsing_IgnoresCurrentCulture()
    {
        var saved = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("da-DK");
            var csv = "SdkSitId,Id,SumFlexStart,SumFlexEnd\n7,10,1.5,-2.25\n";
            var e = FlexChainRepairCsv.ReadExpected(new StringReader(csv)).Single();
            Assert.That(e.SumFlexEnd, Is.EqualTo(-2.25));
            var sw = new StringWriter();
            FlexChainRepairCsv.WriteBeforeImage(sw,
                new[] { new BeforeImageRow(1, 1, 1, 1, 0.5, 1800, 0.5, 1800, 0, 0, 0.5, 1800) }, header: false);
            Assert.That(sw.ToString(), Does.Contain("0.5"));
            Assert.That(sw.ToString(), Does.Not.Contain("0,5"));
        }
        finally { CultureInfo.CurrentCulture = saved; }
    }
}
```

- [ ] **Step 2: Build to verify the tests fail to compile**

Run: `dotnet build Microting.TimePlanningBase.Tests/Microting.TimePlanningBase.Tests.csproj`
Expected: errors — `FlexChainRepairCsv` / `RestoreLine` not found.

- [ ] **Step 3: Implement the types and `FlexChainRepairCsv`** exactly per the Interfaces block above (records give value equality, so the tests compare whole records).

- [ ] **Step 4: Build**

Run: `dotnet build Microting.TimePlanningBase.Tests/Microting.TimePlanningBase.Tests.csproj`
Expected: 0 errors. (Tests execute in CI.)

- [ ] **Step 5: Leave changes uncommitted** — the controller commits after review.

---

### Task 2: `FlexChainRepair.RepairWorkerAsync`

**Files:**
- Create: `Microting.TimePlanningBase/Infrastructure/Helpers/FlexChainRepair.cs`
- Test: `Microting.TimePlanningBase.Tests/FlexChainRepairTests.cs`

**Interfaces:**
- Consumes: Task 1 types; `FlexChainRecompute.RunForwardAsync(TimePlanningPnDbContext db, AssignedSite? assignedSite, int sdkSitId, DateTime fromDateInclusive)`; `DayLock.LockedThroughAsync(TimePlanningPnDbContext db, int sdkSitId)` → `Task<DateTime?>`; `PnBase.CreateVersionSnapshot()` (internal, returns `object`, a `PlanRegistrationVersion` for PlanRegistration).
- Produces:

```csharp
public static class FlexChainRepair
{
    public const double DefaultTolerance = 0.01;
    public static Task<WorkerRepairResult> RepairWorkerAsync(TimePlanningPnDbContext db, int sdkSitId,
        IReadOnlyList<RestoreLine> restores, IReadOnlyDictionary<int, ExpectedBalance> expected,
        bool apply, double tolerance = DefaultTolerance);
}
```

Algorithm (spec §4, steps 1–7):

```csharp
public static async Task<WorkerRepairResult> RepairWorkerAsync(TimePlanningPnDbContext db, int sdkSitId,
    IReadOnlyList<RestoreLine> restores, IReadOnlyDictionary<int, ExpectedBalance> expected,
    bool apply, double tolerance = DefaultTolerance)
{
    var result = new WorkerRepairResult { SdkSitId = sdkSitId };
    db.ChangeTracker.Clear();

    if (await DayLock.LockedThroughAsync(db, sdkSitId) is not null)
    {
        result.Outcome = RepairOutcome.Locked;
        return result;
    }

    var strategy = db.Database.CreateExecutionStrategy();
    await strategy.ExecuteAsync(async () =>
    {
        // a retried attempt starts from a clean slate
        result.BeforeImage.Clear(); result.GuardFailedIds.Clear(); result.Mismatches.Clear();
        result.RowsRestored = 0; result.RowsWalked = 0;
        db.ChangeTracker.Clear();

        await using var tx = await db.Database.BeginTransactionAsync();
        var commit = false;
        try
        {
            commit = await RunAsync(db, sdkSitId, restores, expected, apply, tolerance, result);
            if (commit) await tx.CommitAsync(); else await tx.RollbackAsync();
        }
        finally
        {
            db.ChangeTracker.Clear();
        }
    });
    return result;
}
```

`RunAsync` (private) returns `true` only when `apply` and every check passed; it sets `result.Outcome` in every path:

1. `var live = db.PlanRegistrations.Where(x => x.SdkSitId == sdkSitId && x.WorkflowState != Constants.WorkflowStates.Removed);`
   Load `rows = await live.OrderBy(x => x.Date).ThenBy(x => x.Id).ToListAsync()` (tracked).
   `result.EndBalanceBefore = rows.LastOrDefault()?.SumFlexEnd;`
   Before-image: one `BeforeImageRow` per row with `RepairedVersion = row.Version` for now.
   If `rows.Count == 0`: `Outcome = apply ? Applied : DryRunOk`; return `apply`.
2. Guard: `byId = rows.ToDictionary(x => x.Id)`. For each restore line: fail (add `line.Id` to `GuardFailedIds`) when `!byId.TryGetValue(line.Id, out var r)` (missing, removed, or another worker's row — the query is already scoped) or `line.SdkSitId != sdkSitId` or `r.Version != line.ExpectVersion` or `Math.Abs(r.NettoHours - line.ExpectNettoHours) > 1e-6`. Any failure → `Outcome = GuardFailed`, return `false`.
3. Restore: for each line set `NettoHours`, `NettoHoursInSeconds`, `Version += 1`, `UpdatedAt = DateTime.UtcNow`; `SaveChangesAsync`; then `db.PlanRegistrationVersions.AddAsync((PlanRegistrationVersion)r.CreateVersionSnapshot())` for each; `SaveChangesAsync`. `RowsRestored = restores.Count`.
4. Walk: `var site = await db.AssignedSites.AsNoTracking().FirstOrDefaultAsync(x => x.SiteId == sdkSitId && x.WorkflowState != Constants.WorkflowStates.Removed);`
   `RowsWalked = await FlexChainRecompute.RunForwardAsync(db, site, sdkSitId, rows[0].Date);`
5. Check: `after = await live.AsNoTracking().OrderBy(x => x.Date).ThenBy(x => x.Id).ToListAsync();`
   For each row with an `expected` entry: compare `SumFlexStart`, `SumFlexEnd` (`Field` = the property name).
   For each consecutive pair: `|after[i].SumFlexStart - after[i-1].SumFlexEnd| > tolerance` → mismatch with `Field = "Continuity"`, `Expected = after[i-1].SumFlexEnd`.
   Keep at most 20 mismatches. `EndBalanceAfter = after.Last().SumFlexEnd`. Set every before-image row's `RepairedVersion` from `after` (by Id) — records are immutable, so rebuild the list with `with { RepairedVersion = ... }`.
   Any mismatch → `Outcome = Mismatch`, return `false`.
6. `Outcome = apply ? Applied : DryRunOk`; return `apply`.

- [ ] **Step 1: Write the failing tests** (`FlexChainRepairTests : DbTestFixture`). Reuse the helper style of `FlexChainRecomputeTests` (`Row`, `Site`, `Reload`). Worker `4712`, `D0 = new DateTime(2025, 1, 6)`. Helpers:

```csharp
private const int Worker = 4712;
private static readonly DateTime D0 = new(2025, 1, 6);

private async Task<PlanRegistration> Row(int day, double netto, double plan,
    double sumFlexStart = 0, double sumFlexEnd = 0, bool reconciled = false, int worker = Worker)
{
    var pr = new PlanRegistration
    {
        SdkSitId = worker, Date = D0.AddDays(day), NettoHours = netto,
        NettoHoursInSeconds = (int)Math.Round(netto * 3600), PlanHours = plan, Flex = netto - plan,
        SumFlexStart = sumFlexStart, SumFlexEnd = sumFlexEnd, Reconciled = reconciled
    };
    await pr.Create(DbContext);
    return pr;
}

private async Task Site() => await new AssignedSite { SiteId = Worker }.Create(DbContext);

private PlanRegistration[] Reload() => DbContext.PlanRegistrations.AsNoTracking()
    .Where(x => x.SdkSitId == Worker).OrderBy(x => x.Date).ThenBy(x => x.Id).ToArray();

private static RestoreLine Restore(PlanRegistration r, double hours, string reason = "recompute") =>
    new(r.Id, r.SdkSitId, r.Version, r.NettoHours, hours, (int)Math.Round(hours * 3600), reason);

private static Dictionary<int, ExpectedBalance> Expect(params (PlanRegistration row, double start, double end)[] e) =>
    e.ToDictionary(x => x.row.Id, x => new ExpectedBalance(x.row.SdkSitId, x.row.Id, x.start, x.end));

private int VersionRows() => DbContext.PlanRegistrationVersions.AsNoTracking().Count();
```

Tests (each asserts inside `Assert.Multiple`):

```csharp
[Test]
public async Task Apply_RestoresHours_AndCarriesTheChain()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);      // recomputed wrongly: truth is 8
    var c = await Row(2, 7.5, 7.5, -2.0, -2.0);

    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        Expect((a, 0, 0.5), (b, 0.5, 1.0), (c, 1.0, 1.0)), apply: true);

    var rows = Reload();
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
        Assert.That(res.RowsRestored, Is.EqualTo(1));
        Assert.That(rows[1].NettoHours, Is.EqualTo(8));
        Assert.That(rows[1].NettoHoursInSeconds, Is.EqualTo(28800));
        Assert.That(rows[2].SumFlexEnd, Is.EqualTo(1.0).Within(1e-9));
        Assert.That(res.EndBalanceBefore, Is.EqualTo(-2.0).Within(1e-9));
        Assert.That(res.EndBalanceAfter, Is.EqualTo(1.0).Within(1e-9));
        Assert.That(res.BeforeImage.Single(x => x.Id == b.Id).NettoHours, Is.EqualTo(5));
        Assert.That(res.BeforeImage.Single(x => x.Id == b.Id).RepairedVersion, Is.EqualTo(rows[1].Version));
    });
}

[Test]
public async Task DryRun_WritesNothing_AndReportsTheSameCounts()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var before = Reload(); var versionsBefore = VersionRows();

    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: false);

    var after = Reload();
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.DryRunOk));
        Assert.That(res.RowsRestored, Is.EqualTo(1));
        Assert.That(res.RowsWalked, Is.GreaterThan(0));
        Assert.That(after.Select(x => (x.NettoHours, x.SumFlexEnd, x.Version)),
            Is.EqualTo(before.Select(x => (x.NettoHours, x.SumFlexEnd, x.Version))));
        Assert.That(VersionRows(), Is.EqualTo(versionsBefore));
    });
}

[Test]
public async Task DryRunThenApply_OnSameContext_Applies()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var restores = new[] { Restore(b, 8) };
    var expected = Expect((a, 0, 0.5), (b, 0.5, 1.0));

    await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, restores, expected, apply: false);
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, restores, expected, apply: true);

    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
        Assert.That(Reload()[1].NettoHours, Is.EqualTo(8));
    });
}

[Test]
public async Task Guard_StaleVersion_FailsAndWritesNothing()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var stale = Restore(b, 8) with { ExpectVersion = b.Version - 1 };
    var versionsBefore = VersionRows();

    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { stale },
        Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
        Assert.That(res.GuardFailedIds, Is.EqualTo(new[] { b.Id }));
        Assert.That(Reload()[1].NettoHours, Is.EqualTo(5));
        Assert.That(VersionRows(), Is.EqualTo(versionsBefore));
    });
}

[Test]
public async Task Guard_ChangedHours_Fails()
{
    await Site();
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var line = Restore(b, 8) with { ExpectNettoHours = 5.5 };
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { line },
        new Dictionary<int, ExpectedBalance>(), apply: true);
    Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
}

[Test]
public async Task Guard_AnotherWorkersRow_Fails()
{
    await Site();
    await Row(0, 8, 7.5, 0, 0.5);
    var other = await Row(0, 5, 7.5, 0, -2.5, worker: 9999);
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(other, 8) },
        new Dictionary<int, ExpectedBalance>(), apply: true);
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
        Assert.That(res.GuardFailedIds, Is.EqualTo(new[] { other.Id }));
    });
}

[Test]
public async Task Guard_RemovedRow_Fails()
{
    await Site();
    await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var line = Restore(b, 8);
    await b.Delete(DbContext);   // soft delete after the dump
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { line },
        new Dictionary<int, ExpectedBalance>(), apply: true);
    Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.GuardFailed));
}

[Test]
public async Task OpenShiftLine_RestoresZero_AndTheChainFollows()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var open = await Row(1, -9.58, 7.5, 0.5, -16.58);
    var c = await Row(2, 7.5, 7.5, -16.58, -16.58);

    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(open, 0, "open-shift") },
        Expect((a, 0, 0.5), (open, 0.5, -7.0), (c, -7.0, -7.0)), apply: true);

    var rows = Reload();
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
        Assert.That(rows[1].NettoHours, Is.EqualTo(0));
        Assert.That(rows[1].NettoHoursInSeconds, Is.EqualTo(0));
        Assert.That(rows[2].SumFlexEnd, Is.EqualTo(-7.0).Within(1e-9));
    });
}

[Test]
public async Task NoRestoreLines_TheWalkAloneHealsAStoredBreak()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 8, 7.5, 3.0, 3.5);        // break: start 3.0 != previous end 0.5
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, Array.Empty<RestoreLine>(),
        Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
        Assert.That(res.RowsRestored, Is.EqualTo(0));
        Assert.That(Reload()[1].SumFlexEnd, Is.EqualTo(1.0).Within(1e-9));
    });
}

[Test]
public async Task ExpectedMismatch_RollsBack()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        Expect((a, 0, 0.5), (b, 0.5, 1.5)), apply: true);   // oracle says 1.5, product computes 1.0
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Mismatch));
        Assert.That(res.Mismatches.Single().Id, Is.EqualTo(b.Id));
        Assert.That(res.Mismatches.Single().Field, Is.EqualTo("SumFlexEnd"));
        Assert.That(Reload()[1].NettoHours, Is.EqualTo(5));
    });
}

[Test]
public async Task RowWithoutExpectedEntry_IsStillCheckedForContinuity()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    await Row(30, 0, 7.5, 99, 91.5);               // pre-created future row, not in the oracle
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, Array.Empty<RestoreLine>(),
        Expect((a, 0, 0.5)), apply: true);
    var rows = Reload();
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Applied));
        Assert.That(res.Mismatches, Is.Empty);
        Assert.That(rows[1].SumFlexStart, Is.EqualTo(0.5).Within(1e-9));
        Assert.That(rows[1].SumFlexEnd, Is.EqualTo(-7.0).Within(1e-9));
    });
}

[Test]
public async Task ReconciledDay_IsLocked_AndNothingIsWritten()
{
    await Site();
    await Row(0, 8, 7.5, 0, 0.5, reconciled: true);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        new Dictionary<int, ExpectedBalance>(), apply: true);
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.Locked));
        Assert.That(Reload()[1].NettoHours, Is.EqualTo(5));
    });
}

[Test]
public async Task Apply_AddsOneVersionRowPerRestoreAndPerWalkedRow()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var c = await Row(2, 7.5, 7.5, -2.0, -2.0);
    var versionsBefore = VersionRows();

    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        Expect((a, 0, 0.5), (b, 0.5, 1.0), (c, 1.0, 1.0)), apply: true);

    Assert.That(VersionRows() - versionsBefore, Is.EqualTo(res.RowsRestored + res.RowsWalked));
}

[Test]
public async Task WorkerWithNoRows_IsANoOp()
{
    await Site();
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, Array.Empty<RestoreLine>(),
        new Dictionary<int, ExpectedBalance>(), apply: false);
    Assert.Multiple(() =>
    {
        Assert.That(res.Outcome, Is.EqualTo(RepairOutcome.DryRunOk));
        Assert.That(res.RowsWalked, Is.EqualTo(0));
        Assert.That(res.EndBalanceAfter, Is.Null);
    });
}
```

Note on `Row`: `PnBase.Create` already writes one version row per created row — tests compare version counts relative to `versionsBefore` taken after setup. `PnBase.Delete` soft-deletes (sets `WorkflowState = removed`) on the same context.

- [ ] **Step 2: Build — tests fail to compile** (`FlexChainRepair` missing).

Run: `dotnet build Microting.TimePlanningBase.Tests/Microting.TimePlanningBase.Tests.csproj`

- [ ] **Step 3: Implement `FlexChainRepair.RepairWorkerAsync`** per the algorithm above, with an XML summary explaining guard → restore → walk → check and dry run.

- [ ] **Step 4: Build** — 0 errors.

- [ ] **Step 5: Leave uncommitted** for controller review.

---

### Task 3: `FlexChainRepair.RevertAsync`

**Files:**
- Modify: `Microting.TimePlanningBase/Infrastructure/Helpers/FlexChainRepair.cs`
- Modify: `Microting.TimePlanningBase.Tests/FlexChainRepairTests.cs`

**Interfaces:**
- Consumes: Task 1 `BeforeImageRow`, `RevertResult`; Task 2 `RepairWorkerAsync`.
- Produces:

```csharp
public static Task<RevertResult> RevertAsync(TimePlanningPnDbContext db,
    IReadOnlyList<BeforeImageRow> rows, bool apply);
```

Algorithm: one transaction through the execution strategy (same shape as Task 2, `ChangeTracker.Clear()` before and after). Load tracked `PlanRegistrations` whose `Id` is in the list (any workflow state). For each before-image row: not found → `MissingIds`; `Version != RepairedVersion` → `ChangedIds`; else write `NettoHours`, `NettoHoursInSeconds`, `Flex`, `FlexInSeconds`, `SumFlexStart`, `SumFlexStartInSeconds`, `SumFlexEnd`, `SumFlexEndInSeconds` back, `Version += 1`, `UpdatedAt = now`, `Reverted++`. `SaveChangesAsync`, then one version snapshot per reverted row, `SaveChangesAsync`. Commit only when `apply`; `result.Applied = apply`. Changed/missing rows never block the others — they are reported.

- [ ] **Step 1: Write the failing tests**

```csharp
[Test]
public async Task Revert_AfterApply_RestoresEveryCapturedColumn()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var original = Reload();
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

    var rev = await FlexChainRepair.RevertAsync(DbContext, res.BeforeImage, apply: true);

    var rows = Reload();
    Assert.Multiple(() =>
    {
        Assert.That(rev.Reverted, Is.EqualTo(2));
        Assert.That(rev.ChangedIds, Is.Empty);
        Assert.That(rows.Select(x => (x.NettoHours, x.NettoHoursInSeconds, x.Flex, x.SumFlexStart, x.SumFlexEnd)),
            Is.EqualTo(original.Select(x => (x.NettoHours, x.NettoHoursInSeconds, x.Flex, x.SumFlexStart, x.SumFlexEnd))));
    });
}

[Test]
public async Task Revert_SkipsARowChangedAfterTheRepair()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

    var edited = await DbContext.PlanRegistrations.SingleAsync(x => x.Id == b.Id);
    edited.NettoHours = 9;
    await edited.Update(DbContext);          // Version + 1: the customer edited it after the repair

    var rev = await FlexChainRepair.RevertAsync(DbContext, res.BeforeImage, apply: true);

    Assert.Multiple(() =>
    {
        Assert.That(rev.ChangedIds, Is.EqualTo(new[] { b.Id }));
        Assert.That(Reload()[1].NettoHours, Is.EqualTo(9));
    });
}

[Test]
public async Task Revert_DryRun_WritesNothing()
{
    await Site();
    var a = await Row(0, 8, 7.5, 0, 0.5);
    var b = await Row(1, 5, 7.5, 0.5, -2.0);
    var res = await FlexChainRepair.RepairWorkerAsync(DbContext, Worker, new[] { Restore(b, 8) },
        Expect((a, 0, 0.5), (b, 0.5, 1.0)), apply: true);

    var rev = await FlexChainRepair.RevertAsync(DbContext, res.BeforeImage, apply: false);

    Assert.Multiple(() =>
    {
        Assert.That(rev.Applied, Is.False);
        Assert.That(rev.Reverted, Is.EqualTo(2));
        Assert.That(Reload()[1].NettoHours, Is.EqualTo(8));
    });
}
```

- [ ] **Step 2: Build — fails to compile** (`RevertAsync` missing).
- [ ] **Step 3: Implement `RevertAsync`.**
- [ ] **Step 4: Build** — 0 errors.
- [ ] **Step 5: Leave uncommitted.**

---

### Task 4: Console `tools/FlexChainRepair`

**Files:**
- Create: `tools/FlexChainRepair/FlexChainRepair.csproj`
- Create: `tools/FlexChainRepair/Program.cs`
- Create: `tools/FlexChainRepair/README.md`

**Interfaces:**
- Consumes: Tasks 1–3 public API; `TimePlanningPnContextFactory().CreateDbContext(new[] { connectionString })`.

`FlexChainRepair.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <IsPackable>false</IsPackable>
    </PropertyGroup>
    <ItemGroup>
        <ProjectReference Include="..\..\Microting.TimePlanningBase\Microting.TimePlanningBase.csproj" />
    </ItemGroup>
</Project>
```

`Program.cs` behaviour (no logic beyond this list):

- Commands: `repair` and `revert`, flags as in spec §5: `--connection`, `--restore`, `--expected`, `--before-image`, `--out`, `--worker`, `--apply`, `--confirm-database`. Unknown flag or missing required flag → usage text on stderr, exit 2.
- `--apply` without `--confirm-database`, or with a value different from the connection's database name (`db.Database.GetDbConnection().Database`), → error, exit 2, before any query.
- `repair`: read both CSVs. Workers = `AssignedSites` where `WorkflowState != removed && !Resigned`, distinct `SiteId`, ascending, narrowed by `--worker`. A restore line whose `SdkSitId` is not in that set → error, exit 2, before any write. For each worker: new context from the factory, `RepairWorkerAsync(db, worker, linesForWorker, expectedForWorker, apply)`; print one line `sdkSitId outcome restored walked before→after`; append to `<out>/report.csv` (`SdkSitId,Outcome,RowsRestored,RowsWalked,EndBalanceBefore,EndBalanceAfter`); append mismatches to `<out>/mismatches.csv` (`SdkSitId,Id,Date,Field,Expected,Actual`) and guard failures to `<out>/guard-failed.csv` (`SdkSitId,Id`); on `Applied`, append its before-image to `<out>/before-image.csv` (header written once). A worker throwing is caught, printed, reported as `Error`, and the loop continues. Exit 0 only when every worker is `DryRunOk`/`Applied`, else 1.
- `revert`: read before-image, one `RevertAsync` call, print reverted/changed/missing counts, write `<out>/revert-changed.csv` and `<out>/revert-missing.csv` (`Id`). Exit 0 when no changed/missing, else 1.
- `--out` directory is created if missing; refuse to start if `<out>/report.csv` or `<out>/before-image.csv` already exists (never append to a previous run).
- All numbers written with `CultureInfo.InvariantCulture`.

`README.md`: purpose, the two commands with examples using placeholder values (`Server=127.0.0.1;Port=3306;Database=<tenant-db>;...`), the run procedure from spec §6, and "dry run is the default".

- [ ] **Step 1: Create the three files.**
- [ ] **Step 2: Build**

Run: `dotnet build tools/FlexChainRepair/FlexChainRepair.csproj`
Expected: 0 errors.

- [ ] **Step 3: Smoke-check argument handling locally** (no database):

Run: `dotnet run --project tools/FlexChainRepair -- repair --apply --connection "Server=127.0.0.1;Database=x" --restore a --expected b --out /tmp/x`
Expected: exit 2 with a message that `--confirm-database` is required.

- [ ] **Step 4: Leave uncommitted.**
