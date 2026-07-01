using paskalON.Communication.Protocols.C37118.Types;
using paskalON.Communication.Protocols.Modbus.Configurations;
using paskalON.Communication.Protocols.Modbus.Enums;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.PhysicalUnits;
using paskalON.PhysicalUnits.Electricals.Powers;
using System.Net.Sockets;

namespace paskalON.Devices.Infrastructure.IntegrationTest.Storage.SampleData
{
    /// <summary>
    /// Used to create at least one entity of db set.
    /// </summary>
    public class SimpleSetBess
    {
        // Core
        public ModbusConnectionConfig? ModbusConnectionConfig { get; set; }

        // Maps
        public PowerMeterMapC37Config? PowerMeterMapC37Config { get; set; }
        public PowerMeterMapModbusConfig? PowerMeterMapModbusConfig { get; set; }
        public GenericModbusMapConfig? GenericModbusMapConfig { get; set; }
        public GenericModbusMapConfig? GenericModbusMapCircuitConfig { get; set; }
        public GenericModbusMapConfig? GenericModbusMapTransferSwitchConfig { get; set; }
        public GenericModbusCoilPointConfig? GenericModbusCoilPointConfig { get; set; }
        public GenericModbusDiscreteInputPointConfig? GenericModbusDiscreteInputPointConfig { get; set; }
        public GenericModbusHoldingRegisterConfig? GenericModbusHoldingRegisterConfig { get; set; }
        public GenericModbusInputRegisterConfig? GenericModbusInputRegisterConfig { get; set; }


        // Devices
        public PowerConversionSystemDeviceConfig? PowerConversionSystemDeviceConfig { get; set; }
        public BatteryBankDeviceConfig? BatteryBankDeviceConfig { get; set; }
        public SolarPanelDeviceConfig? SolarPanelDeviceConfig { get; set; }
        public GenericModbusDeviceConfig? GenericModbusDeviceConfig { get; set; }
        public CircuitBreakerDeviceConfig? CircuitBreakerDeviceConfig { get; set; }
        public AutomaticTransferSwitchDeviceConfig? AutomaticTransferSwitchDeviceConfig { get; set; }
        public PowerMeterDeviceConfig? PowerMeterDeviceC37Config { get; set; }
        public PowerMeterDeviceConfig? PowerMeterDeviceModbusConfig { get; set; }

        // Configurations
        public DerConfig? DerConfig { get; set; }
        public DerGroupConfig? DerGroupConfig { get; set; }
        public DerCircuitConfig? DerCircuitBessConfig { get; set; }
        public DerBatteryStorageUnitConfig? DerBatteryStorageUnitConfig { get; set; }
        public ModbusConfig? DerContainerModbusConfig { get; set; }
        public DerContainerConfig? DerContainerConfig { get; set; }
        public ModbusConfig? BatteryBankModbusConfig { get; set; }
        public BatteryBankConfig? BatteryBankConfig { get; set; }
        public ModbusConfig? PowerConversionSystemBessModbusConfig { get; set; }
        public PowerConversionSystemConfig? PowerConversionSystemBessConfig { get; set; }

        //Meters
        public C37Config? SystemPowerMeterC37Config { get; set; }
        public SystemPowerMeterConfig? SystemPowerMeterConfig { get; set; }
        public C37Config? CircuitPowerMeterC37Config { get; set; }
        public CircuitPowerMeterConfig? CircuitPowerMeterConfig { get; set; }
        public ModbusConfig? AuxiliaryPowerMeterModbusConfig { get; set; }
        public AuxiliaryPowerMeterConfig? AuxiliaryPowerMeterConfig { get; set; }
        public ModbusConfig? ExternalPowerMeterModbusConfig { get; set; }
        public ExternalPowerMeterConfig? ExternalPowerMeterConfig { get; set; }

        // Gmds
        public GenericModbusConfig? GenericModbusConfig { get; set; }
        public CircuitBreakerConfig? CircuitBreakerConfig { get; set; }
        public AutomaticTransferSwitchConfig? AutomaticTransferSwitchConfig { get; set; }



