// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Communication.Protocols.C37118.Types;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.PhysicalUnits.Electricals.Powers;
using System.Net.Sockets;

namespace paskalON.DemoSimple.Devices.Data
{
    /// <summary>
    /// Create domain data for the service here.
    /// </summary>
    static class ServiceData
    {
        /// <summary>
        /// Initial changed by user.
        /// </summary>
        private const string ChangedBy = "System Init";


        /// <summary>
        /// Main method to create the service data.
        /// </summary>
        /// <param name="context">DB context interface.</param>
        public static async Task CreateAsync(IDeviceServiceContext context)
        {
            DerConfig derConfig = await CreateStructureAndDevicesAsync(context);
            await CreateMetersAsync(context, derConfig);
        }


        /// <summary>
        /// Create the DER structure and its devices.
        /// </summary>
        /// <param name="context">Database context.</param>
        private static async Task<DerConfig> CreateStructureAndDevicesAsync(IDeviceServiceContext context)
        {
            DerConfig derConfig = new DerConfig { ChangedBy = ChangedBy, Name = "Der 1", };
            context.DerConfigs.Add(derConfig);
            DerGroupConfig derGroupConfig = new DerGroupConfig { ChangedBy = ChangedBy, Name = "Der Group 1", DerConfig = derConfig };
            context.DerGroupConfigs.Add(derGroupConfig);
            DerCircuitConfig derCircuitBessConfig = new DerCircuitConfig { ChangedBy = ChangedBy, Name = "Circuit Bess 1", DerGroupConfig = derGroupConfig, };
            context.DerCircuitConfigs.Add(derCircuitBessConfig);

            DerBatteryStorageUnitConfig unit1 = new DerBatteryStorageUnitConfig
            {
                ChangedBy = ChangedBy,
                Name = "BMS-Unit 1",
                IncludeBatteryInOperations = true,
                DerCircuitConfig = derCircuitBessConfig
            };
            context.DerBatteryStorageUnitConfigs.Add(unit1);

            DerBatteryStorageUnitConfig unit2 = new DerBatteryStorageUnitConfig
            {
                ChangedBy = ChangedBy,
                Name = "BMS-Unit 2",
                IncludeBatteryInOperations = true,
                DerCircuitConfig = derCircuitBessConfig
            };
            context.DerBatteryStorageUnitConfigs.Add(unit2);

            DerBatteryStorageUnitConfig unit3 = new DerBatteryStorageUnitConfig
            {
                ChangedBy = ChangedBy,
                Name = "BMS-Unit 3",
                IncludeBatteryInOperations = true,
                DerCircuitConfig = derCircuitBessConfig
            };
            context.DerBatteryStorageUnitConfigs.Add(unit3);

            ModbusConnectionConfig modbusConnection = new ModbusConnectionConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConnection for all",
                PollingIntervalMilliseconds = 1000,
                MasterHeartBeatIntervalMilliseconds = 1000,
                IsPipeliningEnabled = false,
                ConnectionTimeoutMilliseconds = 5000,
                DisconnectionTimeoutMilliseconds = 5000,
                ConnectRetryCount = 3,
                ConnectRetryIntervalMilliseconds = 5000,
                SendTimeoutMilliseconds = 1000,
                SendRetryCount = 1,
                SendRetryIntervalMilliseconds = 5000,
                ServerToClientAliveIntervalSeconds = -1,
                ServerMaximumConnections = 5
            };
            context.ModbusConnectionConfigs.Add(modbusConnection);

            PowerConversionSystemDeviceConfig devicePcs = new PowerConversionSystemDeviceConfig
            {
                ChangedBy = ChangedBy,
                Name = "Device PCS 1",
                ClassName = "paskalON.Devices.Equipments.PowerConversionSystems.Simples.PcsSimpleV1Proxy",
                NameplateMaximumActivePower = 3630000,
                NameplateMaximumReactivePower = 3630000,
                NameplateMaximumApparentPower = 3630000,
                NameplateMaximumACCurrent = 3175,
                MaximumDCVoltage = 1140,
                MinimumDCVoltage = 100,
                ZeroOutputOnCommLoss = true
            };
            context.PowerConversionSystemDeviceConfigs.Add(devicePcs);

