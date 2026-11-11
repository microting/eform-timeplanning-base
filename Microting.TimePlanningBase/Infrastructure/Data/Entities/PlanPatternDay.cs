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

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

/// <summary>
/// One day of a <see cref="PlanPattern"/> rotation.
/// </summary>
public class PlanPatternDay : PnBase
{
    public int PlanPatternId { get; set; }
    public virtual PlanPattern PlanPattern { get; set; }

    /// <summary>Week of the rotation, 0 .. RotationWeeks - 1.</summary>
    public int WeekIndex { get; set; }

    /// <summary>
    /// ISO day of week, 0 = Monday ... 6 = Sunday. This is NOT
    /// <see cref="System.DayOfWeek"/> (where 0 = Sunday); it matches ISO weeks
    /// and the rule that <see cref="Entities.PlanPattern.AnchorDate"/> is a Monday.
    /// </summary>
    public int DayOfWeek { get; set; }

    /// <summary>
    /// Canonical plan text, e.g. <c>7:00-15:00/0.5</c>. Null or empty means
    /// no plan that day.
    /// </summary>
    public string PlanText { get; set; }
}
