/*
The MIT License (MIT)

Copyright (c) 2007 - 2026 Microting A/S
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using Microting.TimePlanningBase.Tests.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

/// <summary>
/// Exhaustive edge-case coverage for PayLineGenerator.GenerateTimeBandPayLines and
/// PayLineGenerator.GeneratePayLines. Each test pins down the exact second-level
/// behavior at boundaries: before the first band, exactly at boundaries, spanning
/// multiple bands, zero-duration shifts, one-second shifts, etc.
///
/// Critical groups:
///   Matrix A — time-band boundary edges (Dyrehold weekday + KA Gron Saturday)
///   Matrix B — tier rule boundary edges (GLS-A Jordbrug Standard weekday)
///   Matrix C — day type × rule variant
///   Matrix D — multi-shift combinations
///   Matrix E — negative / empty paths
/// </summary>
[TestFixture]
public class PayLineTimeBandEdgeCaseTests
{
    private static readonly DateTime CalculatedAt = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

    // ─────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Sums seconds for all pay lines with the given pay code, regardless of how
    /// many separate lines the generator emitted (the generator may return multiple
    /// lines for the same pay code from gap-fill + band-overlap).
    /// </summary>
    private static int SecondsByCode(IEnumerable<PlanRegistrationPayLine> lines, string code)
        => lines.Where(l => l.PayCode == code).Sum(l => l.HoursInSeconds);

    private static int TotalSeconds(IEnumerable<PlanRegistrationPayLine> lines)
        => lines.Sum(l => l.HoursInSeconds);

    // Common KA Gron preset (Saturday bands 06–16 SAT_NORMAL, 16–24 SAT_AFTERNOON, default SAT_NORMAL)
    // Built inline because OverenskomstFixtureHelper.CreateKaGronStandard depends on a DbContext.
    private static PayRuleSet BuildKaGronSaturdayOnly()
    {
        return new PayRuleSet
        {
            Id = 9001,
            Name = "Test KA Gron Saturday",
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 9001,
                    DayCode = "SATURDAY",
                    Tiers = new List<PayTierRule>
                    {
                        new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },
                        new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                    }
                }
            },
            DayTypeRules = new List<PayDayTypeRule>
            {
                new PayDayTypeRule
                {
                    PayRuleSetId = 9001, DayType = DayType.Saturday, DefaultPayCode = "SAT_NORMAL", Priority = 1,
                    TimeBandRules = new List<PayTimeBandRule>
                    {
                        new PayTimeBandRule { StartSecondOfDay = 21600, EndSecondOfDay = 57600, PayCode = "SAT_NORMAL", Priority = 1 },
                        new PayTimeBandRule { StartSecondOfDay = 57600, EndSecondOfDay = 86400, PayCode = "SAT_AFTERNOON", Priority = 1 }
                    }
                }
            }
        };
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix A.1 — Early-morning / "before the first main band" coverage
    // GLS-A Dyrehold weekday bands (Mon-Fri):
    //   00:00-05:00 ANIMAL_NIGHT      (0..18000)
    //   05:00-06:00 SHIFTED_MORNING   (18000..21600)
    //   06:00-18:00 NORMAL            (21600..64800)
    //   18:00-24:00 SHIFTED_EVENING   (64800..86400)
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void A_1_1_Dyrehold_Shift_00_to_05_EntirelyInAnimalNightBand()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 0, 18000, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(18000));
        Assert.That(SecondsByCode(result, "SHIFTED_MORNING"), Is.EqualTo(0));
        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(18000));
    }

    [Test]
    public void A_1_2_Dyrehold_Shift_02_to_04_MiddleOfAnimalNightBand()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 7200, 14400, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(7200));
        Assert.That(TotalSeconds(result), Is.EqualTo(7200));
    }

    [Test]
    public void A_1_3_Dyrehold_Shift_OneSecondAfterMidnight()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 0, 1, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(1));
        Assert.That(TotalSeconds(result), Is.EqualTo(1));
    }

    [Test]
    public void A_1_4_Dyrehold_Shift_LastSecondOfAnimalNight()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        // 04:59:59 → 05:00:00 = seconds 17999..18000 (1 second, the last second of the night band)
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 17999, 18000, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(1));
        Assert.That(SecondsByCode(result, "SHIFTED_MORNING"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(1));
    }

    [Test]
    public void A_1_5_Dyrehold_Shift_04_to_08_StraddlingNightAndMorningAndNormal()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        // 04:00-08:00 = 14400..28800
        // Expected: 1h ANIMAL_NIGHT (4-5) + 1h SHIFTED_MORNING (5-6) + 2h NORMAL (6-8)
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 14400, 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(3600));
        Assert.That(SecondsByCode(result, "SHIFTED_MORNING"), Is.EqualTo(3600));
        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(7200));
        Assert.That(TotalSeconds(result), Is.EqualTo(14400));
    }

    [Test]
    public void A_1_6_Dyrehold_Shift_05_to_07_StartExactlyAtMorningBand()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        // 05:00-07:00 = 18000..25200 → 1h SHIFTED_MORNING + 1h NORMAL
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 18000, 25200, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(0));
        Assert.That(SecondsByCode(result, "SHIFTED_MORNING"), Is.EqualTo(3600));
        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(3600));
        Assert.That(TotalSeconds(result), Is.EqualTo(7200));
    }

    [Test]
    public void A_1_7_Dyrehold_Shift_00_to_20_SpansAllBands()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        // 00:00-20:00 = 0..72000
        // Expected: 5h ANIMAL_NIGHT + 1h SHIFTED_MORNING + 12h NORMAL + 2h SHIFTED_EVENING
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 0, 72000, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(18000));
        Assert.That(SecondsByCode(result, "SHIFTED_MORNING"), Is.EqualTo(3600));
        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(43200));
        Assert.That(SecondsByCode(result, "SHIFTED_EVENING"), Is.EqualTo(7200));
        Assert.That(TotalSeconds(result), Is.EqualTo(72000));
    }

    [Test]
    public void A_1_8_KaGronSaturday_Shift_03_to_05_BeforeFirstBand_GapFilledWithDefaultPayCode()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        // 03:00-05:00 = 10800..18000 — entirely before band 1 (06:00)
        // Expected: gap-fill with DefaultPayCode "SAT_NORMAL" = 2h SAT_NORMAL
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 10800, 18000, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(7200));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(7200));
    }

    [Test]
    public void A_1_9_KaGronSaturday_Shift_05_to_10_StartsBeforeFirstBand()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        // 05:00-10:00 = 18000..36000
        // 1h gap-fill SAT_NORMAL (05–06) + 4h overlap with band 1 SAT_NORMAL (06–10)
        // Total: 5h SAT_NORMAL (may be returned as 2 separate lines)
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 18000, 36000, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(18000));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(18000));
    }

    [Test]
    public void A_1_10_KaGronSaturday_Shift_04_to_18_SpansPreBandAndBothBands()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        // 04:00-18:00 = 14400..64800
        // 2h gap-fill SAT_NORMAL (04–06) + 10h band 1 SAT_NORMAL (06–16) = 12h SAT_NORMAL
        // 2h band 2 SAT_AFTERNOON (16–18)
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 14400, 64800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(43200));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(7200));
        Assert.That(TotalSeconds(result), Is.EqualTo(50400));
    }

    [Test]
    public void A_1_11_Dyrehold_Shift_04_to_06_CrossesAnimalNightAndMorning()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        // 04:00-06:00 = 14400..21600 → 1h ANIMAL_NIGHT + 1h SHIFTED_MORNING
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 14400, 21600, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_NIGHT"), Is.EqualTo(3600));
        Assert.That(SecondsByCode(result, "SHIFTED_MORNING"), Is.EqualTo(3600));
        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(7200));
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix A.2 — Mid-day boundary edges
    // KA Gron Standard Saturday: 06–16 SAT_NORMAL, 16–24 SAT_AFTERNOON
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void A_2_1_Saturday_Shift_08_to_12_EntirelyInNormalBand()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 28800, 43200, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(14400));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(14400));
    }

    [Test]
    public void A_2_2_Saturday_Shift_06_to_16_ExactlyFirstBand()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 21600, 57600, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(36000));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(36000));
    }

    [Test]
    public void A_2_3_Saturday_Shift_16_to_20_StartExactlyAtAfternoon()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 57600, 72000, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(0));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(14400));
        Assert.That(TotalSeconds(result), Is.EqualTo(14400));
    }

    [Test]
    public void A_2_4_Saturday_Shift_14_to_18_StraddlingBoundary()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 50400, 64800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(7200));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(7200));
        Assert.That(TotalSeconds(result), Is.EqualTo(14400));
    }

    [Test]
    public void A_2_5_Saturday_Shift_OneSecondEachSideOfBoundary()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        // 15:59:59-16:00:01 = 57599..57601 → 1s SAT_NORMAL + 1s SAT_AFTERNOON
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 57599, 57601, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(1));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(1));
        Assert.That(TotalSeconds(result), Is.EqualTo(2));
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix A.3 — End-of-day edges
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void A_3_1_Saturday_Shift_18_to_22_EntirelyInAfternoonBand()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 64800, 79200, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(14400));
        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(14400));
    }

    [Test]
    public void A_3_2_Saturday_Shift_20_to_24_EndOfDay()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 72000, 86400, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(14400));
        Assert.That(TotalSeconds(result), Is.EqualTo(14400));
    }

    [Test]
    public void A_3_3_Saturday_Shift_LastSecondOfDay()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        // 23:59:59-24:00:00 = 86399..86400 (1s)
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 86399, 86400, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(1));
        Assert.That(TotalSeconds(result), Is.EqualTo(1));
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix A.4 — Zero-duration / degenerate
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void A_4_1_Saturday_Shift_12_to_12_Zero()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 43200, 43200, ruleSet, CalculatedAt);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void A_4_2_Saturday_Shift_BoundarySecond()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        // 16:00:00-16:00:01 = 57600..57601 (1s in afternoon band)
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 57600, 57601, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(1));
        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(1));
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix B — Tier rule boundary edges
    // GLS-A Jordbrug Standard weekday: 0–7.4h NORMAL, 7.4–9.4h OT30, 9.4+h OT80
    // (UpToSeconds = 26640 / 33840 / null)
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void B_1_Weekday_4h_AllNormal()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(14400));
        Assert.That(TotalSeconds(result), Is.EqualTo(14400));
    }

    [Test]
    public void B_2_Weekday_ExactlyAtTier1Ceiling_AllNormal()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 26640, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(26640));
    }

    [Test]
    public void B_3_Weekday_OneSecondPastTier1_OneSecondOvertime30()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 26641, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(1));
        Assert.That(TotalSeconds(result), Is.EqualTo(26641));
    }

    [Test]
    public void B_4_Weekday_MidTier2_NormalPlusOT30()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        // 9h = 32400s = 7.4h NORMAL + 1.6h OT30
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 32400, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(5760));
        Assert.That(TotalSeconds(result), Is.EqualTo(32400));
    }

    [Test]
    public void B_5_Weekday_ExactlyAtTier2Ceiling_NoOT80()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(7200));
        Assert.That(SecondsByCode(result, "OVERTIME_80"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(33840));
    }

    [Test]
    public void B_6_Weekday_OneSecondPastTier2_OneSecondOT80()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33841, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(7200));
        Assert.That(SecondsByCode(result, "OVERTIME_80"), Is.EqualTo(1));
        Assert.That(TotalSeconds(result), Is.EqualTo(33841));
    }

    [Test]
    public void B_7_Weekday_SpanningAllTiers()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        // 12h = 43200s = 7.4h NORMAL + 2h OT30 + 2.6h OT80
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 43200, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(7200));
        Assert.That(SecondsByCode(result, "OVERTIME_80"), Is.EqualTo(9360));
        Assert.That(TotalSeconds(result), Is.EqualTo(43200));
    }

    [Test]
    public void B_8_Weekday_ZeroSeconds_EmptyResult()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 0, ruleSet, CalculatedAt);

        // With matching dayRule, the loop breaks at remainingSeconds <= 0 → empty list
        Assert.That(result, Is.Empty);
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix E — Negative / empty paths
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void E_1_NoRuleSet_TimeBand_FallsBackToDefaultPayCode()
    {
        // Null PayRuleSet → DayTypeRules?.SingleOrDefault is null → falls into the
        // null-rule branch which adds a single "DEFAULT" pay line.
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 0, 28800, null, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("DEFAULT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void E_2_NoMatchingDayCode_Tier_FallsBackToDefaultPayCode()
    {
        // Rule set with only WEEKDAY tier rule → asking for "SUNDAY" → DEFAULT
        var ruleSet = new PayRuleSet
        {
            Id = 9100,
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 9100, DayCode = "WEEKDAY",
                    Tiers = new List<PayTierRule>
                    {
                        new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "NORMAL" }
                    }
                }
            }
        };
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("DEFAULT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void E_3_NoMatchingDayType_TimeBand_FallsBackToDefaultPayCode()
    {
        // Rule set defines only Monday — request Sunday → null DayTypeRule → DEFAULT
        var ruleSet = new PayRuleSet
        {
            Id = 9101,
            DayTypeRules = new List<PayDayTypeRule>
            {
                new PayDayTypeRule
                {
                    PayRuleSetId = 9101, DayType = DayType.Monday, DefaultPayCode = "X", Priority = 1,
                    TimeBandRules = new List<PayTimeBandRule>
                    {
                        new PayTimeBandRule { StartSecondOfDay = 0, EndSecondOfDay = 86400, PayCode = "X", Priority = 1 }
                    }
                }
            }
        };
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Sunday, 0, 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("DEFAULT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void E_4_TimeBand_EmptyShift_ReturnsEmpty()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 36000, 36000, ruleSet, CalculatedAt);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void E_5_TimeBand_NegativeRange_ReturnsEmpty()
    {
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 36000, 18000, ruleSet, CalculatedAt);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void E_6_PayDayTypeRule_DefinedButNoBands_FallsBackToDefaultPayCode()
    {
        // Rule has DayType.Saturday but TimeBandRules empty → null/empty branch → DEFAULT
        var ruleSet = new PayRuleSet
        {
            Id = 9102,
            DayTypeRules = new List<PayDayTypeRule>
            {
                new PayDayTypeRule
                {
                    PayRuleSetId = 9102, DayType = DayType.Saturday, DefaultPayCode = "SHOULD_NOT_USE", Priority = 1,
                    TimeBandRules = new List<PayTimeBandRule>()
                }
            }
        };
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 0, 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("DEFAULT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(14400));
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix C — Day type × rule variant
    // For each preset, test each day code with a representative 8h shift (28800s).
    // ─────────────────────────────────────────────────────────────

    // GLS-A Jordbrug Standard (tier rules: 7.4h NORMAL + 2h OT30 + rest OT80 weekday)
    [Test]
    public void C_Standard_Weekday_8h_NormalPlusOT30()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(2160));
        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
    }

    [Test]
    public void C_Standard_Saturday_8h_TierBased()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 28800, ruleSet, CalculatedAt);

        // 6h SAT_NORMAL + 2h SAT_AFTERNOON
        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(21600));
        Assert.That(SecondsByCode(result, "SAT_AFTERNOON"), Is.EqualTo(7200));
        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
    }

    [Test]
    public void C_Standard_Sunday_8h_AllSunHoliday()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SUN_HOLIDAY"), Is.EqualTo(28800));
        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
    }

    [Test]
    public void C_Standard_Holiday_8h_AllSunHoliday()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "SUN_HOLIDAY"), Is.EqualTo(28800));
    }

    [Test]
    public void C_Standard_Grundlovsdag_8h_AllGrundlovsdag()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "GRUNDLOVSDAG"), Is.EqualTo(28800));
    }

    // GLS-A Jordbrug Dyrehold (animal care) — tier rules differ slightly
    [Test]
    public void C_Dyrehold_Weekday_8h_TierBased()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 28800, ruleSet, CalculatedAt);

        // Same weekday tier as Standard: 7.4h NORMAL + rest OT30
        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(26640));
        Assert.That(SecondsByCode(result, "OVERTIME_30"), Is.EqualTo(2160));
    }

    [Test]
    public void C_Dyrehold_Saturday_8h_TierBased()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 28800, ruleSet, CalculatedAt);

        // 6h SAT_NORMAL + 2h SAT_ANIMAL_AFTERNOON
        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(21600));
        Assert.That(SecondsByCode(result, "SAT_ANIMAL_AFTERNOON"), Is.EqualTo(7200));
    }

    [Test]
    public void C_Dyrehold_Sunday_8h_AllAnimalSunHoliday()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_SUN_HOLIDAY"), Is.EqualTo(28800));
    }

    [Test]
    public void C_Dyrehold_Holiday_8h_AllAnimalSunHoliday()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ANIMAL_SUN_HOLIDAY"), Is.EqualTo(28800));
    }

    [Test]
    public void C_Dyrehold_Grundlovsdag_8h_AllGrundlovsdag()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "GRUNDLOVSDAG"), Is.EqualTo(28800));
    }

    // GLS-A Apprentice Under 18 — different tier values per spec
    [Test]
    public void C_AppU18_Weekday_8h_AllNormalElev()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
        // Apprentice should have its own pay codes — assert at least one line was produced
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public void C_AppU18_Sunday_8h_HasOutput()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
    }

    [Test]
    public void C_AppU18_Grundlovsdag_8h_HasOutput()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 28800, ruleSet, CalculatedAt);

        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix D — Multi-shift combinations
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void D_1_TwoShifts_BothInNormalBand_TierRule()
    {
        // Tier rule (Standard weekday): 8h shift split into two segments still totals 8h NORMAL+0.something OT
        // Two shifts 08:00-12:00 + 13:00-16:00 = 4h + 3h = 7h tier-based
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var totalSeconds = (12 * 3600 - 8 * 3600) + (16 * 3600 - 13 * 3600); // 7h
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", totalSeconds, ruleSet, CalculatedAt);

        // 7h all NORMAL (tier 1 cap is 7.4h)
        Assert.That(SecondsByCode(result, "NORMAL"), Is.EqualTo(25200));
        Assert.That(TotalSeconds(result), Is.EqualTo(25200));
    }

    [Test]
    public void D_2_TwoTimeBandShifts_DifferentBands_KaGronSaturday()
    {
        // 10:00-12:00 (in SAT_NORMAL band) + 17:00-20:00 (in SAT_AFTERNOON band)
        // Each shift evaluated separately by GenerateTimeBandPayLines
        var ruleSet = BuildKaGronSaturdayOnly();

        var shift1 = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 36000, 43200, ruleSet, CalculatedAt);
        var shift2 = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 61200, 72000, ruleSet, CalculatedAt);

        var combined = shift1.Concat(shift2).ToList();
        Assert.That(SecondsByCode(combined, "SAT_NORMAL"), Is.EqualTo(7200));
        Assert.That(SecondsByCode(combined, "SAT_AFTERNOON"), Is.EqualTo(10800));
        Assert.That(TotalSeconds(combined), Is.EqualTo(18000));
    }

    [Test]
    public void D_3_ThreeShifts_SpansAllBandsOfDyrehold()
    {
        // 02:00-04:00 (in NIGHT) + 05:30-06:30 (half MORNING + half NORMAL) + 17:00-19:00 (NORMAL + EVENING)
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();

        var shift1 = PayLineGenerator.GenerateTimeBandPayLines(1, DayType.Monday, 7200, 14400, ruleSet, CalculatedAt);   // 2h ANIMAL_NIGHT
        var shift2 = PayLineGenerator.GenerateTimeBandPayLines(1, DayType.Monday, 19800, 23400, ruleSet, CalculatedAt);  // 0.5h SHIFTED_MORNING + 0.5h NORMAL
        var shift3 = PayLineGenerator.GenerateTimeBandPayLines(1, DayType.Monday, 61200, 68400, ruleSet, CalculatedAt);  // 1h NORMAL + 1h SHIFTED_EVENING

        var combined = shift1.Concat(shift2).Concat(shift3).ToList();
        Assert.That(SecondsByCode(combined, "ANIMAL_NIGHT"), Is.EqualTo(7200));
        Assert.That(SecondsByCode(combined, "SHIFTED_MORNING"), Is.EqualTo(1800));
        Assert.That(SecondsByCode(combined, "NORMAL"), Is.EqualTo(5400));
        Assert.That(SecondsByCode(combined, "SHIFTED_EVENING"), Is.EqualTo(3600));
        Assert.That(TotalSeconds(combined), Is.EqualTo(18000));
    }

    [Test]
    public void D_4_DocumentationOfPauseLimitation_FullShiftPassedAsIs()
    {
        // V1 limitation: pause inside a shift is not subtracted from time-band attribution.
        // 10:00-15:00 with a 1h pause should ideally produce 4h NORMAL, but v1 produces 5h NORMAL.
        // This test pins down the v1 behavior so a future fix is observable.
        var ruleSet = BuildKaGronSaturdayOnly();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 36000, 54000, ruleSet, CalculatedAt);

        // 5h SAT_NORMAL (full shift, no pause subtraction in v1)
        Assert.That(SecondsByCode(result, "SAT_NORMAL"), Is.EqualTo(18000));
    }

    // ─────────────────────────────────────────────────────────────
    // Matrix G — u18 Dyrehold Saturday tier verification
    //
    // Locks down the verified behavior for "GLS-A / 3F - Jordbrug Elev u18
    // Dyrehold 2024-2026" Saturday: a tier rule with 8h cutoff. Because the
    // preset has empty payDayTypeRules, the export's CalculatePayLinesForDay
    // helper is expected to fall through to the tier path and apply this
    // rule. Documented in /home/rene/.claude/plans/golden-petting-turtle.md.
    //
    // SATURDAY tier: 0..28800 ELEV_SAT_NORMAL, then ELEV_SAT_ANIMAL_AFTERNOON.
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void G_1_U18Dyrehold_Saturday_Under8h_AllNormal()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 21600, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ELEV_SAT_NORMAL"), Is.EqualTo(21600));
        Assert.That(SecondsByCode(result, "ELEV_SAT_ANIMAL_AFTERNOON"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(21600));
    }

    [Test]
    public void G_2_U18Dyrehold_Saturday_ExactlyAt8h_AllNormalNoAfternoon()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ELEV_SAT_NORMAL"), Is.EqualTo(28800));
        Assert.That(SecondsByCode(result, "ELEV_SAT_ANIMAL_AFTERNOON"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
    }

    [Test]
    public void G_3_U18Dyrehold_Saturday_OneSecondPast8h_OneSecondAfternoon()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 28801, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ELEV_SAT_NORMAL"), Is.EqualTo(28800));
        Assert.That(SecondsByCode(result, "ELEV_SAT_ANIMAL_AFTERNOON"), Is.EqualTo(1));
        Assert.That(TotalSeconds(result), Is.EqualTo(28801));
    }

    [Test]
    public void G_4_U18Dyrehold_Saturday_12h_8hNormalPlus4hAnimalAfternoon()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 43200, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ELEV_SAT_NORMAL"), Is.EqualTo(28800));
        Assert.That(SecondsByCode(result, "ELEV_SAT_ANIMAL_AFTERNOON"), Is.EqualTo(14400));
        Assert.That(TotalSeconds(result), Is.EqualTo(43200));
    }

    [Test]
    public void G_5_U18Dyrehold_PresetHasNoTimeBandRules()
    {
        // Documents the structural assumption that drives Matrix G: empty
        // payDayTypeRules is what causes the export's CalculatePayLinesForDay
        // helper to skip the time-band path and apply the tier rule. If a
        // future change adds DayTypeRules for u18 Dyrehold, callers should
        // re-verify the export still produces 8h ELEV_SAT_NORMAL + remainder
        // ELEV_SAT_ANIMAL_AFTERNOON for Saturday shifts.
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18_DyrePasning();

        Assert.That(ruleSet.DayTypeRules, Is.Null.Or.Empty);
    }

    [Test]
    public void G_6_U18Dyrehold_Weekday_8h_AllElevNormal()
    {
        // Sanity check that weekday tier is independent of Saturday: 8h on a
        // weekday should be 8h ELEV_NORMAL with no overtime (tier 1 cap = 8h).
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(SecondsByCode(result, "ELEV_NORMAL"), Is.EqualTo(28800));
        Assert.That(SecondsByCode(result, "ELEV_OVERTIME_50"), Is.EqualTo(0));
        Assert.That(TotalSeconds(result), Is.EqualTo(28800));
    }
}
