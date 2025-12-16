/*
The MIT License (MIT)

Copyright (c) 2007 - 2025 Microting A/S

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

using System.Collections.Generic;

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

public class BreakPolicy : PnBase
{
    public string Name { get; set; }

    // Optional condition: only apply split if the employee worked at least X minutes that day
    public int? AppliesOnlyIfWorkMinutesAtLeast { get; set; } // e.g. 360 (6 hours)

    // If true: any registered pause minutes beyond (unpaid+paid) count as unpaid
    public bool ExtraPauseCountsAsUnpaid { get; set; } = true;

    public virtual ICollection<BreakPolicyRule> Rules { get; set; } = new List<BreakPolicyRule>();
}
