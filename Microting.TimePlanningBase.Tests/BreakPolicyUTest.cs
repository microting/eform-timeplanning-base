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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class BreakPolicyUTest : DbTestFixture
{
    [Test]
    public async Task BreakPolicy_Create_DoesCreate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Standard Break Policy",
            AppliesOnlyIfWorkMinutesAtLeast = 360,
            ExtraPauseCountsAsUnpaid = true
        };

        // Act
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicies = DbContext.BreakPolicies.AsNoTracking().ToList();
        var breakPolicyVersions = DbContext.BreakPolicyVersions.AsNoTracking().ToList();

        Assert.That(breakPolicies, Is.Not.Null);
        Assert.That(breakPolicyVersions, Is.Not.Null);

        Assert.That(breakPolicies.Count, Is.EqualTo(1));
        Assert.That(breakPolicyVersions.Count, Is.EqualTo(1));

        Assert.That(breakPolicies[0].Name, Is.EqualTo("Standard Break Policy"));
        Assert.That(breakPolicies[0].AppliesOnlyIfWorkMinutesAtLeast, Is.EqualTo(360));
        Assert.That(breakPolicies[0].ExtraPauseCountsAsUnpaid, Is.EqualTo(true));
        Assert.That(breakPolicies[0].Version, Is.EqualTo(1));
        Assert.That(breakPolicies[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(breakPolicyVersions[0].Name, Is.EqualTo("Standard Break Policy"));
        Assert.That(breakPolicyVersions[0].AppliesOnlyIfWorkMinutesAtLeast, Is.EqualTo(360));
        Assert.That(breakPolicyVersions[0].ExtraPauseCountsAsUnpaid, Is.EqualTo(true));
        Assert.That(breakPolicyVersions[0].BreakPolicyId, Is.EqualTo(breakPolicies[0].Id));
    }

    [Test]
    public async Task BreakPolicy_Update_DoesUpdate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Original Policy",
            AppliesOnlyIfWorkMinutesAtLeast = 300,
            ExtraPauseCountsAsUnpaid = true
        };
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        // Act
        breakPolicy.Name = "Updated Policy";
        breakPolicy.AppliesOnlyIfWorkMinutesAtLeast = 400;
        breakPolicy.ExtraPauseCountsAsUnpaid = false;
        await breakPolicy.Update(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicies = DbContext.BreakPolicies.AsNoTracking().ToList();
        var breakPolicyVersions = DbContext.BreakPolicyVersions.AsNoTracking().ToList();

        Assert.That(breakPolicies, Is.Not.Null);
        Assert.That(breakPolicyVersions, Is.Not.Null);

        Assert.That(breakPolicies.Count, Is.EqualTo(1));
        Assert.That(breakPolicyVersions.Count, Is.EqualTo(2));

        Assert.That(breakPolicies[0].Name, Is.EqualTo("Updated Policy"));
        Assert.That(breakPolicies[0].AppliesOnlyIfWorkMinutesAtLeast, Is.EqualTo(400));
        Assert.That(breakPolicies[0].ExtraPauseCountsAsUnpaid, Is.EqualTo(false));
        Assert.That(breakPolicies[0].Version, Is.EqualTo(2));
        Assert.That(breakPolicies[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
    }

    [Test]
    public async Task BreakPolicy_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Test Policy",
            AppliesOnlyIfWorkMinutesAtLeast = 360,
            ExtraPauseCountsAsUnpaid = true
        };
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        // Act
        await breakPolicy.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicies = DbContext.BreakPolicies.AsNoTracking().ToList();
        var breakPolicyVersions = DbContext.BreakPolicyVersions.AsNoTracking().ToList();

        Assert.That(breakPolicies, Is.Not.Null);
        Assert.That(breakPolicyVersions, Is.Not.Null);

        Assert.That(breakPolicies.Count, Is.EqualTo(1));
        Assert.That(breakPolicyVersions.Count, Is.EqualTo(2));

        Assert.That(breakPolicies[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(breakPolicies[0].Name, Is.EqualTo("Test Policy"));
        Assert.That(breakPolicies[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task BreakPolicy_CreateWithNullMinutes_DoesCreate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "No Minimum Policy",
            AppliesOnlyIfWorkMinutesAtLeast = null,
            ExtraPauseCountsAsUnpaid = false
        };

        // Act
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicies = DbContext.BreakPolicies.AsNoTracking().ToList();

        Assert.That(breakPolicies, Is.Not.Null);
        Assert.That(breakPolicies.Count, Is.EqualTo(1));
        Assert.That(breakPolicies[0].Name, Is.EqualTo("No Minimum Policy"));
        Assert.That(breakPolicies[0].AppliesOnlyIfWorkMinutesAtLeast, Is.Null);
        Assert.That(breakPolicies[0].ExtraPauseCountsAsUnpaid, Is.EqualTo(false));
    }

    [Test]
    public async Task BreakPolicy_CreateWithDefaultValues_DoesCreate()
    {
        // Arrange
        var breakPolicy = new BreakPolicy
        {
            Name = "Default Values Policy"
        };

        // Act
        await breakPolicy.Create(DbContext).ConfigureAwait(false);

        // Assert
        var breakPolicies = DbContext.BreakPolicies.AsNoTracking().ToList();

        Assert.That(breakPolicies, Is.Not.Null);
        Assert.That(breakPolicies.Count, Is.EqualTo(1));
        Assert.That(breakPolicies[0].Name, Is.EqualTo("Default Values Policy"));
        Assert.That(breakPolicies[0].ExtraPauseCountsAsUnpaid, Is.EqualTo(true));
    }
}
