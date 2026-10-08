// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.PhysicalUnits.Electricals.Powers;
using System.Net.Sockets;

namespace paskalON.Devices.Domain.Configs.UnitTest
{
    /// <summary>
    /// Creates fully instantiated configuration graphs for unit tests.
    /// </summary>
    internal static class TestConfigFactory
    {
        /// <summary>
        /// Name used for the ChangedBy audit field.
        /// </summary>
        internal const string ChangedBy = "Test";


        /// <summary>
        /// Creates a DER configuration root.
        /// </summary>
        /// <returns>
        /// A new <see cref="DerConfig"/>.
        /// </returns>
        internal static DerConfig CreateDerConfig()
        {
            return new DerConfig { ChangedBy = ChangedBy, Name = "Project 1" };
        }


        /// <summary>
        /// Creates a battery storage unit including its parent circuit, group and DER configuration.
        /// </summary>
        /// <returns>A new <see cref="DerBatteryStorageUnitConfig"/>.</returns>
        internal static DerBatteryStorageUnitConfig CreateBatteryStorageUnitConfig()
        {
            DerGroupConfig group = new DerGroupConfig { ChangedBy = ChangedBy, Name = "Group 1", DerConfig = CreateDerConfig() };
            DerCircuitConfig circuit = new DerCircuitConfig { ChangedBy = ChangedBy, Name = "Circuit 1", DerGroupConfig = group };

            return new DerBatteryStorageUnitConfig { ChangedBy = ChangedBy, Name = "Unit 1", DerCircuitConfig = circuit };
        }


        /// <summary>
        /// Creates a Modbus TCP configuration.
        /// </summary>
        /// <returns>A new <see cref="ModbusConfig"/>.</returns>
        internal static ModbusConfig CreateModbusConfig()
        {
            ModbusConnectionConfig connection = new ModbusConnectionConfig { ChangedBy = ChangedBy, Name = "Connection 1" };

            return new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "Modbus PCS 1",
                ModbusConnectionConfig = connection,
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                UnitId = 1
            };
        }


        /// <summary>
        /// Creates a C37 configuration.
        /// </summary>
        /// <returns>A new <see cref="C37Config"/>.</returns>
        internal static C37Config CreateC37Config()
        {
            C37ConnectionConfig connection = new C37ConnectionConfig { ChangedBy = ChangedBy, Name = "C37 Connection 1" };

            return new C37Config
            {
                ChangedBy = ChangedBy,
                Name = "PMU 1",
                C37ConnectionConfig = connection,
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartMeter,
                AddressFamily = AddressFamily.InterNetwork,
                TransportLayer = C37TransportLayer.TCP,
                StationName = "PMU",
                StreamId = 1
            };
        }


        /// <summary>
        /// Creates a solar panel device configuration.
        /// </summary>
        /// <returns>A new <see cref="SolarPanelDeviceConfig"/> with 0-40.8 V and 0-12.87 A.</returns>
        internal static SolarPanelDeviceConfig CreateSolarPanelDeviceConfig()
        {
            return new SolarPanelDeviceConfig
            {
                ChangedBy = ChangedBy,
                Name = "Device PV 1",
                ClassName = "SolarPanelSimpleV1Proxy",
                MaximumVoltage = 40.8,
                MinimumVoltage = 0,
                MaximumCurrent = 12.87,
                MinimumCurrent = 0
            };
        }


        /// <summary>
        /// Creates a generic Modbus map configuration.
        /// </summary>
        /// <returns>A new <see cref="GenericModbusMapConfig"/>.</returns>
        internal static GenericModbusMapConfig CreateGenericModbusMapConfig()
        {
            return new GenericModbusMapConfig { ChangedBy = ChangedBy, Name = "Map 1" };
        }


        /// <summary>
        /// Creates all configuration types that derive from <see cref="DeviceIdNameBase"/> with a valid device Id of 1.
        /// </summary>
        /// <returns>One row per configuration type.</returns>
        internal static IEnumerable<object[]> CreateDeviceIdNameBaseConfigs()
        {
            DerBatteryStorageUnitConfig unit = CreateBatteryStorageUnitConfig();
            DerConfig derConfig = unit.DerCircuitConfig.DerGroupConfig.DerConfig;

            yield return new object[]
            {
                new BatteryBankConfig
                {
                    ChangedBy = ChangedBy,
                    Name = "Battery Bank 1",
                    DeviceId = 1,
                    IsActive = true,
                    DerUnitConfig = unit,
                    BatteryBankDeviceConfig = new BatteryBankDeviceConfig { ChangedBy = ChangedBy, Name = "Device Battery 1", ClassName = "BatteryBankSimpleV1Proxy" },
                    ModbusConfig = CreateModbusConfig()
                }
            };

            yield return new object[]
            {
                new PowerConversionSystemConfig
                {
                    ChangedBy = ChangedBy,
                    Name = "PCS 1",
                    DeviceId = 1,
                    IsActive = true,
                    DerUnitConfig = unit,
                    PowerConversionSystemDeviceConfig = new PowerConversionSystemDeviceConfig { ChangedBy = ChangedBy, Name = "Device PCS 1", ClassName = "PcsSimpleV1Proxy" },
                    ModbusConfig = CreateModbusConfig()
                }
            };

            yield return new object[]
            {
                new DerContainerConfig { ChangedBy = ChangedBy, Name = "Container 1", DeviceId = 1, IsActive = true }
            };

            yield return new object[]
            {
                new SolarPanelConfig
                {
                    ChangedBy = ChangedBy,
                    Name = "PV 1",
                    DeviceId = 1,
                    IsActive = true,
                    DerUnitConfig = unit,
                    SolarPanelDeviceConfig = CreateSolarPanelDeviceConfig()
                }
            };

            yield return new object[]
            {
                new SystemPowerMeterConfig
                {
                    ChangedBy = ChangedBy,
                    Name = "System Meter 1",
                    DeviceId = 1,
                    IsActive = true,
                    DerConfig = derConfig,
                    PowerMeterDeviceConfig = new PowerMeterDeviceConfig { ChangedBy = ChangedBy, Name = "Device Meter 1", ClassName = "PowerMeterSimpleV1Proxy" },
                    PowerFactorStandard = PowerFactorStandard.IEEE,
                    C37Config = CreateC37Config()
                }
            };
        }
    }
}
