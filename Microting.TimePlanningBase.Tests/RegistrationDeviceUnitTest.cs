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
        Assert.That(registrationDeviceFromDb.OtpCode, Is.Not.EqualTo("123456"));
        Assert.That(registrationDeviceFromDb.OtpEnabled);
        Assert.That(DbContext.RegistrationDevices.Count(), Is.EqualTo(1));
        Assert.That(DbContext.RegistrationDeviceVersions.Count(), Is.EqualTo(2));
        Assert.That(registrationDeviceFromDb.Version, Is.EqualTo(2));
        Assert.That(registrationDeviceFromDb.WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));
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
        Assert.That(registrationDeviceFromDb.Token, Is.Not.Null);
        Assert.That(DbContext.RegistrationDevices.Count(), Is.EqualTo(1));
        Assert.That(DbContext.RegistrationDeviceVersions.Count(), Is.EqualTo(2));
        Assert.That(registrationDeviceFromDb.Version, Is.EqualTo(2));
        Assert.That(registrationDeviceFromDb.WorkflowState, Is.EqualTo(Constants.WorkflowStates.Created));

    }

}