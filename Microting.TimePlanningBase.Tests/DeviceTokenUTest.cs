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
    public async Task DeviceToken_Create_DoesCreate()
    {
        // Arrange
        var deviceToken = new DeviceToken
        {
            AppId = "time",
            InstallationId = "install-1",
            FcmToken = "tok-1",
            SdkSiteId = 200,
            Platform = "android",
            AppBuildNumber = 31221
        };

        // Act
        await deviceToken.Create(DbContext).ConfigureAwait(false);

        // Assert
        var deviceTokens = DbContext.DeviceTokens.AsNoTracking().ToList();
        var deviceTokenVersions = DbContext.DeviceTokenVersions.AsNoTracking().ToList();

        Assert.That(deviceTokens, Has.Count.EqualTo(1));
        Assert.That(deviceTokenVersions, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(deviceTokens[0].AppId, Is.EqualTo("time"));
            Assert.That(deviceTokens[0].InstallationId, Is.EqualTo("install-1"));
            Assert.That(deviceTokens[0].FcmToken, Is.EqualTo("tok-1"));
            Assert.That(deviceTokens[0].SdkSiteId, Is.EqualTo(200));
            Assert.That(deviceTokens[0].Platform, Is.EqualTo("android"));
            Assert.That(deviceTokens[0].AppBuildNumber, Is.EqualTo(31221));
            Assert.That(deviceTokens[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

            Assert.That(deviceTokenVersions[0].DeviceTokenId, Is.EqualTo(deviceTokens[0].Id));
            Assert.That(deviceTokenVersions[0].AppId, Is.EqualTo("time"));
            Assert.That(deviceTokenVersions[0].InstallationId, Is.EqualTo("install-1"));
            Assert.That(deviceTokenVersions[0].FcmToken, Is.EqualTo("tok-1"));
            Assert.That(deviceTokenVersions[0].SdkSiteId, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task DeviceToken_UpdateWithRotatedFcmToken_DoesUpdateInPlace()
    {
        // Arrange
        var deviceToken = new DeviceToken
        {
            AppId = "time",
            InstallationId = "install-rot",
            FcmToken = "old",
            SdkSiteId = 201,
            Platform = "android"
        };
        await deviceToken.Create(DbContext).ConfigureAwait(false);

        // Act
        deviceToken.FcmToken = "new";
        await deviceToken.Update(DbContext).ConfigureAwait(false);

        // Assert
        var deviceTokens = DbContext.DeviceTokens.AsNoTracking().ToList();

        Assert.That(deviceTokens, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(deviceTokens[0].FcmToken, Is.EqualTo("new"));
            Assert.That(deviceTokens[0].InstallationId, Is.EqualTo("install-rot"));
        });
    }

    [Test]
    public async Task DeviceToken_Delete_DoesSetWorkflowStateToRemoved()
    {
        // Arrange
        var deviceToken = new DeviceToken
        {
            AppId = "time",
            InstallationId = "install-d",
            FcmToken = "tok-d",
            SdkSiteId = 203,
            Platform = "android"
        };
        await deviceToken.Create(DbContext).ConfigureAwait(false);

        // Act
        await deviceToken.Delete(DbContext).ConfigureAwait(false);

        // Assert
        var deviceTokens = DbContext.DeviceTokens.AsNoTracking().ToList();

        Assert.That(deviceTokens, Has.Count.EqualTo(1));
        Assert.That(deviceTokens[0].WorkflowState, Is.EqualTo(Constants.WorkflowStates.Removed));
    }

    [Test]
    public async Task DeviceToken_SecondRowForSameAppIdAndInstallationId_DoesThrow()
    {
        // Arrange
        await NewDeviceToken("install-dup", "tok-first", 210).Create(DbContext).ConfigureAwait(false);

        // Act & Assert
        // This is the whole point of the re-key: one row per app install.
        Assert.ThrowsAsync<DbUpdateException>(async () =>
            await NewDeviceToken("install-dup", "tok-second", 211).Create(DbContext).ConfigureAwait(false));
    }

    [Test]
    public async Task DeviceToken_SameFcmTokenOnTwoInstalls_DoesCreateBothRows()
    {
        // Arrange & Act
        // Newly permitted. The old schema had a unique index on the token alone
        // and rejected this outright.
        await NewDeviceToken("install-a", "shared-tok", 220).Create(DbContext).ConfigureAwait(false);
        await NewDeviceToken("install-b", "shared-tok", 221).Create(DbContext).ConfigureAwait(false);

        // Assert
        var deviceTokens = DbContext.DeviceTokens.AsNoTracking().ToList();

        Assert.That(deviceTokens, Has.Count.EqualTo(2));
        Assert.That(deviceTokens.Select(x => x.InstallationId),
            Is.EquivalentTo(new[] { "install-a", "install-b" }));
    }

    [Test]
    public async Task DeviceToken_SameInstallationIdUnderDifferentAppId_DoesCreateBothRows()
    {
        // Arrange & Act
        // AppId is half the key, so a second app on the same install is its own
        // row rather than a collision.
        await NewDeviceToken("shared-install", "tok-time", 230).Create(DbContext).ConfigureAwait(false);

        var otherApp = NewDeviceToken("shared-install", "tok-eform", 230);
        otherApp.AppId = "eform";
        await otherApp.Create(DbContext).ConfigureAwait(false);

        // Assert
        Assert.That(DbContext.DeviceTokens.AsNoTracking().ToList(), Has.Count.EqualTo(2));
    }

    [Test]
    public void DeviceToken_WithoutInstallationId_DoesThrow()
    {
        // Act & Assert
        // AppId and InstallationId are [Required]. Nullable reference types are
        // off in this project, so only the database constraint enforces this -
        // which is exactly why it is worth pinning.
        var deviceToken = NewDeviceToken(null, "tok-null", 240);

        Assert.ThrowsAsync<DbUpdateException>(async () =>
            await deviceToken.Create(DbContext).ConfigureAwait(false));
    }

    private static DeviceToken NewDeviceToken(string installationId, string fcmToken, int sdkSiteId) =>
        new()
        {
            AppId = "time",
            InstallationId = installationId,
            FcmToken = fcmToken,
            SdkSiteId = sdkSiteId,
            Platform = "android"
        };
}
