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

using System.Collections.Generic;

namespace Microting.TimePlanningBase.Infrastructure.Data.DataMigrations;

/// <summary>
/// Data fix for the two "GLS-A / 3F - Udenlandske praktikanter Landbrug" presets
/// (Staldarbejde, Andet arbejde), rewriting existing customer rows to the § 50
/// tiers the catalogue was corrected to on 2026-08-07.
///
/// Presets are copy-at-create-time snapshots: choosing one writes its values into
/// PayRuleSet/PayDayRule/PayTierRule rows and nothing re-reads the catalogue
/// afterwards. The catalogue correction therefore never reached rule sets that
/// already existed — those rows still encode e.g. Staldarbejde SATURDAY as
/// [21600 SAT_NORMAL, null SAT_ANIMAL_AFTERNOON], a mirror of the clock bands
/// with no overtime steps, so a praktikant working a long Saturday is credited
/// zero overtime minutes. This SQL brings such rows to the corrected shape, after
/// which the engine's normal-time/overtime split applies to them with no further
/// code change.
///
/// Scope and behaviour:
/// - Matches PayRuleSet.Name with the trailing validity period normalised away
///   (rows may carry "2024-2026" or "2026-2029"; hyphen, en dash and em dash are
///   accepted), mirroring the plugin's PayRuleSetLock.NormalizePresetName. No
///   other preset is touched.
/// - Idempotent: a tier already holding its target values is not written (no
///   Version bump, no UpdatedAt change), so a second run is a no-op.
/// - Updates existing tiers in place and inserts missing ones; never deletes,
///   so Order stays contiguous from 1.
/// - Skips rows with WorkflowState 'removed' and does not resurrect them.
/// - Maintains the PayTierRuleVersions audit table the same way PnBase.Update()
///   does: every write bumps Version and snapshots the new state as a version row.
/// - PayDayTypeRule time bands were not changed by the catalogue correction and
///   are left alone. All five day rules (WEEKDAY, SATURDAY, SUNDAY, HOLIDAY,
///   GRUNDLOVSDAG) have shipped with both presets since their introduction and
///   the presets are locked against customer edits, so day rules are only ever
///   corrected, never created.
///
/// FROZEN: this class is executed by the CorrectPraktikantSection50Tiers EF
/// migration. Do not edit it for later corrections — a historical migration must
/// keep doing exactly what it did; write a new data migration instead.
/// </summary>
public static class PraktikantSection50TierCorrection
{
    private sealed record TargetTier(int Order, int? UpToSeconds, string PayCode);

    private sealed record TargetDayRule(string DayCode, TargetTier[] Tiers);

    private sealed record TargetPreset(string NormalizedName, TargetDayRule[] DayRules);

    private static readonly TargetTier[] NormalProgression =
    [
        new TargetTier(1, 26640, "NORMAL"),
        new TargetTier(2, 33840, "OVERTIME_50"),
        new TargetTier(3, null, "OVERTIME_80")
    ];

    private static readonly TargetPreset[] Targets =
    [
        new TargetPreset(
            "GLS-A / 3F - Udenlandske praktikanter Landbrug Staldarbejde",
            [
                new TargetDayRule("WEEKDAY", NormalProgression),
                new TargetDayRule("SATURDAY",
                [
                    new TargetTier(1, 26640, "SAT_NORMAL"),
                    new TargetTier(2, 33840, "OVERTIME_50"),
                    new TargetTier(3, null, "OVERTIME_80")
                ]),
                new TargetDayRule("SUNDAY",
                [
                    new TargetTier(1, 26640, "ANIMAL_SUN_HOLIDAY"),
                    new TargetTier(2, 33840, "OVERTIME_50"),
                    new TargetTier(3, null, "OVERTIME_80")
                ]),
                new TargetDayRule("HOLIDAY",
                [
                    new TargetTier(1, 26640, "ANIMAL_SUN_HOLIDAY"),
                    new TargetTier(2, 33840, "OVERTIME_50"),
                    new TargetTier(3, null, "OVERTIME_80")
                ]),
                new TargetDayRule("GRUNDLOVSDAG", NormalProgression)
            ]),
        new TargetPreset(
            "GLS-A / 3F - Udenlandske praktikanter Landbrug Andet arbejde",
            [
                new TargetDayRule("WEEKDAY", NormalProgression),
                new TargetDayRule("SATURDAY", NormalProgression),
                new TargetDayRule("SUNDAY",
                [
                    new TargetTier(1, 7200, "OVERTIME_50"),
                    new TargetTier(2, null, "OVERTIME_80")
                ]),
                new TargetDayRule("HOLIDAY",
                [
                    new TargetTier(1, 7200, "OVERTIME_50"),
                    new TargetTier(2, null, "OVERTIME_80")
                ]),
                new TargetDayRule("GRUNDLOVSDAG", NormalProgression)
            ])
    ];

    /// <summary>
    /// The correction as ordered SQL statements. The EF migration executes them
    /// via migrationBuilder.Sql; tests execute them via ExecuteSqlRaw against a
    /// seeded database.
    /// </summary>
    public static IReadOnlyList<string> SqlStatements { get; } = BuildStatements();

    private static List<string> BuildStatements()
    {
        var statements = new List<string>();

        foreach (var preset in Targets)
        {
            var nameRegex = BuildNameRegex(preset.NormalizedName);

            foreach (var dayRule in preset.DayRules)
            {
                foreach (var tier in dayRule.Tiers)
                {
                    statements.Add(BuildUpdateStatement(nameRegex, dayRule.DayCode, tier));
                    statements.Add(BuildInsertStatement(nameRegex, dayRule.DayCode, tier));
                }
            }

            statements.Add(BuildVersionBackfillStatement(nameRegex));
        }

        return statements;
    }

