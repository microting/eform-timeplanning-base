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
public class AssignedSiteManagingTagUTest : DbTestFixture
{
    [Test]
    public async Task AssignedSiteManagingTag_Create_DoesCreate()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            IsManager = true
        };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        var managingTag = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite.Id,
            TagId = 100
        };

        // Act
        await managingTag.Create(DbContext).ConfigureAwait(false);

        // Assert
        var managingTags = DbContext.AssignedSiteManagingTags.AsNoTracking().ToList();
        var managingTagVersions = DbContext.AssignedSiteManagingTagVersions.AsNoTracking().ToList();

        Assert.That(managingTags, Is.Not.Null);
        Assert.That(managingTagVersions, Is.Not.Null);

        Assert.That(managingTags.Count, Is.EqualTo(1));
        Assert.That(managingTagVersions.Count, Is.EqualTo(1));

        Assert.That(managingTags[0].AssignedSiteId, Is.EqualTo(assignedSite.Id));
        Assert.That(managingTags[0].TagId, Is.EqualTo(100));
        Assert.That(managingTags[0].Version, Is.EqualTo(1));
        Assert.That(managingTags[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

        Assert.That(managingTagVersions[0].AssignedSiteId, Is.EqualTo(assignedSite.Id));
        Assert.That(managingTagVersions[0].TagId, Is.EqualTo(100));
        Assert.That(managingTagVersions[0].AssignedSiteManagingTagId, Is.EqualTo(managingTags[0].Id));
    }

    [Test]
    public async Task AssignedSiteManagingTag_Update_DoesUpdate()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            IsManager = true
        };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        var managingTag = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite.Id,
            TagId = 100
        };
        await managingTag.Create(DbContext).ConfigureAwait(false);

        // Act
        managingTag.TagId = 200;
        await managingTag.Update(DbContext).ConfigureAwait(false);

        // Assert
        var managingTags = DbContext.AssignedSiteManagingTags.AsNoTracking().ToList();
        var managingTagVersions = DbContext.AssignedSiteManagingTagVersions.AsNoTracking().ToList();

        Assert.That(managingTags, Is.Not.Null);
        Assert.That(managingTagVersions, Is.Not.Null);

        Assert.That(managingTags.Count, Is.EqualTo(1));
        Assert.That(managingTagVersions.Count, Is.EqualTo(2));

        Assert.That(managingTags[0].TagId, Is.EqualTo(200));
        Assert.That(managingTags[0].Version, Is.EqualTo(2));

        Assert.That(managingTagVersions[0].TagId, Is.EqualTo(100));
        Assert.That(managingTagVersions[1].TagId, Is.EqualTo(200));
    }

    [Test]
    public async Task AssignedSiteManagingTag_Delete_DoesDelete()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            IsManager = true
        };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        var managingTag = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite.Id,
            TagId = 100
        };
        await managingTag.Create(DbContext).ConfigureAwait(false);

        // Act
        await managingTag.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var managingTags = DbContext.AssignedSiteManagingTags.AsNoTracking().ToList();
        var managingTagVersions = DbContext.AssignedSiteManagingTagVersions.AsNoTracking().ToList();

        Assert.That(managingTags, Is.Not.Null);
        Assert.That(managingTagVersions, Is.Not.Null);

        Assert.That(managingTags.Count, Is.EqualTo(1));
        Assert.That(managingTagVersions.Count, Is.EqualTo(2));

        Assert.That(managingTags[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
        Assert.That(managingTags[0].Version, Is.EqualTo(2));
    }

    [Test]
    public async Task AssignedSiteManagingTag_UniqueIndex_PreventsDuplicates()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            IsManager = true
        };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        var managingTag1 = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite.Id,
            TagId = 100
        };
        await managingTag1.Create(DbContext).ConfigureAwait(false);

        var managingTag2 = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite.Id,
            TagId = 100
        };

        // Act & Assert
        Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            await managingTag2.Create(DbContext).ConfigureAwait(false);
        });
    }

    [Test]
    public async Task AssignedSiteManagingTag_MultipleTagsForSameSite_DoesCreate()
    {
        // Arrange
        var assignedSite = new AssignedSite
        {
            SiteId = 1,
            IsManager = true
        };
        await assignedSite.Create(DbContext).ConfigureAwait(false);

        var managingTag1 = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite.Id,
            TagId = 100
        };
        var managingTag2 = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite.Id,
            TagId = 200
        };

        // Act
        await managingTag1.Create(DbContext).ConfigureAwait(false);
        await managingTag2.Create(DbContext).ConfigureAwait(false);

        // Assert
        var managingTags = DbContext.AssignedSiteManagingTags.AsNoTracking().ToList();

        Assert.That(managingTags.Count, Is.EqualTo(2));
        Assert.That(managingTags[0].TagId, Is.EqualTo(100));
        Assert.That(managingTags[1].TagId, Is.EqualTo(200));
    }

    [Test]
    public async Task AssignedSiteManagingTag_SameTagForDifferentSites_DoesCreate()
    {
        // Arrange
        var assignedSite1 = new AssignedSite
        {
            SiteId = 1,
            IsManager = true
        };
        await assignedSite1.Create(DbContext).ConfigureAwait(false);

        var assignedSite2 = new AssignedSite
        {
            SiteId = 2,
            IsManager = true
        };
        await assignedSite2.Create(DbContext).ConfigureAwait(false);

        var managingTag1 = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite1.Id,
            TagId = 100
        };
        var managingTag2 = new AssignedSiteManagingTag
        {
            AssignedSiteId = assignedSite2.Id,
            TagId = 100
        };

        // Act
        await managingTag1.Create(DbContext).ConfigureAwait(false);
        await managingTag2.Create(DbContext).ConfigureAwait(false);

        // Assert
        var managingTags = DbContext.AssignedSiteManagingTags.AsNoTracking().ToList();

        Assert.That(managingTags.Count, Is.EqualTo(2));
        Assert.That(managingTags[0].AssignedSiteId, Is.EqualTo(assignedSite1.Id));
        Assert.That(managingTags[1].AssignedSiteId, Is.EqualTo(assignedSite2.Id));
        Assert.That(managingTags[0].TagId, Is.EqualTo(100));
        Assert.That(managingTags[1].TagId, Is.EqualTo(100));
    }
}
