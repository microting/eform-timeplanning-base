namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

public class DeviceToken : PnBase
{
    public int SdkSiteId { get; set; }
    public string Token { get; set; }
    public string Platform { get; set; }
}
