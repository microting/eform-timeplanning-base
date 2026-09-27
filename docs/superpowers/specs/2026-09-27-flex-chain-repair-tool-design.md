# Flex chain repair tool — design

**Date:** 2026-09-27
**Scope:** Sub-project 3 of the 2026-09 flex drift programme — repair the affected tenant.
Sub-projects 1 + 2 (open shift → 0, balance-only `FlexChain.CarryChain`,
`FlexChainRecompute.RunForwardAsync`, every write path walks forward) and the service's
nightly 7-day walk are deployed. This spec covers the tool that repairs history written
before they were.

## 1. Problem

On the affected tenant, an August repair walk recomputed worked hours on old rows from
stale device stamps. The rows' inputs (start/stop/pause ids, stamps, one-minute marker)
are intact; only `NettoHours` / `NettoHoursInSeconds` — and so `Flex` and every later
balance — are wrong. The investigation dump (2026-09-24) found:

- ~1,120 such rows across ~32 workers, dated 2024-11 to 2026-07.
- 2 open-shift rows stored with negative hours (the rule is: an open shift counts 0 h).
- Stored chain breaks from edits that were never carried forward, on most workers.

Goal: every active worker on the tenant ends with the balance their real hours produce,
carried unbroken from their first day to their last row.

## 2. Decisions

| Decision | Choice |
|---|---|
| What carries balances forward | The product's own `FlexChainRecompute.RunForwardAsync` — never a second chain implementation |
| Where the tool lives | This repo: logic in the library (`FlexChainRepair`), thin console in `tools/FlexChainRepair` (not in the `.sln`) |
| Hours to restore | The last pre-incident version's `NettoHours` + `NettoHoursInSeconds`; open-shift rows → 0 / 0 |
| Rows edited since the incident | Keep the customer's value — not in the manifest |
| Which workers are walked | Every non-removed, non-resigned worker on the tenant, from their first row |
| Oracle | An independent offline replay (not in this repo) supplies expected balances; the tool refuses to commit a worker that disagrees |
| Default mode | Dry run — every step runs in a transaction that is rolled back |

## 3. Inputs

Both files are generated offline from a fresh read-only dump. They hold row ids and
numbers, no names, and are kept outside the repository.

**Restore manifest** (`restore.csv`, header row, invariant culture):

| Column | Meaning |
|---|---|
| `Id` | PlanRegistration id |
| `SdkSitId` | the worker, for grouping and a sanity check |
| `ExpectVersion` | the row's `Version` at dump time — guard |
| `ExpectNettoHours` | the row's `NettoHours` at dump time — guard (tolerance 1e-6) |
| `RestoreNettoHours` | value to write |
| `RestoreNettoHoursInSeconds` | value to write |
| `Reason` | `recompute` or `open-shift` (reported only) |

**Expected balances** (`expected.csv`): `SdkSitId,Id,SumFlexStart,SumFlexEnd` for every
live row of every worker to walk, as the replay computes them after the restore.

## 4. Library: `FlexChainRepair` (Microting.TimePlanningBase, `Infrastructure/Helpers`)

```csharp
public static Task<WorkerRepairResult> RepairWorkerAsync(
    TimePlanningPnDbContext db, int sdkSitId,
    IReadOnlyList<RestoreLine> restores,          // this worker's manifest lines (may be empty)
    IReadOnlyDictionary<int, ExpectedBalance> expected,  // keyed by row Id
    bool apply, double tolerance = 0.01);

public static Task<RevertResult> RevertAsync(
    TimePlanningPnDbContext db, IReadOnlyList<BeforeImageRow> rows, bool apply);
```

`RepairWorkerAsync`, for one worker, inside one transaction run through the context's
execution strategy (the factory enables retry-on-failure, so a user transaction must go
through `db.Database.CreateExecutionStrategy().ExecuteAsync`):

1. **Lock check.** If `DayLock.LockedThroughAsync` returns a boundary, the worker is
   refused (`Outcome = Locked`): the walk cannot pass a reconciled day and the repair
   must not half-apply.
2. **Before-image.** Capture every live row of the worker (`Id`, `Version`, `NettoHours`,
   `NettoHoursInSeconds`, `Flex`, `FlexInSeconds`, `SumFlexStart`, `SumFlexStartInSeconds`,
   `SumFlexEnd`, `SumFlexEndInSeconds`) into the result.
