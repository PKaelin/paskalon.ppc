using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace paskalON.Devices.Infrastructure.Storage.Migrations
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
                name: "BatteryBankDeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: false),
                    BatteryType = table.Column<int>(type: "integer", nullable: false),
                    NameplateCapacity = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumChargeRate = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumDischargeRate = table.Column<double>(type: "double precision", nullable: false),
                    RackCount = table.Column<int>(type: "integer", nullable: false),
                    ModulesPerRackCount = table.Column<int>(type: "integer", nullable: false),
                    InverterBusNumber = table.Column<int>(type: "integer", nullable: false),
                    AbsoluteMinimumStateOfCharge = table.Column<double>(type: "double precision", nullable: false),
                    AbsoluteMaximumStateOfCharge = table.Column<double>(type: "double precision", nullable: false),
                    AbsoluteMinimumTemperature = table.Column<double>(type: "double precision", nullable: false),
                    AbsoluteMaximumTemperature = table.Column<double>(type: "double precision", nullable: false),
                    PreferredMinimumStateOfCharge = table.Column<double>(type: "double precision", nullable: false),
                    PreferredMaximumStateOfCharge = table.Column<double>(type: "double precision", nullable: false),
                    PreferredMinimumTemperature = table.Column<double>(type: "double precision", nullable: false),
                    PreferredMaximumTemperature = table.Column<double>(type: "double precision", nullable: false),
                    AbsoluteMaxDischargeCurrentAmps = table.Column<double>(type: "double precision", nullable: false),
                    AbsoluteMaxChargeCurrentAmps = table.Column<double>(type: "double precision", nullable: false),
                    MinimumDcVoltage = table.Column<double>(type: "double precision", nullable: false),
                    MaximumDcVoltage = table.Column<double>(type: "double precision", nullable: false),
                    ZeroCapacityOnCommLoss = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryBankDeviceConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "C37Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    TransportLayer = table.Column<int>(type: "integer", nullable: false),
                    StationName = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    StreamId = table.Column<int>(type: "integer", nullable: false),
                    ConfigFrameTimeoutMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    DataFrameTimeoutMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    DataFrameRetryCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_C37Config", x => x.Id);
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
                name: "DerConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DerContainerConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    DerUnitConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusConfigId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerContainerConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusMapConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    SlaveHeartbeatRegister = table.Column<int>(type: "integer", nullable: true),
                    MasterHeartbeatRegister = table.Column<int>(type: "integer", nullable: true),
                    MasterHeartbeatPollingInterval = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusMapConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModbusConnectionConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PollingIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    PollingFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    PollingFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    PollingFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    PollingFactorClass4 = table.Column<int>(type: "integer", nullable: false),
                    PollingFactorClass5 = table.Column<int>(type: "integer", nullable: false),
                    MasterHeartBeatIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    IsPipeliningEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ConnectionTimeoutMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    DisconnectionTimeoutMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    ConnectRetryCount = table.Column<int>(type: "integer", nullable: false),
                    ConnectRetryIntervalMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    SendTimeoutMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    SendRetryCount = table.Column<int>(type: "integer", nullable: false),
                    SendRetryIntervalMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    ServerToClientAliveIntervalSeconds = table.Column<int>(type: "integer", nullable: false),
                    ServerMaximumConnections = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusConnectionConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModbusRegisterMapEntryConfig",
                columns: table => new
                {
                    ModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    ModbusRegisterFormat = table.Column<int>(type: "integer", nullable: true),
                    Scale = table.Column<double>(type: "double precision", nullable: false),
                    IndividualOffset = table.Column<int>(type: "integer", nullable: false),
                    UnitPrefix = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusRegisterMapEntryConfig", x => x.ModbusRegisterMapEntryConfigId);
                });

            migrationBuilder.CreateTable(
                name: "PowerConversionSystemDeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: false),
                    NameplateMaximumActivePower = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumReactivePower = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumApparentPower = table.Column<double>(type: "double precision", nullable: false),
                    NameplateMaximumACCurrent = table.Column<double>(type: "double precision", nullable: false),
                    MinimumDCVoltage = table.Column<double>(type: "double precision", nullable: false),
                    MaximumDCVoltage = table.Column<double>(type: "double precision", nullable: false),
                    ZeroOutputOnCommLoss = table.Column<bool>(type: "boolean", nullable: false),
                    StandbyActivePowerKiloWatts = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerConversionSystemDeviceConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerMeterMapC37Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ApparentPower = table.Column<string>(type: "text", nullable: true),
                    CurrentA = table.Column<string>(type: "text", nullable: true),
                    CurrentB = table.Column<string>(type: "text", nullable: true),
                    CurrentC = table.Column<string>(type: "text", nullable: true),
                    EnergyDelivered = table.Column<string>(type: "text", nullable: true),
                    EnergyReceived = table.Column<string>(type: "text", nullable: true),
                    ReactiveEnergyDelivered = table.Column<string>(type: "text", nullable: true),
                    ReactiveEnergyReceived = table.Column<string>(type: "text", nullable: true),
                    ReactivePower = table.Column<string>(type: "text", nullable: true),
                    ReactivePowerA = table.Column<string>(type: "text", nullable: true),
                    ReactivePowerB = table.Column<string>(type: "text", nullable: true),
                    ReactivePowerC = table.Column<string>(type: "text", nullable: true),
                    ActivePower = table.Column<string>(type: "text", nullable: true),
                    ActivePowerA = table.Column<string>(type: "text", nullable: true),
                    ActivePowerB = table.Column<string>(type: "text", nullable: true),
                    ActivePowerC = table.Column<string>(type: "text", nullable: true),
                    VoltagePositiveSequence = table.Column<string>(type: "text", nullable: true),
                    VoltageA = table.Column<string>(type: "text", nullable: true),
                    VoltageB = table.Column<string>(type: "text", nullable: true),
                    VoltageC = table.Column<string>(type: "text", nullable: true),
                    VoltageLLAvg = table.Column<string>(type: "text", nullable: true),
                    VoltageAB = table.Column<string>(type: "text", nullable: true),
                    VoltageBC = table.Column<string>(type: "text", nullable: true),
                    VoltageCA = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerMeterMapC37Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarPanelDeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: false),
                    MinimumVoltage = table.Column<double>(type: "double precision", nullable: false),
                    MaximumVoltage = table.Column<double>(type: "double precision", nullable: false),
                    MinimumCurrent = table.Column<double>(type: "double precision", nullable: false),
                    MaximumCurrent = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarPanelDeviceConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BatteryBankDeviceCustomConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Key = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    BatteryBankDeviceConfigId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryBankDeviceCustomConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatteryBankDeviceCustomConfig_BatteryBankDeviceConfig_Batte~",
                        column: x => x.BatteryBankDeviceConfigId,
                        principalTable: "BatteryBankDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DerGroupConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DerConfigId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerGroupConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DerGroupConfig_DerConfig_DerConfigId",
                        column: x => x.DerConfigId,
                        principalTable: "DerConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusCoilPointConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GenericModbusMapConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusDataType = table.Column<int>(type: "integer", nullable: false),
                    ModbusNumber = table.Column<int>(type: "integer", nullable: false),
                    PollingInterval = table.Column<int>(type: "integer", nullable: false),
                    IsAlarm = table.Column<bool>(type: "boolean", nullable: false),
                    IsAlarmReset = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusCoilPointConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericModbusCoilPointConfig_GenericModbusMapConfig_Generic~",
                        column: x => x.GenericModbusMapConfigId,
                        principalTable: "GenericModbusMapConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusDeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GenericModbusMapConfigId = table.Column<int>(type: "integer", nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusDeviceConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericModbusDeviceConfig_GenericModbusMapConfig_GenericMod~",
                        column: x => x.GenericModbusMapConfigId,
                        principalTable: "GenericModbusMapConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusDiscreteInputPointConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GenericModbusMapConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusDataType = table.Column<int>(type: "integer", nullable: false),
                    ModbusNumber = table.Column<int>(type: "integer", nullable: false),
                    PollingInterval = table.Column<int>(type: "integer", nullable: false),
                    IsAlarm = table.Column<bool>(type: "boolean", nullable: false),
                    IsAlarmReset = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusDiscreteInputPointConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericModbusDiscreteInputPointConfig_GenericModbusMapConfi~",
                        column: x => x.GenericModbusMapConfigId,
                        principalTable: "GenericModbusMapConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusHoldingRegisterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GenericModbusMapConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusDataType = table.Column<int>(type: "integer", nullable: false),
                    ModbusNumber = table.Column<int>(type: "integer", nullable: false),
                    PollingInterval = table.Column<int>(type: "integer", nullable: false),
                    ModbusScale = table.Column<double>(type: "double precision", nullable: false),
                    IndividualOffset = table.Column<int>(type: "integer", nullable: false),
                    BitIndex = table.Column<short>(type: "smallint", nullable: false),
                    ReverseSign = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusHoldingRegisterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericModbusHoldingRegisterConfig_GenericModbusMapConfig_G~",
                        column: x => x.GenericModbusMapConfigId,
                        principalTable: "GenericModbusMapConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusInputRegisterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GenericModbusMapConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusDataType = table.Column<int>(type: "integer", nullable: false),
                    ModbusNumber = table.Column<int>(type: "integer", nullable: false),
                    PollingInterval = table.Column<int>(type: "integer", nullable: false),
                    ModbusScale = table.Column<double>(type: "double precision", nullable: false),
                    IndividualOffset = table.Column<int>(type: "integer", nullable: false),
                    BitIndex = table.Column<short>(type: "smallint", nullable: false),
                    ReverseSign = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusInputRegisterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericModbusInputRegisterConfig_GenericModbusMapConfig_Gen~",
                        column: x => x.GenericModbusMapConfigId,
                        principalTable: "GenericModbusMapConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModbusConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ModbusConnectionConfigId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    AddressFamily = table.Column<int>(type: "integer", nullable: false),
                    StationId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModbusConfig_ModbusConnectionConfig_ModbusConnectionConfigId",
                        column: x => x.ModbusConnectionConfigId,
                        principalTable: "ModbusConnectionConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerMeterMapModbusConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    StartingHoldingRegister = table.Column<int>(type: "integer", nullable: false),
                    StartingInputRegister = table.Column<int>(type: "integer", nullable: false),
                    StartingDiscreteInput = table.Column<int>(type: "integer", nullable: false),
                    StartingCoil = table.Column<int>(type: "integer", nullable: false),
                    ActivePowerMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ReactivePowerMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ApparentPowerMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    PowerFactorMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    FrequencyMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    VoltageAMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    VoltageBMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    VoltageCMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    VoltageLLAvgMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    CurrentAMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    CurrentBMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    CurrentCMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ActivePowerAMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ActivePowerBMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ActivePowerCMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ReactivePowerAMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ReactivePowerBMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ReactivePowerCMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    EnergyDeliveredMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    EnergyReceivedMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ReactiveEnergyDeliveredMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true),
                    ReactiveEnergyReceivedMapModbusRegisterMapEntryConfigId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerMeterMapModbusConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Acti~",
                        column: x => x.ActivePowerAMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Act~1",
                        column: x => x.ActivePowerBMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Act~2",
                        column: x => x.ActivePowerCMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Act~3",
                        column: x => x.ActivePowerMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Appa~",
                        column: x => x.ApparentPowerMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Curr~",
                        column: x => x.CurrentAMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Cur~1",
                        column: x => x.CurrentBMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Cur~2",
                        column: x => x.CurrentCMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Ener~",
                        column: x => x.EnergyDeliveredMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Ene~1",
                        column: x => x.EnergyReceivedMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Freq~",
                        column: x => x.FrequencyMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Powe~",
                        column: x => x.PowerFactorMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Reac~",
                        column: x => x.ReactiveEnergyDeliveredMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Rea~1",
                        column: x => x.ReactiveEnergyReceivedMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Rea~2",
                        column: x => x.ReactivePowerAMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Rea~3",
                        column: x => x.ReactivePowerBMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Rea~4",
                        column: x => x.ReactivePowerCMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Rea~5",
                        column: x => x.ReactivePowerMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Volt~",
                        column: x => x.VoltageAMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Vol~1",
                        column: x => x.VoltageBMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Vol~2",
                        column: x => x.VoltageCMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                    table.ForeignKey(
                        name: "FK_PowerMeterMapModbusConfig_ModbusRegisterMapEntryConfig_Vol~3",
                        column: x => x.VoltageLLAvgMapModbusRegisterMapEntryConfigId,
                        principalTable: "ModbusRegisterMapEntryConfig",
                        principalColumn: "ModbusRegisterMapEntryConfigId");
                });

            migrationBuilder.CreateTable(
                name: "PowerConversionSystemConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    DerUnitConfigId = table.Column<int>(type: "integer", nullable: false),
                    PowerConversionSystemDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusConfigId = table.Column<int>(type: "integer", nullable: false),
                    InitiallyStarted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerConversionSystemConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerConversionSystemConfig_PowerConversionSystemDeviceConf~",
                        column: x => x.PowerConversionSystemDeviceConfigId,
                        principalTable: "PowerConversionSystemDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerConversionSystemDeviceCustomConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Key = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    PowerConversionSystemDeviceConfigId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerConversionSystemDeviceCustomConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerConversionSystemDeviceCustomConfig_PowerConversionSyst~",
                        column: x => x.PowerConversionSystemDeviceConfigId,
                        principalTable: "PowerConversionSystemDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DerCircuitConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DerGroupConfigId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerCircuitConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DerCircuitConfig_DerGroupConfig_DerGroupConfigId",
                        column: x => x.DerGroupConfigId,
                        principalTable: "DerGroupConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomaticTransferSwitchDeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GenericModbusMapConfigId = table.Column<int>(type: "integer", nullable: true),
                    ClassName = table.Column<string>(type: "text", nullable: false),
                    GridConnectedId = table.Column<int>(type: "integer", nullable: true),
                    BackupConnectedId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomaticTransferSwitchDeviceConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomaticTransferSwitchDeviceConfig_GenericModbusCoilPointC~",
                        column: x => x.BackupConnectedId,
                        principalTable: "GenericModbusCoilPointConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AutomaticTransferSwitchDeviceConfig_GenericModbusCoilPoint~1",
                        column: x => x.GridConnectedId,
                        principalTable: "GenericModbusCoilPointConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AutomaticTransferSwitchDeviceConfig_GenericModbusMapConfig_~",
                        column: x => x.GenericModbusMapConfigId,
                        principalTable: "GenericModbusMapConfig",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ModbusConnectionConfigId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    AddressFamily = table.Column<int>(type: "integer", nullable: false),
                    StationId = table.Column<byte>(type: "smallint", nullable: false),
                    DerConfigId = table.Column<int>(type: "integer", nullable: false),
                    GenericModbusDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericModbusConfig_DerConfig_DerConfigId",
                        column: x => x.DerConfigId,
                        principalTable: "DerConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericModbusConfig_GenericModbusDeviceConfig_GenericModbus~",
                        column: x => x.GenericModbusDeviceConfigId,
                        principalTable: "GenericModbusDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericModbusConfig_ModbusConnectionConfig_ModbusConnection~",
                        column: x => x.ModbusConnectionConfigId,
                        principalTable: "ModbusConnectionConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericModbusUnitConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ModbusConnectionConfigId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    AddressFamily = table.Column<int>(type: "integer", nullable: false),
                    StationId = table.Column<byte>(type: "smallint", nullable: false),
                    DerUnitConfigId = table.Column<int>(type: "integer", nullable: false),
                    GenericModbusDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericModbusUnitConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericModbusUnitConfig_GenericModbusDeviceConfig_GenericMo~",
                        column: x => x.GenericModbusDeviceConfigId,
                        principalTable: "GenericModbusDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericModbusUnitConfig_ModbusConnectionConfig_ModbusConnec~",
                        column: x => x.ModbusConnectionConfigId,
                        principalTable: "ModbusConnectionConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CircuitBreakerDeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    GenericModbusMapConfigId = table.Column<int>(type: "integer", nullable: true),
                    ClassName = table.Column<string>(type: "text", nullable: false),
                    CircuitBreakerOperation = table.Column<int>(type: "integer", nullable: false),
                    BreakerStateRegisterId = table.Column<int>(type: "integer", nullable: true),
                    SynchronousVoltageId = table.Column<int>(type: "integer", nullable: true),
                    CloseCommandId = table.Column<int>(type: "integer", nullable: true),
                    CloseCommandCoilId = table.Column<int>(type: "integer", nullable: true),
                    OpenCommandId = table.Column<int>(type: "integer", nullable: true),
                    OpenCommandCoilId = table.Column<int>(type: "integer", nullable: true),
                    SettingsGroupStateId = table.Column<int>(type: "integer", nullable: true),
                    SettingsGroupCommandId = table.Column<int>(type: "integer", nullable: true),
                    TripStateId = table.Column<int>(type: "integer", nullable: true),
                    ResetTripId = table.Column<int>(type: "integer", nullable: true),
                    OpenBreakerDelayMilliseconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircuitBreakerDeviceConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusCoilPointConfig_Clo~",
                        column: x => x.CloseCommandCoilId,
                        principalTable: "GenericModbusCoilPointConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusCoilPointConfig_Ope~",
                        column: x => x.OpenCommandCoilId,
                        principalTable: "GenericModbusCoilPointConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterConf~",
                        column: x => x.BreakerStateRegisterId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterCon~1",
                        column: x => x.CloseCommandId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterCon~2",
                        column: x => x.OpenCommandId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterCon~3",
                        column: x => x.ResetTripId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterCon~4",
                        column: x => x.SettingsGroupCommandId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterCon~5",
                        column: x => x.SettingsGroupStateId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterCon~6",
                        column: x => x.SynchronousVoltageId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusHoldingRegisterCon~7",
                        column: x => x.TripStateId,
                        principalTable: "GenericModbusHoldingRegisterConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitBreakerDeviceConfig_GenericModbusMapConfig_GenericMo~",
                        column: x => x.GenericModbusMapConfigId,
                        principalTable: "GenericModbusMapConfig",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ModbusRegisterMapPollingRangeConfig",
                columns: table => new
                {
                    ModbusRegisterMapPollingRangeConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PowerMeterMapModbusConfigId = table.Column<int>(type: "integer", nullable: true),
                    Start = table.Column<int>(type: "integer", nullable: false),
                    End = table.Column<int>(type: "integer", nullable: false),
                    PollingClass = table.Column<string>(type: "text", nullable: true),
                    IsInputRegisterRange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModbusRegisterMapPollingRangeConfig", x => x.ModbusRegisterMapPollingRangeConfigId);
                    table.ForeignKey(
                        name: "FK_ModbusRegisterMapPollingRangeConfig_PowerMeterMapModbusConf~",
                        column: x => x.PowerMeterMapModbusConfigId,
                        principalTable: "PowerMeterMapModbusConfig",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PowerMeterDeviceConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PowerMeterMapC37ConfigId = table.Column<int>(type: "integer", nullable: true),
                    PowerMeterMapModbusConfigId = table.Column<int>(type: "integer", nullable: true),
                    ClassName = table.Column<string>(type: "text", nullable: false),
                    IsReversePowerFlow = table.Column<bool>(type: "boolean", nullable: false),
                    IsCurrentSigned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerMeterDeviceConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerMeterDeviceConfig_PowerMeterMapC37Config_PowerMeterMap~",
                        column: x => x.PowerMeterMapC37ConfigId,
                        principalTable: "PowerMeterMapC37Config",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PowerMeterDeviceConfig_PowerMeterMapModbusConfig_PowerMeter~",
                        column: x => x.PowerMeterMapModbusConfigId,
                        principalTable: "PowerMeterMapModbusConfig",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DerBatteryStorageUnitConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DerCircuitConfigId = table.Column<int>(type: "integer", nullable: false),
                    InMaintenanceMode = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeBatteryInOperations = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerBatteryStorageUnitConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DerBatteryStorageUnitConfig_DerCircuitConfig_DerCircuitConf~",
                        column: x => x.DerCircuitConfigId,
                        principalTable: "DerCircuitConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DerSolarUnitConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DerCircuitConfigId = table.Column<int>(type: "integer", nullable: false),
                    InMaintenanceMode = table.Column<bool>(type: "boolean", nullable: false),
                    SolarPanelConfigId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DerSolarUnitConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DerSolarUnitConfig_DerCircuitConfig_DerCircuitConfigId",
                        column: x => x.DerCircuitConfigId,
                        principalTable: "DerCircuitConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomaticTransferSwitchConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ModbusConnectionConfigId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    AddressFamily = table.Column<int>(type: "integer", nullable: false),
                    StationId = table.Column<byte>(type: "smallint", nullable: false),
                    DerConfigId = table.Column<int>(type: "integer", nullable: false),
                    AutomaticTransferSwitchDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomaticTransferSwitchConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomaticTransferSwitchConfig_AutomaticTransferSwitchDevice~",
                        column: x => x.AutomaticTransferSwitchDeviceConfigId,
                        principalTable: "AutomaticTransferSwitchDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutomaticTransferSwitchConfig_DerConfig_DerConfigId",
                        column: x => x.DerConfigId,
                        principalTable: "DerConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutomaticTransferSwitchConfig_ModbusConnectionConfig_Modbus~",
                        column: x => x.ModbusConnectionConfigId,
                        principalTable: "ModbusConnectionConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CircuitBreakerConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ModbusConnectionConfigId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    AddressFamily = table.Column<int>(type: "integer", nullable: false),
                    StationId = table.Column<byte>(type: "smallint", nullable: false),
                    DerCircuitConfigId = table.Column<int>(type: "integer", nullable: false),
                    CircuitBreakerDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircuitBreakerConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CircuitBreakerConfig_CircuitBreakerDeviceConfig_CircuitBrea~",
                        column: x => x.CircuitBreakerDeviceConfigId,
                        principalTable: "CircuitBreakerDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CircuitBreakerConfig_DerCircuitConfig_DerCircuitConfigId",
                        column: x => x.DerCircuitConfigId,
                        principalTable: "DerCircuitConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CircuitBreakerConfig_ModbusConnectionConfig_ModbusConnectio~",
                        column: x => x.ModbusConnectionConfigId,
                        principalTable: "ModbusConnectionConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuxiliaryPowerMeterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    PowerMeterDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusConfigId = table.Column<int>(type: "integer", nullable: true),
                    C37ConfigId = table.Column<int>(type: "integer", nullable: true),
                    RedundantPowerMeterConfigId = table.Column<int>(type: "integer", nullable: true),
                    PowerFactorStandard = table.Column<int>(type: "integer", nullable: false),
                    DerConfigId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuxiliaryPowerMeterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuxiliaryPowerMeterConfig_C37Config_C37ConfigId",
                        column: x => x.C37ConfigId,
                        principalTable: "C37Config",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuxiliaryPowerMeterConfig_DerConfig_DerConfigId",
                        column: x => x.DerConfigId,
                        principalTable: "DerConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuxiliaryPowerMeterConfig_PowerMeterDeviceConfig_PowerMeter~",
                        column: x => x.PowerMeterDeviceConfigId,
                        principalTable: "PowerMeterDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CircuitPowerMeterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    PowerMeterDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusConfigId = table.Column<int>(type: "integer", nullable: true),
                    C37ConfigId = table.Column<int>(type: "integer", nullable: true),
                    RedundantPowerMeterConfigId = table.Column<int>(type: "integer", nullable: true),
                    PowerFactorStandard = table.Column<int>(type: "integer", nullable: false),
                    DerCircuitConfigId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircuitPowerMeterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CircuitPowerMeterConfig_C37Config_C37ConfigId",
                        column: x => x.C37ConfigId,
                        principalTable: "C37Config",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CircuitPowerMeterConfig_DerCircuitConfig_DerCircuitConfigId",
                        column: x => x.DerCircuitConfigId,
                        principalTable: "DerCircuitConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CircuitPowerMeterConfig_PowerMeterDeviceConfig_PowerMeterDe~",
                        column: x => x.PowerMeterDeviceConfigId,
                        principalTable: "PowerMeterDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalPowerMeterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    PowerMeterDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusConfigId = table.Column<int>(type: "integer", nullable: true),
                    C37ConfigId = table.Column<int>(type: "integer", nullable: true),
                    RedundantPowerMeterConfigId = table.Column<int>(type: "integer", nullable: true),
                    PowerFactorStandard = table.Column<int>(type: "integer", nullable: false),
                    DerConfigId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPowerMeterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalPowerMeterConfig_C37Config_C37ConfigId",
                        column: x => x.C37ConfigId,
                        principalTable: "C37Config",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExternalPowerMeterConfig_DerConfig_DerConfigId",
                        column: x => x.DerConfigId,
                        principalTable: "DerConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalPowerMeterConfig_PowerMeterDeviceConfig_PowerMeterD~",
                        column: x => x.PowerMeterDeviceConfigId,
                        principalTable: "PowerMeterDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemPowerMeterConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    PowerMeterDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusConfigId = table.Column<int>(type: "integer", nullable: true),
                    C37ConfigId = table.Column<int>(type: "integer", nullable: true),
                    RedundantPowerMeterConfigId = table.Column<int>(type: "integer", nullable: true),
                    PowerFactorStandard = table.Column<int>(type: "integer", nullable: false),
                    DerConfigId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPowerMeterConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemPowerMeterConfig_C37Config_C37ConfigId",
                        column: x => x.C37ConfigId,
                        principalTable: "C37Config",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SystemPowerMeterConfig_DerConfig_DerConfigId",
                        column: x => x.DerConfigId,
                        principalTable: "DerConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemPowerMeterConfig_PowerMeterDeviceConfig_PowerMeterDev~",
                        column: x => x.PowerMeterDeviceConfigId,
                        principalTable: "PowerMeterDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BatteryBankConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    DerUnitConfigId = table.Column<int>(type: "integer", nullable: false),
                    BatteryBankDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    ModbusConfigId = table.Column<int>(type: "integer", nullable: false),
                    InitiallyConnected = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatteryBankConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatteryBankConfig_BatteryBankDeviceConfig_BatteryBankDevice~",
                        column: x => x.BatteryBankDeviceConfigId,
                        principalTable: "BatteryBankDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatteryBankConfig_DerBatteryStorageUnitConfig_DerUnitConfig~",
                        column: x => x.DerUnitConfigId,
                        principalTable: "DerBatteryStorageUnitConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolarPanelConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"DomainBaseSequence\"')"),
                    ChangedBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ChangedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    DerUnitConfigId = table.Column<int>(type: "integer", nullable: false),
                    SolarPanelDeviceConfigId = table.Column<int>(type: "integer", nullable: false),
                    NumberOfPanels = table.Column<int>(type: "integer", nullable: false),
                    ConnectionType = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MetricsIntervalMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    MetricsFactorClass1 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass2 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass3 = table.Column<int>(type: "integer", nullable: false),
                    MetricsFactorClass4 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarPanelConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolarPanelConfig_DerSolarUnitConfig_DerUnitConfigId",
                        column: x => x.DerUnitConfigId,
                        principalTable: "DerSolarUnitConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolarPanelConfig_SolarPanelDeviceConfig_SolarPanelDeviceCon~",
                        column: x => x.SolarPanelDeviceConfigId,
                        principalTable: "SolarPanelDeviceConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchConfig_AutomaticTransferSwitchDevice~",
                table: "AutomaticTransferSwitchConfig",
                column: "AutomaticTransferSwitchDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchConfig_DerConfigId",
                table: "AutomaticTransferSwitchConfig",
                column: "DerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchConfig_DeviceId",
                table: "AutomaticTransferSwitchConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchConfig_ModbusConnectionConfigId",
                table: "AutomaticTransferSwitchConfig",
                column: "ModbusConnectionConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchConfig_Name",
                table: "AutomaticTransferSwitchConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchDeviceConfig_BackupConnectedId",
                table: "AutomaticTransferSwitchDeviceConfig",
                column: "BackupConnectedId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchDeviceConfig_GenericModbusMapConfigId",
                table: "AutomaticTransferSwitchDeviceConfig",
                column: "GenericModbusMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchDeviceConfig_GridConnectedId",
                table: "AutomaticTransferSwitchDeviceConfig",
                column: "GridConnectedId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomaticTransferSwitchDeviceConfig_Name",
                table: "AutomaticTransferSwitchDeviceConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryPowerMeterConfig_C37ConfigId",
                table: "AuxiliaryPowerMeterConfig",
                column: "C37ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryPowerMeterConfig_DerConfigId",
                table: "AuxiliaryPowerMeterConfig",
                column: "DerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryPowerMeterConfig_DeviceId",
                table: "AuxiliaryPowerMeterConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryPowerMeterConfig_ModbusConfigId",
                table: "AuxiliaryPowerMeterConfig",
                column: "ModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryPowerMeterConfig_Name",
                table: "AuxiliaryPowerMeterConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryPowerMeterConfig_PowerMeterDeviceConfigId",
                table: "AuxiliaryPowerMeterConfig",
                column: "PowerMeterDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryPowerMeterConfig_RedundantPowerMeterConfigId",
                table: "AuxiliaryPowerMeterConfig",
                column: "RedundantPowerMeterConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryBankConfig_BatteryBankDeviceConfigId",
                table: "BatteryBankConfig",
                column: "BatteryBankDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryBankConfig_DerUnitConfigId",
                table: "BatteryBankConfig",
                column: "DerUnitConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryBankConfig_DeviceId",
                table: "BatteryBankConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatteryBankConfig_ModbusConfigId",
                table: "BatteryBankConfig",
                column: "ModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BatteryBankConfig_Name",
                table: "BatteryBankConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatteryBankDeviceConfig_Name",
                table: "BatteryBankDeviceConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatteryBankDeviceCustomConfig_BatteryBankDeviceConfigId",
                table: "BatteryBankDeviceCustomConfig",
                column: "BatteryBankDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_C37Config_Name",
                table: "C37Config",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerConfig_CircuitBreakerDeviceConfigId",
                table: "CircuitBreakerConfig",
                column: "CircuitBreakerDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerConfig_DerCircuitConfigId",
                table: "CircuitBreakerConfig",
                column: "DerCircuitConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerConfig_DeviceId",
                table: "CircuitBreakerConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerConfig_ModbusConnectionConfigId",
                table: "CircuitBreakerConfig",
                column: "ModbusConnectionConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerConfig_Name",
                table: "CircuitBreakerConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_BreakerStateRegisterId",
                table: "CircuitBreakerDeviceConfig",
                column: "BreakerStateRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_CloseCommandCoilId",
                table: "CircuitBreakerDeviceConfig",
                column: "CloseCommandCoilId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_CloseCommandId",
                table: "CircuitBreakerDeviceConfig",
                column: "CloseCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_GenericModbusMapConfigId",
                table: "CircuitBreakerDeviceConfig",
                column: "GenericModbusMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_Name",
                table: "CircuitBreakerDeviceConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_OpenCommandCoilId",
                table: "CircuitBreakerDeviceConfig",
                column: "OpenCommandCoilId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_OpenCommandId",
                table: "CircuitBreakerDeviceConfig",
                column: "OpenCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_ResetTripId",
                table: "CircuitBreakerDeviceConfig",
                column: "ResetTripId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_SettingsGroupCommandId",
                table: "CircuitBreakerDeviceConfig",
                column: "SettingsGroupCommandId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_SettingsGroupStateId",
                table: "CircuitBreakerDeviceConfig",
                column: "SettingsGroupStateId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_SynchronousVoltageId",
                table: "CircuitBreakerDeviceConfig",
                column: "SynchronousVoltageId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitBreakerDeviceConfig_TripStateId",
                table: "CircuitBreakerDeviceConfig",
                column: "TripStateId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitPowerMeterConfig_C37ConfigId",
                table: "CircuitPowerMeterConfig",
                column: "C37ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitPowerMeterConfig_DerCircuitConfigId",
                table: "CircuitPowerMeterConfig",
                column: "DerCircuitConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitPowerMeterConfig_DeviceId",
                table: "CircuitPowerMeterConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitPowerMeterConfig_ModbusConfigId",
                table: "CircuitPowerMeterConfig",
                column: "ModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitPowerMeterConfig_Name",
                table: "CircuitPowerMeterConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircuitPowerMeterConfig_PowerMeterDeviceConfigId",
                table: "CircuitPowerMeterConfig",
                column: "PowerMeterDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CircuitPowerMeterConfig_RedundantPowerMeterConfigId",
                table: "CircuitPowerMeterConfig",
                column: "RedundantPowerMeterConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DerBatteryStorageUnitConfig_DerCircuitConfigId",
                table: "DerBatteryStorageUnitConfig",
                column: "DerCircuitConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DerBatteryStorageUnitConfig_Name",
                table: "DerBatteryStorageUnitConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerCircuitConfig_DerGroupConfigId",
                table: "DerCircuitConfig",
                column: "DerGroupConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DerCircuitConfig_Name",
                table: "DerCircuitConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerConfig_Name",
                table: "DerConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerContainerConfig_DerUnitConfigId",
                table: "DerContainerConfig",
                column: "DerUnitConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerContainerConfig_DeviceId",
                table: "DerContainerConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerContainerConfig_ModbusConfigId",
                table: "DerContainerConfig",
                column: "ModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DerContainerConfig_Name",
                table: "DerContainerConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerGroupConfig_DerConfigId",
                table: "DerGroupConfig",
                column: "DerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DerGroupConfig_Name",
                table: "DerGroupConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DerSolarUnitConfig_DerCircuitConfigId",
                table: "DerSolarUnitConfig",
                column: "DerCircuitConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_DerSolarUnitConfig_Name",
                table: "DerSolarUnitConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPowerMeterConfig_C37ConfigId",
                table: "ExternalPowerMeterConfig",
                column: "C37ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPowerMeterConfig_DerConfigId",
                table: "ExternalPowerMeterConfig",
                column: "DerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPowerMeterConfig_DeviceId",
                table: "ExternalPowerMeterConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPowerMeterConfig_ModbusConfigId",
                table: "ExternalPowerMeterConfig",
                column: "ModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPowerMeterConfig_Name",
                table: "ExternalPowerMeterConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPowerMeterConfig_PowerMeterDeviceConfigId",
                table: "ExternalPowerMeterConfig",
                column: "PowerMeterDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPowerMeterConfig_RedundantPowerMeterConfigId",
                table: "ExternalPowerMeterConfig",
                column: "RedundantPowerMeterConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusCoilPointConfig_GenericModbusMapConfigId",
                table: "GenericModbusCoilPointConfig",
                column: "GenericModbusMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusCoilPointConfig_Name",
                table: "GenericModbusCoilPointConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusConfig_DerConfigId",
                table: "GenericModbusConfig",
                column: "DerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusConfig_DeviceId",
                table: "GenericModbusConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusConfig_GenericModbusDeviceConfigId",
                table: "GenericModbusConfig",
                column: "GenericModbusDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusConfig_ModbusConnectionConfigId",
                table: "GenericModbusConfig",
                column: "ModbusConnectionConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusConfig_Name",
                table: "GenericModbusConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusDeviceConfig_GenericModbusMapConfigId",
                table: "GenericModbusDeviceConfig",
                column: "GenericModbusMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusDeviceConfig_Name",
                table: "GenericModbusDeviceConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusDiscreteInputPointConfig_GenericModbusMapConfi~",
                table: "GenericModbusDiscreteInputPointConfig",
                column: "GenericModbusMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusDiscreteInputPointConfig_Name",
                table: "GenericModbusDiscreteInputPointConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusHoldingRegisterConfig_GenericModbusMapConfigId",
                table: "GenericModbusHoldingRegisterConfig",
                column: "GenericModbusMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusHoldingRegisterConfig_Name",
                table: "GenericModbusHoldingRegisterConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusInputRegisterConfig_GenericModbusMapConfigId",
                table: "GenericModbusInputRegisterConfig",
                column: "GenericModbusMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusInputRegisterConfig_Name",
                table: "GenericModbusInputRegisterConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusMapConfig_Name",
                table: "GenericModbusMapConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusUnitConfig_DerUnitConfigId",
                table: "GenericModbusUnitConfig",
                column: "DerUnitConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusUnitConfig_DeviceId",
                table: "GenericModbusUnitConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusUnitConfig_GenericModbusDeviceConfigId",
                table: "GenericModbusUnitConfig",
                column: "GenericModbusDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusUnitConfig_ModbusConnectionConfigId",
                table: "GenericModbusUnitConfig",
                column: "ModbusConnectionConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericModbusUnitConfig_Name",
                table: "GenericModbusUnitConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModbusConfig_ModbusConnectionConfigId",
                table: "ModbusConfig",
                column: "ModbusConnectionConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ModbusConfig_Name",
                table: "ModbusConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModbusConnectionConfig_Name",
                table: "ModbusConnectionConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModbusRegisterMapPollingRangeConfig_PowerMeterMapModbusConf~",
                table: "ModbusRegisterMapPollingRangeConfig",
                column: "PowerMeterMapModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerConversionSystemConfig_DerUnitConfigId",
                table: "PowerConversionSystemConfig",
                column: "DerUnitConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerConversionSystemConfig_DeviceId",
                table: "PowerConversionSystemConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerConversionSystemConfig_ModbusConfigId",
                table: "PowerConversionSystemConfig",
                column: "ModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerConversionSystemConfig_Name",
                table: "PowerConversionSystemConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerConversionSystemConfig_PowerConversionSystemDeviceConf~",
                table: "PowerConversionSystemConfig",
                column: "PowerConversionSystemDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerConversionSystemDeviceConfig_Name",
                table: "PowerConversionSystemDeviceConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerConversionSystemDeviceCustomConfig_PowerConversionSyst~",
                table: "PowerConversionSystemDeviceCustomConfig",
                column: "PowerConversionSystemDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterDeviceConfig_Name",
                table: "PowerMeterDeviceConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterDeviceConfig_PowerMeterMapC37ConfigId",
                table: "PowerMeterDeviceConfig",
                column: "PowerMeterMapC37ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterDeviceConfig_PowerMeterMapModbusConfigId",
                table: "PowerMeterDeviceConfig",
                column: "PowerMeterMapModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapC37Config_Name",
                table: "PowerMeterMapC37Config",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ActivePowerAMapModbusRegisterMapE~",
                table: "PowerMeterMapModbusConfig",
                column: "ActivePowerAMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ActivePowerBMapModbusRegisterMapE~",
                table: "PowerMeterMapModbusConfig",
                column: "ActivePowerBMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ActivePowerCMapModbusRegisterMapE~",
                table: "PowerMeterMapModbusConfig",
                column: "ActivePowerCMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ActivePowerMapModbusRegisterMapEn~",
                table: "PowerMeterMapModbusConfig",
                column: "ActivePowerMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ApparentPowerMapModbusRegisterMap~",
                table: "PowerMeterMapModbusConfig",
                column: "ApparentPowerMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_CurrentAMapModbusRegisterMapEntry~",
                table: "PowerMeterMapModbusConfig",
                column: "CurrentAMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_CurrentBMapModbusRegisterMapEntry~",
                table: "PowerMeterMapModbusConfig",
                column: "CurrentBMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_CurrentCMapModbusRegisterMapEntry~",
                table: "PowerMeterMapModbusConfig",
                column: "CurrentCMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_EnergyDeliveredMapModbusRegisterM~",
                table: "PowerMeterMapModbusConfig",
                column: "EnergyDeliveredMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_EnergyReceivedMapModbusRegisterMa~",
                table: "PowerMeterMapModbusConfig",
                column: "EnergyReceivedMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_FrequencyMapModbusRegisterMapEntr~",
                table: "PowerMeterMapModbusConfig",
                column: "FrequencyMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_Name",
                table: "PowerMeterMapModbusConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_PowerFactorMapModbusRegisterMapEn~",
                table: "PowerMeterMapModbusConfig",
                column: "PowerFactorMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ReactiveEnergyDeliveredMapModbusR~",
                table: "PowerMeterMapModbusConfig",
                column: "ReactiveEnergyDeliveredMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ReactiveEnergyReceivedMapModbusRe~",
                table: "PowerMeterMapModbusConfig",
                column: "ReactiveEnergyReceivedMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ReactivePowerAMapModbusRegisterMa~",
                table: "PowerMeterMapModbusConfig",
                column: "ReactivePowerAMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ReactivePowerBMapModbusRegisterMa~",
                table: "PowerMeterMapModbusConfig",
                column: "ReactivePowerBMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ReactivePowerCMapModbusRegisterMa~",
                table: "PowerMeterMapModbusConfig",
                column: "ReactivePowerCMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_ReactivePowerMapModbusRegisterMap~",
                table: "PowerMeterMapModbusConfig",
                column: "ReactivePowerMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_VoltageAMapModbusRegisterMapEntry~",
                table: "PowerMeterMapModbusConfig",
                column: "VoltageAMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_VoltageBMapModbusRegisterMapEntry~",
                table: "PowerMeterMapModbusConfig",
                column: "VoltageBMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_VoltageCMapModbusRegisterMapEntry~",
                table: "PowerMeterMapModbusConfig",
                column: "VoltageCMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerMeterMapModbusConfig_VoltageLLAvgMapModbusRegisterMapE~",
                table: "PowerMeterMapModbusConfig",
                column: "VoltageLLAvgMapModbusRegisterMapEntryConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SolarPanelConfig_DerUnitConfigId",
                table: "SolarPanelConfig",
                column: "DerUnitConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarPanelConfig_DeviceId",
                table: "SolarPanelConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarPanelConfig_Name",
                table: "SolarPanelConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolarPanelConfig_SolarPanelDeviceConfigId",
                table: "SolarPanelConfig",
                column: "SolarPanelDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SolarPanelDeviceConfig_Name",
                table: "SolarPanelDeviceConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerMeterConfig_C37ConfigId",
                table: "SystemPowerMeterConfig",
                column: "C37ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerMeterConfig_DerConfigId",
                table: "SystemPowerMeterConfig",
                column: "DerConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerMeterConfig_DeviceId",
                table: "SystemPowerMeterConfig",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerMeterConfig_ModbusConfigId",
                table: "SystemPowerMeterConfig",
                column: "ModbusConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerMeterConfig_Name",
                table: "SystemPowerMeterConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerMeterConfig_PowerMeterDeviceConfigId",
                table: "SystemPowerMeterConfig",
                column: "PowerMeterDeviceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPowerMeterConfig_RedundantPowerMeterConfigId",
                table: "SystemPowerMeterConfig",
                column: "RedundantPowerMeterConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutomaticTransferSwitchConfig");

            migrationBuilder.DropTable(
                name: "AuxiliaryPowerMeterConfig");

            migrationBuilder.DropTable(
                name: "BatteryBankConfig");

            migrationBuilder.DropTable(
                name: "BatteryBankDeviceCustomConfig");

            migrationBuilder.DropTable(
                name: "CircuitBreakerConfig");

            migrationBuilder.DropTable(
                name: "CircuitPowerMeterConfig");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "DerContainerConfig");

            migrationBuilder.DropTable(
                name: "ExternalPowerMeterConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusDiscreteInputPointConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusInputRegisterConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusUnitConfig");

            migrationBuilder.DropTable(
                name: "ModbusConfig");

            migrationBuilder.DropTable(
                name: "ModbusRegisterMapPollingRangeConfig");

            migrationBuilder.DropTable(
                name: "PowerConversionSystemConfig");

            migrationBuilder.DropTable(
                name: "PowerConversionSystemDeviceCustomConfig");

            migrationBuilder.DropTable(
                name: "SolarPanelConfig");

            migrationBuilder.DropTable(
                name: "SystemPowerMeterConfig");

            migrationBuilder.DropTable(
                name: "AutomaticTransferSwitchDeviceConfig");

            migrationBuilder.DropTable(
                name: "DerBatteryStorageUnitConfig");

            migrationBuilder.DropTable(
                name: "BatteryBankDeviceConfig");

            migrationBuilder.DropTable(
                name: "CircuitBreakerDeviceConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusDeviceConfig");

            migrationBuilder.DropTable(
                name: "ModbusConnectionConfig");

            migrationBuilder.DropTable(
                name: "PowerConversionSystemDeviceConfig");

            migrationBuilder.DropTable(
                name: "DerSolarUnitConfig");

            migrationBuilder.DropTable(
                name: "SolarPanelDeviceConfig");

            migrationBuilder.DropTable(
                name: "C37Config");

            migrationBuilder.DropTable(
                name: "PowerMeterDeviceConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusCoilPointConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusHoldingRegisterConfig");

            migrationBuilder.DropTable(
                name: "DerCircuitConfig");

            migrationBuilder.DropTable(
                name: "PowerMeterMapC37Config");

            migrationBuilder.DropTable(
                name: "PowerMeterMapModbusConfig");

            migrationBuilder.DropTable(
                name: "GenericModbusMapConfig");

            migrationBuilder.DropTable(
                name: "DerGroupConfig");

            migrationBuilder.DropTable(
                name: "ModbusRegisterMapEntryConfig");

            migrationBuilder.DropTable(
                name: "DerConfig");

            migrationBuilder.DropSequence(
                name: "DomainBaseSequence");
        }
    }
}
