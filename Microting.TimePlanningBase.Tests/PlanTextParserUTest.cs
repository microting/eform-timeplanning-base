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
using System.Linq;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class PlanTextParserUTest
{
    /// <summary>
    /// Asserts a PlanText yields exactly one shift with the given minutes.
    /// The break defaults to 0, so every case also pins "no break" rather
    /// than leaving it unasserted.
    /// </summary>
    private static void AssertSingleShift(string planText, int start, int end, int breakMinutes = 0)
    {
        var shifts = PlanTextParser.Parse(planText).Where(s => s.HasValue).Select(s => s!.Value).ToList();

        Assert.That(shifts, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(shifts[0].StartMinutes, Is.EqualTo(start), "start");
            Assert.That(shifts[0].EndMinutes, Is.EqualTo(end), "end");
            Assert.That(shifts[0].BreakMinutes, Is.EqualTo(breakMinutes), "break");
        });
    }

    private static void AssertNoShifts(string planText)
    {
        Assert.That(PlanTextParser.Parse(planText), Is.All.Null);
    }

    //  Break: decimal hours, no one-hour ceiling.
    //  Everything above 60 is what the old lookup table silently zeroed.

    [TestCase("0", 0)]
    [TestCase("0.5", 30)]
    [TestCase("0,5", 30)]
    [TestCase(",5", 30)]
    [TestCase(".5", 30)]
    [TestCase("0,50", 30)]
    [TestCase("0.75", 45)]
    [TestCase("0,75", 45)]
    [TestCase("1", 60)]
    [TestCase("1.0", 60)]
    [TestCase("1,0", 60)]
    [TestCase("1.5", 90)]
    [TestCase("1,5", 90)]
    [TestCase("1.25", 75)]
    [TestCase("2", 120)]
    [TestCase("3", 180)]
    [TestCase("4.75", 285)]
    [TestCase("7", 420)]
    [TestCase("½", 30)]
    [TestCase("¾", 45)]
    [TestCase("1½", 90)]
    [TestCase("0:30", 30)]
    [TestCase("1:15", 75)]
    public void ParseBreakMinutes_ReadsDecimalHours(string input, int expected)
    {
        Assert.That(PlanTextParser.ParseBreakMinutes(input), Is.EqualTo(expected));
    }

    //  Break tokens carrying trailing prose, straight from production data.

    [TestCase("2 helligdag", 120)]
    [TestCase("2 arbejdsdag", 120)]
    [TestCase("½ + AT", 30)]
    [TestCase("½+AT", 30)]
    public void ParseBreakMinutes_IgnoresTrailingProse(string input, int expected)
    {
        Assert.That(PlanTextParser.ParseBreakMinutes(input), Is.EqualTo(expected));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("ferie")]
    [TestCase("-")]
    [TestCase(".")]
    [TestCase("-1")]
    [TestCase(null)]
    public void ParseBreakMinutes_UnrecognisedYieldsNoBreak(string input)
    {
        Assert.That(PlanTextParser.ParseBreakMinutes(input), Is.EqualTo(0));
    }

    //  A break cannot exceed a day. Without the clamp a large decimal
    //  saturates the cast to int and lands 2,147,483,647 minutes in the DB.

    [TestCase("99999999999999999999")]
    [TestCase("100000")]
    [TestCase("25")]
    public void ParseBreakMinutes_ClampsToOneDay(string input)
    {
        Assert.That(PlanTextParser.ParseBreakMinutes(input), Is.EqualTo(24 * 60));
    }

    //  Times: two-digit fractions are clock minutes.

    [TestCase("7:30-15:30", 450, 930)]
    [TestCase("7.30-15.30", 450, 930)]
    [TestCase("7,30-15,30", 450, 930)]
    [TestCase("7.00-15.00", 420, 900)]
    [TestCase("7,00-15,00", 420, 900)]
    [TestCase("8.15-14.30", 495, 870)]
    [TestCase("06.45-16.00", 405, 960)]
    [TestCase("6,45-15,15", 405, 915)]
    public void Parse_TwoDigitFractionIsClockMinutes(string planText, int start, int end)
    {
        AssertSingleShift(planText, start, end);
    }

    [TestCase("8-16", 480, 960)]
    [TestCase("6-13", 360, 780)]
    public void Parse_BareHours(string planText, int start, int end)
    {
        AssertSingleShift(planText, start, end);
    }

    //  Times: single-digit shorthand. '3' is a dropped trailing zero and
    //  '5' a decimal half hour — different tenants, same half past.

    [TestCase("6.3-16", 390, 960)]
    [TestCase("6,3-16", 390, 960)]
    [TestCase("7-14.3", 420, 870)]
    [TestCase("6.3-14.3", 390, 870)]
    [TestCase("7,5-16", 450, 960)]
    [TestCase("7-13,5", 420, 810)]
    [TestCase("12,5-15", 750, 900)]
    [TestCase("7.0-10", 420, 600)]
    [TestCase("7½-16", 450, 960)]
    public void Parse_SingleDigitShorthand(string planText, int start, int end)
    {
        AssertSingleShift(planText, start, end);
    }

    //  A colon is always explicit clock minutes, so the shorthand above
    //  must not leak into it.

    [Test]
    public void Parse_ColonSingleDigitIsLiteralMinutes()
    {
        AssertSingleShift("7:3-16", 423, 960);
    }

    //  Ambiguous or impossible values are rejected rather than guessed.

    [TestCase("7,4-16")]
    [TestCase("7,1-16")]
    [TestCase("7,9-16")]
    [TestCase("7.75-15")]
    [TestCase("8.99-15")]
    [TestCase("7:75-15")]
    [TestCase("26.00-30.00")]
    [TestCase("24:30-25:00")]
    public void Parse_AmbiguousOrImpossibleIsRejected(string planText)
    {
        AssertNoShifts(planText);
    }

    //  The day boundary is checked on the resolved time, not the hour digits,
    //  so the cutoff does not land mid-hour.

    [TestCase("23:59-24:00", 1439, 1440)]
    [TestCase("8:00-24:00", 480, 1440)]
    public void Parse_MidnightIsALegalEndOfDay(string planText, int start, int end)
    {
        AssertSingleShift(planText, start, end);
    }

    //  A string with no '-' is plan hours, not a shift.

    [TestCase("7,5")]
    [TestCase("7,4")]
    [TestCase("6,5")]
    [TestCase("8")]
    [TestCase("ferie")]
    [TestCase("")]
    [TestCase(null)]
    public void Parse_WithoutRangeIsNotAShift(string planText)
    {
        AssertNoShifts(planText);
    }

    //  Non-conforming text around a real shift is stripped, never fatal.

    [TestCase("8-16 hjemme", 480, 960)]
    [TestCase("ferie 7,5-15,5", 450, 930)]
    [TestCase("  8:00-16:00  ", 480, 960)]
    public void Parse_SalvagesConformingPart(string planText, int start, int end)
    {
        AssertSingleShift(planText, start, end);
    }

    [Test]
    public void Parse_SalvagesShiftAndBreakFromNoisyCell()
    {
        AssertSingleShift("7,5-15,5/0,5 !!", 450, 930, 30);
    }

    //  Salvage keeps scanning past a token that looks like a shift but carries
    //  an ambiguous fraction or an impossible hour.

    [TestCase("7,4-16 8:00-16:00", 480, 960)]
    [TestCase("7.75-15 7:30-15:00", 450, 900)]
    [TestCase("26-30 8:00-16:00", 480, 960)]
    public void Parse_RetriesAfterAFailedValueCheck(string planText, int start, int end)
    {
        AssertSingleShift(planText, start, end);
    }

    //  A match must not start inside a longer number.

    [Test]
    public void Parse_DoesNotMatchInsideALongerNumber()
    {
        AssertNoShifts("100-200");
    }

    //  Five shifts, with and without breaks. The old service-plugin parser
    //  dropped shifts 3-5 entirely whenever they carried a break.

    [Test]
    public void Parse_FiveShiftsWithoutBreaks()
    {
        var shifts = PlanTextParser.Parse("6:00-8:00;9:00-11:00;12:00-14:00;15:00-17:00;18:00-20:00");

        Assert.That(shifts, Is.All.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(shifts[2]!.Value.StartMinutes, Is.EqualTo(720));
            Assert.That(shifts[4]!.Value.StartMinutes, Is.EqualTo(1080));
            Assert.That(shifts[4]!.Value.EndMinutes, Is.EqualTo(1200));
        });
    }

    [Test]
    public void Parse_FiveShiftsAllWithBreaks()
    {
        var shifts = PlanTextParser.Parse(
            "6:00-8:00/0.5;9:00-11:00/0.5;12:00-14:00/1;15:00-17:00/1.5;18:00-20:00/0.25");

        Assert.That(shifts, Is.All.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(shifts[2]!.Value.BreakMinutes, Is.EqualTo(60));
            Assert.That(shifts[3]!.Value.BreakMinutes, Is.EqualTo(90));
            Assert.That(shifts[4]!.Value.BreakMinutes, Is.EqualTo(15));
        });
    }

    [Test]
    public void Parse_IgnoresSegmentsBeyondTheFifth()
    {
        var shifts = PlanTextParser.Parse("6-7;8-9;10-11;12-13;14-15;16-17");

        Assert.That(shifts, Has.Length.EqualTo(PlanTextParser.MaxShifts));
        Assert.That(shifts[4]!.Value.StartMinutes, Is.EqualTo(840));
    }

    //  Slots are positional: a non-conforming segment leaves its own slot
    //  empty instead of pulling later segments forward.

    [Test]
    public void Parse_KeepsSegmentPositionWhenOneIsSkipped()
    {
        var shifts = PlanTextParser.Parse("8:00-12:00/0.25;ferie;13:00-17:00/0.5");

        Assert.Multiple(() =>
        {
            Assert.That(shifts[0]!.Value.BreakMinutes, Is.EqualTo(15));
            Assert.That(shifts[1], Is.Null, "the skipped segment must not be back-filled");
            Assert.That(shifts[2]!.Value.StartMinutes, Is.EqualTo(780));
            Assert.That(shifts[3], Is.Null);
            Assert.That(shifts[4], Is.Null);
        });
    }

    [Test]
    public void Parse_AlwaysReturnsAllSlotsSoCallersClearStaleShifts()
    {
        Assert.That(PlanTextParser.Parse("8-16"), Has.Length.EqualTo(PlanTextParser.MaxShifts));
        Assert.That(PlanTextParser.Parse(""), Has.Length.EqualTo(PlanTextParser.MaxShifts));
    }

    //  Nothing in the parse path may throw: a malformed cell must cost its own
    //  segment, never abort an import.

    [TestCase("8-16/99999999999999999999")]
    [TestCase("--")]
    [TestCase("; ;")]
    [TestCase(";;;;;;;;;;")]
    [TestCase("/")]
    [TestCase("8-")]
    [TestCase("-16")]
    [TestCase("½-½")]
    [TestCase("99:99-99:99")]
    [TestCase("\0")]
    [TestCase("8-16/")]
    [TestCase(null)]
    public void Parse_NeverThrows(string planText)
    {
        Assert.DoesNotThrow(() => PlanTextParser.Parse(planText));
        Assert.DoesNotThrow(() => PlanTextParser.ParseBreakMinutes(planText));
    }

    [Test]
    public void Parse_NeverThrowsOnAVeryLongToken()
    {
        var absurd = "8-16/" + new string('9', 400);

        Assert.DoesNotThrow(() => PlanTextParser.Parse(absurd));
    }

    //  Round trip: generate then parse must land on the same shifts, for every
    //  representable break rather than a sampled few.

    [Test]
    public void GenerateThenParse_RoundTripsEveryBreakValue()
    {
        for (var breakMinutes = 0; breakMinutes <= 600; breakMinutes++)
        {
            var original = new PlanTextParser.Shift(480, 960, breakMinutes);

            var reparsed = PlanTextParser.Parse(PlanTextParser.Generate([original]))[0];

            Assert.That(reparsed, Is.EqualTo(original), $"break of {breakMinutes} minutes");
        }
    }

    [Test]
    public void GenerateThenParse_RoundTripsEveryStartMinute()
    {
        for (var start = 0; start <= 1380; start++)
        {
            var original = new PlanTextParser.Shift(start, 1440, 7);

            var reparsed = PlanTextParser.Parse(PlanTextParser.Generate([original]))[0];

            Assert.That(reparsed, Is.EqualTo(original), $"start at minute {start}");
        }
    }

    [Test]
    public void Generate_OmitsEmptyAndNullSlots()
    {
        var text = PlanTextParser.Generate([
            new PlanTextParser.Shift(480, 960, 30),
            null,
            new PlanTextParser.Shift(0, 0, 0),
            new PlanTextParser.Shift(1020, 1200, 0)
        ]);

        Assert.That(text, Is.EqualTo("8:00-16:00/0.5;17:00-20:00"));
    }

    [Test]
    public void Generate_HandlesNullAndEmpty()
    {
        Assert.Multiple(() =>
        {
            Assert.That(PlanTextParser.Generate(null), Is.Empty);
            Assert.That(PlanTextParser.Generate([]), Is.Empty);
        });
    }

    //  Regression corpus: distinct shapes observed in production, asserted
    //  against the clock times their planners intend.

    [TestCase("7-15.30/1", 420, 930, 60)]
    [TestCase("6-13.30/½", 360, 810, 30)]
    [TestCase("16.30-22.30/½", 990, 1350, 30)]
    [TestCase("7.30-15.30/1", 450, 930, 60)]
    [TestCase("7.30-16.00/1", 450, 960, 60)]
    [TestCase("5.00-12.30/0.5", 300, 750, 30)]
    [TestCase("06.45-16.00/1", 405, 960, 60)]
    [TestCase("6,45-15,15/1", 405, 915, 60)]
    [TestCase("06.45-12.00/0,5", 405, 720, 30)]
    [TestCase("6-13.30/0.5", 360, 810, 30)]
    [TestCase("7-13.30/,5", 420, 810, 30)]
    [TestCase("6.30 -15.00/1", 390, 900, 60)]
    [TestCase("7,00-15,00/1", 420, 900, 60)]
    [TestCase("8:00-16:00/0,5", 480, 960, 30)]
    [TestCase("6.3-16/1", 390, 960, 60)]
    [TestCase("7-13,5/0,5", 420, 810, 30)]
    [TestCase("12,5-15/0", 750, 900, 0)]
    [TestCase("7.0-16.15/1", 420, 975, 60)]
    [TestCase("7-14.3/1", 420, 870, 60)]
    [TestCase("8.15-14.30/½", 495, 870, 30)]
    [TestCase("19.30-22.30", 1170, 1350, 0)]
    public void Parse_ProductionCorpus(string planText, int start, int end, int breakMinutes)
    {
        AssertSingleShift(planText, start, end, breakMinutes);
    }
}