        /// <summary>
        /// Constructor that unusually creates all data.
        /// </summary>
        public SimpleSetBess()
        {
            CreateCore();
            CreateMaps();
            CreateDevices();
            CreateConfiguration();
            CreateMeters();
            CreateGmds();
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


        // CreateMaps
        private void CreateMaps()
        {
            PowerMeterMapC37Config = new PowerMeterMapC37Config
            {
                ChangedBy = "Test",
                Name = "Power Meter Map C37 1",
                ApparentPower = "Analog0",
                CurrentA = "Phasor4",
                CurrentAngleA = "Phasor4",
                CurrentB = "Phasor5",
                CurrentAngleB = "Phasor5",
                CurrentC = "Phasor6",
                CurrentAngleC = "Phasor6",
                PowerFactor = "Analog4",
                ReactivePower = "Analog5",
                ReactivePowerA = "Analog6",
                ReactivePowerB = "Analog7",
                ReactivePowerC = "Analog8",
                ActivePower = "Analog9",
                ActivePowerA = "Analog10",
                ActivePowerB = "Analog11",
                ActivePowerC = "Analog12",
                VoltagePositiveSequence = "Phasor3",
                VoltagePositiveSequenceAngle = "Phasor3",
                VoltageA = "Phasor0",
                VoltageAngleA = "Phasor0",
                VoltageB = "Phasor1",
                VoltageAngleB = "Phasor1",
                VoltageC = "Phasor2",
                VoltageAngleC = "Phasor2",
                VoltageAB = "Analog17",
                VoltageBC = "Analog18",
                VoltageCA = "Analog19"
            };

            PowerMeterMapModbusConfig = new PowerMeterMapModbusConfig
            {
                ChangedBy = "Test",
                Name = "Power Meter Map Modbus 1",
                StartingHoldingRegister = 40001,
                StartingInputRegister = 30001,
                StartingDiscreteInput = 10001,
                StartingCoil = 1,
                PollingRange = new List<ModbusRegisterMapPollingRangeConfig> { new ModbusRegisterMapPollingRangeConfig { Start = 30373, End = 30387, IsInputRegisterRange = true } },
                ActivePowerMap = new ModbusRegisterMapEntryConfig { Index = 30373, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ReactivePowerMap = new ModbusRegisterMapEntryConfig { Index = 30374, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ApparentPowerMap = new ModbusRegisterMapEntryConfig { Index = 30375, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                PowerFactorMap = new ModbusRegisterMapEntryConfig { Index = 30376, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 100 },
                FrequencyMap = new ModbusRegisterMapEntryConfig { Index = 30377, ModbusRegisterFormat = ModbusRegisterFormat.MbUint16, Scale = 100 },
                VoltageAMap = new ModbusRegisterMapEntryConfig { Index = 30345, ModbusRegisterFormat = ModbusRegisterFormat.MbUint16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                VoltageBMap = new ModbusRegisterMapEntryConfig { Index = 30347, ModbusRegisterFormat = ModbusRegisterFormat.MbUint16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                VoltageCMap = new ModbusRegisterMapEntryConfig { Index = 30349, ModbusRegisterFormat = ModbusRegisterFormat.MbUint16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                CurrentAMap = new ModbusRegisterMapEntryConfig { Index = 30321, ModbusRegisterFormat = ModbusRegisterFormat.MbUint16, Scale = 1 },
                CurrentBMap = new ModbusRegisterMapEntryConfig { Index = 30323, ModbusRegisterFormat = ModbusRegisterFormat.MbUint16, Scale = 1 },
                CurrentCMap = new ModbusRegisterMapEntryConfig { Index = 30325, ModbusRegisterFormat = ModbusRegisterFormat.MbUint16, Scale = 1 },
                ActivePowerAMap = new ModbusRegisterMapEntryConfig { Index = 30361, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ActivePowerBMap = new ModbusRegisterMapEntryConfig { Index = 30365, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ActivePowerCMap = new ModbusRegisterMapEntryConfig { Index = 30369, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ReactivePowerAMap = new ModbusRegisterMapEntryConfig { Index = 30362, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ReactivePowerBMap = new ModbusRegisterMapEntryConfig { Index = 30366, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ReactivePowerCMap = new ModbusRegisterMapEntryConfig { Index = 30370, ModbusRegisterFormat = ModbusRegisterFormat.MbInt16, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                EnergyDeliveredMap = new ModbusRegisterMapEntryConfig { Index = 30383, ModbusRegisterFormat = ModbusRegisterFormat.MbInt32Be, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                EnergyReceivedMap = new ModbusRegisterMapEntryConfig { Index = 30381, ModbusRegisterFormat = ModbusRegisterFormat.MbInt32Be, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ReactiveEnergyDeliveredMap = new ModbusRegisterMapEntryConfig { Index = 30387, ModbusRegisterFormat = ModbusRegisterFormat.MbInt32Be, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
                ReactiveEnergyReceivedMap = new ModbusRegisterMapEntryConfig { Index = 30385, ModbusRegisterFormat = ModbusRegisterFormat.MbInt32Be, Scale = 1, UnitPrefix = MetricPrefix.Kilo },
            };


            // Generic
            GenericModbusMapConfig = new GenericModbusMapConfig
            {
                ChangedBy = "Test",
                Name = "Generic Modbus Map 1",
            };

            GenericModbusDiscreteInputPointConfig = new GenericModbusDiscreteInputPointConfig
            {
                ChangedBy = "Test",
                Name = "Discrete Input 1",
                ModbusNumber = 0,
                IsAlarm = true,
                ModbusRegisterFormat = ModbusRegisterFormat.MbBool,
                GenericModbusMapConfig = GenericModbusMapConfig!
            };

            GenericModbusHoldingRegisterConfig = new GenericModbusHoldingRegisterConfig
            {
                ChangedBy = "Test",
                Name = "Holding 1",
                ModbusNumber = 1,
                ModbusRegisterFormat = ModbusRegisterFormat.MbInt16,
                GenericModbusMapConfig = GenericModbusMapConfig!
            };


            // Circuit
            GenericModbusMapCircuitConfig = new GenericModbusMapConfig
            {
                ChangedBy = "Test",
                Name = "Generic Modbus Map Circuit",
            };


            GenericModbusInputRegisterConfig = new GenericModbusInputRegisterConfig
            {
                ChangedBy = "Test",
                Name = "BreakerStatus",
                ModbusNumber = 42,
                BitIndex = 2,
                GenericModbusMapConfig = GenericModbusMapCircuitConfig!
            };


            // Transfer switch
            GenericModbusMapTransferSwitchConfig = new GenericModbusMapConfig
            {
                ChangedBy = "Test",
                Name = "Generic Modbus Map Transfer Switch",
            };


            GenericModbusCoilPointConfig = new GenericModbusCoilPointConfig
            {
                ChangedBy = "Test",
                Name = "AutoSwitchCoil",
                ModbusNumber = 1,
                GenericModbusMapConfig = GenericModbusMapTransferSwitchConfig!
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

            BatteryBankDeviceConfig = new BatteryBankDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device BMS 1",
                ClassName = "BatteryBankTest",
                NameplateCapacity = 372700,
                NameplateMaximumChargeRate = 186000,
                NameplateMaximumDischargeRate = 186000,
                BatteryType = BatteryType.LithiumIon,
                RackCount = 5,
                StringsPerRackCount = 1,
                InverterBusNumber = 1,
                AbsoluteMinimumStateOfCharge = 0,
                AbsoluteMaximumStateOfCharge = 100,
                PreferredMinimumStateOfCharge = 5,
                PreferredMaximumStateOfCharge = 95,
                AbsoluteMaximumTemperature = 60,
                AbsoluteMinimumTemperature = -30,
                PreferredMinimumTemperature = 55,
                PreferredMaximumTemperature = -25,
                AbsoluteMaxChargeCurrentAmps = 900,
                AbsoluteMaxDischargeCurrentAmps = 900,
                MaximumDcVoltage = 1164.8,
                MinimumDcVoltage = 1497.6,
                ZeroCapacityOnCommLoss = true
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


            GenericModbusDeviceConfig = new GenericModbusDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Generic Modbus Device 1",
                ClassName = "GenericClass",
                GenericModbusMapConfig = GenericModbusMapConfig!
            };


            CircuitBreakerDeviceConfig = new CircuitBreakerDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device CircuitBreaker 1",
                ClassName = "CircuitBreakerTest",
                BreakerStatusRegister = GenericModbusHoldingRegisterConfig!,
                CircuitBreakerOperation = CircuitBreakerOperation.TripAndReset,
                GenericModbusMapConfig = GenericModbusMapCircuitConfig!
            };


            AutomaticTransferSwitchDeviceConfig = new AutomaticTransferSwitchDeviceConfig
            {
                ChangedBy = "Test",
                Name = "AutoStatus",
                ClassName = "AutomaticTransferSwitchTest",
                GridConnected = GenericModbusCoilPointConfig!,
                GenericModbusMapConfig = GenericModbusMapTransferSwitchConfig!
            };


            PowerMeterDeviceC37Config = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device power Meter C37 1",
                ClassName = "PowerMeterC37Test",
                IsReversePowerFlow = false,
                IsCurrentSigned = true,
                PowerMeterMapC37Config = PowerMeterMapC37Config,
            };

            PowerMeterDeviceModbusConfig = new PowerMeterDeviceConfig
            {
                ChangedBy = "Test",
                Name = "Device power Meter Modbus 1",
                ClassName = "PowerMeterModbusTest",
                IsReversePowerFlow = false,
                IsCurrentSigned = true,
                PowerMeterMapModbusConfig = PowerMeterMapModbusConfig,
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

            DerCircuitBessConfig = new DerCircuitConfig
            {
                ChangedBy = "Test",
                Name = "Circuit Bess",
                DerGroupConfig = DerGroupConfig,
            };

            DerBatteryStorageUnitConfig = new DerBatteryStorageUnitConfig
            {
                ChangedBy = "Test",
                Name = "BMS-Unit",
                IncludeBatteryInOperations = true,
                DerCircuitConfig = DerCircuitBessConfig
            };


            DerContainerModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "DerContainerModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartContainer,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = ModbusConnectionConfig!
            };


            DerContainerConfig = new DerContainerConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "Container 1",
                DeviceId = 0,
                ModbusConfig = DerContainerModbusConfig,
                DerUnitConfig = DerBatteryStorageUnitConfig
            };

            BatteryBankModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = ModbusConnectionConfig!
            };

            BatteryBankConfig = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "BMS 1",
                DeviceId = 0,
                InitiallyConnected = true,
                ModbusConfig = BatteryBankModbusConfig,
                BatteryBankDeviceConfig = BatteryBankDeviceConfig!,
                DerUnitConfig = DerBatteryStorageUnitConfig
            };


            PowerConversionSystemBessModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "PowerConversionSystemBessConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                StationId = 1,
                ModbusConnectionConfig = ModbusConnectionConfig!
            };

            PowerConversionSystemBessConfig = new PowerConversionSystemConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "PCS Bess",
                DeviceId = 0,
                InitiallyStarted = true,
                ModbusConfig = PowerConversionSystemBessModbusConfig,
                PowerConversionSystemDeviceConfig = PowerConversionSystemDeviceConfig!,
                DerUnitConfig = DerBatteryStorageUnitConfig
            };

            DerBatteryStorageUnitConfig.PowerConversionSystemConfig = PowerConversionSystemBessConfig;
        }


        // CreateMeters
        private void CreateMeters()
        {
            SystemPowerMeterC37Config = new C37Config
            {
                ChangedBy = "Test",
                Name = "SystemPowerMeterC37Config",
                IpAddress = Constants.Ip4Localhost,
                Port = Constants.PortStartMeter,
                ConfigFrameTimeoutMilliseconds = 3000,
                DataFrameRetryCount = 3,
                DataFrameTimeoutMilliseconds = 500,
                IdOfDataBlock = 1,
                IdOfDataStream = 1,
                TransportLayer = C37TransportLayer.UDP,
            };


            // System meter communication
            SystemPowerMeterConfig = new SystemPowerMeterConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "C37 System Power Meter 1",
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                C37Config = SystemPowerMeterC37Config,
                PowerMeterDeviceConfig = PowerMeterDeviceC37Config!,
                DerConfig = DerConfig!
            };


            CircuitPowerMeterC37Config = new C37Config
            {
                ChangedBy = "Test",
                Name = "CircuitPowerMeterConfig",
                IpAddress = Constants.Ip4Localhost,
                Port = Constants.PortStartMeter + 1,
                ConfigFrameTimeoutMilliseconds = 3000,
                DataFrameRetryCount = 3,
                DataFrameTimeoutMilliseconds = 500,
                IdOfDataBlock = 1,
                IdOfDataStream = 1,
                TransportLayer = C37TransportLayer.UDP,
            };

            // Circuit meter communication
            CircuitPowerMeterConfig = new CircuitPowerMeterConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "Circuit Power Meter C37 1",
                DeviceId = 2,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                C37Config = CircuitPowerMeterC37Config,
                PowerMeterDeviceConfig = PowerMeterDeviceC37Config!,
                DerCircuitConfig = DerCircuitBessConfig!
            };


            AuxiliaryPowerMeterModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "AuxiliaryPowerMeterModbusConfig",
                StationId = 1,
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartMeter + 2,
                AddressFamily = AddressFamily.InterNetwork,
                ModbusConnectionConfig = ModbusConnectionConfig!
            };


            // Auxiliary meter communication
            AuxiliaryPowerMeterConfig = new AuxiliaryPowerMeterConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "Auxiliary Power Meter Modbus 1",
                DeviceId = 3,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                ModbusConfig = AuxiliaryPowerMeterModbusConfig,
                PowerMeterDeviceConfig = PowerMeterDeviceModbusConfig!,
                DerConfig = DerConfig!
            };


            ExternalPowerMeterModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "ExternalPowerMeterModbusConfig",
                StationId = 1,
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartMeter + 3,
                AddressFamily = AddressFamily.InterNetwork,
                ModbusConnectionConfig = ModbusConnectionConfig!
            };

            // External meter communication
            ExternalPowerMeterConfig = new ExternalPowerMeterConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "External Power Meter Modbus Config 1",
                DeviceId = 4,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                ModbusConfig = ExternalPowerMeterModbusConfig,
                PowerMeterDeviceConfig = PowerMeterDeviceModbusConfig!,
                DerConfig = DerConfig!
            };
        }


        // CreateGmds
        private void CreateGmds()
        {
            GenericModbusConfig = new GenericModbusConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "Generic Modbus 1",
                DeviceId = 1,
                StationId = 1,
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartGmd,
                AddressFamily = AddressFamily.InterNetwork,
                ModbusConnectionConfig = ModbusConnectionConfig!,
                GenericModbusDeviceConfig = GenericModbusDeviceConfig!,
                DerConfig = DerConfig!
            };

            CircuitBreakerConfig = new CircuitBreakerConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "Circuit Breaker 1",
                DeviceId = 2,
                StationId = 1,
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartGmd + 1,
                AddressFamily = AddressFamily.InterNetwork,
                ModbusConnectionConfig = ModbusConnectionConfig!,
                CircuitBreakerDeviceConfig = CircuitBreakerDeviceConfig!,
                DerCircuitConfig = DerCircuitBessConfig!
            };

            AutomaticTransferSwitchConfig = new AutomaticTransferSwitchConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "Automatic Transfer Switch 1",
                DeviceId = 3,
                StationId = 1,
                Address = Constants.Ip4Localhost,
                AddressFamily = AddressFamily.InterNetwork,
                Port = Constants.PortStartGmd + 2,
                ModbusConnectionConfig = ModbusConnectionConfig!,
                AutomaticTransferSwitchDeviceConfig = AutomaticTransferSwitchDeviceConfig!,
                DerConfig = DerConfig!
            };
        }

    }
}