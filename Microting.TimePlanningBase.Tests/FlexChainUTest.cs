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
}
