# FlexChainRepair

A console tool for repairing one tenant's flex chain — restoring a set of
`PlanRegistration` rows to their last pre-incident `NettoHours` and walking
the affected workers' chains forward with the product's own
`FlexChainRecompute.RunForwardAsync`, so `Flex` / `SumFlexStart` /
`SumFlexEnd` follow correctly again. See
`docs/superpowers/specs/2026-09-27-flex-chain-repair-tool-design.md` for the
full design; this file covers only how to run the tool.

All logic lives in `Microting.TimePlanningBase.Infrastructure.Helpers`
(`FlexChainRepair`, `FlexChainRepairCsv`). This project is a thin console
wrapper and is intentionally **not** part of `Microting.TimePlanningBase.sln`.

**Dry run is the default.** Neither command writes anything unless you pass
`--apply`, and `--apply` itself requires `--confirm-database` to equal the
connection string's database name, so a pasted command can never silently
hit the wrong tenant.

## Commands

### `repair`

```
dotnet run --project tools/FlexChainRepair -- repair \
  --connection "Server=127.0.0.1;Port=3306;Database=<tenant-db>;user=<user>;password=<password>;" \
  --restore restore.csv \
  --expected expected.csv \
  --out ./out \
  [--worker <sdkSitId>] \
  [--apply --confirm-database <tenant-db>]
```

- `--restore` / `--expected` are the two manifest CSVs produced offline by the
  independent replay (see spec §3). They are never committed to this
  repository.
- Workers walked: every `AssignedSite` on the tenant that is not removed and
  not resigned, distinct `SiteId`, ascending — narrowed to one worker with
  `--worker`. Before anything runs: `--worker <sdkSitId>` naming a worker that
  isn't in that full active set is refused (a typo, or a resigned/removed
  worker); a `restore.csv` or `expected.csv` line naming a worker outside the
  full active set is refused the same way. All three checks run against the
  *full* active set, not narrowed by `--worker` — so the full-tenant
  `restore.csv` / `expected.csv` can be pointed at unmodified for a
  one-worker canary run:

  ```
  dotnet run --project tools/FlexChainRepair -- repair \
    --connection "Server=127.0.0.1;Port=3306;Database=<tenant-db>;user=<user>;password=<password>;" \
    --restore restore.csv \
    --expected expected.csv \
    --out ./out-canary \
    --worker <sdkSitId> \
    --apply --confirm-database <tenant-db>
  ```

  Only `<sdkSitId>` is walked and written to `./out-canary`; every other
  worker's manifest lines are simply not used this run.
- Also before anything runs, one read-only lookup maps every `expected.csv`
  and `restore.csv` `Id` to its row's actual `SdkSitId`. An `expected.csv`
  line whose `Id` does not exist or belongs to another worker, or a
  `restore.csv` line whose row belongs to another worker, is refused (up to 20
  listed). Every worker that will be walked must have at least one
  `expected.csv` line (`no expected balances for worker <id>`) — otherwise
  it would be reported OK with only continuity checked.
- `--out` is created if missing. The tool refuses to start if it already
  holds `report.csv`, `before-image.csv` or any `revert-*.csv` from an
  earlier run — always point `--out` at a fresh directory per run.
- Writes to `--out`: `report.csv` (one line per worker), `mismatches.csv`,
  `guard-failed.csv`, and — for every worker actually applied —
  `before-image.csv`, which `revert` reads back later. `before-image.csv` is
  flushed to disk immediately after each applied worker's before-image is
  written, before that worker's other output rows — it is the only way to
  undo a committed repair, so it must survive a crash right after.
- **With `--apply`, the first worker whose call throws stops the run right
  there** (after flushing every output file for that worker) instead of
  moving on to the next worker — an exception means this worker's true state
  isn't known for certain, so nothing after it should be touched. Two
  distinct stderr messages cover this:
  - `worker <id>: state UNKNOWN — the commit may have landed; before-image.csv
    covers only the workers reported Applied` — the call itself threw, so
    even the outcome is unknown (a `before-image.csv` from this run does not
    include this worker no matter what actually happened in the database).
  - `worker <id>: APPLIED but its before-image is NOT durable` — the repair
    call returned `Applied` (so it genuinely committed), but writing or
    flushing that worker's before-image afterward failed; `report.csv` still
    gets an `Applied` row for it if that write is still possible, but this
    worker has no working `revert` path until its before-image is recovered
    some other way.

  A dry run keeps going past a per-worker exception, exactly as before —
  only `--apply` stops the run, since only `--apply` can leave a worker in an
  uncertain database state.
