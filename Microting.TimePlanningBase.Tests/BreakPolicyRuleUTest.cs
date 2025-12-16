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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class BreakPolicyRuleUTest : DbTestFixture
{
    [Test]
    public async Task BreakPolicyRule_Create_DoesCreate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Standard Break Policy"
        };
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        var breakPolicyRule = new BreakPolicyRule
        {
            BreakPolicyId = breakPolicy.Id,
            DayOfWeek = DayOfWeek.Monday,
            UnpaidBreakMinutes = 30,
            PaidBreakMinutes = 15
        };

        // Act
        await breakPolicyRule.Create(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicyRules = DbContext.BreakPolicyRules.AsNoTracking().ToList();
        var breakPolicyRuleVersions = DbContext.BreakPolicyRuleVersions.AsNoTracking().ToList();

        Assert.That(breakPolicyRules, Is.Not.Null);
        Assert.That(breakPolicyRuleVersions, Is.Not.Null);

        Assert.That(breakPolicyRules.Count, Is.EqualTo(1));
        Assert.That(breakPolicyRuleVersions.Count, Is.EqualTo(1));

        Assert.That(breakPolicyRules[0].BreakPolicyId, Is.EqualTo(breakPolicy.Id));
        Assert.That(breakPolicyRules[0].DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(breakPolicyRules[0].UnpaidBreakMinutes, Is.EqualTo(30));
        Assert.That(breakPolicyRules[0].PaidBreakMinutes, Is.EqualTo(15));
        Assert.That(breakPolicyRules[0].Version, Is.EqualTo(1));
        Assert.That(breakPolicyRules[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(breakPolicyRuleVersions[0].BreakPolicyId, Is.EqualTo(breakPolicy.Id));
        Assert.That(breakPolicyRuleVersions[0].DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(breakPolicyRuleVersions[0].UnpaidBreakMinutes, Is.EqualTo(30));
        Assert.That(breakPolicyRuleVersions[0].PaidBreakMinutes, Is.EqualTo(15));
        Assert.That(breakPolicyRuleVersions[0].BreakPolicyRuleId, Is.EqualTo(breakPolicyRules[0].Id));
    }

    [Test]
    public async Task BreakPolicyRule_Update_DoesUpdate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Standard Break Policy"
        };
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        var breakPolicyRule = new BreakPolicyRule
        {
            BreakPolicyId = breakPolicy.Id,
            DayOfWeek = DayOfWeek.Monday,
            UnpaidBreakMinutes = 30,
            PaidBreakMinutes = 15
        };
        await breakPolicyRule.Create(DbContext).ConfigureAwait(false);

        // Act
        breakPolicyRule.DayOfWeek = DayOfWeek.Tuesday;
        breakPolicyRule.UnpaidBreakMinutes = 45;
        breakPolicyRule.PaidBreakMinutes = 20;
        await breakPolicyRule.Update(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicyRules = DbContext.BreakPolicyRules.AsNoTracking().ToList();
        var breakPolicyRuleVersions = DbContext.BreakPolicyRuleVersions.AsNoTracking().ToList();

        Assert.That(breakPolicyRules, Is.Not.Null);
        Assert.That(breakPolicyRuleVersions, Is.Not.Null);

        Assert.That(breakPolicyRules.Count, Is.EqualTo(1));
        Assert.That(breakPolicyRuleVersions.Count, Is.EqualTo(2));

        Assert.That(breakPolicyRules[0].DayOfWeek, Is.EqualTo(DayOfWeek.Tuesday));
        Assert.That(breakPolicyRules[0].UnpaidBreakMinutes, Is.EqualTo(45));
        Assert.That(breakPolicyRules[0].PaidBreakMinutes, Is.EqualTo(20));
        Assert.That(breakPolicyRules[0].Version, Is.EqualTo(2));
        Assert.That(breakPolicyRules[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task BreakPolicyRule_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Standard Break Policy"
        };
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        var breakPolicyRule = new BreakPolicyRule
        {
            BreakPolicyId = breakPolicy.Id,
            DayOfWeek = DayOfWeek.Wednesday,
            UnpaidBreakMinutes = 30,
            PaidBreakMinutes = 15
        };
        await breakPolicyRule.Create(DbContext).ConfigureAwait(false);

        // Act
        await breakPolicyRule.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicyRules = DbContext.BreakPolicyRules.AsNoTracking().ToList();
        var breakPolicyRuleVersions = DbContext.BreakPolicyRuleVersions.AsNoTracking().ToList();

        Assert.That(breakPolicyRules, Is.Not.Null);
        Assert.That(breakPolicyRuleVersions, Is.Not.Null);

        Assert.That(breakPolicyRules.Count, Is.EqualTo(1));
        Assert.That(breakPolicyRuleVersions.Count, Is.EqualTo(2));

        Assert.That(breakPolicyRules[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(breakPolicyRules[0].DayOfWeek, Is.EqualTo(DayOfWeek.Wednesday));
        Assert.That(breakPolicyRules[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task BreakPolicyRule_CreateMultipleDays_DoesCreate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Weekly Break Policy"
        };
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        var mondayRule = new BreakPolicyRule
        {
            BreakPolicyId = breakPolicy.Id,
            DayOfWeek = DayOfWeek.Monday,
            UnpaidBreakMinutes = 30,
            PaidBreakMinutes = 15
        };

        var tuesdayRule = new BreakPolicyRule
        {
            BreakPolicyId = breakPolicy.Id,
            DayOfWeek = DayOfWeek.Tuesday,
            UnpaidBreakMinutes = 30,
            PaidBreakMinutes = 15
        };

        // Act
        await mondayRule.Create(DbContext).ConfigureAwait(false);
        await tuesdayRule.Create(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicyRules = DbContext.BreakPolicyRules.AsNoTracking().ToList();

        Assert.That(breakPolicyRules, Is.Not.Null);
        Assert.That(breakPolicyRules.Count, Is.EqualTo(2));
        Assert.That(breakPolicyRules[0].DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(breakPolicyRules[1].DayOfWeek, Is.EqualTo(DayOfWeek.Tuesday));
    }

    [Test]
    public async Task BreakPolicyRule_CreateWithZeroMinutes_DoesCreate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "No Break Policy"
        };
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        var breakPolicyRule = new BreakPolicyRule
        {
            BreakPolicyId = breakPolicy.Id,
            DayOfWeek = DayOfWeek.Sunday,
            UnpaidBreakMinutes = 0,
            PaidBreakMinutes = 0
        };

        // Act
        await breakPolicyRule.Create(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicyRules = DbContext.BreakPolicyRules.AsNoTracking().ToList();

        Assert.That(breakPolicyRules, Is.Not.Null);
        Assert.That(breakPolicyRules.Count, Is.EqualTo(1));
        Assert.That(breakPolicyRules[0].UnpaidBreakMinutes, Is.EqualTo(0));
        Assert.That(breakPolicyRules[0].PaidBreakMinutes, Is.EqualTo(0));
    }
}
