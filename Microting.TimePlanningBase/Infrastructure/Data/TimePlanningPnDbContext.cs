/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

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

namespace Microting.TimePlanningBase.Infrastructure.Data
{
    using eFormApi.BasePn.Abstractions;
    using eFormApi.BasePn.Infrastructure.Database.Entities;
    using Entities;
    using Extensions.Seed;
    using Microsoft.EntityFrameworkCore;
    using WorkingTime;

    public class TimePlanningPnDbContext : DbContext, IPluginDbContext
    {
        public TimePlanningPnDbContext() { }

        public TimePlanningPnDbContext(DbContextOptions<TimePlanningPnDbContext> options) : base(options)
        {
        }

        public DbSet<AssignedSite> AssignedSites { get; set; }
        public DbSet<AssignedSiteVersion> AssignedSiteVersions { get; set; }
        public DbSet<AssignedSiteManagingTag> AssignedSiteManagingTags { get; set; }
        public DbSet<AssignedSiteManagingTagVersion> AssignedSiteManagingTagVersions { get; set; }

        public DbSet<PlanRegistration> PlanRegistrations { get; set; }
        public DbSet<PlanRegistrationVersion> PlanRegistrationVersions { get; set; }

        public DbSet<Message> Messages { get; set; }

        // common tables
        public DbSet<PluginConfigurationValue> PluginConfigurationValues { get; set; }
        public DbSet<PluginConfigurationValueVersion> PluginConfigurationValueVersions { get; set; }
        public DbSet<PluginPermission> PluginPermissions { get; set; }
        public DbSet<PluginGroupPermission> PluginGroupPermissions { get; set; }
        public DbSet<PluginGroupPermissionVersion> PluginGroupPermissionVersions { get; set; }
        public DbSet<RegistrationDevice> RegistrationDevices { get; set; }
        public DbSet<RegistrationDeviceVersion> RegistrationDeviceVersions { get; set; }
        public DbSet<GpsCoordinate> GpsCoordinates { get; set; }
        public DbSet<GpsCoordinateVersion> GpsCoordinateVersions { get; set; }
        public DbSet<PictureSnapshot> PictureSnapshots { get; set; }
        public DbSet<PictureSnapshotVersion> PictureSnapshotVersions { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HolidayVersion> HolidayVersions { get; set; }
        public DbSet<WorkingTimeRuleSettings> WorkingTimeRuleSettings { get; set; }
        public DbSet<WorkingTimeRuleSettingsVersion> WorkingTimeRuleSettingsVersions { get; set; }
        public DbSet<BreakPolicy> BreakPolicies { get; set; }
        public DbSet<BreakPolicyVersion> BreakPolicyVersions { get; set; }
        public DbSet<BreakPolicyRule> BreakPolicyRules { get; set; }
        public DbSet<BreakPolicyRuleVersion> BreakPolicyRuleVersions { get; set; }
        public DbSet<WorkingTimeRuleSet> WorkingTimeRuleSets { get; set; }
        public DbSet<WorkingTimeRuleSetVersion> WorkingTimeRuleSetVersions { get; set; }
        public DbSet<PayRuleSet> PayRuleSets { get; set; }
        public DbSet<PayRuleSetVersion> PayRuleSetVersions { get; set; }
        public DbSet<PayDayRule> PayDayRules { get; set; }
        public DbSet<PayDayRuleVersion> PayDayRuleVersions { get; set; }
        public DbSet<PayTierRule> PayTierRules { get; set; }
        public DbSet<PayTierRuleVersion> PayTierRuleVersions { get; set; }
        public DbSet<PlanRegistrationPayLine> PlanRegistrationPayLines { get; set; }
        public DbSet<PlanRegistrationPayLineVersion> PlanRegistrationPayLineVersions { get; set; }
        public DbSet<PlanRegistrationContentHandoverRequest> PlanRegistrationContentHandoverRequests { get; set; }
        public DbSet<PlanRegistrationContentHandoverRequestVersion> PlanRegistrationContentHandoverRequestVersions { get; set; }
        public DbSet<AbsenceRequest> AbsenceRequests { get; set; }
        public DbSet<AbsenceRequestVersion> AbsenceRequestVersions { get; set; }
        public DbSet<AbsenceRequestDay> AbsenceRequestDays { get; set; }
        public DbSet<AbsenceRequestDayVersion> AbsenceRequestDayVersions { get; set; }
        public DbSet<PayDayTypeRule> PayDayTypeRules { get; set; }
        public DbSet<PayDayTypeRuleVersion> PayDayTypeRuleVersions { get; set; }
        public DbSet<PayTimeBandRule> PayTimeBandRules { get; set; }
        public DbSet<PayTimeBandRuleVersion> PayTimeBandRuleVersions { get; set; }
        public DbSet<AssignedSiteRuleSetAssignments> AssignedSiteRuleSetAssignments { get; set; }
        public DbSet<AssignedSiteRuleSetAssignmentsVersion> AssignedSiteRuleSetAssignmentsVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique index for PlanRegistrations
            modelBuilder.Entity<PlanRegistration>()
                .HasIndex(p => new { p.SdkSitId, p.Date, p.WorkflowState })
                .IsUnique();

            // Configure unique index for PayDayRule (PayRuleSetId, DayCode)
            modelBuilder.Entity<PayDayRule>()
                .HasIndex(p => new { p.PayRuleSetId, p.DayCode })
                .IsUnique();

            // Configure index for PayTierRule (PayDayRuleId, Order)
            modelBuilder.Entity<PayTierRule>()
                .HasIndex(p => new { p.PayDayRuleId, p.Order });

            // Configure index for PlanRegistrationPayLine (PlanRegistrationId)
            modelBuilder.Entity<PlanRegistrationPayLine>()
                .HasIndex(p => p.PlanRegistrationId);

            // Configure unique index for PlanRegistrationPayLine (PlanRegistrationId, PayCode)
            modelBuilder.Entity<PlanRegistrationPayLine>()
                .HasIndex(p => new { p.PlanRegistrationId, p.PayCode })
                .IsUnique();

            // Configure index for PlanRegistrationContentHandoverRequest (ToSdkSitId, Status, Date) - receiver inbox
            modelBuilder.Entity<PlanRegistrationContentHandoverRequest>()
                .HasIndex(p => new { p.ToSdkSitId, p.Status, p.Date });

            // Configure index for PlanRegistrationContentHandoverRequest (FromSdkSitId, Status, Date) - sender overview
            modelBuilder.Entity<PlanRegistrationContentHandoverRequest>()
                .HasIndex(p => new { p.FromSdkSitId, p.Status, p.Date });

            // Configure unique index for AssignedSiteManagingTag (AssignedSiteId, TagId)
            modelBuilder.Entity<AssignedSiteManagingTag>()
                .HasIndex(p => new { p.AssignedSiteId, p.TagId })
                .IsUnique();

            // Configure index for PayDayTypeRule (PayRuleSetId, DayType, Priority)
            modelBuilder.Entity<PayDayTypeRule>()
                .HasIndex(p => new { p.PayRuleSetId, p.DayType, p.Priority });

            // Configure index for PayTimeBandRule (PayDayTypeRuleId, Priority)
            modelBuilder.Entity<PayTimeBandRule>()
                .HasIndex(p => new { p.PayDayTypeRuleId, p.Priority });

            // Configure index for AssignedSiteRuleSetAssignments (AssignedSiteId, ValidFromDate)
            modelBuilder.Entity<AssignedSiteRuleSetAssignments>()
                .HasIndex(p => new { p.AssignedSiteId, p.ValidFromDate });

            modelBuilder.SeedLatest();
        }
    }
}