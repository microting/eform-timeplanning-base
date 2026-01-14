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

using System;
using System.Collections.Generic;
using System.Linq;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;

namespace Microting.TimePlanningBase.Infrastructure.Helpers;

public static class PayLineGenerator
{
    public static List<PlanRegistrationPayLine> GeneratePayLines(
        int planRegistrationId,
        string dayCode,
        int totalSeconds,
        PayRuleSet payRuleSet,
        DateTime calculatedAtUtc)
    {
        var result = new List<PlanRegistrationPayLine>();

        // Find PayDayRule by dayCode
        var dayRule = payRuleSet?.DayRules?.SingleOrDefault(dr => dr.DayCode == dayCode);

        if (dayRule == null || dayRule.Tiers == null || !dayRule.Tiers.Any())
        {
            // Fallback: create a single "DEFAULT" line with all seconds
            result.Add(new PlanRegistrationPayLine
            {
                PlanRegistrationId = planRegistrationId,
                PayCode = "DEFAULT",
                HoursInSeconds = totalSeconds,
                Hours = totalSeconds / 3600.0,
                PayRuleSetId = payRuleSet?.Id,
                CalculatedAt = calculatedAtUtc
            });
            return result;
        }

        // Order tiers by Order field
        var orderedTiers = dayRule.Tiers.OrderBy(t => t.Order).ToList();

        int remainingSeconds = totalSeconds;
        int cumulativeSeconds = 0;

        foreach (var tier in orderedTiers)
        {
            if (remainingSeconds <= 0)
                break;

            int tierSeconds;
            if (tier.UpToSeconds.HasValue)
            {
                // Calculate how many seconds belong to this tier
                int tierCap = tier.UpToSeconds.Value - cumulativeSeconds;
                tierSeconds = Math.Min(remainingSeconds, tierCap);
            }
            else
            {
                // No cap - allocate all remaining seconds
                tierSeconds = remainingSeconds;
            }

            if (tierSeconds > 0)
            {
                result.Add(new PlanRegistrationPayLine
                {
                    PlanRegistrationId = planRegistrationId,
                    PayCode = tier.PayCode,
                    HoursInSeconds = tierSeconds,
                    Hours = tierSeconds / 3600.0,
                    PayRuleSetId = payRuleSet.Id,
                    CalculatedAt = calculatedAtUtc
                });

                remainingSeconds -= tierSeconds;
                cumulativeSeconds += tierSeconds;
            }
        }

        return result;
    }
}
