namespace Microting.TimePlanningBase.Infrastructure.Data.Models;

public class TimePlanningBaseSettings
{
    public int? EformId{ get; set; }

    public int? FolderId { get; set; }

    public int? InfoeFormId { get; set; }

    public int? MaxHistoryDays { get; set; }

    public int? MaxDaysEditable { get; set; }

    public int? SiteIdsForCheck { get; set; }
}