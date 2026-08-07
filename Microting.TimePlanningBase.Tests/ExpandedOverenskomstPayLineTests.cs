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
/// Unit tests for expanded overenskomst pay rule sets (Gartneri, Skovbrug,
/// KA Landbrug, KA Gron, Golf, Agroindustri). All tests are pure in-memory
/// -- no database required. Tests validate tier-based pay-line splitting via
/// PayLineGenerator for each distinct OT pattern across the 32 presets.
/// </summary>
[TestFixture]
public class ExpandedOverenskomstPayLineTests
{
    private static readonly DateTime CalculatedAt = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);

    // ───────────────────────────────────────────────────────────────
    // 50%/100% with 2h OT window (Gartneri Standard, KA Svine Standard)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Gartneri_Standard_Weekday_Normal()
    {
        // 7.4h = 26640s => all NORMAL
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 26640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(26640));
    }

    [Test]
    public void Gartneri_Standard_Weekday_OT_2h()
    {
        // 9.4h = 33840s => 7.4h NORMAL + 2h OVERTIME_50
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));     // 2h
    }

    [Test]
    public void Gartneri_Standard_Weekday_OT_4h()
    {
        // 11.4h = 41040s => 7.4h NORMAL + 2h OVERTIME_50 + 2h OVERTIME_100
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 41040, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");
        var ot100 = result.First(l => l.PayCode == "OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void Gartneri_Standard_Saturday_SpanNoon()
    {
        // 28000s => 23400s SAT_NORMAL + 4600s SAT_AFTERNOON
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 28000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var satNormal = result.First(l => l.PayCode == "SAT_NORMAL");
        var satAfternoon = result.First(l => l.PayCode == "SAT_AFTERNOON");

        Assert.That(satNormal.HoursInSeconds, Is.EqualTo(23400));   // 6.5h
        Assert.That(satAfternoon.HoursInSeconds, Is.EqualTo(4600));
    }

    [Test]
    public void Gartneri_Standard_Sunday_8h()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void KA_Svine_Standard_Weekday_OT_4h()
    {
        // 11.4h = 41040s => 7.4h NORMAL + 2h OVERTIME_50 + 2h OVERTIME_100
        // Same pattern as Gartneri but verifies KA Svine fixture
        var ruleSet = OverenskomstFixtureHelper.KA_Landbrug_Svine_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 41040, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");
        var ot100 = result.First(l => l.PayCode == "OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));
    }

    // ───────────────────────────────────────────────────────────────
    // 30%/100% with 2h OT window (Skovbrug Standard)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Skovbrug_Standard_Weekday_OT_4h()
    {
        // 11.4h = 41040s => 7.4h NORMAL + 2h OVERTIME_30 + 2h OVERTIME_100
        var ruleSet = OverenskomstFixtureHelper.GlsA_Skovbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 41040, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");
        var ot100 = result.First(l => l.PayCode == "OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void Skovbrug_Standard_Sunday_8h()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Skovbrug_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    // ───────────────────────────────────────────────────────────────
    // 50%/100% with 3h OT window (KA Plantebrug, KA Gron)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void KA_Plante_Standard_Weekday_Normal()
    {
        // 7.4h = 26640s => all NORMAL
        var ruleSet = OverenskomstFixtureHelper.KA_Landbrug_Plante_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 26640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(26640));
    }

    [Test]
    public void KA_Plante_Standard_Weekday_OT_3h()
    {
        // 10.4h = 37440s => 7.4h NORMAL + 3h OVERTIME_50
        var ruleSet = OverenskomstFixtureHelper.KA_Landbrug_Plante_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 37440, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(10800));    // 3h
    }

    [Test]
    public void KA_Plante_Standard_Weekday_OT_5h()
    {
        // 12.4h = 44640s => 7.4h NORMAL + 3h OVERTIME_50 + 2h OVERTIME_100
        var ruleSet = OverenskomstFixtureHelper.KA_Landbrug_Plante_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 44640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");
        var ot100 = result.First(l => l.PayCode == "OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(10800));    // 3h
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));    // 2h
    }

    [Test]
    public void KA_Gron_Standard_Weekday_OT_3h()
    {
        // 10.4h = 37440s => 7.4h NORMAL + 3h OVERTIME_50
        // Same pattern as Plantebrug, verifies KA Gron fixture
        var ruleSet = OverenskomstFixtureHelper.KA_Gron_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 37440, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(10800));    // 3h
    }

    // ───────────────────────────────────────────────────────────────
    // 30%/80% (KA Maskinstation Standard)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void KA_Maskin_Standard_Weekday_OT_4h()
    {
        // 11.4h = 41040s => 7.4h NORMAL + 2h OVERTIME_30 + 2h OVERTIME_80
        var ruleSet = OverenskomstFixtureHelper.KA_Landbrug_Maskin_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 41040, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");
        var ot80 = result.First(l => l.PayCode == "OVERTIME_80");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));
        Assert.That(ot80.HoursInSeconds, Is.EqualTo(7200));
    }

    // ───────────────────────────────────────────────────────────────
    // Elev patterns (one per distinct variant)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Gartneri_ElevU18_Weekday_Over_10h()
    {
        // 10h = 36000s => 8h ELEV_NORMAL + 2h ELEV_OVERTIME_50
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Elev_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "ELEV_NORMAL");
        var ot50 = result.First(l => l.PayCode == "ELEV_OVERTIME_50");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(28800));  // 8h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));     // 2h
    }

    [Test]
    public void Gartneri_ElevU18_Sunday_4h()
    {
        // 4h = 14400s => 2h ELEV_SUN_OT_50 + 2h ELEV_SUN_OT_100
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Elev_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var ot50 = result.First(l => l.PayCode == "ELEV_SUN_OT_50");
        var ot100 = result.First(l => l.PayCode == "ELEV_SUN_OT_100");

        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));   // 2h
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));  // 2h
    }

    [Test]
    public void Skovbrug_ElevU18_Weekday_Over_10h()
    {
        // 10h = 36000s => 8h ELEV_NORMAL + 2h ELEV_OVERTIME_30
        var ruleSet = OverenskomstFixtureHelper.GlsA_Skovbrug_Elev_Under18();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "ELEV_NORMAL");
        var ot30 = result.First(l => l.PayCode == "ELEV_OVERTIME_30");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(28800));  // 8h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));     // 2h
    }

    [Test]
    public void KA_Maskin_Elev_Weekday_Over_10h()
    {
        // 10h = 36000s => 8h ELEV_NORMAL + 2h ELEV_OVERTIME_30
        var ruleSet = OverenskomstFixtureHelper.KA_Landbrug_Maskin_Elev();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "ELEV_NORMAL");
        var ot30 = result.First(l => l.PayCode == "ELEV_OVERTIME_30");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(28800));  // 8h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));     // 2h
    }

    [Test]
    public void KA_Maskin_Elev_Sunday_4h()
    {
        // 4h = 14400s => 2h ELEV_SUN_OT_30 + 2h ELEV_SUN_OT_80
        var ruleSet = OverenskomstFixtureHelper.KA_Landbrug_Maskin_Elev();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var ot30 = result.First(l => l.PayCode == "ELEV_SUN_OT_30");
        var ot80 = result.First(l => l.PayCode == "ELEV_SUN_OT_80");

        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));   // 2h
        Assert.That(ot80.HoursInSeconds, Is.EqualTo(7200));   // 2h
    }

    [Test]
    public void KA_Gron_Elev_Weekday_Over_10h()
    {
        // 10h = 36000s => 8h ELEV_NORMAL + 2h ELEV_OVERTIME_50
        var ruleSet = OverenskomstFixtureHelper.KA_Gron_Elev();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "ELEV_NORMAL");
        var ot50 = result.First(l => l.PayCode == "ELEV_OVERTIME_50");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(28800));  // 8h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(7200));     // 2h
    }

    // ───────────────────────────────────────────────────────────────
    // Grundlovsdag
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Gartneri_Standard_Grundlovsdag_4h()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Gartneri_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("GRUNDLOVSDAG"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(14400));
    }

    // ───────────────────────────────────────────────────────────────
    // Flat 100% OT (Golf Standard)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Golf_Standard_Weekday_OT_4h()
    {
        // 11.4h = 41040s => 7.4h NORMAL + 4h OVERTIME_100
        var ruleSet = OverenskomstFixtureHelper.GlsA_Golf_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 41040, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot100 = result.First(l => l.PayCode == "OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(14400));   // 4h
    }

    [Test]
    public void Golf_Standard_Sunday_8h()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Golf_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("SUN_HOLIDAY"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
    }

    [Test]
    public void Golf_Elev_Weekday_Over_10h()
    {
        // 10h = 36000s => 8h ELEV_NORMAL + 2h ELEV_OVERTIME_100
        var ruleSet = OverenskomstFixtureHelper.GlsA_Golf_Elev();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 36000, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "ELEV_NORMAL");
        var ot100 = result.First(l => l.PayCode == "ELEV_OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(28800));  // 8h
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));    // 2h
    }

    // ───────────────────────────────────────────────────────────────
    // 30%/50%/100% 4-tier (Agroindustri Fjerkrae)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Agro_Fjerkrae_Weekday_OT_2h()
    {
        // 9.4h = 33840s => 7.4h NORMAL + 2h OVERTIME_30
        var ruleSet = OverenskomstFixtureHelper.GlsA_Agro_Fjerkrae_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));     // 2h
    }

    [Test]
    public void Agro_Fjerkrae_Weekday_OT_3h()
    {
        // 10.4h = 37440s => 7.4h NORMAL + 2h OVERTIME_30 + 1h OVERTIME_50
        var ruleSet = OverenskomstFixtureHelper.GlsA_Agro_Fjerkrae_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 37440, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));     // 2h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(3600));     // 1h
    }

    [Test]
    public void Agro_Fjerkrae_Weekday_OT_5h()
    {
        // 12.4h = 44640s => 7.4h NORMAL + 2h OVERTIME_30 + 1h OVERTIME_50 + 2h OVERTIME_100
        var ruleSet = OverenskomstFixtureHelper.GlsA_Agro_Fjerkrae_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 44640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(4));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");
        var ot50 = result.First(l => l.PayCode == "OVERTIME_50");
        var ot100 = result.First(l => l.PayCode == "OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));     // 2h
        Assert.That(ot50.HoursInSeconds, Is.EqualTo(3600));     // 1h
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));    // 2h
    }

    // ───────────────────────────────────────────────────────────────
    // 40%/100% (Agroindustri Grovvare)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Agro_Grovvare_Weekday_OT_3h()
    {
        // 10.4h = 37440s => 7.4h NORMAL + 3h OVERTIME_40
        var ruleSet = OverenskomstFixtureHelper.GlsA_Agro_Grovvare_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 37440, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot40 = result.First(l => l.PayCode == "OVERTIME_40");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot40.HoursInSeconds, Is.EqualTo(10800));    // 3h
    }

    [Test]
    public void Agro_Grovvare_Weekday_OT_5h()
    {
        // 12.4h = 44640s => 7.4h NORMAL + 3h OVERTIME_40 + 2h OVERTIME_100
        var ruleSet = OverenskomstFixtureHelper.GlsA_Agro_Grovvare_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 44640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot40 = result.First(l => l.PayCode == "OVERTIME_40");
        var ot100 = result.First(l => l.PayCode == "OVERTIME_100");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot40.HoursInSeconds, Is.EqualTo(10800));    // 3h
        Assert.That(ot100.HoursInSeconds, Is.EqualTo(7200));    // 2h
    }

    // ───────────────────────────────────────────────────────────────
    // 30%/80% with Agroindustri fixture (Ovrige)
    // ───────────────────────────────────────────────────────────────

    [Test]
    public void Agro_Ovrige_Weekday_OT_4h()
    {
        // 11.4h = 41040s => 7.4h NORMAL + 2h OVERTIME_30 + 2h OVERTIME_80
        var ruleSet = OverenskomstFixtureHelper.GlsA_Agro_Ovrige_Standard();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 41040, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));

        var normal = result.First(l => l.PayCode == "NORMAL");
        var ot30 = result.First(l => l.PayCode == "OVERTIME_30");
        var ot80 = result.First(l => l.PayCode == "OVERTIME_80");

        Assert.That(normal.HoursInSeconds, Is.EqualTo(26640));  // 7.4h
        Assert.That(ot30.HoursInSeconds, Is.EqualTo(7200));     // 2h
        Assert.That(ot80.HoursInSeconds, Is.EqualTo(7200));     // 2h
    }

    // ═════════════════════════════════════════════════════════════
    // GENERATOR-LEVEL TESTS - Udenlandske praktikanter (both presets)
    //
    // Everything from here to the end of the file calls PayLineGenerator
    // directly. That proves the TIER ARITHMETIC of the corrected presets and
    // nothing more - it says nothing about which code path production takes
    // for a given day. In production CalculatePayLinesForDay does the routing:
    // it decides, per day, whether the clock-time bands run, whether the tiers
    // run, or - for the stald preset's Saturday/Sunday/Holiday - both, with the
    // bands attributing normal time up to the first tier's boundary and the
    // tiers attributing every minute past it. That end-to-end routing behaviour
    // is covered by PraktikantPayLineRoutingTests in
    // eform-angular-timeplanning-plugin, not here.
    //
    // The distinction is load-bearing. Before the § 50 preset corrections the
    // Saturday/Sunday/Holiday tests below asserted a tier path the router could
    // never reach: they passed happily while describing a calculation that never
    // ran in production. Assert tier arithmetic here; assert routing there.
    // ═════════════════════════════════════════════════════════════

    // ─────────────────────────────────────────────────────────────
    // GLS-A / 3F - Udenlandske praktikanter Landbrug (Andet arbejde)
    //
    // Field-work variant. Same 7.4h + 2h + rest split as standard, but
    // the middle tier is +50% (not +30%). Sundays and holidays fall outside
    // the permitted Mon-Sat work window, so every minute worked is overtime
    // (first 2h @ 50%, remainder @ 80%). Grundlovsdag, by contrast, is
    // ordinary working time and follows the weekday progression.
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void PraktikantUdlAndet_Weekday_UnderTier1_AllNormal()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(14400));
    }

    [Test]
    public void PraktikantUdlAndet_Weekday_ExactlyAt7h24m_AllNormal()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 26640, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(26640));
    }

    [Test]
    public void PraktikantUdlAndet_Weekday_9h24m_NormalPlusOvertime50()
    {
        // 9h24m = 33840s → 7h24m NORMAL + 2h OVERTIME_50
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "NORMAL").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void PraktikantUdlAndet_Weekday_12h_AllThreeTiers()
    {
        // 12h = 43200s → 7h24m NORMAL + 2h OVERTIME_50 + 2h36m OVERTIME_80
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 43200, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.First(l => l.PayCode == "NORMAL").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(7200));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_80").HoursInSeconds, Is.EqualTo(9360));
    }

    [Test]
    public void PraktikantUdlAndet_Saturday_SameAsWeekday()
    {
        // Field work is allowed Mon-Sat, so Saturday uses the same tier
        // structure as weekdays (unlike Standard which has a Saturday split).
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "NORMAL").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void PraktikantUdlAndet_Sunday_AllOvertime()
    {
        // Field work is not permitted on Sundays; if worked, all hours OT.
        // 4h = 14400s → 2h OVERTIME_50 + 2h OVERTIME_80.
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(7200));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_80").HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void PraktikantUdlAndet_Holiday_AllOvertime_SameAsSunday()
    {
        // Structurally identical to Sunday - the preset has its own HOLIDAY
        // PayDayRule that must be exercised so a future divergence is caught.
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(7200));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_80").HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void PraktikantUdlAndet_Grundlovsdag_OrdinaryWorkingTime_ThenOvertimeSteps()
    {
        // Grundlovsdag is a working day (a half day per Jordbrug § 29), not a
        // søgnehelligdag, so it is NOT all-overtime the way Sunday and Holiday
        // are. The tiers therefore read as: ordinary working time up to the
        // daily norm, then the two overtime steps - the same shape as a weekday.
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 43200, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.First(l => l.PayCode == "NORMAL").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(7200));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_80").HoursInSeconds, Is.EqualTo(9360));
        Assert.That(result.Sum(l => l.HoursInSeconds), Is.EqualTo(43200));
    }

    // ─────────────────────────────────────────────────────────────
    // GLS-A / 3F - Udenlandske praktikanter Landbrug (Staldarbejde)
    //
    // Animal-care variant. Weekday tiers match Andet arbejde.
    //
    // On Saturday, Sunday and Holiday the tiers are not a duplicate of the
    // time bands: the first tier marks where normal time ends (7h24m), and the
    // tiers past it are the overtime steps. The stald supplements are payable
    // per § 50 stk. 4 d only "for arbejde i normal arbejdstid", so overtime
    // minutes carry OVERTIME_50 / OVERTIME_80 and no supplement code. Which
    // supplement code the normal-time portion gets is decided by clock time -
    // Saturday splits at 12:00 into SAT_NORMAL / SAT_ANIMAL_AFTERNOON, Sunday
    // and Holiday are ANIMAL_SUN_HOLIDAY all day - and that split is the time
    // bands' job, exercised by GenerateTimeBandPayLines below and by the
    // plugin's routing tests. The first tier here carries the code the bands
    // would have produced anyway, so the tier path stays coherent on its own.
    // ─────────────────────────────────────────────────────────────

    [Test]
    public void PraktikantUdlStald_Weekday_9h24m_NormalPlusOvertime50()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", 33840, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "NORMAL").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(7200));
    }

    [Test]
    public void PraktikantUdlStald_Saturday_NormalTime_ThenOvertimeSteps()
    {
        // The Saturday tiers express "normal time, then overtime", not a repeat
        // of the 12:00 band split: everything up to the daily norm is normal
        // Saturday time, and the excess is overtime carrying no stald
        // supplement. An 8h Saturday is therefore mostly normal time with a
        // short tail in the first overtime step.
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "SAT_NORMAL").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(2160));
        Assert.That(result.Sum(l => l.HoursInSeconds), Is.EqualTo(28800));
    }

    [Test]
    public void PraktikantUdlStald_Saturday_TimeBand_09to18_Split_At_Noon()
    {
        // Time-band path (GenerateTimeBandPayLines) for a 09:00-18:00 shift:
        // 3h SAT_NORMAL (09-12) + 6h SAT_ANIMAL_AFTERNOON (12-18).
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GenerateTimeBandPayLines(
            1, DayType.Saturday, 32400, 64800, ruleSet, CalculatedAt);

        Assert.That(result.Where(l => l.PayCode == "SAT_NORMAL").Sum(l => l.HoursInSeconds), Is.EqualTo(10800));
        Assert.That(result.Where(l => l.PayCode == "SAT_ANIMAL_AFTERNOON").Sum(l => l.HoursInSeconds), Is.EqualTo(21600));
    }

    [Test]
    public void PraktikantUdlStald_Sunday_NormalTime_ThenOvertimeSteps()
    {
        // Animal care is genuine Sunday work, so the normal-time portion keeps
        // the Sunday/holiday supplement - but only up to the daily norm. Past
        // that boundary the supplement stops (§ 50 stk. 4 d: "i normal
        // arbejdstid") and the minutes are plain overtime.
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "ANIMAL_SUN_HOLIDAY").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(2160));
        Assert.That(result.Sum(l => l.HoursInSeconds), Is.EqualTo(28800));
    }

    [Test]
    public void PraktikantUdlStald_Holiday_NormalTime_ThenOvertimeSteps()
    {
        // Holiday has its own PayDayRule and is structurally identical to
        // Sunday; exercised separately so a future divergence is caught.
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 28800, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First(l => l.PayCode == "ANIMAL_SUN_HOLIDAY").HoursInSeconds, Is.EqualTo(26640));
        Assert.That(result.First(l => l.PayCode == "OVERTIME_50").HoursInSeconds, Is.EqualTo(2160));
        Assert.That(result.Sum(l => l.HoursInSeconds), Is.EqualTo(28800));
    }

    [Test]
    public void PraktikantUdlStald_Grundlovsdag_OrdinaryWorkingTime()
    {
        // Grundlovsdag is a working day (a half day per Jordbrug § 29), not a
        // søgnehelligdag, so it does not carry the Sunday/holiday animal
        // supplement. A short day stays entirely within ordinary working time
        // and produces a single NORMAL line - no overtime step is reached.
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 14400, ruleSet, CalculatedAt);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("NORMAL"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(14400));
    }

    // ─────────────────────────────────────────────────────────────
    // Boundary matrix for the day rules corrected by the § 50 preset fix
    //
    // Every corrected day rule has the same three-tier shape: normal time up
    // to the daily norm, then a first overtime step of two hours, then an
    // uncapped second step. Only the first tier's pay code differs per day.
    // Each rule is walked across the four points where the tier arithmetic can
    // go wrong: exactly at the normal-time boundary, one second past it,
    // exactly at the ceiling of the first overtime step, and past that ceiling.
    //
    // Still generator-level: this is tier arithmetic, not routing. See the
    // header block above.
    // ─────────────────────────────────────────────────────────────

    private const int DailyNormSeconds = 26640;          // 7h24m - end of normal time
    private const int FirstOvertimeStepEnd = 33840;      // +2h    - end of OVERTIME_50

    /// <summary>
    /// Asserts that the generated lines carry exactly the expected pay codes with
    /// exactly the expected seconds, and that the per-code seconds add back up to
    /// the seconds worked - no minute lost, invented or double-counted.
    /// </summary>
    private static void AssertTierSplit(
        List<PlanRegistrationPayLine> lines,
        int workedSeconds,
        params (string PayCode, int Seconds)[] expected)
    {
        Assert.Multiple(() =>
        {
            Assert.That(lines.Select(l => l.PayCode), Is.EquivalentTo(expected.Select(e => e.PayCode)),
                "pay codes emitted");

            foreach (var (payCode, seconds) in expected)
            {
                Assert.That(lines.Where(l => l.PayCode == payCode).Sum(l => l.HoursInSeconds),
                    Is.EqualTo(seconds), $"seconds attributed to {payCode}");
            }

            Assert.That(lines.Sum(l => l.HoursInSeconds), Is.EqualTo(workedSeconds),
                "per-code seconds must sum to the seconds worked");
        });
    }

    // --- Staldarbejde SATURDAY: SAT_NORMAL -> OVERTIME_50 -> OVERTIME_80 ---

    [Test]
    public void PraktikantUdlStald_Saturday_AtDailyNorm_NoOvertime()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", DailyNormSeconds, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds, ("SAT_NORMAL", 26640));
    }

    [Test]
    public void PraktikantUdlStald_Saturday_OneSecondOverDailyNorm_OneSecondOvertime50()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", DailyNormSeconds + 1, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds + 1, ("SAT_NORMAL", 26640), ("OVERTIME_50", 1));
    }

    [Test]
    public void PraktikantUdlStald_Saturday_AtFirstOvertimeStepCeiling_NoOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", FirstOvertimeStepEnd, ruleSet, CalculatedAt);

        AssertTierSplit(result, FirstOvertimeStepEnd, ("SAT_NORMAL", 26640), ("OVERTIME_50", 7200));
    }

    [Test]
    public void PraktikantUdlStald_Saturday_PastFirstOvertimeStep_RemainderOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SATURDAY", 43200, ruleSet, CalculatedAt);

        AssertTierSplit(result, 43200,
            ("SAT_NORMAL", 26640), ("OVERTIME_50", 7200), ("OVERTIME_80", 9360));
    }

    // --- Staldarbejde SUNDAY: ANIMAL_SUN_HOLIDAY -> OVERTIME_50 -> OVERTIME_80 ---

    [Test]
    public void PraktikantUdlStald_Sunday_AtDailyNorm_NoOvertime()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", DailyNormSeconds, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds, ("ANIMAL_SUN_HOLIDAY", 26640));
    }

    [Test]
    public void PraktikantUdlStald_Sunday_OneSecondOverDailyNorm_OneSecondOvertime50()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", DailyNormSeconds + 1, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds + 1, ("ANIMAL_SUN_HOLIDAY", 26640), ("OVERTIME_50", 1));
    }

    [Test]
    public void PraktikantUdlStald_Sunday_AtFirstOvertimeStepCeiling_NoOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", FirstOvertimeStepEnd, ruleSet, CalculatedAt);

        AssertTierSplit(result, FirstOvertimeStepEnd, ("ANIMAL_SUN_HOLIDAY", 26640), ("OVERTIME_50", 7200));
    }

    [Test]
    public void PraktikantUdlStald_Sunday_PastFirstOvertimeStep_RemainderOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", 43200, ruleSet, CalculatedAt);

        AssertTierSplit(result, 43200,
            ("ANIMAL_SUN_HOLIDAY", 26640), ("OVERTIME_50", 7200), ("OVERTIME_80", 9360));
    }

    // --- Staldarbejde HOLIDAY: ANIMAL_SUN_HOLIDAY -> OVERTIME_50 -> OVERTIME_80 ---

    [Test]
    public void PraktikantUdlStald_Holiday_AtDailyNorm_NoOvertime()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", DailyNormSeconds, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds, ("ANIMAL_SUN_HOLIDAY", 26640));
    }

    [Test]
    public void PraktikantUdlStald_Holiday_OneSecondOverDailyNorm_OneSecondOvertime50()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", DailyNormSeconds + 1, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds + 1, ("ANIMAL_SUN_HOLIDAY", 26640), ("OVERTIME_50", 1));
    }

    [Test]
    public void PraktikantUdlStald_Holiday_AtFirstOvertimeStepCeiling_NoOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", FirstOvertimeStepEnd, ruleSet, CalculatedAt);

        AssertTierSplit(result, FirstOvertimeStepEnd, ("ANIMAL_SUN_HOLIDAY", 26640), ("OVERTIME_50", 7200));
    }

    [Test]
    public void PraktikantUdlStald_Holiday_PastFirstOvertimeStep_RemainderOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", 43200, ruleSet, CalculatedAt);

        AssertTierSplit(result, 43200,
            ("ANIMAL_SUN_HOLIDAY", 26640), ("OVERTIME_50", 7200), ("OVERTIME_80", 9360));
    }

    // --- Staldarbejde GRUNDLOVSDAG: NORMAL -> OVERTIME_50 -> OVERTIME_80 ---

    [Test]
    public void PraktikantUdlStald_Grundlovsdag_AtDailyNorm_NoOvertime()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", DailyNormSeconds, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds, ("NORMAL", 26640));
    }

    [Test]
    public void PraktikantUdlStald_Grundlovsdag_OneSecondOverDailyNorm_OneSecondOvertime50()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", DailyNormSeconds + 1, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds + 1, ("NORMAL", 26640), ("OVERTIME_50", 1));
    }

    [Test]
    public void PraktikantUdlStald_Grundlovsdag_AtFirstOvertimeStepCeiling_NoOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", FirstOvertimeStepEnd, ruleSet, CalculatedAt);

        AssertTierSplit(result, FirstOvertimeStepEnd, ("NORMAL", 26640), ("OVERTIME_50", 7200));
    }

    [Test]
    public void PraktikantUdlStald_Grundlovsdag_PastFirstOvertimeStep_RemainderOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Staldarbejde();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 43200, ruleSet, CalculatedAt);

        AssertTierSplit(result, 43200,
            ("NORMAL", 26640), ("OVERTIME_50", 7200), ("OVERTIME_80", 9360));
    }

    // --- Andet arbejde GRUNDLOVSDAG: NORMAL -> OVERTIME_50 -> OVERTIME_80 ---

    [Test]
    public void PraktikantUdlAndet_Grundlovsdag_AtDailyNorm_NoOvertime()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", DailyNormSeconds, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds, ("NORMAL", 26640));
    }

    [Test]
    public void PraktikantUdlAndet_Grundlovsdag_OneSecondOverDailyNorm_OneSecondOvertime50()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", DailyNormSeconds + 1, ruleSet, CalculatedAt);

        AssertTierSplit(result, DailyNormSeconds + 1, ("NORMAL", 26640), ("OVERTIME_50", 1));
    }

    [Test]
    public void PraktikantUdlAndet_Grundlovsdag_AtFirstOvertimeStepCeiling_NoOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", FirstOvertimeStepEnd, ruleSet, CalculatedAt);

        AssertTierSplit(result, FirstOvertimeStepEnd, ("NORMAL", 26640), ("OVERTIME_50", 7200));
    }

    [Test]
    public void PraktikantUdlAndet_Grundlovsdag_PastFirstOvertimeStep_RemainderOvertime80()
    {
        var ruleSet = OverenskomstFixtureHelper.GlsA_Jordbrug_Praktikant_Udenlandsk_Andet();
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", 43200, ruleSet, CalculatedAt);

        AssertTierSplit(result, 43200,
            ("NORMAL", 26640), ("OVERTIME_50", 7200), ("OVERTIME_80", 9360));
    }
}
