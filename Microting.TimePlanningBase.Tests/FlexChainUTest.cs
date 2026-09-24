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
public class FlexChainUTest
{
    [Test]
    public void SecondsOrDecimalFallback_PrefersThePopulatedSecondsColumn()
    {
        Assert.That(FlexChain.SecondsOrDecimalFallback(7200, 5.0), Is.EqualTo(7200));
    }

    [Test]
    public void SecondsOrDecimalFallback_DerivesFromTheDecimalWhenSecondsAreZero()
    {
        Assert.That(FlexChain.SecondsOrDecimalFallback(0, 5.0), Is.EqualTo(18000));
    }

    [Test]
    public void ApplyNettoFlexChainDecimal_CarriesTheBalanceAndClearsSeconds()
    {
        var pre = new PlanRegistration { SumFlexEnd = 12.5, SumFlexEndInSeconds = 45000 };
        var pr = new PlanRegistration
        {
            NettoHours = 8.0, PlanHours = 7.5, PaiedOutFlex = 0,
            SumFlexStartInSeconds = 999, SumFlexEndInSeconds = 999
        };

        FlexChain.ApplyNettoFlexChainDecimal(pr, pre);

        Assert.Multiple(() =>
        {
            Assert.That(pr.SumFlexStart, Is.EqualTo(12.5));
            Assert.That(pr.Flex, Is.EqualTo(0.5));
            Assert.That(pr.SumFlexEnd, Is.EqualTo(13.0));
            Assert.That(pr.SumFlexStartInSeconds, Is.EqualTo(0),
                "a five-minute row must carry no seconds");
            Assert.That(pr.SumFlexEndInSeconds, Is.EqualTo(0));
        });
    }

    [Test]
    public void SumFlexEndSecondsWithFallback_IgnoresStaleSecondsOnAFiveMinutePredecessor()
    {
        var pre = new PlanRegistration { SumFlexEnd = -3.97, SumFlexEndInSeconds = -290456 };

        Assert.Multiple(() =>
        {
            Assert.That(FlexChain.SumFlexEndSecondsWithFallback(pre, preIsOneMinute: false),
                Is.EqualTo(-14292), "five-minute predecessor: derive from the decimal");
            Assert.That(FlexChain.SumFlexEndSecondsWithFallback(pre, preIsOneMinute: true),
                Is.EqualTo(-290456), "one-minute predecessor: trust the column");
            Assert.That(FlexChain.SumFlexEndSecondsWithFallback(null), Is.EqualTo(0));
        });
    }

    [Test]
    public void ComputeNettoMinutesFlagOff_CountsAClosedShiftMinusItsPause()
    {
        // 07:00 (id 85) to 15:00 (id 181) = 96 ticks = 480 min; Pause1Id 7 = 30 min
        var pr = new PlanRegistration { Start1Id = 85, Stop1Id = 181, Pause1Id = 7 };

        Assert.That(FlexChain.ComputeNettoMinutesFlagOff(pr), Is.EqualTo(450));
    }

    [Test]
    public void ComputeNettoMinutesFlagOff_OpenShiftCountsZeroAndIgnoresItsPause()
    {
        var pr = new PlanRegistration { Start1Id = 115, Stop1Id = 0, Pause1Id = 7 };

        Assert.That(FlexChain.ComputeNettoMinutesFlagOff(pr), Is.EqualTo(0));
    }

    [Test]
    public void ComputeNettoMinutesFlagOff_StopBeforeStartCountsZero()
    {
        var pr = new PlanRegistration { Start1Id = 181, Stop1Id = 85, Pause1Id = 1 };

        Assert.That(FlexChain.ComputeNettoMinutesFlagOff(pr), Is.EqualTo(0));
    }

    [Test]
    public void ComputeNettoMinutesFlagOff_OneOpenShiftDoesNotCancelAClosedOne()
    {
        var pr = new PlanRegistration
        {
            Start1Id = 85, Stop1Id = 133, Pause1Id = 1,   // 4 h, no pause
            Start2Id = 157, Stop2Id = 0, Pause2Id = 4     // open second shift
        };

        Assert.That(FlexChain.ComputeNettoMinutesFlagOff(pr), Is.EqualTo(240));
    }

    [Test]
    public void ComputeNettoMinutesFlagOff_SumsShiftsThreeToFive()
    {
        var pr = new PlanRegistration
        {
            Start3Id = 13, Stop3Id = 25,   // 60 min
            Start4Id = 37, Stop4Id = 43,   // 30 min
            Start5Id = 61, Stop5Id = 64    // 15 min
        };

        Assert.That(FlexChain.ComputeNettoMinutesFlagOff(pr), Is.EqualTo(105));
    }

