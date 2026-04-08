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
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using Microting.TimePlanningBase.Tests.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

/// <summary>
/// Unit tests for GLS-A / 3F Jordbrugsoverenskomsten 2024-2026 pay rule sets.
/// All tests are pure in-memory — no database required.
///
/// Note: Saturday noon splits and animal care night supplements (00:00-05:00)
/// are approximated using tier-based UpToSeconds. A future time-band-aware
/// generator (PayDayTypeRule + PayTimeBandRule) will handle actual clock-time splitting.
/// </summary>
[TestFixture]
public class GlsAJordbrugPayLineTests
{
    private static readonly DateTime CalculatedAt = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

    // ───────────────────────────────────────────────────────────────
    // Standard Agriculture Tests
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Standard_Weekday_Normal_7h24m()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 26640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(26640));
    }

    [Test]
    public void Standard_Weekday_Overtime_2h()
    {
        // 9.4h = 33840s => 7.4h normal + 2h OT30
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640)); // 7.4h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));    // 2h
    }

    [Test]
    public void Standard_Weekday_Overtime_4h()
    {
        // 11.4h = 41040s => 7.4h normal + 2h OT30 + 2h OT80
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 41040, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");
        var ot80 = result.First(l => l.PayCode == "OVERTIME_80");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));
        Assert.That(ot80.HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void Standard_Weekday_Short_4h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(14400));
    }

    [Test]
    public void Standard_Saturday_BeforeNoon_6h()
    {
        // 6h = 21600s fits entirely in SAT_NORMAL tier
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 21600, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("SAT_NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(21600));
    }

    [Test]
    public void Standard_Saturday_SpanNoon_10h()
    {
        // 10h = 36000s => 6h SAT_NORMAL + 4h SAT_AFTERNOON
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var satNormal = result.First(l => l.PayCode == "SAT_NORMAL");
        var satAfternoon = result.First(l => l.PayCode == "SAT_AFTERNOON");

        Assert.That(satNormal.HoursInSeconds, Is.EqualTo(21600));   // 6h
        Assert.That(satAfternoon.HoursInSeconds, Is.EqualTo(14400)); // 4h
    }

    [Test]
    public void Standard_Sunday_8h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void Standard_Holiday_8h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void Standard_Grundlovsdag_4h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("GRUNDLOVSDAG"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(14400));
    }

    // ───────────────────────────────────────────────────────────────
    // Animal Care (DyrePasning) Tests
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Animal_Saturday_SpanNoon_10h()
    {
        // 10h = 36000s => 6h SAT_NORMAL + 4h SAT_ANIMAL_AFTERNOON
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var satNormal = result.First(l => l.PayCode == "SAT_NORMAL");
        var satAnimal = result.First(l => l.PayCode == "SAT_ANIMAL_AFTERNOON");

        Assert.That(satNormal.HoursInSeconds, Is.EqualTo(21600));
        Assert.That(satAnimal.HoursInSeconds, Is.EqualTo(14400));
    }

    [Test]
    public void Animal_Sunday_8h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ANIMAL_SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void Animal_Holiday_8h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ANIMAL_SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    // ───────────────────────────────────────────────────────────────
    // Apprentice Under-18 Tests
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void ElevU18_Weekday_Normal_8h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ELEV_NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void ElevU18_Weekday_Over_10h()
    {
        // 10h = 36000s => 8h ELEV_NORMAL + 2h ELEV_OVERTIME_50
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "ELEV_NORMAL");
        var ot50 = result.First(l => l.PayCode == "ELEV_OVERTIME_50");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(28800));
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void ElevU18_Sunday_2h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 7200, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ELEV_SUN_OT_50"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void ElevU18_Sunday_4h()
    {
        // 4h = 14400s => 2h ELEV_SUN_OT_50 + 2h ELEV_SUN_OT_80
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var ot50 = result.First(l => l.PayCode == "ELEV_SUN_OT_50");
        var ot80 = result.First(l => l.PayCode == "ELEV_SUN_OT_80");

        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));
        Assert.That(ot80.HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void ElevU18_Saturday_6h()
    {
        // 6h = 21600s, within the 8h ELEV_SAT_NORMAL tier
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 21600, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ELEV_SAT_NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(21600));
    }

    // ───────────────────────────────────────────────────────────────
    // Apprentice Over-18 Tests
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void ElevO18_Weekday_Normal()
    {
        // 7.4h = 26640s, exactly fills ELEV_NORMAL tier
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Over18();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 26640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ELEV_NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(26640));
    }

    [Test]
    public void ElevO18_Weekday_OT_2h()
    {
        // 9.4h = 33840s => 7.4h ELEV_NORMAL + 2h ELEV_OVERTIME_30
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Over18();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "ELEV_NORMAL");
        var ot30 = result.First(l => l.PayCode == "ELEV_OVERTIME_30");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void ElevO18_Sunday_2h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Over18();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 7200, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ELEV_SUN_OT_50"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void ElevO18_Sunday_5h()
    {
        // 5h = 18000s => 2h ELEV_SUN_OT_50 + 3h ELEV_SUN_OT_80
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Over18();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 18000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var ot50 = result.First(l => l.PayCode == "ELEV_SUN_OT_50");
        var ot80 = result.First(l => l.PayCode == "ELEV_SUN_OT_80");

        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));   // 2h
        Assert.That(ot80.HoursInSeconds, Is.EqualTo(10800));  // 3h
    }

    // ───────────────────────────────────────────────────────────────
    // Apprentice Under-18 DyrePasning Tests
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void ElevU18Animal_Saturday_10h()
    {
        // 10h = 36000s => 8h ELEV_SAT_NORMAL + 2h ELEV_SAT_ANIMAL_AFTERNOON
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18_DyrePasning();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var satNormal = result.First(l => l.PayCode == "ELEV_SAT_NORMAL");
        var satAnimal = result.First(l => l.PayCode == "ELEV_SAT_ANIMAL_AFTERNOON");

        Assert.That(satNormal.HoursInSeconds, Is.EqualTo(28800));   // 8h
        Assert.That(satAnimal.HoursInSeconds, Is.EqualTo(7200));    // 2h
    }

    [Test]
    public void ElevU18_Holiday_4h()
    {
        // 4h = 14400s => 2h ELEV_HOL_OT_50 + 2h ELEV_HOL_OT_80
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var ot50 = result.First(l => l.PayCode == "ELEV_HOL_OT_50");
        var ot80 = result.First(l => l.PayCode == "ELEV_HOL_OT_80");

        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));   // 2h
        Assert.That(ot80.HoursInSeconds, Is.EqualTo(7200));   // 2h
    }

    [Test]
    public void ElevU18_Grundlovsdag_4h()
    {
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("GRUNDLOVSDAG"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(14400));
    }

    // ───────────────────────────────────────────────────────────────
    // Time-Band Tests: Standard fixture
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void TimeBand_Standard_Weekday_Normal_0600_1800()
    {
        // 06:00 (21600) to 18:00 (64800) = 12h all within NORMAL band
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 21600, 64800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(43200));
    }

    [Test]
    public void TimeBand_Standard_Weekday_ShiftedMorning_0400_0800()
    {
        // 04:00 (14400) to 08:00 (28800) spans SHIFTED_MORNING (04:00-06:00) + NORMAL (06:00-08:00)
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 14400, 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var shifted = result.First(l => l.PayCode == "SHIFTED_MORNING");
        var normal = result.First(l => l.PayCode == "NORMAL");

        Assert.That(shifted.HoursInSeconds, Is.EqualTo(7200));  // 2h
        Assert.That(normal.HoursInSeconds, Is.EqualTo(7200));   // 2h
    }

    [Test]
    public void TimeBand_Standard_Saturday_SpanNoon_0600_1600()
    {
        // 06:00 (21600) to 16:00 (57600) => SAT_NORMAL 06:00-12:00 (21600s) + SAT_AFTERNOON 12:00-16:00 (14400s)
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 21600, 57600, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var satNormal = result.First(l => l.PayCode == "SAT_NORMAL");
        var satAfternoon = result.First(l => l.PayCode == "SAT_AFTERNOON");

        Assert.That(satNormal.HoursInSeconds, Is.EqualTo(21600));    // 6h
        Assert.That(satAfternoon.HoursInSeconds, Is.EqualTo(14400)); // 4h
    }

    // ───────────────────────────────────────────────────────────────
    // Time-Band Tests: DyrePasning (Animal Care) fixture
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void TimeBand_Animal_Weekday_Night_0000_0500()
    {
        // 00:00 (0) to 05:00 (18000) = 5h ANIMAL_NIGHT
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 0, 18000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ANIMAL_NIGHT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(18000));
    }

    [Test]
    public void TimeBand_Animal_Weekday_NightAndDay_0300_1200()
    {
        // 03:00 (10800) to 12:00 (43200) spans:
        //   ANIMAL_NIGHT 03:00-05:00 (7200s)
        //   SHIFTED_MORNING 05:00-06:00 (3600s)
        //   NORMAL 06:00-12:00 (21600s)
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Monday, 10800, 43200, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var night = result.First(l => l.PayCode == "ANIMAL_NIGHT");
        var shifted = result.First(l => l.PayCode == "SHIFTED_MORNING");
        var normal = result.First(l => l.PayCode == "NORMAL");

        Assert.That(night.HoursInSeconds, Is.EqualTo(7200));    // 2h
        Assert.That(shifted.HoursInSeconds, Is.EqualTo(3600));  // 1h
        Assert.That(normal.HoursInSeconds, Is.EqualTo(21600));  // 6h
    }

    [Test]
    public void TimeBand_Animal_Saturday_SpanNoon_0800_1800()
    {
        // 08:00 (28800) to 18:00 (64800) =>
        //   SAT_NORMAL 08:00-12:00 (14400s)
        //   SAT_ANIMAL_AFTERNOON 12:00-18:00 (21600s)
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 28800, 64800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var satNormal = result.First(l => l.PayCode == "SAT_NORMAL");
        var satAnimal = result.First(l => l.PayCode == "SAT_ANIMAL_AFTERNOON");

        Assert.That(satNormal.HoursInSeconds, Is.EqualTo(14400));   // 4h
        Assert.That(satAnimal.HoursInSeconds, Is.EqualTo(21600));   // 6h
    }

    [Test]
    public void TimeBand_Animal_Sunday_Full_0600_1400()
    {
        // 06:00 (21600) to 14:00 (50400) = 8h ANIMAL_SUN_HOLIDAY
        var ruleSet = GlsAFixtureHelper.GlsA_Jordbrug_DyrePasning();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Sunday, 21600, 50400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("ANIMAL_SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    // ───────────────────────────────────────────────────────────────
    // Transition Test: Apprentice to Standard rule set
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Transition_Laerling_To_Standard()
    {
        // Scenario: An apprentice (Under-18) works 8h weekday under apprentice rules,
        // then transitions to standard rules for another 8h weekday.
        // Each call is independent — PayLineGenerator is stateless per call.

        // Apprentice Under-18: 8h weekday = 28800s
        var apprenticeRuleSet = GlsAFixtureHelper.GlsA_Jordbrug_Laerling_Under18();
        var apprenticeResult = PayLineGenerator.GeneratePayLines(
            1, "WEEKDAY", 28800, apprenticeRuleSet, CalculatedAt);

        Assert.That(apprenticeResult.Count, Is.EqualTo(1));
        Assert.That(apprenticeResult[0].PayCode, Is.EqualTo("ELEV_NORMAL"));
        Assert.That(apprenticeResult[0].HoursInSeconds, Is.EqualTo(28800)); // 8h

        // Standard: 8h weekday = 28800s
        // 7.4h (26640s) NORMAL + remaining 0.6h (2160s) OVERTIME_30
        var standardRuleSet = GlsAFixtureHelper.GlsA_Jordbrug_Standard();
        var standardResult = PayLineGenerator.GeneratePayLines(
            2, "WEEKDAY", 28800, standardRuleSet, CalculatedAt);

        Assert.That(standardResult.Count, Is.EqualTo(2));

        var normal = standardResult.First(l => l.PayCode == "NORMAL");
        var ot30 = standardResult.First(l => l.PayCode == "OVERTIME_30");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(2160));     // 0.6h (28800 - 26640)
    }
}
