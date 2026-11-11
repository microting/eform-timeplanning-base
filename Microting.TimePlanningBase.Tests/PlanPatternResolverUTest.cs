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
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class PlanPatternResolverUTest
{
    // 2026-01-05 is a Monday.
    private static readonly DateTime Anchor = new(2026, 1, 5);

    private static PlanPattern Pattern(int rotationWeeks, DateTime? validFrom = null) => new()
    {
        RotationWeeks = rotationWeeks,
        AnchorDate = Anchor,
        ValidFrom = validFrom ?? Anchor
    };

    private static PlanPatternDay Day(int weekIndex, int dayOfWeek, string planText) => new()
    {
        WeekIndex = weekIndex,
        DayOfWeek = dayOfWeek,
        PlanText = planText,
        WorkflowState = Constants.WorkflowStates.Created
    };

    // One distinct text per (week, day) so a wrong index is visible.
    private static List<PlanPatternDay> FullRotation(int rotationWeeks)
    {
        var days = new List<PlanPatternDay>();
        for (var w = 0; w < rotationWeeks; w++)
        {
            for (var d = 0; d < 7; d++)
            {
                days.Add(Day(w, d, $"w{w}d{d}"));
            }
        }
        return days;
    }

    [Test]
    public void Rotation1_EveryWeekUsesWeekZero()
    {
        var pattern = Pattern(1);
        var days = FullRotation(1);

        Assert.Multiple(() =>
        {
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor), Is.EqualTo("w0d0"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(9)), Is.EqualTo("w0d2"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(7 * 52 + 6)), Is.EqualTo("w0d6"));
        });
    }

    [Test]
    public void Rotation5_CyclesThroughWeeks()
    {
        var pattern = Pattern(5);
        var days = FullRotation(5);

        Assert.Multiple(() =>
        {
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(7 * 1 + 1)), Is.EqualTo("w1d1"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(7 * 4 + 4)), Is.EqualTo("w4d4"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(7 * 5)), Is.EqualTo("w0d0"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(7 * 13 + 3)), Is.EqualTo("w3d3"));
        });
    }

    [Test]
    public void Rotation8_LastWeekAndWrap()
    {
        var pattern = Pattern(8);
        var days = FullRotation(8);

        Assert.Multiple(() =>
        {
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(7 * 7 + 6)), Is.EqualTo("w7d6"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(7 * 8)), Is.EqualTo("w0d0"));
        });
    }

    [Test]
    public void DateBeforeAnchor_UsesPositiveModulo()
    {
        // ValidFrom well before the anchor so the pattern governs these dates.
        var pattern = Pattern(5, validFrom: Anchor.AddDays(-365));
        var days = FullRotation(5);

        Assert.Multiple(() =>
        {
            // anchor - 1 day is the Sunday of the last rotation week.
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(-1)), Is.EqualTo("w4d6"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(-7)), Is.EqualTo("w4d0"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(-8)), Is.EqualTo("w3d6"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(-35)), Is.EqualTo("w0d0"));
        });
    }

    [Test]
    public void DateBeforeValidFrom_ReturnsNull()
    {
        var pattern = Pattern(2, validFrom: Anchor.AddDays(14));
        var days = FullRotation(2);

        Assert.Multiple(() =>
        {
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(13)), Is.Null);
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(14)), Is.EqualTo("w0d0"));
        });
    }

    [Test]
    public void ValidFromComparedByDateOnly()
    {
        // ValidFrom carries a time; the same calendar day at an earlier time still counts.
        var pattern = Pattern(1, validFrom: Anchor.AddHours(12));
        var days = FullRotation(1);

        Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddHours(1)), Is.EqualTo("w0d0"));
    }

    [Test]
    public void NullPattern_ReturnsNull()
    {
        Assert.That(PlanPatternResolver.PlanTextFor(null, FullRotation(1), Anchor), Is.Null);
    }

    [Test]
    public void MissingOrEmptyDay_ReturnsEmptyString()
    {
        var pattern = Pattern(1);
        var days = new List<PlanPatternDay>
        {
            Day(0, 0, "7:00-15:00/0.5"),
            Day(0, 1, ""),
            Day(0, 2, null)
        };

        Assert.Multiple(() =>
        {
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor), Is.EqualTo("7:00-15:00/0.5"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(1)), Is.EqualTo(""));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(2)), Is.EqualTo(""));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(3)), Is.EqualTo(""));
        });
    }

    [Test]
    public void RemovedDay_IsIgnored()
    {
        var pattern = Pattern(1);
        var removed = Day(0, 0, "7:00-15:00");
        removed.WorkflowState = Constants.WorkflowStates.Removed;

        Assert.That(PlanPatternResolver.PlanTextFor(pattern, new[] { removed }, Anchor), Is.EqualTo(""));
    }

    [Test]
    public void TimeOfDay_IsIgnored()
    {
        var pattern = Pattern(2);
        var days = FullRotation(2);

        Assert.Multiple(() =>
        {
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(8).AddHours(23).AddMinutes(59)), Is.EqualTo("w1d1"));
            Assert.That(PlanPatternResolver.PlanTextFor(pattern, days, Anchor.AddDays(-1).AddHours(23)), Is.Null);
        });
    }

    [Test]
    public void AnchorNotMonday_Throws()
    {
        var pattern = Pattern(1);
        pattern.AnchorDate = new DateTime(2026, 1, 7); // Wednesday

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PlanPatternResolver.PlanTextFor(pattern, FullRotation(1), Anchor));
    }

    [TestCase(0)]
    [TestCase(9)]
    [TestCase(-1)]
    public void RotationWeeksOutOfRange_Throws(int rotationWeeks)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PlanPatternResolver.PlanTextFor(Pattern(rotationWeeks), new List<PlanPatternDay>(), Anchor));
    }
}