3. **Guard.** Every restore line's row must exist, be live, belong to `sdkSitId`, and have
   `Version == ExpectVersion` and `|NettoHours − ExpectNettoHours| ≤ 1e-6`. Any failure →
   rollback, `Outcome = GuardFailed`, with the offending ids.
4. **Restore.** Set `NettoHours` / `NettoHoursInSeconds`; `Version + 1`, `UpdatedAt = now`;
   save; add `CreateVersionSnapshot()` for each; save.
5. **Walk.** `RunForwardAsync(db, assignedSite, sdkSitId, firstRowDate)` where
   `firstRowDate` is the worker's earliest live row date and `assignedSite` is the worker's
   non-removed AssignedSite (null when none).
6. **Check.** Reload the worker's live rows. Every row with an expected entry must match
   `SumFlexStart` and `SumFlexEnd` within `tolerance`; every row must satisfy
   `SumFlexStart(n+1) == SumFlexEnd(n)` within `tolerance` (covers rows created after the
   dump). Any failure → rollback, `Outcome = Mismatch`, with the first 20 differing rows.
7. **Finish.** `apply == false` → rollback, `Outcome = DryRunOk`. `apply == true` → commit,
   `Outcome = Applied`.

The result carries: `SdkSitId`, `Outcome`, rows restored, rows written by the walk, end
balance before and after, the before-image, and any mismatch/guard details.

`RevertAsync` writes each before-image row's captured columns back verbatim, only where
the live row's `Version` equals the version the repair left (read back in step 6 and
recorded in the before-image as `RepairedVersion`); otherwise
that row is reported as `Changed` and skipped. Each reverted row gets `Version + 1`, a
version snapshot, one transaction for all rows, dry run by default.

CSV reading/writing for the three file shapes lives next to it (`FlexChainRepairCsv`),
invariant culture, so the console holds no logic.

## 5. Console: `tools/FlexChainRepair`

```
FlexChainRepair repair --connection "<cs>" --restore restore.csv --expected expected.csv
                       --out <dir> [--apply --confirm-database <db name>] [--worker <sdkSitId>]
FlexChainRepair revert --connection "<cs>" --before-image <file> --out <dir>
                       [--apply --confirm-database <db name>]
```

- Without `--apply` it is a dry run. `--apply` requires `--confirm-database` equal to the
  connection's database name, so a pasted command cannot hit the wrong tenant.
- Workers: every non-removed, non-resigned AssignedSite's `SiteId`, optionally narrowed by
  `--worker`. A manifest line for a worker outside that set is an error before anything runs.
- One fresh context per worker. Writes to `--out`: `report.csv` (one line per worker),
  `before-image.csv` (apply only, appended per committed worker), `mismatches.csv`.
- Exit code 0 only when every worker is `DryRunOk` / `Applied`.

## 6. Run procedure (operational, not code)

1. Fresh read-only dump; build `restore.csv` + `expected.csv` offline; drop rows whose
   inputs changed since the incident; per-worker summary to the owner (terminal only).
2. Dry run against the tenant. Report to the owner. **No apply without the owner's go.**
3. Apply outside the 03 UTC nightly-walk hour, at a quiet time.
4. Verify: re-dump; every worker matches expected within 0.01 h; stored-break scan finds 0
   breaks on the tenant; restored/written counts equal the dry run.
5. The next nightly walk re-carries the last 7 days (heals a punch during the window).
6. Hand the owner a per-worker old/new/delta table with a one-line reason each.

## 7. Tests (Microting.TimePlanningBase.Tests, Testcontainers MariaDB in CI)

`FlexChainRepairTests`:
- restore + walk: a recompute row's hours are restored, later balances follow, `Applied`.
- dry run leaves every row, version and history row untouched, reports the same counts.
- guard: `ExpectVersion` stale → `GuardFailed`, nothing written.
- guard: row of another worker / removed row → `GuardFailed`.
- open-shift line restores 0 and the chain follows.
- no restore lines: a stored break is healed by the walk alone.
- expected mismatch → `Mismatch`, rolled back.
- a row with no expected entry is still checked for continuity.
- reconciled day → `Locked`, nothing written.
- version history: one snapshot per restored row and per walked row.
- revert: apply then revert restores every captured column; a row changed after the repair
  is reported `Changed` and left alone.

`FlexChainRepairCsvTests`: round-trip of the three shapes; decimal comma culture does not
change parsing.

## 8. Non-goals

- Repairing other tenants (sub-project 4 — the tool is written to be reused).
- Any change to `RunForwardAsync`, `CarryChain`, the read path or the service.
- Producing the manifest or the expected balances (offline, outside the repository).
