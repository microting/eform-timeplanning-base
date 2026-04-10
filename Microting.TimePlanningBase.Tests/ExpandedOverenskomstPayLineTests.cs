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
}
