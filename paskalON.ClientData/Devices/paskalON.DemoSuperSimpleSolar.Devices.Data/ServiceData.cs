// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Communication.Protocols.C37118.Types;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.PhysicalUnits.Electricals.Powers;
using System.Net.Sockets;

namespace paskalON.DemoSuperSimpleSolar.Devices.Data
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


        // TODO: Refactor
        private static string Pcs1 = "paskalon.devicesimulator.service";
        private static string PmSys1 = "paskalon.devicesimulator.service";


        /// <summary>
        /// Main method to create the service data.
        /// </summary>
        /// <param name="context">DB context interface.</param>
        public static async Task CreateAsync(IDeviceServiceContext context, bool createSimulatorData)
        {
            await CreateCore(context);
            DerConfig derConfig = await CreateStructureAndDevicesAsync(context, createSimulatorData);
            await CreateMetersAsync(context, derConfig);
        }


        /// <summary>
        /// Create core configuration of the service.
        /// </summary>
        /// <param name="context">Database context.</param>
        private static async Task CreateCore(IDeviceServiceContext context)
        {
            SystemConfig systemConfig = new SystemConfig
            {
                ChangedBy = ChangedBy,
                PollingIntervalMilliseconds = 1000,
                MetricsIntervalMilliseconds = 5000,
                DeviceIntervalMilliseconds = 1000,
                DeviceHeartbeatIntervalMilliseconds = 1000,
                DeviceFactorCore = 1,
                DeviceFactorDetail = 5,
                PublisherTopicPcsCore = "ppc:device:pcs:core",
                PublisherTopicPcsDetail = "ppc:device:pcs:detail",
                PublisherTopicSolarPanelCore = "ppc:device:pv:core",
                PublisherTopicSolarPanelDetail = "ppc:device:pv:detail",
                PublisherTopicSystemPowerMeterCore = "ppc:device:pm:system:core",
                PublisherTopicSystemPowerMeterDetail = "ppc:device:pm:system:detail",

            };
            context.SystemConfigs.Add(systemConfig);

            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Create the DER structure and its devices.
        /// </summary>
        /// <param name="context">Database context.</param>
        private static async Task<DerConfig> CreateStructureAndDevicesAsync(IDeviceServiceContext context, bool createSimulatorData)
        {
            DerConfig derConfig = new DerConfig { ChangedBy = ChangedBy, Name = "Der 1", };
            context.DerConfigs.Add(derConfig);
            DerGroupConfig derGroupConfig = new DerGroupConfig { ChangedBy = ChangedBy, Name = "Group 1", DerConfig = derConfig };
            context.DerGroupConfigs.Add(derGroupConfig);
            DerCircuitConfig derCircuitConfig = new DerCircuitConfig { ChangedBy = ChangedBy, Name = "Circuit 1", DerGroupConfig = derGroupConfig, };
            context.DerCircuitConfigs.Add(derCircuitConfig);

            DerSolarUnitConfig unit1 = new DerSolarUnitConfig
            {
                ChangedBy = ChangedBy,
                Name = "PV-Unit 1",
                DerCircuitConfig = derCircuitConfig
            };
            context.DerSolarUnitConfigs.Add(unit1);

            ModbusConnectionConfig modbusConnection = new ModbusConnectionConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConnection for all",
                IsPipeliningEnabled = false,
                ConnectionTimeoutMilliseconds = 5000,
                DisconnectionTimeoutMilliseconds = 5000,
                ConnectRetryCount = 3,
                ConnectRetryIntervalMilliseconds = 5000,
                OperationTimeoutMilliseconds = 500,
                SendRetryCount = 1,
                SendRetryIntervalMilliseconds = 100,
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

            if (createSimulatorData == true)
            {
                ServiceSimulatorData.CreatePowerConversionSystemDeviceSim(context, devicePcs);
            }

            ModbusConfig pcs1Modbus = new ModbusConfig
            {
                ChangedBy = ChangedBy,
                Name = "ModbusConfigPcs1",
                Address = Pcs1,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                UnitId = 1,
                ModbusConnectionConfig = modbusConnection
            };
            context.ModbusConfigs.Add(pcs1Modbus);

            PowerConversionSystemConfig pcs1 = new PowerConversionSystemConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "PCS PV 1",
                DeviceId = 1,
                InitiallyStarted = true,
                ModbusConfig = pcs1Modbus,
                PowerConversionSystemDeviceConfig = devicePcs,
                DerUnitConfig = unit1
            };
            context.PowerConversionSystemConfigs.Add(pcs1);

            SolarPanelDeviceConfig pvDevice = new SolarPanelDeviceConfig
            {
                ChangedBy = ChangedBy,
                Name = "Device PV 1",
                ClassName = "paskalON.Devices.Equipments.EnergyResources.Solars.Simples.SolarPanelSimpleV1Proxy",
                MinimumVoltage = 10,
                MaximumVoltage = 50,
                MinimumCurrent = 0,
                MaximumCurrent = 20
            };
            context.SolarPanelDeviceConfigs.Add(pvDevice);

            SolarPanelConfig pv1 = new SolarPanelConfig
            {
                IsActive = true,
                ChangedBy = ChangedBy,
                Name = "PV 1",
                DeviceId = 1,
                NumberOfPanels = 10,
                ConnectionType = SolarConnectionType.Series,
                SolarPanelDeviceConfig = pvDevice,
                DerUnitConfig = unit1
            };
            context.SolarPanelConfigs.Add(pv1);

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
                ChangedBy = ChangedBy,
                Name = "System Power Meter Device",
                ClassName = "paskalON.Devices.Equipments.Meters.PowerMeters.Simples.SystemPowerMeterSimpleV1Proxy",
                IsReversePowerFlow = false,
                IsCurrentSigned = true,
                PowerMeterMapC37Config = powerMeterMap,
            };
            context.PowerMeterDeviceConfigs.Add(systemPowerMeterDevice);

            C37ConnectionConfig c37ConnectionConfig = new C37ConnectionConfig
            {
                ChangedBy = ChangedBy,
                Name = "C37 connection for all C37",
                ConnectionTimeoutMilliseconds = 5000,
                DisconnectionTimeoutMilliseconds = 5000,
                ConnectRetryCount = 3,
                ConnectRetryIntervalMilliseconds = 5000,
                OperationTimeoutMilliseconds = 5000
            };
            context.C37ConnectionConfigs.Add(c37ConnectionConfig);


            C37Config systemMeterC37 = new C37Config
            {
                ChangedBy = ChangedBy,
                Name = "SystemPowerMeter 1",
                C37ConnectionConfig = c37ConnectionConfig,
                AddressFamily = AddressFamily.InterNetwork,
                Address = PmSys1,
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
                ChangedBy = ChangedBy,
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