    /// <summary>
    /// Anchored match for the preset name with an optional trailing validity
    /// period — the SQL twin of the plugin's PayRuleSetLock.NormalizePresetName
    /// (which strips @"\s+\d{4}\s*[-–—]\s*\d{4}$" after trimming). The year is
    /// spelled as four digit classes instead of "[0-9]{4}" because these
    /// statements are also executed through ExecuteSqlRaw, which applies
    /// string.Format semantics and would read "{4}" as a placeholder.
    /// The name is embedded verbatim, so it must contain no regex
    /// metacharacters — true for both praktikant preset names.
    /// </summary>
    private static string BuildNameRegex(string normalizedName)
    {
        const string year = "[0-9][0-9][0-9][0-9]";
        return "^[[:space:]]*" + normalizedName +
               $"([[:space:]]+{year}[[:space:]]*(-|–|—)[[:space:]]*{year})?[[:space:]]*$";
    }

    private static string SqlLiteral(int? value)
    {
        return value?.ToString() ?? "NULL";
    }

    private static string BuildUpdateStatement(string nameRegex, string dayCode, TargetTier tier)
    {
        return $"""
                UPDATE `PayTierRules` t
                JOIN `PayDayRules` d ON d.`Id` = t.`PayDayRuleId`
                JOIN `PayRuleSets` s ON s.`Id` = d.`PayRuleSetId`
                SET t.`UpToSeconds` = {SqlLiteral(tier.UpToSeconds)},
                    t.`PayCode` = '{tier.PayCode}',
                    t.`Version` = t.`Version` + 1,
                    t.`UpdatedAt` = UTC_TIMESTAMP(6)
                WHERE s.`Name` REGEXP '{nameRegex}'
                  AND NOT (s.`WorkflowState` <=> 'removed')
                  AND NOT (d.`WorkflowState` <=> 'removed')
                  AND NOT (t.`WorkflowState` <=> 'removed')
                  AND d.`DayCode` = '{dayCode}'
                  AND t.`Order` = {tier.Order}
                  AND NOT (t.`UpToSeconds` <=> {SqlLiteral(tier.UpToSeconds)}
                           AND t.`PayCode` <=> '{tier.PayCode}');
                """;
    }

    private static string BuildInsertStatement(string nameRegex, string dayCode, TargetTier tier)
    {
        return $"""
                INSERT INTO `PayTierRules`
                    (`PayDayRuleId`, `UpToSeconds`, `PayCode`, `PayrollCode`, `Order`,
                     `CreatedAt`, `UpdatedAt`, `Version`, `WorkflowState`,
                     `CreatedByUserId`, `UpdatedByUserId`)
                SELECT d.`Id`, {SqlLiteral(tier.UpToSeconds)}, '{tier.PayCode}', NULL, {tier.Order},
                       UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'created', 0, 0
                FROM `PayDayRules` d
                JOIN `PayRuleSets` s ON s.`Id` = d.`PayRuleSetId`
                WHERE s.`Name` REGEXP '{nameRegex}'
                  AND NOT (s.`WorkflowState` <=> 'removed')
                  AND NOT (d.`WorkflowState` <=> 'removed')
                  AND d.`DayCode` = '{dayCode}'
                  AND NOT EXISTS (SELECT 1 FROM `PayTierRules` x
                                  WHERE x.`PayDayRuleId` = d.`Id`
                                    AND x.`Order` = {tier.Order}
                                    AND NOT (x.`WorkflowState` <=> 'removed'));
                """;
    }

    /// <summary>
    /// Snapshots every live tier of the matched (live) presets whose current
    /// Version has no PayTierRuleVersions row — the rows the statements above
    /// updated (Version was bumped past the last snapshot) or inserted
    /// (Version 1, no snapshot yet), plus any pre-existing tier anomalously
    /// missing a snapshot for its current Version. Rows left untouched by a
    /// correctly versioned history already have a snapshot for their current
    /// Version, so this writes nothing for them and stays idempotent.
    /// </summary>
    private static string BuildVersionBackfillStatement(string nameRegex)
    {
        return $"""
                INSERT INTO `PayTierRuleVersions`
                    (`PayTierRuleId`, `PayDayRuleId`, `UpToSeconds`, `PayCode`, `PayrollCode`, `Order`,
                     `CreatedAt`, `UpdatedAt`, `Version`, `WorkflowState`,
                     `CreatedByUserId`, `UpdatedByUserId`)
                SELECT t.`Id`, t.`PayDayRuleId`, t.`UpToSeconds`, t.`PayCode`, t.`PayrollCode`, t.`Order`,
                       t.`CreatedAt`, t.`UpdatedAt`, t.`Version`, t.`WorkflowState`,
                       t.`CreatedByUserId`, t.`UpdatedByUserId`
                FROM `PayTierRules` t
                JOIN `PayDayRules` d ON d.`Id` = t.`PayDayRuleId`
                JOIN `PayRuleSets` s ON s.`Id` = d.`PayRuleSetId`
                WHERE s.`Name` REGEXP '{nameRegex}'
                  AND NOT (s.`WorkflowState` <=> 'removed')
                  AND NOT (d.`WorkflowState` <=> 'removed')
                  AND NOT (t.`WorkflowState` <=> 'removed')
                  AND NOT EXISTS (SELECT 1 FROM `PayTierRuleVersions` v
                                  WHERE v.`PayTierRuleId` = t.`Id`
                                    AND v.`Version` = t.`Version`);
                """;
    }
}
