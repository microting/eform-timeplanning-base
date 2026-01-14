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
using Microting.TimePlanningBase.Infrastructure.Helpers;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class PayLineGeneratorUTest
{
    [Test]
    public void PayLineGenerator_Sunday14Hours_SplitIntoTwoTiers()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Id = 1,
            Name = "Sunday Rule",
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 1,
                    DayCode = "SUNDAY",
                    Tiers = new List<PayTierRule>
                    {
                        new PayTierRule
                        {
                            Order = 1,
                            UpToSeconds = 39600, // 11 hours
                            PayCode = "SUN_80"
                        },
                        new PayTierRule
                        {
                            Order = 2,
                            UpToSeconds = null, // remainder
                            PayCode = "SUN_100"
                        }
                    }
                }
            }
        };

        int totalSeconds = 50400; // 14 hours
        var calculatedAt = DateTime.UtcNow;

        // Act
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", totalSeconds, payRuleSet, calculatedAt);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));

        var sun80 = result.FirstOrDefault(l => l.PayCode == "SUN_80");
        var sun100 = result.FirstOrDefault(l => l.PayCode == "SUN_100");

        Assert.That(sun80, Is.Not.Null);
        Assert.That(sun80.HoursInSeconds, Is.EqualTo(39600)); // 11 hours
        Assert.That(sun80.Hours, Is.EqualTo(11.0));
        Assert.That(sun80.PlanRegistrationId, Is.EqualTo(1));
        Assert.That(sun80.PayRuleSetId, Is.EqualTo(1));

        Assert.That(sun100, Is.Not.Null);
        Assert.That(sun100.HoursInSeconds, Is.EqualTo(10800)); // 3 hours
        Assert.That(sun100.Hours, Is.EqualTo(3.0));
        Assert.That(sun100.PlanRegistrationId, Is.EqualTo(1));
        Assert.That(sun100.PayRuleSetId, Is.EqualTo(1));
    }

    [Test]
    public void PayLineGenerator_Grundlovsdag14Hours_SplitIntoTwoTiers()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Id = 2,
            Name = "Grundlovsdag Rule",
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 2,
                    DayCode = "GRUNDLOVSDAG",
                    Tiers = new List<PayTierRule>
                    {
                        new PayTierRule
                        {
                            Order = 1,
                            UpToSeconds = 39600, // 11 hours
                            PayCode = "GRUND_0"
                        },
                        new PayTierRule
                        {
                            Order = 2,
                            UpToSeconds = null, // remainder
                            PayCode = "GRUND_100"
                        }
                    }
                }
            }
        };

        int totalSeconds = 50400; // 14 hours
        var calculatedAt = DateTime.UtcNow;

        // Act
        var result = PayLineGenerator.GeneratePayLines(1, "GRUNDLOVSDAG", totalSeconds, payRuleSet, calculatedAt);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));

        var grund0 = result.FirstOrDefault(l => l.PayCode == "GRUND_0");
        var grund100 = result.FirstOrDefault(l => l.PayCode == "GRUND_100");

        Assert.That(grund0, Is.Not.Null);
        Assert.That(grund0.HoursInSeconds, Is.EqualTo(39600)); // 11 hours
        Assert.That(grund0.Hours, Is.EqualTo(11.0));

        Assert.That(grund100, Is.Not.Null);
        Assert.That(grund100.HoursInSeconds, Is.EqualTo(10800)); // 3 hours
        Assert.That(grund100.Hours, Is.EqualTo(3.0));
    }

    [Test]
    public void PayLineGenerator_MissingDayCode_ReturnsFallbackDefault()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Id = 3,
            Name = "Test Rule Set",
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 3,
                    DayCode = "SUNDAY",
                    Tiers = new List<PayTierRule>
                    {
                        new PayTierRule
                        {
                            Order = 1,
                            UpToSeconds = 39600,
                            PayCode = "SUN_80"
                        }
                    }
                }
            }
        };

        int totalSeconds = 50400; // 14 hours
        var calculatedAt = DateTime.UtcNow;

        // Act - using a dayCode that doesn't exist
        var result = PayLineGenerator.GeneratePayLines(1, "WEEKDAY", totalSeconds, payRuleSet, calculatedAt);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("DEFAULT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(50400));
        Assert.That(result[0].Hours, Is.EqualTo(14.0));
    }

    [Test]
    public void PayLineGenerator_TotalLessThanTierCap_OnlyFirstTier()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Id = 4,
            Name = "Test Rule Set",
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 4,
                    DayCode = "SUNDAY",
                    Tiers = new List<PayTierRule>
                    {
                        new PayTierRule
                        {
                            Order = 1,
                            UpToSeconds = 39600, // 11 hours
                            PayCode = "SUN_80"
                        },
                        new PayTierRule
                        {
                            Order = 2,
                            UpToSeconds = null,
                            PayCode = "SUN_100"
                        }
                    }
                }
            }
        };

        int totalSeconds = 28800; // 8 hours (less than 11 hours cap)
        var calculatedAt = DateTime.UtcNow;

        // Act
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", totalSeconds, payRuleSet, calculatedAt);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("SUN_80"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(28800));
        Assert.That(result[0].Hours, Is.EqualTo(8.0));
    }

    [Test]
    public void PayLineGenerator_NullPayRuleSet_ReturnsFallbackDefault()
    {
        // Arrange
        int totalSeconds = 36000; // 10 hours
        var calculatedAt = DateTime.UtcNow;

        // Act
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", totalSeconds, null, calculatedAt);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("DEFAULT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(36000));
        Assert.That(result[0].Hours, Is.EqualTo(10.0));
        Assert.That(result[0].PayRuleSetId, Is.Null);
    }

    [Test]
    public void PayLineGenerator_EmptyTiers_ReturnsFallbackDefault()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Id = 5,
            Name = "Test Rule Set",
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 5,
                    DayCode = "SUNDAY",
                    Tiers = new List<PayTierRule>() // Empty tiers
                }
            }
        };

        int totalSeconds = 36000; // 10 hours
        var calculatedAt = DateTime.UtcNow;

        // Act
        var result = PayLineGenerator.GeneratePayLines(1, "SUNDAY", totalSeconds, payRuleSet, calculatedAt);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].PayCode, Is.EqualTo("DEFAULT"));
        Assert.That(result[0].HoursInSeconds, Is.EqualTo(36000));
        Assert.That(result[0].Hours, Is.EqualTo(10.0));
    }

    [Test]
    public void PayLineGenerator_ThreeTiers_CorrectSplitting()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Id = 6,
            Name = "Multi-tier Rule Set",
            DayRules = new List<PayDayRule>
            {
                new PayDayRule
                {
                    PayRuleSetId = 6,
                    DayCode = "HOLIDAY",
                    Tiers = new List<PayTierRule>
                    {
                        new PayTierRule
                        {
                            Order = 1,
                            UpToSeconds = 14400, // 4 hours
                            PayCode = "HOL_50"
                        },
                        new PayTierRule
                        {
                            Order = 2,
                            UpToSeconds = 28800, // next 4 hours (total 8)
                            PayCode = "HOL_75"
                        },
                        new PayTierRule
                        {
                            Order = 3,
                            UpToSeconds = null, // remainder
                            PayCode = "HOL_100"
                        }
                    }
                }
            }
        };

        int totalSeconds = 39600; // 11 hours
        var calculatedAt = DateTime.UtcNow;

        // Act
        var result = PayLineGenerator.GeneratePayLines(1, "HOLIDAY", totalSeconds, payRuleSet, calculatedAt);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));

        var hol50 = result.FirstOrDefault(l => l.PayCode == "HOL_50");
        var hol75 = result.FirstOrDefault(l => l.PayCode == "HOL_75");
        var hol100 = result.FirstOrDefault(l => l.PayCode == "HOL_100");

        Assert.That(hol50, Is.Not.Null);
        Assert.That(hol50.HoursInSeconds, Is.EqualTo(14400)); // 4 hours
        Assert.That(hol50.Hours, Is.EqualTo(4.0));

        Assert.That(hol75, Is.Not.Null);
        Assert.That(hol75.HoursInSeconds, Is.EqualTo(14400)); // 4 hours
        Assert.That(hol75.Hours, Is.EqualTo(4.0));

        Assert.That(hol100, Is.Not.Null);
        Assert.That(hol100.HoursInSeconds, Is.EqualTo(10800)); // 3 hours
        Assert.That(hol100.Hours, Is.EqualTo(3.0));
    }
}
