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

using System.Collections.Generic;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Tests.Helpers;

/// <summary>
/// Static helper class that creates in-memory PayRuleSet objects for
/// expanded overenskomst presets (Gartneri, Skovbrug, KA Landbrug, KA Gron).
/// These fixtures are used by ExpandedOverenskomstPayLineTests to validate
/// tier-based pay-line splitting via PayLineGenerator.
/// </summary>
public static class OverenskomstFixtureHelper
{
    // ───────────────────────────────────────────────────────────────
    // GLS-A Gartneri
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// GLS-A Gartneri Standard worker.
    /// Weekday: 7.4h normal, +2h OT50%, then OT100%.
    /// Saturday: 6.5h morning (23400s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Gartneri_Standard() => new PayRuleSet
    {
        Id = 200,
        Name = "GLS-A / 3F - Gartneri Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 200,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_50" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 200,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 23400, PayCode = "SAT_NORMAL" },     // 6.5h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 200,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 200,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 200,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Gartneri Elev Under 18 (apprentice under 18).
    /// Weekday: 8h normal, then OT50%.
    /// Saturday: 8h normal, then OT50%.
    /// Sunday/Holiday: 2h OT50%, then OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Gartneri_Elev_Under18() => new PayRuleSet
    {
        Id = 201,
        Name = "GLS-A / 3F - Gartneri Elev u18 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 201,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 201,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 201,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 201,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 201,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Gartneri Elev Over 18 (apprentice over 18).
    /// Weekday: 7.4h normal, +2h OT50%, then OT100%.
    /// Saturday: 6.5h normal (23400s), then afternoon.
    /// Sunday/Holiday: 2h OT50%, then OT100% (same as u18).
    /// </summary>
    public static PayRuleSet GlsA_Gartneri_Elev_Over18() => new PayRuleSet
    {
        Id = 202,
        Name = "GLS-A / 3F - Gartneri Elev o18 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 202,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "ELEV_NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "ELEV_OVERTIME_50" },     // 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "ELEV_OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 202,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 23400, PayCode = "ELEV_SAT_NORMAL" },     // 6.5h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 202,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },       // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 202,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },       // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 202,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    // ───────────────────────────────────────────────────────────────
    // GLS-A Skovbrug
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// GLS-A Skovbrug Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT100%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Skovbrug_Standard() => new PayRuleSet
    {
        Id = 203,
        Name = "GLS-A / 3F - Skovbrug Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 203,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 203,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 203,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 203,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 203,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Skovbrug Elev Under 18 (apprentice under 18).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: 2h OT50%, then OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Skovbrug_Elev_Under18() => new PayRuleSet
    {
        Id = 204,
        Name = "GLS-A / 3F - Skovbrug Elev u18 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 204,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 204,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 204,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 204,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 204,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Skovbrug Elev Over 18 (apprentice over 18).
    /// Weekday: 7.4h normal, +2h OT30%, then OT100%.
    /// Saturday: 6h normal (21600s), then afternoon.
    /// Sunday/Holiday: 2h OT50%, then OT100% (same as u18).
    /// </summary>
    public static PayRuleSet GlsA_Skovbrug_Elev_Over18() => new PayRuleSet
    {
        Id = 205,
        Name = "GLS-A / 3F - Skovbrug Elev o18 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 205,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "ELEV_NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "ELEV_OVERTIME_30" },     // 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "ELEV_OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 205,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "ELEV_SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 205,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },       // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 205,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },       // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 205,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    // ───────────────────────────────────────────────────────────────
    // KA Landbrug - Svinebrug
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// KA Landbrug Svinebrug Standard worker.
    /// Weekday: 7.4h normal, +2h OT50%, then OT100%.
    /// Saturday: single flat tier (SAT_WORK).
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet KA_Landbrug_Svine_Standard() => new PayRuleSet
    {
        Id = 206,
        Name = "KA / Krifa - Landbrug Svine/Kvaeg Standard 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 206,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_50" },     // 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 206,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SAT_WORK" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 206,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 206,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 206,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// KA Landbrug Svinebrug Elev (apprentice).
    /// Weekday: 8h normal, then OT50%.
    /// Saturday: 8h normal, then OT50%.
    /// Sunday/Holiday: 2h OT50%, then OT100%.
    /// </summary>
    public static PayRuleSet KA_Landbrug_Svine_Elev() => new PayRuleSet
    {
        Id = 207,
        Name = "KA / Krifa - Landbrug Svine/Kvaeg Elev 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 207,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 207,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 207,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 207,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 207,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    // ───────────────────────────────────────────────────────────────
    // KA Landbrug - Plantebrug
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// KA Landbrug Plantebrug Standard worker.
    /// Weekday: 7.4h normal, +3h OT50% (37440s), then OT100%.
    /// Saturday: single flat tier (SAT_WORK).
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet KA_Landbrug_Plante_Standard() => new PayRuleSet
    {
        Id = 208,
        Name = "KA / Krifa - Landbrug Plantebrug Standard 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 208,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 37440, PayCode = "OVERTIME_50" },     // 7.4h + 3h = 10.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 208,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SAT_WORK" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 208,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 208,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 208,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// KA Landbrug Plantebrug Elev (apprentice).
    /// Weekday: 8h normal, then OT50%.
    /// Saturday: 8h normal, then OT50% (same as Svine Elev).
    /// Sunday/Holiday: 2h OT50%, then OT100% (same as Svine Elev).
    /// </summary>
    public static PayRuleSet KA_Landbrug_Plante_Elev() => new PayRuleSet
    {
        Id = 209,
        Name = "KA / Krifa - Landbrug Plantebrug Elev 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 209,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 209,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 209,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 209,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 209,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    // ───────────────────────────────────────────────────────────────
    // KA Landbrug - Maskinstation
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// KA Landbrug Maskinstation Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT80%.
    /// Saturday: single flat tier (SAT_WORK).
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet KA_Landbrug_Maskin_Standard() => new PayRuleSet
    {
        Id = 210,
        Name = "KA / Krifa - Landbrug Maskinstation Standard 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 210,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 210,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SAT_WORK" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 210,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 210,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 210,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// KA Landbrug Maskinstation Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: 2h OT30%, then OT80%.
    /// </summary>
    public static PayRuleSet KA_Landbrug_Maskin_Elev() => new PayRuleSet
    {
        Id = 211,
        Name = "KA / Krifa - Landbrug Maskinstation Elev 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 211,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 211,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 211,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_30" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 211,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_30" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 211,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    // ───────────────────────────────────────────────────────────────
    // KA Gron (Greenkeeper / Groundskeeper)
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// KA Gron Standard worker.
    /// Weekday: 7.4h normal, +3h OT50% (37440s), then OT100%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet KA_Gron_Standard() => new PayRuleSet
    {
        Id = 212,
        Name = "KA / Krifa - Gron Standard 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 212,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 37440, PayCode = "OVERTIME_50" },     // 7.4h + 3h = 10.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 212,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 212,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 212,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 212,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// KA Gron Elev (apprentice).
    /// Weekday: 8h normal, then OT50%.
    /// Saturday: 8h normal, then OT50%.
    /// Sunday/Holiday: 2h OT50%, then OT100%.
    /// </summary>
    public static PayRuleSet KA_Gron_Elev() => new PayRuleSet
    {
        Id = 213,
        Name = "KA / Krifa - Gron Elev 2025-2028",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 213,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 213,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_50" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 213,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 213,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 7200, PayCode = "ELEV_SUN_OT_50" },      // 2h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 213,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    // ───────────────────────────────────────────────────────────────
    // GLS-A Golf
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// GLS-A Golf Standard worker.
    /// Weekday: 7.4h normal, then flat OT100%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Golf_Standard() => new PayRuleSet
    {
        Id = 214,
        Name = "GLS-A / 3F - Golf Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 214,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 214,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 214,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 214,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 214,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Golf Elev (apprentice).
    /// Weekday: 8h normal, then OT100%.
    /// Saturday: 8h normal, then OT100%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Golf_Elev() => new PayRuleSet
    {
        Id = 215,
        Name = "GLS-A / 3F - Golf Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 215,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 215,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 215,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 215,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 215,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    // ───────────────────────────────────────────────────────────────
    // GLS-A Agroindustri
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// GLS-A Agroindustri Fjerkrae Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, +1h OT50%, then OT100%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Fjerkrae_Standard() => new PayRuleSet
    {
        Id = 216,
        Name = "GLS-A / 3F - Agroindustri Fjerkrae Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 216,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = 37440, PayCode = "OVERTIME_50" },     // 9.4h + 1h = 10.4h
                    new PayTierRule { Order = 4, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 216,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 216,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 216,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 216,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Fjerkrae Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Fjerkrae_Elev() => new PayRuleSet
    {
        Id = 217,
        Name = "GLS-A / 3F - Agroindustri Fjerkrae Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 217,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 217,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 217,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 217,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 217,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Grovvare Standard worker.
    /// Weekday: 7.4h normal, +3h OT40%, then OT100%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Grovvare_Standard() => new PayRuleSet
    {
        Id = 218,
        Name = "GLS-A / 3F - Agroindustri Grovvare Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 218,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 37440, PayCode = "OVERTIME_40" },     // 7.4h + 3h = 10.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 218,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 218,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 218,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 218,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Grovvare Elev (apprentice).
    /// Weekday: 8h normal, then OT40%.
    /// Saturday: 8h normal, then OT40%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Grovvare_Elev() => new PayRuleSet
    {
        Id = 219,
        Name = "GLS-A / 3F - Agroindustri Grovvare Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 219,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_40" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 219,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_40" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 219,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 219,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 219,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Gulerod Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT80%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Gulerod_Standard() => new PayRuleSet
    {
        Id = 220,
        Name = "GLS-A / 3F - Agroindustri Gulerod Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 220,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 220,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 220,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 220,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 220,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Gulerod Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Gulerod_Elev() => new PayRuleSet
    {
        Id = 221,
        Name = "GLS-A / 3F - Agroindustri Gulerod Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 221,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 221,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 221,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 221,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 221,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Kartoffelmel Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, +1h OT50%, then OT100%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Kartoffelmel_Standard() => new PayRuleSet
    {
        Id = 222,
        Name = "GLS-A / 3F - Agroindustri Kartoffelmel Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 222,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = 37440, PayCode = "OVERTIME_50" },     // 9.4h + 1h = 10.4h
                    new PayTierRule { Order = 4, UpToSeconds = null, PayCode = "OVERTIME_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 222,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 222,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 222,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 222,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Kartoffelmel Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Kartoffelmel_Elev() => new PayRuleSet
    {
        Id = 223,
        Name = "GLS-A / 3F - Agroindustri Kartoffelmel Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 223,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 223,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 223,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 223,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 223,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Kartoffelsorter Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT80%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Kartoffelsorter_Standard() => new PayRuleSet
    {
        Id = 224,
        Name = "GLS-A / 3F - Agroindustri Kartoffelsorter Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 224,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 224,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 224,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 224,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 224,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Kartoffelsorter Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Kartoffelsorter_Elev() => new PayRuleSet
    {
        Id = 225,
        Name = "GLS-A / 3F - Agroindustri Kartoffelsorter Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 225,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 225,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 225,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 225,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 225,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Lucerne Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT80%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Lucerne_Standard() => new PayRuleSet
    {
        Id = 226,
        Name = "GLS-A / 3F - Agroindustri Lucerne Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 226,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 226,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 226,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 226,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 226,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Lucerne Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Lucerne_Elev() => new PayRuleSet
    {
        Id = 227,
        Name = "GLS-A / 3F - Agroindustri Lucerne Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 227,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 227,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 227,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 227,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 227,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Minkfoder Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT80%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Minkfoder_Standard() => new PayRuleSet
    {
        Id = 228,
        Name = "GLS-A / 3F - Agroindustri Minkfoder Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 228,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 228,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 228,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 228,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 228,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Minkfoder Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Minkfoder_Elev() => new PayRuleSet
    {
        Id = 229,
        Name = "GLS-A / 3F - Agroindustri Minkfoder Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 229,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 229,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 229,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 229,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 229,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Ovrige Standard worker.
    /// Weekday: 7.4h normal, +2h OT30%, then OT80%.
    /// Saturday: 6h morning (21600s) / afternoon split.
    /// Sunday/Holiday: flat rate. Grundlovsdag: flat rate.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Ovrige_Standard() => new PayRuleSet
    {
        Id = 230,
        Name = "GLS-A / 3F - Agroindustri Ovrige Standard 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 230,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 26640, PayCode = "NORMAL" },         // 7.4h
                    new PayTierRule { Order = 2, UpToSeconds = 33840, PayCode = "OVERTIME_30" },     // 7.4h + 2h = 9.4h
                    new PayTierRule { Order = 3, UpToSeconds = null, PayCode = "OVERTIME_80" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 230,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 21600, PayCode = "SAT_NORMAL" },     // 6h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "SAT_AFTERNOON" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 230,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 230,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "SUN_HOLIDAY" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 230,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };

    /// <summary>
    /// GLS-A Agroindustri Ovrige Elev (apprentice).
    /// Weekday: 8h normal, then OT30%.
    /// Saturday: 8h normal, then OT30%.
    /// Sunday/Holiday: flat OT100%.
    /// </summary>
    public static PayRuleSet GlsA_Agro_Ovrige_Elev() => new PayRuleSet
    {
        Id = 231,
        Name = "GLS-A / 3F - Agroindustri Ovrige Elev 2024-2026",
        DayRules = new List<PayDayRule>
        {
            new PayDayRule
            {
                PayRuleSetId = 231,
                DayCode = "WEEKDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_NORMAL" },        // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 231,
                DayCode = "SATURDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = 28800, PayCode = "ELEV_SAT_NORMAL" },    // 8h
                    new PayTierRule { Order = 2, UpToSeconds = null, PayCode = "ELEV_SAT_OVERTIME_30" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 231,
                DayCode = "SUNDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_SUN_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 231,
                DayCode = "HOLIDAY",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "ELEV_HOL_OT_100" }
                }
            },
            new PayDayRule
            {
                PayRuleSetId = 231,
                DayCode = "GRUNDLOVSDAG",
                Tiers = new List<PayTierRule>
                {
                    new PayTierRule { Order = 1, UpToSeconds = null, PayCode = "GRUNDLOVSDAG" }
                }
            }
        }
    };
}
