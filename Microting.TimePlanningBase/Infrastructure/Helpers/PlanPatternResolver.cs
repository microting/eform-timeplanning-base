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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

/// <summary>
/// Resolves which <see cref="PlanPatternDay"/> of a <see cref="PlanPattern"/>
/// applies to a calendar date.
///
/// Lives beside <see cref="PlanRegistrationPlanText"/> so the plugin and the
/// background service generate PlanRegistration rows from one shared rule.
/// </summary>
public static class PlanPatternResolver
{
    public const int MinRotationWeeks = 1;
    public const int MaxRotationWeeks = 8;

    /// <summary>
    /// The plan text the pattern prescribes for <paramref name="date"/>. Only
    /// the date component of every DateTime is used.
    ///
    /// The rotation week is floor((date - AnchorDate).Days / 7) mod
    /// RotationWeeks, with a positive modulo so dates before the anchor still
    /// map into 0 .. RotationWeeks - 1 (anchor - 1 day is the Sunday of the
    /// last week). The day is the ISO day of week, 0 = Monday ... 6 = Sunday.
    /// Days with WorkflowState removed are ignored.
    /// </summary>
    /// <returns>
    /// <c>null</c> when the pattern does not govern the date (no pattern, or
    /// the date is before ValidFrom); <c>""</c> when it governs the date but
    /// prescribes no plan (no matching day, or its PlanText is null/empty);
    /// otherwise the day's PlanText.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// RotationWeeks is outside 1 .. 8, or AnchorDate is not a Monday.
    /// </exception>
    public static string? PlanTextFor(PlanPattern? pattern, IEnumerable<PlanPatternDay>? days, DateTime date)
    {
        if (pattern is null)
        {
            return null;
        }

        if (pattern.RotationWeeks is < MinRotationWeeks or > MaxRotationWeeks)
        {
            throw new ArgumentOutOfRangeException(nameof(pattern),
                pattern.RotationWeeks,
                $"RotationWeeks must be between {MinRotationWeeks} and {MaxRotationWeeks}.");
        }

        if (IsoDayOfWeek(pattern.AnchorDate) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pattern),
                pattern.AnchorDate,
                "AnchorDate must be a Monday.");
        }

        var day = date.Date;
        if (day < pattern.ValidFrom.Date)
        {
            return null;
        }

        var dayOffset = (day - pattern.AnchorDate.Date).Days;
        var rotationDays = pattern.RotationWeeks * 7;
        var dayInRotation = ((dayOffset % rotationDays) + rotationDays) % rotationDays;
        var weekIndex = dayInRotation / 7;
        var dayOfWeek = IsoDayOfWeek(day);

        var match = days?.FirstOrDefault(d =>
            d.WorkflowState != Constants.WorkflowStates.Removed
            && d.WeekIndex == weekIndex
            && d.DayOfWeek == dayOfWeek);

        return match?.PlanText ?? "";
    }

    /// <summary>0 = Monday ... 6 = Sunday.</summary>
    private static int IsoDayOfWeek(DateTime date) => ((int)date.DayOfWeek + 6) % 7;
}
