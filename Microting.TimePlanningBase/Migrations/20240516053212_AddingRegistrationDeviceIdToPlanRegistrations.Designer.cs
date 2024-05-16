﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microting.TimePlanningBase.Infrastructure.Data;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    [DbContext(typeof(TimePlanningPnDbContext))]
    [Migration("20240516053212_AddingRegistrationDeviceIdToPlanRegistrations")]
    partial class AddingRegistrationDeviceIdToPlanRegistrations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.AssignedSite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CaseMicrotingUid")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("SiteId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("AssignedSites");
                });

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.AssignedSiteVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssignedSiteId")
                        .HasColumnType("int");

                    b.Property<int?>("CaseMicrotingUid")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("SiteId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("AssignedSiteVersions");
                });

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DaName")
                        .HasColumnType("longtext");

                    b.Property<string>("DeName")
                        .HasColumnType("longtext");

                    b.Property<string>("EnName")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Messages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DaName = "Fridag",
                            DeName = "Freitag",
                            EnName = "Day off",
                            Name = "Day Off"
                        },
                        new
                        {
                            Id = 2,
                            DaName = "Ferie",
                            DeName = "Ferien",
                            EnName = "Vacation",
                            Name = "Vacation"
                        },
                        new
                        {
                            Id = 3,
                            DaName = "Sygdom",
                            DeName = "Krank",
                            EnName = "Sick",
                            Name = "Sick"
                        },
                        new
                        {
                            Id = 4,
                            DaName = "Kursus",
                            DeName = "Kurs",
                            EnName = "Course",
                            Name = "Course"
                        },
                        new
                        {
                            Id = 5,
                            DaName = "Orlov",
                            DeName = "Urlaub",
                            EnName = "Leave of absence",
                            Name = "Leave of absence"
                        },
                        new
                        {
                            Id = 6,
                            DaName = "",
                            DeName = "",
                            EnName = "",
                            Name = "Care"
                        },
                        new
                        {
                            Id = 7,
                            DaName = "Barns 1. sygedag",
                            DeName = "Kinder 1. Krank",
                            EnName = "Children's 1st sick",
                            Name = "Children's 1st sick"
                        },
                        new
                        {
                            Id = 8,
                            DaName = "Barns 2. sygedag",
                            DeName = "Kinder 2. Krank",
                            EnName = "Children's 2nd sick",
                            Name = "Children's 2nd sick"
                        },
                        new
                        {
                            Id = 9,
                            DaName = "Tidsoff",
                            DeName = "Urlaub",
                            EnName = "Time off",
                            Name = "Time off"
                        });
                });

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.PlanRegistration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CommentOffice")
                        .HasColumnType("longtext");

                    b.Property<string>("CommentOfficeAll")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<bool>("DataFromDevice")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("Flex")
                        .HasColumnType("double");

                    b.Property<int?>("MessageId")
                        .HasColumnType("int");

                    b.Property<double>("NettoHours")
                        .HasColumnType("double");

                    b.Property<double>("PaiedOutFlex")
                        .HasColumnType("double");

                    b.Property<int>("Pause1Id")
                        .HasColumnType("int");

                    b.Property<int>("Pause2Id")
                        .HasColumnType("int");

                    b.Property<double>("PlanHours")
                        .HasColumnType("double");

                    b.Property<string>("PlanText")
                        .HasColumnType("longtext");

                    b.Property<int?>("RegistrationDeviceId")
                        .HasColumnType("int");

                    b.Property<int>("SdkSitId")
                        .HasColumnType("int");

                    b.Property<int>("Start1Id")
                        .HasColumnType("int");

                    b.Property<int>("Start2Id")
                        .HasColumnType("int");

                    b.Property<int>("StatusCaseId")
                        .HasColumnType("int");

                    b.Property<int>("Stop1Id")
                        .HasColumnType("int");

                    b.Property<int>("Stop2Id")
                        .HasColumnType("int");

                    b.Property<double>("SumFlexEnd")
                        .HasColumnType("double");

                    b.Property<double>("SumFlexStart")
                        .HasColumnType("double");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkerComment")
                        .HasColumnType("longtext");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("PlanRegistrations");
                });

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.PlanRegistrationVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CommentOffice")
                        .HasColumnType("longtext");

                    b.Property<string>("CommentOfficeAll")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<bool>("DataFromDevice")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("Flex")
                        .HasColumnType("double");

                    b.Property<int?>("MessageId")
                        .HasColumnType("int");

                    b.Property<double>("NettoHours")
                        .HasColumnType("double");

                    b.Property<double>("PaiedOutFlex")
                        .HasColumnType("double");

                    b.Property<int>("Pause1Id")
                        .HasColumnType("int");

                    b.Property<int>("Pause2Id")
                        .HasColumnType("int");

                    b.Property<double>("PlanHours")
                        .HasColumnType("double");

                    b.Property<int>("PlanRegistrationId")
                        .HasColumnType("int");

                    b.Property<string>("PlanText")
                        .HasColumnType("longtext");

                    b.Property<int?>("RegistrationDeviceId")
                        .HasColumnType("int");

                    b.Property<int>("SdkSitId")
                        .HasColumnType("int");

                    b.Property<int>("Start1Id")
                        .HasColumnType("int");

                    b.Property<int>("Start2Id")
                        .HasColumnType("int");

                    b.Property<int>("StatusCaseId")
                        .HasColumnType("int");

                    b.Property<int>("Stop1Id")
                        .HasColumnType("int");

                    b.Property<int>("Stop2Id")
                        .HasColumnType("int");

                    b.Property<double>("SumFlexEnd")
                        .HasColumnType("double");

                    b.Property<double>("SumFlexStart")
                        .HasColumnType("double");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkerComment")
                        .HasColumnType("longtext");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("PlanRegistrationVersions");
                });

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.RegistrationDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("LastIp")
                        .HasColumnType("longtext");

                    b.Property<string>("LastKnownLocation")
                        .HasColumnType("longtext");

                    b.Property<string>("LookedUpIp")
                        .HasColumnType("longtext");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("longtext");

                    b.Property<string>("Model")
                        .HasColumnType("longtext");

                    b.Property<string>("OsVersion")
                        .HasColumnType("longtext");

                    b.Property<string>("OtpCode")
                        .HasColumnType("longtext");

                    b.Property<bool>("OtpEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SoftwareVersion")
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("RegistrationDevices");
                });

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.RegistrationDeviceVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("LastIp")
                        .HasColumnType("longtext");

                    b.Property<string>("LastKnownLocation")
                        .HasColumnType("longtext");

                    b.Property<string>("LookedUpIp")
                        .HasColumnType("longtext");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("longtext");

                    b.Property<string>("Model")
                        .HasColumnType("longtext");

                    b.Property<string>("OsVersion")
                        .HasColumnType("longtext");

                    b.Property<string>("OtpCode")
                        .HasColumnType("longtext");

                    b.Property<bool>("OtpEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("RegistrationDeviceId")
                        .HasColumnType("int");

                    b.Property<string>("SoftwareVersion")
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("RegistrationDeviceVersions");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.PluginConfigurationValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("PluginConfigurationValues");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.PluginConfigurationValueVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("PluginConfigurationValueVersions");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.PluginGroupPermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.ToTable("PluginGroupPermissions");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.PluginGroupPermissionVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.Property<int>("PluginGroupPermissionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("PluginGroupPermissionVersions");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.PluginPermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimName")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("PermissionName")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UpdatedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("WorkflowState")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("PluginPermissions");
                });

            modelBuilder.Entity("Microting.TimePlanningBase.Infrastructure.Data.Entities.PlanRegistration", b =>
                {
                    b.HasOne("Microting.TimePlanningBase.Infrastructure.Data.Entities.Message", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.PluginGroupPermission", b =>
                {
                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.PluginPermission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");
                });
#pragma warning restore 612, 618
        }
    }
}
