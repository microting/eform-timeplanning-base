namespace Microting.TimePlanningBase.Infrastructure.Data.Entities;

using System.ComponentModel.DataAnnotations;

// Identity is the app install, not the token: (AppId, InstallationId) is
// unique. FcmToken is mutable (it rotates); SdkSiteId is a mutable owner
// reassigned when a different user logs in on the same device.
//
// The unique index has no WorkflowState filter and PnBase.Delete() only
// soft-deletes, so consumers MUST upsert on (AppId, InstallationId)
// including soft-deleted rows and flip WorkflowState back to Created.
public class DeviceToken : PnBase
{
    // [Required] on AppId and InstallationId is load-bearing: nullable reference
    // types are disabled in this project, so without it EF maps both as NULL-able
    // and MySQL's unique index stops collapsing duplicate installs (NULLs never
    // collide).
    [Required]
    [StringLength(32)]
    public string AppId { get; set; }

    [Required]
    [StringLength(128)]
    public string InstallationId { get; set; }

    [StringLength(512)]
    public string FcmToken { get; set; }

    // SDK Site.Id of the worker owning the device.
    public int SdkSiteId { get; set; }

    // e.g. "android" or "ios"
    public string Platform { get; set; }

    public int AppBuildNumber { get; set; }
}
