# ShiftIdRepair

A console tool that puts back corrupted `PlanRegistrations.Start1Id` /
`Stop1Id` values (and, where the manifest says so, `Start1StartedAt` /
`Stop1StoppedAt`) on one tenant. A now-fixed read-path bug divided the two ids
by 6, and on five-minute cross-midnight rows also overwrote `Stop1StoppedAt`.
The correct values come from a per-tenant manifest generated offline from
version history; this tool only applies that manifest, safely.

All logic lives in `Microting.TimePlanningBase.Infrastructure.Helpers`
(`ShiftIdRepair`, `ShiftIdRepairCsv`, `ShiftIdRepairTypes`). This project is a
thin console wrapper and is intentionally **not** part of
`Microting.TimePlanningBase.sln`.

**Dry run is the default.** Nothing is written unless you pass `--apply`, and
`--apply` requires `--confirm-database` to equal the connection string's
database name, so a pasted command can never silently hit the wrong tenant.

## Usage

```
dotnet run --project tools/ShiftIdRepair -- \
  --connection "Server=127.0.0.1;Port=3306;Database=<tenant-db>;user=<user>;password=<password>;" \
  --manifest manifest.csv \
  --out ./out \
  [--apply --confirm-database <tenant-db>]
```

`--confirm-database` is checked whenever it is given, so the dry run can use
the exact apply command line minus `--apply`.

## Manifest

Plain comma-separated values, no quoting (so `Reason` must not contain a
comma), header row exactly:

```
Id,SdkSitId,Date,ExpectVersion,ExpectStart1Id,ExpectStop1Id,NewStart1Id,NewStop1Id,NewStart1StartedAt,NewStop1StoppedAt,Reason
```

- `Date`: the row's day, `yyyy-MM-dd` (as the generator writes it) or a full
  `yyyy-MM-dd HH:mm:ss` timestamp. Required.
- `NewStart1StartedAt`, `NewStop1StoppedAt`: `yyyy-MM-dd HH:mm:ss` (a
  fractional `.fffffff` part is also accepted; a bare date is not). An empty
  `New*At` leaves that stamp unchanged.
- All values are parsed with the invariant culture, whatever the machine's locale.
- `Reason` is free text, e.g. `one-minute-div`, `five-minute-cross-midnight`.
- A wrong header, a wrong column count, a non-integer id, a bad timestamp or a
  duplicate `Id` is refused (exit 2) with the 1-based line number.
- Manifests are tenant data: never commit them to this repository.

## What happens per line

The manifest is processed in chunks of 200 lines; each chunk is one
transaction on a fresh database context. Inside it every manifest row is
locked (`SELECT ... FOR UPDATE`) and each line gets one outcome:

| Outcome | When | Written? |
|---|---|---|
| `Missing` | no row with that `Id`, or its `SdkSitId` or `Date` differ from the manifest | no |
| `Locked` | the row is reconciled, or its date is on/before the worker's reconciled-day lock | no |
| `Changed` | the row's `Version`, `Start1Id` or `Stop1Id` differ from `Expect*` (someone edited it since the manifest was built) | no |
| `Repaired` | otherwise: the new ids (and given stamps) are set and saved through the product's own `PnBase.Update`, so `Version` moves on and a `PlanRegistrationVersions` row is written like for any other edit | yes |

Nothing else on the row is touched — `NettoHours`, flex and the rest stay as
they are. A line that is not repaired never blocks the other lines. A dry run
does all of the above and then rolls back, so its report shows what `--apply`
would do.

## Output

`--out` is created if missing; the tool refuses (exit 2) to run against a
directory that is not empty — use a fresh `--out` per run.

- `report.csv` — `Id,SdkSitId,Outcome` per manifest line. Besides the four
  outcomes, `Error` marks the lines of a chunk whose call threw and `NotRun`
  the lines after an `--apply` run stopped.
- `before-image.csv` — `--apply` only. One row per repaired row:
  `Id,SdkSitId,Date,Version,RepairedVersion,Start1Id,Stop1Id,Start1StartedAt,Stop1StoppedAt,RepairedStart1Id,RepairedStop1Id`
  (values before the repair, plus the Version and ids the repair left). Each
  chunk's rows are written and flushed to disk before that chunk's report rows.
- stdout — the mode, the line count, and a count per outcome.

**With `--apply`, the first chunk whose call throws stops the run** and the
rest is reported `NotRun`. stderr then says either
`chunk <n> (Ids <first>..<last>): state UNKNOWN — the commit may have landed; ...`
(the call threw, so that chunk's true state is unknown; run a dry run with the
same manifest to see which lines are still unrepaired — a landed line now
reports `Changed`), or `... APPLIED but its before-image is NOT durable`
(the chunk committed but its undo record could not be written). A dry run
keeps going past a failed chunk.

## Undo

The repair is undone by running the tool again with a **reversed manifest**
built from `before-image.csv`: expect the repaired state, write the old values
back. Column mapping (before-image → manifest):

| Manifest column | From before-image |
|---|---|
| `Id`, `SdkSitId`, `Date` | same |
| `ExpectVersion` | `RepairedVersion` |
| `ExpectStart1Id`, `ExpectStop1Id` | `RepairedStart1Id`, `RepairedStop1Id` |
| `NewStart1Id`, `NewStop1Id` | `Start1Id`, `Stop1Id` |
| `NewStart1StartedAt`, `NewStop1StoppedAt` | `Start1StartedAt`, `Stop1StoppedAt` |
| `Reason` | `revert` |

```
awk -F, -v OFS=, 'NR==1 { print "Id,SdkSitId,Date,ExpectVersion,ExpectStart1Id,ExpectStop1Id,NewStart1Id,NewStop1Id,NewStart1StartedAt,NewStop1StoppedAt,Reason"; next }
  NF { print $1,$2,$3,$5,$10,$11,$6,$7,$8,$9,"revert" }' out/before-image.csv > reverse.csv
```

(`ShiftIdRepair.ReverseLine` + `ShiftIdRepairCsv.WriteManifest` do the same in
code.) Then dry-run and apply `reverse.csv` like any manifest, into a fresh
`--out`. A row edited after the repair reports `Changed` and is left alone. A
stamp that was empty before the repair cannot be put back to empty this way (an
empty stamp means "leave unchanged").

## Exit codes

| Code | Meaning |
|---|---|
| `0` | Every line `Repaired` (applied, or would be in a dry run). |
| `1` | Any line `Changed` / `Missing` / `Locked`, a chunk threw (`Error` / `NotRun`), or a before-image write failed. |
| `2` | The invocation was wrong: unknown, missing or repeated flags, a value flag followed by another `--flag` (e.g. `--out --apply` is never a dry run), `--apply` without `--confirm-database`, a `--confirm-database` that does not match the connection's database, an unreadable or invalid manifest, or a non-empty `--out`. Nothing is written to disk or the database. |

## Run procedure

1. Confirm the read-path fix (plugin PR #1732, removal of `Start1Id /= 5 + 1`)
   is deployed on the tenant before `--apply` — otherwise the next planning
   read divides the restored ids again.
2. Generate the manifest offline from a fresh read-only dump.
3. Dry run against the tenant; review `report.csv`. **No `--apply` without the
   owner's go.**
4. Take a fresh snapshot of `PlanRegistrations` right before applying (e.g.
   `mysqldump --single-transaction -h <host> -P <port> -u <user> -p <tenant-db> PlanRegistrations > <snapshot>.sql`).
5. `--apply --confirm-database <tenant-db>` into a fresh `--out`.
6. Keep `before-image.csv` and the snapshot until the tenant is verified.