    [Test]
    public void CarryChain_OneMinute_UsesStoredSecondsAndIgnoresStamps()
    {
        var pre = new PlanRegistration { SumFlexEnd = 2.0, SumFlexEndInSeconds = 7200 };
        var pr = new PlanRegistration
        {
            NettoHoursInSeconds = 28800, NettoHours = 8.0,            // stored 8 h
            PlanHours = 7.5, PlanHoursInSeconds = 27000,
            Start1StartedAt = new System.DateTime(2026, 1, 5, 12, 0, 0),  // stale stamps: 1 h
            Stop1StoppedAt = new System.DateTime(2026, 1, 5, 13, 0, 0)
        };

        FlexChain.CarryChain(pr, pre, rowIsOneMinute: true, predecessorIsOneMinute: true);

        Assert.Multiple(() =>
        {
            Assert.That(pr.NettoHoursInSeconds, Is.EqualTo(28800), "hours are never recomputed");
            Assert.That(pr.NettoHours, Is.EqualTo(8.0));
            Assert.That(pr.FlexInSeconds, Is.EqualTo(1800));
            Assert.That(pr.SumFlexStartInSeconds, Is.EqualTo(7200));
            Assert.That(pr.SumFlexEndInSeconds, Is.EqualTo(9000));
            Assert.That(pr.SumFlexEnd, Is.EqualTo(2.5));
        });
    }

    [Test]
    public void CarryChain_OneMinute_FallsBackToDecimalsWhenSecondsAreZero()
    {
        // legacy predecessor: seconds 0, decimal balance 10 h
        var pre = new PlanRegistration { SumFlexEnd = 10.0, SumFlexEndInSeconds = 0 };
        var pr = new PlanRegistration { NettoHours = 7.0, NettoHoursInSeconds = 0, PlanHours = 7.5 };

        FlexChain.CarryChain(pr, pre, rowIsOneMinute: true, predecessorIsOneMinute: true);

        Assert.Multiple(() =>
        {
            Assert.That(pr.SumFlexStartInSeconds, Is.EqualTo(36000));
            Assert.That(pr.SumFlexEndInSeconds, Is.EqualTo(34200));
            Assert.That(pr.NettoHoursInSeconds, Is.EqualTo(0), "the fallback is read-only");
        });
    }

    [Test]
    public void CarryChain_OneMinute_UsesTheOverrideWhenActive()
    {
        var pre = new PlanRegistration { SumFlexEnd = 0, SumFlexEndInSeconds = 0 };
        var pr = new PlanRegistration
        {
            NettoHoursInSeconds = 28800, NettoHours = 8.0,
            NettoHoursOverrideActive = true, NettoHoursOverride = 10.0,
            PlanHours = 8.0
        };

        FlexChain.CarryChain(pr, pre, rowIsOneMinute: true, predecessorIsOneMinute: true);

        Assert.Multiple(() =>
        {
            Assert.That(pr.FlexInSeconds, Is.EqualTo(7200));
            Assert.That(pr.Flex, Is.EqualTo(2.0));
            Assert.That(pr.SumFlexEndInSeconds / 3600.0, Is.EqualTo(pr.SumFlexEnd).Within(1e-9),
                "decimal and seconds columns stay in step");
        });
    }

    [Test]
    public void CarryChain_FiveMinute_MatchesTheDecimalChainAndClearsSeconds()
    {
        var pre = new PlanRegistration { SumFlexEnd = 12.5, SumFlexEndInSeconds = 45000 };
        var pr = new PlanRegistration
        {
            NettoHours = 8.0, PlanHours = 7.5, PaiedOutFlex = 1.0,
            SumFlexStartInSeconds = 999, SumFlexEndInSeconds = 999
        };

        FlexChain.CarryChain(pr, pre, rowIsOneMinute: false, predecessorIsOneMinute: false);

        Assert.Multiple(() =>
        {
            Assert.That(pr.SumFlexStart, Is.EqualTo(12.5));
            Assert.That(pr.SumFlexEnd, Is.EqualTo(12.0));
            Assert.That(pr.SumFlexEndInSeconds, Is.EqualTo(0));
            Assert.That(pr.NettoHours, Is.EqualTo(8.0));
        });
    }

    [Test]
    public void CarryChain_OneMinuteAfterFiveMinute_SeedsFromTheDecimalNotStaleSeconds()
    {
        var pre = new PlanRegistration { SumFlexEnd = -3.97, SumFlexEndInSeconds = -290456 };
        var pr = new PlanRegistration { NettoHoursInSeconds = 3600, NettoHours = 1.0, PlanHours = 1.0 };

        FlexChain.CarryChain(pr, pre, rowIsOneMinute: true, predecessorIsOneMinute: false);

        Assert.That(pr.SumFlexStartInSeconds, Is.EqualTo(-14292));
    }

    [Test]
    public void CarryChain_FirstRow_StartsAtZero()
    {
        var pr = new PlanRegistration { NettoHoursInSeconds = 3600, NettoHours = 1.0, PlanHours = 0 };

        FlexChain.CarryChain(pr, null, rowIsOneMinute: true, predecessorIsOneMinute: null);

        Assert.Multiple(() =>
        {
            Assert.That(pr.SumFlexStartInSeconds, Is.EqualTo(0));
            Assert.That(pr.SumFlexEndInSeconds, Is.EqualTo(3600));
        });
    }
}