- Exit code 0 only when every worker ends `DryRunOk` or `Applied` — see
  **Exit codes** below.

### `revert`

```
dotnet run --project tools/FlexChainRepair -- revert \
  --connection "Server=127.0.0.1;Port=3306;Database=<tenant-db>;user=<user>;password=<password>;" \
  --before-image ./out/before-image.csv \
  --out ./revert-out \
  [--apply --confirm-database <tenant-db>]
```

- Reads a `before-image.csv` written by a previous `repair --apply` run,
  groups it by worker, and reverts one worker per call (a fresh database
  context per worker).
- Writes to `--out`: `revert-changed.csv` and `revert-missing.csv` (rows a
  previous edit or a missing row kept from being reverted), and
  `revert-breaks.csv` — any chain break found by re-checking a reverted
  worker's live rows afterwards. Rows created after the original repair are
  never in the before-image, so a revert can legitimately leave a break;
  this file makes that visible instead of hiding it.
- Same stop-on-error rule as `repair`: with `--apply`, the first worker whose
  `RevertAsync` call throws stops the run there (`worker <id>: state UNKNOWN
  — the commit may have landed; ...`), after flushing every output file for
  that worker. A dry run keeps going past errors.
- Exit code 0 only when nothing was changed, missing or errored — see
  **Exit codes** below.

## Exit codes

| Code | Meaning |
|---|---|
| `0` | Every worker ended `DryRunOk` / `Applied` (`repair`), or nothing was changed/missing/errored (`revert`). |
| `1` | A worker or a setup step ran into a real problem: `GuardFailed` / `Mismatch` / `Locked`, a worker's call threw (`state UNKNOWN`, or — `repair` only — `APPLIED but its before-image is NOT durable`), a database query failed, a manifest CSV's *content* was malformed, a duplicate `Id` in `expected.csv`, or (`revert`) any changed/missing/errored row. |
| `2` | The invocation itself was wrong: an unknown or missing flag, `--apply` without a matching `--confirm-database`, `--out` already holding a previous run's files, an unreadable input path, `--worker` naming a worker that isn't active, a `restore.csv` / `expected.csv` line naming a worker outside the tenant's active set, an `expected.csv` `Id` that does not exist or belongs to another worker, a `restore.csv` `Id` that belongs to another worker, a walked worker with no expected balances, or a live row of a walked worker that existed at dump time (Id at or below the file's highest expected `Id`) but has no expected balance (a truncated `expected.csv`). A flag that takes a value followed by another `--flag` (e.g. `--out --apply`) is also exit 2, never a dry run. |

Every exit-2 check — flags, `--confirm-database`, `--worker`, the
restore/expected outsider checks, the Id→SdkSitId ownership check and the dump-coverage check — runs before `--out` is even created, so
nothing is ever written to disk (not `--out` itself, let alone a report file
or a database row) before the invocation is known to be valid.

## Run procedure

Operational detail lives in spec §6; in short:

1. Build `restore.csv` + `expected.csv` offline from a fresh read-only dump.
2. `repair` as a dry run against the tenant. Report to the owner. **No
   `--apply` without the owner's go.**
3. Take a fresh read-only snapshot of the tenant's `PlanRegistrations`
   immediately before `--apply` (e.g.
   `mysqldump --single-transaction -h <host> -P <port> -u <user> -p <tenant-db> PlanRegistrations > <snapshot>.sql`);
   it is the recovery record for the moment between a worker's commit and its
   before-image reaching disk.
4. `repair --apply --confirm-database <tenant-db>` outside the nightly walk
   window.
5. Re-dump and verify every worker matches expected balances.
6. Keep `before-image.csv` and the pre-apply snapshot until the tenant is
   verified stable — `before-image.csv` is the only input `revert` takes.
