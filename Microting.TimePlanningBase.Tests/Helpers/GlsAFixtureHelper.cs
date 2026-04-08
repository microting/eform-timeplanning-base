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
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Tests.Helpers;

/// <summary>
/// Static helper class that creates in-memory PayRuleSet objects for each
/// GLS-A / 3F Jordbrugsoverenskomsten 2024-2026 variant.
/// These fixtures are used by GlsAJordbrugPayLineTests to validate
/// tier-based pay-line splitting via PayLineGenerator.
/// </summary>
public static class GlsAFixtureHelper
{
    /// <summary>
    /// GLS-A Jordbrug Standard agriculture worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT80%.
    /// Saturday: tier-based noon split (6h morning / afternoon).
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    /// <remarks>
    /// Saturday noon split is approximated using tier-based UpToSeconds.
    /// A future time-band-aware generator (PayDayTypeRule + PayTimeBandRule)
    /// would allow splitting by actual clock time (before/after 12:00).
    /// </remarks>
    public static PayRuleSet GlsA_Jordbrug_Standard() => new PayRuleSet
    {
        Id = 100,
        Name = "GLS-A Jordbrug Standard",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 100,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },       // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },   // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 100,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    // Tier-based approximation of before-noon (6h) / afternoon split
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },    // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 100,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 100,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 100,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Jordbrug DyrePasning (animal care) worker.
    /// Same weekday overtime tiers as Standard, but animal-specific
    /// Saturday afternoon and Sunday/Holiday codes.
    /// </summary>
    /// <remarks>
    /// Animal care night supplement (00:00-05:00) requires time-band splitting
    /// which is not supported by the current PayLineGenerator. This will need
    /// a future time-band-aware generator (PayDayTypeRule + PayTimeBandRule).
    /// </remarks>
    public static PayRuleSet GlsA_Jordbrug_DyrePasning() => new PayRuleSet
    {
        Id = 101,
        Name = "GLS-A Jordbrug DyrePasning",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 101,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 101,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_ANIMAL_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 101,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ANIMAL_SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 101,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ANIMAL_SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 101,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Jordbrug Laerling Under 18 (apprentice under 18).
    /// Weekday: 8h normal, then OT50%.
    /// Saturday: 8h normal, then OT50%.
    /// Sunday/Holiday: 2h OT50%, then OT80%.
    /// </summary>
    public static PayRuleSet GlsA_Jordbrug_Laerling_Under18() => new PayRuleSet
    {
        Id = 102,
        Name = "GLS-A Jordbrug Laerling Under 18",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 102,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },      // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 102,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },  // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 102,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },    // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 102,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_HOL_OT_50" },    // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_HOL_OT_80" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Jordbrug Laerling Over 18 (apprentice over 18).
    /// Weekday: 7.4h normal, +2h OT30%, then OT80% (same as standard).
    /// Saturday: 6h normal (noon split), then afternoon.
    /// Sunday/Holiday: 2h OT50%, then OT80%.
    /// </summary>
    public static PayRuleSet GlsA_Jordbrug_Laerling_Over18() => new PayRuleSet
    {
        Id = 103,
        Name = "GLS-A Jordbrug Laerling Over 18",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 103,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "ELEV_NORMAL" },       // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "ELEV_OVERTIME_30" },   // 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "ELEV_OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 103,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "ELEV_SAT_NORMAL" },   // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 103,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },     // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 103,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_HOL_OT_50" },     // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_HOL_OT_80" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Jordbrug Laerling Under 18 DyrePasning (apprentice under 18, animal care).
    /// Combines Under-18 overtime tiers with animal care Saturday afternoon code.
    /// </summary>
    /// <remarks>
    /// Animal care night supplement (00:00-05:00) requires time-band splitting
    /// which is not supported by the current PayLineGenerator.
    /// </remarks>
    public static PayRuleSet GlsA_Jordbrug_Laerling_Under18_DyrePasning() => new PayRuleSet
    {
        Id = 104,
        Name = "GLS-A Jordbrug Laerling Under 18 DyrePasning",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 104,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 104,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_ANIMAL_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 104,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 104,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_HOL_OT_50" },
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_HOL_OT_80" }
                }
            }
        }
    };
}
