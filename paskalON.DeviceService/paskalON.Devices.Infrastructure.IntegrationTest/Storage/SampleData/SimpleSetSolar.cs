using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using System.Net.Sockets;

namespace paskalON.Devices.Infrastructure.IntegrationTest.Storage.SampleData
{
    /// <summary>
    /// Used to create at least one entity of db set for solar configurations only.
    /// </summary>
    public class SimpleSetSolar
    {
        // Core
        public ModbusConnectionConfig? ModbusConnectionConfig { get; set; }


        // Devices
        public PowerConversionSystemDeviceConfig? PowerConversionSystemDeviceConfig { get; set; }
        public SolarPanelDeviceConfig? SolarPanelDeviceConfig { get; set; }


        // Configurations
        public DerConfig? DerConfig { get; set; }
        public DerGroupConfig? DerGroupConfig { get; set; }
        public DerCircuitConfig? DerCircuitPvConfig { get; set; }
        public DerSolarUnitConfig? DerSolarUnitConfig { get; set; }
        public ModbusConfig? PowerConversionSystemPvModbusConfig { get; set; }
        public PowerConversionSystemConfig? PowerConversionSystemPvConfig { get; set; }
        public SolarPanelConfig? SolarPanelConfig { get; set; }


        /// <summary>
        /// Constructor that unusually creates all data.
        /// </summary>
        public SimpleSetSolar()
        {
            CreateCore();
            CreateDevices();
            CreateConfiguration();
        }


        void CreateCore()
        {
            ModbusConnectionConfig = new ModbusConnectionConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig for all",
                PollingIntervalMilliseconds = 1001,
                MasterHeartBeatIntervalMilliseconds = 900,
                IsPipeliningEnabled = false,
                ConnectionTimeoutMilliseconds = 1001,
                DisconnectionTimeoutMilliseconds = 1002,
                ConnectRetryCount = 2,
                ConnectRetryIntervalMilliseconds = 4001,
                SendTimeoutMilliseconds = 1003,
                SendRetryCount = 1,
                SendRetryIntervalMilliseconds = 4002,
                ServerToClientAliveIntervalSeconds = -1,
                ServerMaximumConnections = 5
            };
        }

        // CreateDevices
        private void CreateDevices()
        {
            PowerConversionSystemDeviceConfig = new PowerConversionSystemDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device PCS 1",
                ClassName = "PowerConversionSystemTest",
                NameplateMaximumActivePower = 3630000,
                NameplateMaximumReactivePower = 3630000,
                NameplateMaximumApparentPower = 3630000,
                NameplateMaximumACCurrent = 3175,
                MaximumDCVoltage = 1140,
                MinimumDCVoltage = 100,
                ZeroOutputOnCommLoss = true
            };

            SolarPanelDeviceConfig = new SolarPanelDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device SolarPanel 1",
                ClassName = "SolarPanelTest",
                MaximumVoltage = 40.8,
                MinimumVoltage = 0,
                MaximumCurrent = 12.87,
                MinimumCurrent = 0,
            };
        }


        // CreateConfiguration
        private void CreateConfiguration()
        {
            DerConfig = new DerConfig
            {
                ChangedBy = "Test",
                Name = "Der 1",
            };

            DerGroupConfig = new DerGroupConfig { ChangedBy = "Test", Name = "Group 1", DerConfig = DerConfig };

            DerCircuitPvConfig = new DerCircuitConfig { ChangedBy = "Test", Name = "Circuit Pv", DerGroupConfig = DerGroupConfig };

            DerSolarUnitConfig = new DerSolarUnitConfig
            {
                ChangedBy = "Test",
                Name = "PV-Unit 1",
                DerCircuitConfig = DerCircuitPvConfig
            };

            PowerConversionSystemPvModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "PowerConversionSystemPvConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs + 1,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = ModbusConnectionConfig!
            };

            PowerConversionSystemPvConfig = new PowerConversionSystemConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "PCS PV 1",
                DeviceId = 1,
                InitiallyStarted = true,
                ModbusConfig = PowerConversionSystemPvModbusConfig,
                PowerConversionSystemDeviceConfig = PowerConversionSystemDeviceConfig!,
                DerUnitConfig = DerSolarUnitConfig
            };

            DerSolarUnitConfig.PowerConversionSystemConfig = PowerConversionSystemPvConfig;

            SolarPanelConfig = new SolarPanelConfig
            {
                IsActive = true,
                DerUnitConfig = DerSolarUnitConfig,
                ChangedBy = "Test",
                Name = "SolarPanel 1",
                DeviceId = 1,
                ConnectionType = SolarConnectionType.Series,
                NumberOfPanels = 100,
                SolarPanelDeviceConfig = SolarPanelDeviceConfig!
            };
        }
    }
}