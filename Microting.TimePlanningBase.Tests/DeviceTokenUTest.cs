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
public class DeviceTokenUTest : DbTestFixture
{
    [Test]
    public async Task DeviceToken_Create_PersistsAllColumns()
    {
        // Arrange
        var token = new DeviceToken
        {
            AppId = "time",
            InstallationId = "install-1",
            FcmToken = "tok-1",
            SdkSiteId = 200,
            Platform = "android",
            AppBuildNumber = 31221
        };

        // Act
        await token.Create(DbContext).ConfigureAwait(false);

        // Assert
        var row = DbContext.DeviceTokens.AsNoTracking().Single();

        Assert.Multiple(() =>
        {
            Assert.That(row.AppId, Is.EqualTo("time"));
            Assert.That(row.InstallationId, Is.EqualTo("install-1"));
            Assert.That(row.FcmToken, Is.EqualTo("tok-1"));
            Assert.That(row.SdkSiteId, Is.EqualTo(200));
            Assert.That(row.Platform, Is.EqualTo("android"));
            Assert.That(row.AppBuildNumber, Is.EqualTo(31221));
            Assert.That(row.WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
        });
    }

    [Test]
    public async Task DeviceToken_SameInstall_NewToken_UpdatesInPlace()
    {
        // Arrange
        var token = new DeviceToken
        {
            AppId = "time",
            InstallationId = "install-rot",
            FcmToken = "old",
            SdkSiteId = 201,
            Platform = "android"
        };
        await token.Create(DbContext).ConfigureAwait(false);

        // Act
        token.FcmToken = "new";
        await token.Update(DbContext).ConfigureAwait(false);

        // Assert
        var rows = DbContext.DeviceTokens.AsNoTracking().ToList();

        Assert.That(rows, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(rows[0].FcmToken, Is.EqualTo("new"));
            Assert.That(rows[0].InstallationId, Is.EqualTo("install-rot"));
        });
    }

    [Test]
    public async Task DeviceToken_VersionRow_CarriesNewColumns()
    {
        // Arrange & Act
        var token = new DeviceToken
        {
            AppId = "time",
            InstallationId = "install-v",
            FcmToken = "tok-v",
            SdkSiteId = 202,
            Platform = "ios"
        };
        await token.Create(DbContext).ConfigureAwait(false);

        // Assert
        var version = DbContext.DeviceTokenVersions.AsNoTracking().Single();

        Assert.Multiple(() =>
        {
            Assert.That(version.AppId, Is.EqualTo("time"));
            Assert.That(version.InstallationId, Is.EqualTo("install-v"));
            Assert.That(version.FcmToken, Is.EqualTo("tok-v"));
            Assert.That(version.SdkSiteId, Is.EqualTo(202));
            Assert.That(version.DeviceTokenId, Is.EqualTo(token.Id));
        });
    }

    [Test]
    public async Task DeviceToken_Delete_SoftDeletesOnly()
    {
        // Arrange
        var token = new DeviceToken
        {
            AppId = "time",
            InstallationId = "install-d",
            FcmToken = "tok-d",
            SdkSiteId = 203,
            Platform = "android"
        };
        await token.Create(DbContext).ConfigureAwait(false);

        // Act
        await token.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var row = DbContext.DeviceTokens.AsNoTracking().Single();

        Assert.That(row.WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
    }
}
