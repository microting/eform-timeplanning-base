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
public class PayRuleSetUTest : DbTestFixture
{
    [Test]
    public async Task PayRuleSet_Create_DoesCreate()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Name = "Standard Pay Rules"
        };

        // Act
        await payRuleSet.Create(DbContext).ConfigureAwait(false);

        // Assert
        var payRuleSets = DbContext.PayRuleSets.AsNoTracking().ToList();
        var payRuleSetVersions = DbContext.PayRuleSetVersions.AsNoTracking().ToList();

        Assert.That(payRuleSets, Is.Not.Null);
        Assert.That(payRuleSetVersions, Is.Not.Null);

        Assert.That(payRuleSets.Count, Is.EqualTo(1));
        Assert.That(payRuleSetVersions.Count, Is.EqualTo(1));

        Assert.That(payRuleSets[0].Name, Is.EqualTo("Standard Pay Rules"));
        Assert.That(payRuleSets[0].Version, Is.EqualTo(1));
        Assert.That(payRuleSets[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(payRuleSetVersions[0].Name, Is.EqualTo("Standard Pay Rules"));
        Assert.That(payRuleSetVersions[0].PayRuleSetId, Is.EqualTo(payRuleSets[0].Id));
    }

    [Test]
    public async Task PayRuleSet_WithDayRulesAndTiers_DoesCreate()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Name = "Sunday Rules"
        };
        await payRuleSet.Create(DbContext).ConfigureAwait(false);

        var dayRule = new PayDayRule
        {
            PayRuleSetId = payRuleSet.Id,
            DayCode = "SUNDAY"
        };
        await dayRule.Create(DbContext).ConfigureAwait(false);

        var tier1 = new PayTierRule
        {
            PayDayRuleId = dayRule.Id,
            Order = 1,
            UpToSeconds = 39600, // 11 hours
            PayCode = "SUN_80"
        };
        await tier1.Create(DbContext).ConfigureAwait(false);

        var tier2 = new PayTierRule
        {
            PayDayRuleId = dayRule.Id,
            Order = 2,
            UpToSeconds = null,
            PayCode = "SUN_100"
        };
        await tier2.Create(DbContext).ConfigureAwait(false);

        // Act - Reload with includes
        var reloadedPayRuleSet = await DbContext.PayRuleSets
            .Include(p => p.DayRules)
            .ThenInclude(d => d.Tiers)
            .FirstOrDefaultAsync(p => p.Id == payRuleSet.Id)
            .ConfigureAwait(false);

        // Assert
        Assert.That(reloadedPayRuleSet, Is.Not.Null);
        Assert.That(reloadedPayRuleSet.Name, Is.EqualTo("Sunday Rules"));
        Assert.That(reloadedPayRuleSet.DayRules, Is.Not.Null);
        Assert.That(reloadedPayRuleSet.DayRules.Count, Is.EqualTo(1));

        var sundayRule = reloadedPayRuleSet.DayRules.First();
        Assert.That(sundayRule.DayCode, Is.EqualTo("SUNDAY"));
        Assert.That(sundayRule.Tiers, Is.Not.Null);
        Assert.That(sundayRule.Tiers.Count, Is.EqualTo(2));

        var orderedTiers = sundayRule.Tiers.OrderBy(t => t.Order).ToList();
        Assert.That(orderedTiers[0].PayCode, Is.EqualTo("SUN_80"));
        Assert.That(orderedTiers[0].UpToSeconds, Is.EqualTo(39600));
        Assert.That(orderedTiers[1].PayCode, Is.EqualTo("SUN_100"));
        Assert.That(orderedTiers[1].UpToSeconds, Is.Null);
    }

    [Test]
    public async Task PayRuleSet_Update_DoesUpdate()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Name = "Original Rules"
        };
        await payRuleSet.Create(DbContext).ConfigureAwait(false);

        // Act
        payRuleSet.Name = "Updated Rules";
        await payRuleSet.Update(DbContext).ConfigureAwait(false);

        // Assert
        var payRuleSets = DbContext.PayRuleSets.AsNoTracking().ToList();
        var payRuleSetVersions = DbContext.PayRuleSetVersions.AsNoTracking().ToList();

        Assert.That(payRuleSets.Count, Is.EqualTo(1));
        Assert.That(payRuleSetVersions.Count, Is.EqualTo(2));

        Assert.That(payRuleSets[0].Name, Is.EqualTo("Updated Rules"));
        Assert.That(payRuleSets[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task PayRuleSet_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var payRuleSet = new PayRuleSet
        {
            Name = "Test Rules"
        };
        await payRuleSet.Create(DbContext).ConfigureAwait(false);

        // Act
        await payRuleSet.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var payRuleSets = DbContext.PayRuleSets.AsNoTracking().ToList();
        var payRuleSetVersions = DbContext.PayRuleSetVersions.AsNoTracking().ToList();

        Assert.That(payRuleSets.Count, Is.EqualTo(1));
        Assert.That(payRuleSetVersions.Count, Is.EqualTo(2));

        Assert.That(payRuleSets[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(payRuleSets[0].Version, Is.EqualTo(2));
    }
}
