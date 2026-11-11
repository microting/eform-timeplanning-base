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

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

/// <summary>
/// A repeating weekly plan for one worker. A pattern is only a generator: it
/// never replaces <see cref="PlanRegistration"/> rows, it is used to fill
/// PlanText on them, one row per day.
/// One active (non-removed) pattern per AssignedSiteId; enforced by the writer, not by the index.
/// </summary>
public class PlanPattern : PnBase
{
    public int AssignedSiteId { get; set; }
    public virtual AssignedSite AssignedSite { get; set; }

    /// <summary>Length of the rotation in weeks, valid 1-8.</summary>
    public int RotationWeeks { get; set; }

    /// <summary>The Monday that is week 1 (WeekIndex 0) of the rotation.</summary>
    public DateTime AnchorDate { get; set; }

    /// <summary>The pattern governs dates on or after this date.</summary>
    public DateTime ValidFrom { get; set; }

    public virtual ICollection<PlanPatternDay> Days { get; set; } = new List<PlanPatternDay>();
}
