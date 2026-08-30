using Microting.eFormApi.BasePn.Infrastructure.Database.Base;

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

// Snapshot of DeviceToken. PnBase.MapVersion copies BY PROPERTY NAME via
// reflection, so every property here must be named exactly as on DeviceToken
// or its value is silently dropped.
public class DeviceTokenVersion : BaseEntity
{
    public string AppId { get; set; }
    public string InstallationId { get; set; }
    public string FcmToken { get; set; }
    public int SdkSiteId { get; set; }
    public string Platform { get; set; }
    public int DeviceTokenId { get; set; }
    public int AppBuildNumber { get; set; }
}
