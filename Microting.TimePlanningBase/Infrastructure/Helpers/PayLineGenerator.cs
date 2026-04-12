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
                    PayrollCode = tier.PayrollCode,
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

    /// <summary>
    /// Splits a work period defined by clock-time boundaries (seconds of day)
    /// into pay lines based on PayDayTypeRule + PayTimeBandRule configuration.
    /// Unlike <see cref="GeneratePayLines"/> which uses tier-based splitting,
    /// this method splits by actual time-of-day bands.
    /// </summary>
    public static List<PlanRegistrationPayLine> GenerateTimeBandPayLines(
        int planRegistrationId,
        DayType dayType,
        int startSecondOfDay,
        int endSecondOfDay,
        PayRuleSet payRuleSet,
        DateTime calculatedAtUtc)
    {
        var result = new List<PlanRegistrationPayLine>();
        int totalSeconds = endSecondOfDay - startSecondOfDay;

        if (totalSeconds <= 0)
            return result;

        // Find the PayDayTypeRule matching the dayType
        var dayTypeRule = payRuleSet?.DayTypeRules?
            .SingleOrDefault(r => r.DayType == dayType);

        if (dayTypeRule == null || dayTypeRule.TimeBandRules == null || !dayTypeRule.TimeBandRules.Any())
        {
            // Fallback: single DEFAULT pay line with all seconds
            result.Add(CreatePayLine(planRegistrationId, "DEFAULT", null, totalSeconds, payRuleSet?.Id, calculatedAtUtc));
            return result;
        }

        // Order bands by StartSecondOfDay
        var orderedBands = dayTypeRule.TimeBandRules.OrderBy(b => b.StartSecondOfDay).ToList();

        int cursor = startSecondOfDay;

        foreach (var band in orderedBands)
        {
            // Skip bands that end before our work period starts
            if (band.EndSecondOfDay <= startSecondOfDay)
                continue;

            // Stop if we've passed our work period
            if (band.StartSecondOfDay >= endSecondOfDay)
                break;

            // If there is a gap between cursor and this band's start, fill with DefaultPayCode
            if (band.StartSecondOfDay > cursor)
            {
                int gapEnd = Math.Min(band.StartSecondOfDay, endSecondOfDay);
                int gapSeconds = gapEnd - cursor;
                if (gapSeconds > 0)
                {
                    result.Add(CreatePayLine(planRegistrationId, dayTypeRule.DefaultPayCode, null, gapSeconds, payRuleSet?.Id, calculatedAtUtc));
                    cursor = gapEnd;
                }
            }

            // Calculate overlap between work period and this band
            int overlapStart = Math.Max(startSecondOfDay, band.StartSecondOfDay);
            int overlapEnd = Math.Min(endSecondOfDay, band.EndSecondOfDay);
            int overlapSeconds = overlapEnd - overlapStart;

            if (overlapSeconds > 0)
            {
                result.Add(CreatePayLine(planRegistrationId, band.PayCode, band.PayrollCode, overlapSeconds, payRuleSet?.Id, calculatedAtUtc));
                cursor = overlapEnd;
            }
        }

        // If any trailing time falls outside all bands, use DefaultPayCode
        if (cursor < endSecondOfDay)
        {
            int remainingSeconds = endSecondOfDay - cursor;
            result.Add(CreatePayLine(planRegistrationId, dayTypeRule.DefaultPayCode, null, remainingSeconds, payRuleSet?.Id, calculatedAtUtc));
        }

        return result;
    }

    private static PlanRegistrationPayLine CreatePayLine(
        int planRegistrationId, string payCode, string? payrollCode, int seconds, int? payRuleSetId, DateTime calculatedAtUtc)
    {
        return new PlanRegistrationPayLine
        {
            PlanRegistrationId = planRegistrationId,
            PayCode = payCode,
            PayrollCode = payrollCode,
            HoursInSeconds = seconds,
            Hours = seconds / 3600.0,
            PayRuleSetId = payRuleSetId,
            CalculatedAt = calculatedAtUtc
        };
    }
}
