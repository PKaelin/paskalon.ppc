using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace paskalON.OperatingModes.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class v_0_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive_collation", "und-u-ks-level2,und-u-ks-level2,icu,False");

            migrationBuilder.CreateSequence(
                name: "DomainBaseSequence");

            migrationBuilder.CreateTable(
                name: "ActivePowerFixedModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivePowerFixedModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivePowerModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    ProportionalGain = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivePowerModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargeDischargeModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    MaximumStorageReserveDeadband = table.Column<double>(type: "double precision", nullable: false),
                    MinimumStorageReserveDeadband = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeDischargeModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Key = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoordinatedChargeDischargeModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    MaximumStorageReserveDeadband = table.Column<double>(type: "double precision", nullable: false),
                    MinimumStorageReserveDeadband = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoordinatedChargeDischargeModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurvePointConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CurveBaseConfigId = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurvePointConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FrequencyDroopModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    HealthyResponseDelayMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequencyDroopModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FrequencyWattCurveConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    XUnit = table.Column<int>(type: "integer", nullable: false),
                    YUnit = table.Column<int>(type: "integer", nullable: false),
                    UseRamp = table.Column<bool>(type: "boolean", nullable: false),
                    RampUpTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampDownTimeSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequencyWattCurveConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FrequencyWattModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    HealthyResponseDelayMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrequencyWattModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    DerUnitName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceSocModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    UseTargetStateOfCharge = table.Column<bool>(type: "boolean", nullable: false),
                    TargetStateOfCharge = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSocModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaximumActivePowerLimitModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    MaximumActivePowerLimitWatts = table.Column<double>(type: "double precision", nullable: true),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaximumActivePowerLimitModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaximumPowerPointTrackingModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaximumPowerPointTrackingModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerFactorModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    PowerFactorTarget = table.Column<float>(type: "real", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerFactorModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RampRateConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RampTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampUpRatePerSecond = table.Column<double>(type: "double precision", nullable: false),
                    RampDownRatePerSecond = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RampRateConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RampRatePercentageConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RampTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampUpRatePercentPerSecond = table.Column<double>(type: "double precision", nullable: false),
                    RampDownRatePercentPerSecond = table.Column<double>(type: "double precision", nullable: false),
                    RampRatePrecision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RampRatePercentageConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RampTimeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RampTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampUpTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampDownTimeSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RampTimeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RampTimeConstantConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RampTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampUpTimeConstantSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampDownTimeConstantSeconds = table.Column<int>(type: "integer", nullable: false),
                    TuningValue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RampTimeConstantConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReactivePowerFixedModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactivePowerFixedModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReactivePowerModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    ProportionalGain = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactivePowerModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ReferenceFrequency = table.Column<double>(type: "double precision", nullable: false),
                    ReferenceVoltage = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumVoltage = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMinimumVoltage = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumCurrent = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMinimumCurrent = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumActivePowerWatt = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMinimumActivePowerWatt = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumReactivePowerVars = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMinimumReactivePowerVars = table.Column<double>(type: "double precision", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    SubscriberTopicPcsCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicPcsDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicBatteryBankCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicBatteryBankDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSolarPanelCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSolarPanelDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicExternalPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicExternalPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicAuxiliaryPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicAuxiliaryPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicCircuitPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicCircuitPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSystemPowerMeterCore = table.Column<string>(type: "text", nullable: true),
                    SubscriberTopicSystemPowerMeterDetail = table.Column<string>(type: "text", nullable: true),
                    StartupDelay = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoltageModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltageModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoltageVarDroopModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltageVarDroopModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoltageWattDroopModeConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampConfigId = table.Column<int>(type: "integer", nullable: false),
                    CurveConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeadbandSetpoint = table.Column<double>(type: "double precision", nullable: false),
                    DeadbandAvailable = table.Column<double>(type: "double precision", nullable: false),
                    MaximumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerLimitWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerLimitVars = table.Column<double>(type: "double precision", nullable: true),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    DeadbandError = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltageWattDroopModeConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoltVarCurveConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    XUnit = table.Column<int>(type: "integer", nullable: false),
                    YUnit = table.Column<int>(type: "integer", nullable: false),
                    UseRamp = table.Column<bool>(type: "boolean", nullable: false),
                    RampUpTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampDownTimeSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltVarCurveConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoltWattCurveConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, collation: "case_insensitive_collation"),
                    XUnit = table.Column<int>(type: "integer", nullable: false),
                    YUnit = table.Column<int>(type: "integer", nullable: false),
                    UseRamp = table.Column<bool>(type: "boolean", nullable: false),
                    RampUpTimeSeconds = table.Column<int>(type: "integer", nullable: false),
                    RampDownTimeSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltWattCurveConfig", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivePowerFixedModeConfig_CurveConfigId",
                table: "ActivePowerFixedModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivePowerFixedModeConfig_Name",
                table: "ActivePowerFixedModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivePowerFixedModeConfig_RampConfigId",
                table: "ActivePowerFixedModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivePowerModeConfig_CurveConfigId",
                table: "ActivePowerModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivePowerModeConfig_Name",
                table: "ActivePowerModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivePowerModeConfig_RampConfigId",
                table: "ActivePowerModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeDischargeModeConfig_CurveConfigId",
                table: "ChargeDischargeModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeDischargeModeConfig_Name",
                table: "ChargeDischargeModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChargeDischargeModeConfig_RampConfigId",
                table: "ChargeDischargeModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_Key",
                table: "Configuration",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoordinatedChargeDischargeModeConfig_CurveConfigId",
                table: "CoordinatedChargeDischargeModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CoordinatedChargeDischargeModeConfig_Name",
                table: "CoordinatedChargeDischargeModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoordinatedChargeDischargeModeConfig_RampConfigId",
                table: "CoordinatedChargeDischargeModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CurvePointConfig_CurveBaseConfigId",
                table: "CurvePointConfig",
                column: "CurveBaseConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyDroopModeConfig_CurveConfigId",
                table: "FrequencyDroopModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyDroopModeConfig_Name",
                table: "FrequencyDroopModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyDroopModeConfig_RampConfigId",
                table: "FrequencyDroopModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyWattCurveConfig_Name",
                table: "FrequencyWattCurveConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyWattModeConfig_CurveConfigId",
                table: "FrequencyWattModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyWattModeConfig_Name",
                table: "FrequencyWattModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrequencyWattModeConfig_RampConfigId",
                table: "FrequencyWattModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceModeConfig_CurveConfigId",
                table: "MaintenanceModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceModeConfig_Name",
                table: "MaintenanceModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceModeConfig_RampConfigId",
                table: "MaintenanceModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSocModeConfig_CurveConfigId",
                table: "MaintenanceSocModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSocModeConfig_Name",
                table: "MaintenanceSocModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceSocModeConfig_RampConfigId",
                table: "MaintenanceSocModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaximumActivePowerLimitModeConfig_CurveConfigId",
                table: "MaximumActivePowerLimitModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaximumActivePowerLimitModeConfig_Name",
                table: "MaximumActivePowerLimitModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaximumActivePowerLimitModeConfig_RampConfigId",
                table: "MaximumActivePowerLimitModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaximumPowerPointTrackingModeConfig_CurveConfigId",
                table: "MaximumPowerPointTrackingModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_MaximumPowerPointTrackingModeConfig_Name",
                table: "MaximumPowerPointTrackingModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaximumPowerPointTrackingModeConfig_RampConfigId",
                table: "MaximumPowerPointTrackingModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerFactorModeConfig_CurveConfigId",
                table: "PowerFactorModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerFactorModeConfig_Name",
                table: "PowerFactorModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerFactorModeConfig_RampConfigId",
                table: "PowerFactorModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ReactivePowerFixedModeConfig_CurveConfigId",
                table: "ReactivePowerFixedModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ReactivePowerFixedModeConfig_Name",
                table: "ReactivePowerFixedModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReactivePowerFixedModeConfig_RampConfigId",
                table: "ReactivePowerFixedModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ReactivePowerModeConfig_CurveConfigId",
                table: "ReactivePowerModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ReactivePowerModeConfig_Name",
                table: "ReactivePowerModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReactivePowerModeConfig_RampConfigId",
                table: "ReactivePowerModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_VoltageModeConfig_CurveConfigId",
                table: "VoltageModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_VoltageModeConfig_Name",
                table: "VoltageModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoltageModeConfig_RampConfigId",
                table: "VoltageModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_VoltageVarDroopModeConfig_CurveConfigId",
                table: "VoltageVarDroopModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_VoltageVarDroopModeConfig_Name",
                table: "VoltageVarDroopModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoltageVarDroopModeConfig_RampConfigId",
                table: "VoltageVarDroopModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_VoltageWattDroopModeConfig_CurveConfigId",
                table: "VoltageWattDroopModeConfig",
                column: "CurveConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_VoltageWattDroopModeConfig_Name",
                table: "VoltageWattDroopModeConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoltageWattDroopModeConfig_RampConfigId",
                table: "VoltageWattDroopModeConfig",
                column: "RampConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_VoltVarCurveConfig_Name",
                table: "VoltVarCurveConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoltWattCurveConfig_Name",
                table: "VoltWattCurveConfig",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivePowerFixedModeConfig");

            migrationBuilder.DropTable(
                name: "ActivePowerModeConfig");

            migrationBuilder.DropTable(
                name: "ChargeDischargeModeConfig");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "CoordinatedChargeDischargeModeConfig");

            migrationBuilder.DropTable(
                name: "CurvePointConfig");

            migrationBuilder.DropTable(
                name: "FrequencyDroopModeConfig");

            migrationBuilder.DropTable(
                name: "FrequencyWattCurveConfig");

            migrationBuilder.DropTable(
                name: "FrequencyWattModeConfig");

            migrationBuilder.DropTable(
                name: "MaintenanceModeConfig");

            migrationBuilder.DropTable(
                name: "MaintenanceSocModeConfig");

            migrationBuilder.DropTable(
                name: "MaximumActivePowerLimitModeConfig");

            migrationBuilder.DropTable(
                name: "MaximumPowerPointTrackingModeConfig");

            migrationBuilder.DropTable(
                name: "PowerFactorModeConfig");

            migrationBuilder.DropTable(
                name: "RampRateConfig");

            migrationBuilder.DropTable(
                name: "RampRatePercentageConfig");

            migrationBuilder.DropTable(
                name: "RampTimeConfig");

            migrationBuilder.DropTable(
                name: "RampTimeConstantConfig");

            migrationBuilder.DropTable(
                name: "ReactivePowerFixedModeConfig");

            migrationBuilder.DropTable(
                name: "ReactivePowerModeConfig");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "VoltageModeConfig");

            migrationBuilder.DropTable(
                name: "VoltageVarDroopModeConfig");

            migrationBuilder.DropTable(
                name: "VoltageWattDroopModeConfig");

            migrationBuilder.DropTable(
                name: "VoltVarCurveConfig");

            migrationBuilder.DropTable(
                name: "VoltWattCurveConfig");

            migrationBuilder.DropSequence(
                name: "DomainBaseSequence");
        }
    }
}
