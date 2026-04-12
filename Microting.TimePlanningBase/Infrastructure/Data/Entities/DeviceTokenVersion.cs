using Microting.eFormApi.BasePn.Infrastructure.Database.Base;

namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

public class DeviceTokenVersion : BaseEntity
{
    public int SdkSiteId { get; set; }
    public string Token { get; set; }
    public string Platform { get; set; }
    public int DeviceTokenId { get; set; }
}
