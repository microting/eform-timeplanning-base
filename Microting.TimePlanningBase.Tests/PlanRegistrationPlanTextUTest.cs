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

using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class PlanRegistrationPlanTextUTest
{
    [Test]
    public void ParseInto_SingleShiftWithBreak()
    {
        var reg = new PlanRegistration { PlanText = "8:00-16:00/0.5" };

        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedStartOfShift1, Is.EqualTo(480));
            Assert.That(reg.PlannedEndOfShift1, Is.EqualTo(960));
            Assert.That(reg.PlannedBreakOfShift1, Is.EqualTo(30));
            Assert.That(reg.PlanHours, Is.EqualTo(7.5));
        });
    }

    //  The break above one hour is the value the old parser dropped to zero,
    //  which left PlanHours reporting the full shift.

    [Test]
    public void ParseInto_DeductsBreaksAboveAnHour()
    {
        var reg = new PlanRegistration { PlanText = "8:00-16:00/1.5" };

        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedBreakOfShift1, Is.EqualTo(90));
            Assert.That(reg.PlanHours, Is.EqualTo(6.5));
        });
    }

    [Test]
    public void ParseInto_FiveShiftsEachWithABreak()
    {
        var reg = new PlanRegistration
        {
            PlanText = "6:00-8:00/0.5;9:00-11:00/0.5;12:00-14:00/1;15:00-17:00/1.5;18:00-20:00/0.25"
        };

        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedStartOfShift3, Is.EqualTo(720), "shift 3 start");
            Assert.That(reg.PlannedBreakOfShift3, Is.EqualTo(60), "shift 3 break");
            Assert.That(reg.PlannedStartOfShift4, Is.EqualTo(900), "shift 4 start");
            Assert.That(reg.PlannedBreakOfShift4, Is.EqualTo(90), "shift 4 break");
            Assert.That(reg.PlannedStartOfShift5, Is.EqualTo(1080), "shift 5 start");
            Assert.That(reg.PlannedBreakOfShift5, Is.EqualTo(15), "shift 5 break");
        });
    }

    //  A shortened PlanText must clear the shifts it no longer mentions,
    //  rather than leaving the previous values in place.

    [Test]
    public void ParseInto_ClearsShiftsDroppedFromTheText()
    {
        var reg = new PlanRegistration
        {
            PlanText = "6:00-8:00;9:00-11:00;12:00-14:00;15:00-17:00;18:00-20:00"
        };
        PlanRegistrationPlanText.ParseInto(reg);

        reg.PlanText = "6:00-8:00";
        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedStartOfShift1, Is.EqualTo(360));
            Assert.That(reg.PlannedStartOfShift2, Is.Zero, "shift 2 must be cleared");
            Assert.That(reg.PlannedEndOfShift2, Is.Zero);
            Assert.That(reg.PlannedStartOfShift3, Is.Zero, "shift 3 must be cleared");
            Assert.That(reg.PlannedStartOfShift4, Is.Zero, "shift 4 must be cleared");
            Assert.That(reg.PlannedStartOfShift5, Is.Zero, "shift 5 must be cleared");
        });
    }

    [Test]
    public void ParseInto_SkippedSegmentClearsItsOwnSlotOnly()
    {
        var reg = new PlanRegistration { PlanText = "8:00-12:00;ferie;13:00-17:00" };

        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedStartOfShift1, Is.EqualTo(480));
            Assert.That(reg.PlannedStartOfShift2, Is.Zero, "the skipped segment must not be back-filled");
            Assert.That(reg.PlannedStartOfShift3, Is.EqualTo(780));
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("ferie")]
    [TestCase("7,5")]
    public void ParseInto_NonShiftTextClearsEveryShift(string planText)
    {
        var reg = new PlanRegistration { PlanText = "8:00-16:00/0.5" };
        PlanRegistrationPlanText.ParseInto(reg);

        reg.PlanText = planText;
        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedStartOfShift1, Is.Zero);
            Assert.That(reg.PlannedEndOfShift1, Is.Zero);
            Assert.That(reg.PlannedBreakOfShift1, Is.Zero);
        });
    }

    //  Clearing the columns without clearing PlanHours would leave the row
    //  saying "no shift, seven planned hours", and the unchanged PlanHours
    //  would also stop callers noticing the day had changed at all.

    [TestCase("ferie")]
    [TestCase("fri")]
    [TestCase("7,5")]
    [TestCase("0-0")]
    public void ParseInto_TextWithoutAShiftAlsoZeroesPlanHours(string planText)
    {
        var reg = new PlanRegistration { PlanText = "7:00-15:00/1" };
        PlanRegistrationPlanText.ParseInto(reg);
        Assert.That(reg.PlanHours, Is.EqualTo(7.0), "arrange");

        reg.PlanText = planText;
        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedStartOfShift1, Is.Zero);
            Assert.That(reg.PlanHours, Is.Zero, "PlanHours must not survive the shift it came from");
        });
    }

    //  An empty PlanText is the exception: the sheet's separate hours column
    //  owns PlanHours in that case, so it must be left alone.

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void ParseInto_EmptyTextLeavesPlanHoursToTheHoursColumn(string planText)
    {
        var reg = new PlanRegistration { PlanText = planText, PlanHours = 7.4 };

        PlanRegistrationPlanText.ParseInto(reg);

        Assert.Multiple(() =>
        {
            Assert.That(reg.PlannedStartOfShift1, Is.Zero);
            Assert.That(reg.PlanHours, Is.EqualTo(7.4));
        });
    }

    //  A shift whose end is before its start crosses midnight. Summed naively
    //  it contributes a large negative and drags the whole day down with it.

    [TestCase("22:00-0:00", 2.0)]
    [TestCase("22:00-06:00", 8.0)]
    [TestCase("0:00-8:00", 8.0)]
    [TestCase("6:00-14:00;22:00-0:00", 10.0)]
    public void ParseInto_MidnightBoundedShiftsSumForwards(string planText, double expectedHours)
    {
        var reg = new PlanRegistration { PlanText = planText };

        PlanRegistrationPlanText.ParseInto(reg);

        Assert.That(reg.PlanHours, Is.EqualTo(expectedHours));
    }

    [Test]
    public void ParseInto_PlanHoursIsNeverNegative()
    {
        var reg = new PlanRegistration { PlanText = "8:00-9:00/4" };

        PlanRegistrationPlanText.ParseInto(reg);

        Assert.That(reg.PlanHours, Is.Zero);
    }

    [Test]
    public void Generate_IsTheInverseOfParseInto()
    {
        var reg = new PlanRegistration { PlanText = "8:00-12:00/0.25;13:00-17:00/1.5" };
        PlanRegistrationPlanText.ParseInto(reg);

        var generated = PlanRegistrationPlanText.Generate(reg);

        var roundTripped = new PlanRegistration { PlanText = generated };
        PlanRegistrationPlanText.ParseInto(roundTripped);

        Assert.Multiple(() =>
        {
            Assert.That(generated, Is.EqualTo("8:00-12:00/0.25;13:00-17:00/1.5"));
            Assert.That(roundTripped.PlannedStartOfShift2, Is.EqualTo(reg.PlannedStartOfShift2));
            Assert.That(roundTripped.PlannedBreakOfShift2, Is.EqualTo(reg.PlannedBreakOfShift2));
            Assert.That(roundTripped.PlanHours, Is.EqualTo(reg.PlanHours));
        });
    }

    [Test]
    public void ParseInto_ToleratesNull()
    {
        Assert.DoesNotThrow(() => PlanRegistrationPlanText.ParseInto(null));
        Assert.That(PlanRegistrationPlanText.Generate(null), Is.Empty);
    }
}
