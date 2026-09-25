using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace paskalON.PowerControls.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class v_0_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "DomainBaseSequence");

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
                name: "DerUnitEnergyStoragePowerControlConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    DerUnitName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DistributionStrategyType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerUnitEnergyStoragePowerControlConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DerUnitPowerConstraintConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PowerControlBaseConfigId = table.Column<int>(type: "integer", nullable: true),
                    MaximumActivePowerWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerVars = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerUnitPowerConstraintConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DerUnitPowerControlConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    DerUnitName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DistributionStrategyType = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerUnitPowerControlConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DerUnitRampConstraintConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PowerControlBaseConfigId = table.Column<int>(type: "integer", nullable: true),
                    MaximumActivePowerWattRampRatePerSecond = table.Column<double>(type: "double precision", nullable: false),
                    MaximumReactivePowerVarsRampRatePerSecond = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerUnitRampConstraintConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
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
                    StartupDelayForDevices = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemPowerConstraintConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PowerControlBaseConfigId = table.Column<int>(type: "integer", nullable: true),
                    DeratePerUnitStopped = table.Column<bool>(type: "boolean", nullable: false),
                    DeratePerUnitInMaintenance = table.Column<bool>(type: "boolean", nullable: false),
                    MaximumActivePowerWatt = table.Column<double>(type: "double precision", nullable: true),
                    MinimumActivePowerWatt = table.Column<double>(type: "double precision", nullable: true),
                    MaximumReactivePowerVars = table.Column<double>(type: "double precision", nullable: true),
                    MinimumReactivePowerVars = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPowerConstraintConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemPowerControlConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPowerControlConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemRampConstraintConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PowerControlBaseConfigId = table.Column<int>(type: "integer", nullable: true),
                    MaximumActivePowerWattRampRatePerSecond = table.Column<double>(type: "double precision", nullable: false),
                    MaximumReactivePowerVarsRampRatePerSecond = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRampConstraintConfig", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DerUnitEnergyStoragePowerControlConfig_Name",
                table: "DerUnitEnergyStoragePowerControlConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerUnitPowerConstraintConfig_Name",
                table: "DerUnitPowerConstraintConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerUnitPowerConstraintConfig_PowerControlBaseConfigId",
                table: "DerUnitPowerConstraintConfig",
                column: "PowerControlBaseConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DerUnitPowerControlConfig_Name",
                table: "DerUnitPowerControlConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerUnitRampConstraintConfig_Name",
                table: "DerUnitRampConstraintConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerUnitRampConstraintConfig_PowerControlBaseConfigId",
                table: "DerUnitRampConstraintConfig",
                column: "PowerControlBaseConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerConstraintConfig_Name",
                table: "SystemPowerConstraintConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerConstraintConfig_PowerControlBaseConfigId",
                table: "SystemPowerConstraintConfig",
                column: "PowerControlBaseConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerControlConfig_Name",
                table: "SystemPowerControlConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemRampConstraintConfig_Name",
                table: "SystemRampConstraintConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemRampConstraintConfig_PowerControlBaseConfigId",
                table: "SystemRampConstraintConfig",
                column: "PowerControlBaseConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "DerUnitEnergyStoragePowerControlConfig");

            migrationBuilder.DropTable(
                name: "DerUnitPowerConstraintConfig");

            migrationBuilder.DropTable(
                name: "DerUnitPowerControlConfig");

            migrationBuilder.DropTable(
                name: "DerUnitRampConstraintConfig");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "SystemPowerConstraintConfig");

            migrationBuilder.DropTable(
                name: "SystemPowerControlConfig");

            migrationBuilder.DropTable(
                name: "SystemRampConstraintConfig");

            migrationBuilder.DropSequence(
                name: "DomainBaseSequence");
        }
    }
}