            ModbusConfig pcs1Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigPcs1",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(pcs1Modbus);

            PowerConversionSystemConfig pcs1 = new PowerConversionSystemConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "PCS Bess 1",
                DeviceId = 1,
                InitiallyStarted = true,
                ModbusConfig = pcs1Modbus,
                PowerConversionSystemDeviceConfig = devicePcs,
                DerUnitConfig = unit1
            };
            context.PowerConversionSystemConfigs.Add(pcs1);

            ModbusConfig pcs2Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigPcs2",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs + 1,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(pcs1Modbus);

            PowerConversionSystemConfig pcs2 = new PowerConversionSystemConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "PCS Bess 2",
                DeviceId = 2,
                InitiallyStarted = true,
                ModbusConfig = pcs2Modbus,
                PowerConversionSystemDeviceConfig = devicePcs,
                DerUnitConfig = unit2
            };
            context.PowerConversionSystemConfigs.Add(pcs2);

            ModbusConfig pcs3Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigPcs3",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs + 2,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(pcs1Modbus);

            PowerConversionSystemConfig pcs3 = new PowerConversionSystemConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "PCS Bess 3",
                DeviceId = 3,
                InitiallyStarted = true,
                ModbusConfig = pcs3Modbus,
                PowerConversionSystemDeviceConfig = devicePcs,
                DerUnitConfig = unit3
            };
            context.PowerConversionSystemConfigs.Add(pcs3);


            BatteryBankDeviceConfig bbDevice = new BatteryBankDeviceConfig
            {
                ChangedBy = ChangedBy,
                Name = "Device BMS 1",
                ClassName = "paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples.BbSimpleV1Proxy",
                NameplateCapacity = 5000000,
                NameplateMaximumChargeRate = 5000000,
                NameplateMaximumDischargeRate = 5000000,
                BatteryType = BatteryType.LithiumIon,
                RackCount = 5,
                ModulesPerRackCount = 1,
                InverterBusNumber = 1,
                AbsoluteMinimumStateOfCharge = 0,
                AbsoluteMaximumStateOfCharge = 100,
                PreferredMinimumStateOfCharge = 5,
                PreferredMaximumStateOfCharge = 95,
                AbsoluteMaximumTemperature = 40,
                AbsoluteMinimumTemperature = 15,
                PreferredMinimumTemperature = 40,
                PreferredMaximumTemperature = 15,
                AbsoluteMaxChargeCurrentAmps = 1100,
                AbsoluteMaxDischargeCurrentAmps = 1200,
                MaximumDcVoltage = 700,
                MinimumDcVoltage = 1000,
                ZeroCapacityOnCommLoss = true
            };
            context.BatteryBankDeviceConfigs.Add(bbDevice);

            ModbusConfig bb11Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigBB1.1",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(bb11Modbus);

            BatteryBankConfig bb11 = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "BB 1.1",
                DeviceId = 11,
                InitiallyConnected = true,
                ModbusConfig = bb11Modbus,
                BatteryBankDeviceConfig = bbDevice,
                DerUnitConfig = unit1
            };
            context.BatteryBankConfigs.Add(bb11);

            ModbusConfig bb12Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigBB1.2",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms + 1,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(bb11Modbus);

            BatteryBankConfig bb12 = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "BB 1.2",
                DeviceId = 12,
                InitiallyConnected = true,
                ModbusConfig = bb12Modbus,
                BatteryBankDeviceConfig = bbDevice,
                DerUnitConfig = unit1
            };
            context.BatteryBankConfigs.Add(bb12);

            ModbusConfig bb21Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigBB2.1",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms + 2,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(bb21Modbus);

            BatteryBankConfig bb21 = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "BB 2.1",
                DeviceId = 21,
                InitiallyConnected = true,
                ModbusConfig = bb21Modbus,
                BatteryBankDeviceConfig = bbDevice,
                DerUnitConfig = unit2
            };
            context.BatteryBankConfigs.Add(bb21);

            ModbusConfig bb22Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigBB2.2",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms + 3,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(bb22Modbus);

            BatteryBankConfig bb22 = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "BB 2.2",
                DeviceId = 22,
                InitiallyConnected = true,
                ModbusConfig = bb21Modbus,
                BatteryBankDeviceConfig = bbDevice,
                DerUnitConfig = unit2
            };
            context.BatteryBankConfigs.Add(bb22);

            ModbusConfig bb31Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigBB3.1",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms + 4,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(bb31Modbus);

            BatteryBankConfig bb31 = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "BB 3.1",
                DeviceId = 31,
                InitiallyConnected = true,
                ModbusConfig = bb31Modbus,
                BatteryBankDeviceConfig = bbDevice,
                DerUnitConfig = unit3
            };
            context.BatteryBankConfigs.Add(bb31);

            ModbusConfig bb32Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigBB3.2",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms + 5,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(bb32Modbus);

            BatteryBankConfig bb32 = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "BB 3.2",
                DeviceId = 33,
                InitiallyConnected = true,
                ModbusConfig = bb31Modbus,
                BatteryBankDeviceConfig = bbDevice,
                DerUnitConfig = unit3
            };
            context.BatteryBankConfigs.Add(bb32);

            await context.SaveChangesAsync();

            return derConfig;
        }


        /// <summary>
        /// Create the meters.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="derConfig">DerConfig root object.</param>
        /// <returns></returns>
        private static async Task CreateMetersAsync(IDeviceServiceContext context, DerConfig derConfig)
        {
            PowerMeterMapC37Config powerMeterMap = new PowerMeterMapC37Config
            {
                ChangedBy = ChangedBy,
                Name = "Power Meter Map C37",
                // Power
                ApparentPower = "Analog0",
                ActivePower = "Analog1",
                ActivePowerA = "Analog2",
                ActivePowerB = "Analog3",
                ActivePowerC = "Analog4",
                ReactivePower = "Analog5",
                ReactivePowerA = "Analog6",
                ReactivePowerB = "Analog7",
                ReactivePowerC = "Analog8",
                EnergyDelivered = "Analog9",
                EnergyReceived = "Analog10",
                ReactiveEnergyDelivered = "Analog11",
                ReactiveEnergyReceived = "Analog12",
                // Voltage
                VoltageA = "Phasor0",
                VoltageB = "Phasor1",
                VoltageC = "Phasor2",
                VoltageAB = "Phasor3",
                VoltageBC = "Phasor4",
                VoltageCA = "Phasor5",
                VoltagePositiveSequence = "Phasor6",
                VoltageLLAvg = "Analog13",
                // Current
                CurrentA = "Phasor7",
                CurrentB = "Phasor8",
                CurrentC = "Phasor9",
            };
            context.PowerMeterMapC37Configs.Add(powerMeterMap);

            PowerMeterDeviceConfig systemPowerMeterDevice = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "System Power Meter Device",
                ClassName = "paskalON.Devices.Equipments.Meters.PowerMeters.Simples.SystemPowerMeterSimpleV1Proxy",
                IsReversePowerFlow = false,
                IsCurrentSigned = true,
                PowerMeterMapC37Config = powerMeterMap,
            };
            context.PowerMeterDeviceConfigs.Add(systemPowerMeterDevice);

            C37Config systemMeterC37 = new C37Config
            {
                ChangedBy = ChangedBy,
                Name = "SystemPowerMeter 1",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartMeter,
                ConfigFrameTimeoutMilliseconds = 3000,
                DataFrameRetryCount = 3,
                DataFrameTimeoutMilliseconds = 500,
                StreamId = 1,
                StationName = "PMU",
                TransportLayer = C37TransportLayer.UDP,
            };
            context.C37Configs.Add(systemMeterC37);

            SystemPowerMeterConfig powerMeterConfig = new SystemPowerMeterConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "C37 System Power Meter 1",
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                C37Config = systemMeterC37,
                PowerMeterDeviceConfig = systemPowerMeterDevice,
                DerConfig = derConfig
            };
            context.SystemPowerMeterConfigs.Add(powerMeterConfig);

            await context.SaveChangesAsync();
        }
    }
}
