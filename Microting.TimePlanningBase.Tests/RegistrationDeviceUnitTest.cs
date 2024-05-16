using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.TimePlanningBase.Infrastructure.Data.Entities;
using NUnit.Framework;

namespace Microting.TimePlanningBase.Tests;

[TestFixture]
public class RegistrationDeviceUnitTest : DbTestFixture
{

    [Test]
    public async Task RegistrationDevice_GenerateOtp_DoesGenerateOtp()
    {
        // Arrange
        var registrationDevice = new RegistrationDevice
        {
            OtpCode = "123456",
            OtpEnabled = false
        };
        await registrationDevice.Create(DbContext).ConfigureAwait(false);

        // Act
        await registrationDevice.GenerateOtp(DbContext).ConfigureAwait(false);

        // Assert
        var registrationDeviceFromDb = DbContext.RegistrationDevices.AsNoTracking().First();
        Assert.AreNotEqual("123456", registrationDeviceFromDb.OtpCode);
        Assert.IsTrue(registrationDeviceFromDb.OtpEnabled);
        Assert.AreEqual(1, DbContext.RegistrationDevices.Count());
        Assert.AreEqual(2, DbContext.RegistrationDeviceVersions.Count());
        Assert.AreEqual(2, registrationDeviceFromDb.Version);
        Assert.AreEqual(Constants.WorkflowStates.Created, registrationDeviceFromDb.WorkflowState);
    }

    [Test]
    public async Task RegistrationDevice_GenerateToken_DoesGenerateToken()
    {
        // Arrange
        var registrationDevice = new RegistrationDevice();
        await registrationDevice.Create(DbContext).ConfigureAwait(false);

        // Act
        await registrationDevice.GenerateToken(DbContext).ConfigureAwait(false);

        // Assert
        var registrationDeviceFromDb = DbContext.RegistrationDevices.AsNoTracking().First();
        Assert.IsNotNull(registrationDeviceFromDb.Token);
        Assert.AreEqual(1, DbContext.RegistrationDevices.Count());
        Assert.AreEqual(2, DbContext.RegistrationDeviceVersions.Count());
        Assert.AreEqual(2, registrationDeviceFromDb.Version);
        Assert.AreEqual(Constants.WorkflowStates.Created, registrationDeviceFromDb.WorkflowState);

    }

}