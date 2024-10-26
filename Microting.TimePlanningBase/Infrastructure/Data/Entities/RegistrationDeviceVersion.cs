using Microting.eFormApi.BasePn.Infrastructure.Database.Base;

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

public class RegistrationDeviceVersion : BaseEntity
{
    public string Token { get; set; }
    public string SoftwareVersion { get; set; }
    public string Model { get; set; }
    public string Manufacturer { get; set; }
    public string OsVersion { get; set; }
    public string LastIp { get; set; }
    public string LastKnownLocation { get; set; }
    public string LookedUpIp { get; set; }
    public string OtpCode { get; set; }
    public bool OtpEnabled { get; set; }
    public int RegistrationDeviceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

}