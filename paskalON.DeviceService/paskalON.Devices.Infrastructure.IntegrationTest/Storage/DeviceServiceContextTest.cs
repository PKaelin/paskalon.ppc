using Microsoft.EntityFrameworkCore;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.Devices.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public class DeviceServiceContextTest
    {
        // TODO: Implement more tests to check required and relationships.


        [TestMethod]
        public void CreateDeviceServiceContext()
        {
            using DeviceServiceContext context = new DeviceServiceContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }


        [TestMethod]
        public void CreateBessTest()
        {
            SimpleSetBess sample = new SimpleSetBess();
            List<DerConfig> configs = new List<DerConfig>();

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Core
                context.Configurations.Add(new Configuration { ChangedBy = "Test", Key = "Key", Value = "Value", Description = "Desc" });
                context.ModbusConnectionConfigs.Add(sample.ModbusConnectionConfig!);
                // Devices
                context.PowerConversionSystemDeviceConfigs.Add(sample.PowerConversionSystemDeviceConfig!);
                context.BatteryBankDeviceConfigs.Add(sample.BatteryBankDeviceConfig!);
                // Configurations
                context.DerConfigs.Add(sample.DerConfig!);
                context.DerGroupConfigs.Add(sample.DerGroupConfig!);
                context.DerCircuitConfigs.Add(sample.DerCircuitBessConfig!);
                context.DerBatteryStorageUnitConfigs.Add(sample.DerBatteryStorageUnitConfig!);
                context.ModbusConfigs.Add(sample.DerContainerModbusConfig!);
                context.DerContainerConfigs.Add(sample.DerContainerConfig!);
                context.ModbusConfigs.Add(sample.BatteryBankModbusConfig!);
                context.BatteryBankConfigs.Add(sample.BatteryBankConfig!);
                context.ModbusConfigs.Add(sample.PowerConversionSystemBessModbusConfig!);
                context.PowerConversionSystemConfigs.Add(sample.PowerConversionSystemBessConfig!);

                context.SaveChanges();
            }

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                configs = context.DerConfigs
                    // Branch batteries - pcs
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    .ThenInclude(unit => unit.PowerConversionSystemConfig)
                    .ThenInclude(pcs => pcs!.PowerConversionSystemDeviceConfig)
                    // Branch batteries - pcs communication 
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    .ThenInclude(unit => unit.PowerConversionSystemConfig)
                    .ThenInclude(pcs => pcs!.ModbusConfig)
                    .ThenInclude(mc => mc.ModbusConnectionConfig)
                    // Branch batteries - batteries
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    .ThenInclude(unit => ((DerBatteryStorageUnitConfig)unit).BatteryBankConfigs)
                    .ThenInclude(bb => bb!.BatteryBankDeviceConfig)
                    // Branch batteries - batteries communication
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    .ThenInclude(unit => ((DerBatteryStorageUnitConfig)unit).BatteryBankConfigs)
                    .ThenInclude(bb => bb.ModbusConfig)
                    .ThenInclude(mc => mc.ModbusConnectionConfig)
                    .ToList();
            }

            // Do some checks
            Assert.IsNotNull(configs);
            Assert.HasCount(1, configs);
            DerConfig config = configs.First();
            Assert.AreEqual(sample.DerConfig!.Name, config.Name);
            Assert.HasCount(1, config.DerGroupConfigs);
            DerGroupConfig group = config.DerGroupConfigs.First();
            Assert.AreEqual(sample.DerGroupConfig!.Name, group.Name);
            Assert.HasCount(1, group.DerCircuits);
            DerCircuitConfig circuit = group.DerCircuits.First();
            Assert.AreEqual(sample.DerCircuitBessConfig!.Name, circuit.Name);
            Assert.HasCount(1, circuit.DerUnitConfigs.OfType<DerBatteryStorageUnitConfig>());
            // BatteryStorageUnitConfig
            DerBatteryStorageUnitConfig unit = circuit.DerUnitConfigs.OfType<DerBatteryStorageUnitConfig>().First();
            Assert.AreEqual(sample.DerBatteryStorageUnitConfig!.Name, unit.Name);
            Assert.AreEqual(sample.PowerConversionSystemBessConfig!.Name, unit.PowerConversionSystemConfig.Name);
            Assert.AreEqual(sample.PowerConversionSystemDeviceConfig!.Name, unit.PowerConversionSystemConfig.PowerConversionSystemDeviceConfig.Name);
            Assert.HasCount(1, unit.BatteryBankConfigs);
            BatteryBankConfig battery = unit.BatteryBankConfigs.First();
            Assert.AreEqual(sample.BatteryBankConfig!.Name, battery.Name);
            Assert.AreEqual(sample.BatteryBankDeviceConfig!.Name, battery.BatteryBankDeviceConfig.Name);
        }


        [TestMethod]
        public void CreateSolarTest()
        {
            SimpleSetSolar sample = new SimpleSetSolar();
            List<DerConfig> configs = new List<DerConfig>();

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Core
                context.ModbusConnectionConfigs.Add(sample.ModbusConnectionConfig!);
                // Devices
                context.PowerConversionSystemDeviceConfigs.Add(sample.PowerConversionSystemDeviceConfig!);
                context.SolarPanelDeviceConfigs.Add(sample.SolarPanelDeviceConfig!);
                // Configurations
                context.DerConfigs.Add(sample.DerConfig!);
                context.DerGroupConfigs.Add(sample.DerGroupConfig!);
                context.DerCircuitConfigs.Add(sample.DerCircuitPvConfig!);
                context.DerSolarUnitConfigs.Add(sample.DerSolarUnitConfig!);
                context.ModbusConfigs.Add(sample.PowerConversionSystemPvModbusConfig!);
                context.PowerConversionSystemConfigs.Add(sample.PowerConversionSystemPvConfig!);
                context.SolarPanelConfigs.Add(sample.SolarPanelConfig!);

                context.SaveChanges();
            }

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                configs = context.DerConfigs
                    // Branch solar - solar panels
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    .ThenInclude(unit => ((DerSolarUnitConfig)unit).SolarPanelConfig)
                    .ThenInclude(panel => panel.SolarPanelDeviceConfig)
                    // Branch solar - pcs
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    .ThenInclude(pcs => pcs.PowerConversionSystemConfig)
                    .ThenInclude(dev => dev.PowerConversionSystemDeviceConfig)
                    // Branch solar - pcs communication 
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    .ThenInclude(unit => unit.PowerConversionSystemConfig)
                    .ThenInclude(pcs => pcs!.ModbusConfig)
                    .ThenInclude(mc => mc.ModbusConnectionConfig)
                    .ToList();
            }

            // Do some checks
            Assert.IsNotNull(configs);
            Assert.HasCount(1, configs);
            Assert.AreEqual(sample.DerConfig!.Name, configs.First().Name);
            Assert.HasCount(1, configs.First().DerGroupConfigs);
            DerGroupConfig group = configs.First().DerGroupConfigs.First();
            Assert.AreEqual(sample.DerGroupConfig!.Name, group.Name);
            Assert.HasCount(1, group.DerCircuits);
            DerCircuitConfig circuit = group.DerCircuits.First();
            Assert.AreEqual(sample.DerCircuitPvConfig!.Name, circuit.Name);
            Assert.HasCount(1, circuit.DerUnitConfigs.OfType<DerSolarUnitConfig>());
            DerSolarUnitConfig unit = circuit.DerUnitConfigs.OfType<DerSolarUnitConfig>().First();
            Assert.AreEqual(sample.DerSolarUnitConfig!.Name, unit.Name);
            Assert.AreEqual(sample.SolarPanelConfig!.Name, unit.SolarPanelConfig.Name);
            Assert.AreEqual(sample.SolarPanelDeviceConfig!.Name, unit.SolarPanelConfig.SolarPanelDeviceConfig.Name);
            Assert.AreEqual(sample.PowerConversionSystemPvConfig!.Name, unit.PowerConversionSystemConfig.Name);
            Assert.AreEqual(sample.PowerConversionSystemDeviceConfig!.Name, unit.PowerConversionSystemConfig.PowerConversionSystemDeviceConfig.Name);
            Assert.AreEqual(sample.PowerConversionSystemPvModbusConfig!.Name, unit.PowerConversionSystemConfig.ModbusConfig.Name);
        }


        [TestMethod]
        public void CreateMetersTest()
        {
            SimpleSetBess sample = new SimpleSetBess();
            List<DerConfig> configs = new List<DerConfig>();

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Core
                context.ModbusConnectionConfigs.Add(sample.ModbusConnectionConfig!);
                // Maps
                context.PowerMeterMapC37Configs.Add(sample.PowerMeterMapC37Config!);
                context.PowerMeterMapModbusConfigs.Add(sample.PowerMeterMapModbusConfig!);
                // Devices
                context.PowerMeterDeviceConfigs.Add(sample.PowerMeterDeviceC37Config!);
                context.PowerMeterDeviceConfigs.Add(sample.PowerMeterDeviceModbusConfig!);
                // Configurations
                context.DerConfigs.Add(sample.DerConfig!);
                context.DerGroupConfigs.Add(sample.DerGroupConfig!);
                context.DerCircuitConfigs.Add(sample.DerCircuitBessConfig!);
                // Meters
                context.C37Configs.Add(sample.SystemPowerMeterC37Config!);
                context.SystemPowerMeterConfigs.Add(sample.SystemPowerMeterConfig!);
                context.C37Configs.Add(sample.CircuitPowerMeterC37Config!);
                context.CircuitPowerMeterConfigs.Add(sample.CircuitPowerMeterConfig!);
                context.ModbusConfigs.Add(sample.AuxiliaryPowerMeterModbusConfig!);
                context.AuxiliaryPowerMeterConfigs.Add(sample.AuxiliaryPowerMeterConfig!);
                context.ModbusConfigs.Add(sample.ExternalPowerMeterModbusConfig!);
                context.ExternalPowerMeterConfigs.Add(sample.ExternalPowerMeterConfig!);

                context.SaveChanges();
            }

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                configs = context.DerConfigs
                    // Meters C37
                    .Include(sm => sm.SystemPowerMeterConfigs)
                    .ThenInclude(dev => dev.PowerMeterDeviceConfig)
                    .ThenInclude(map => map.PowerMeterMapC37Config)
                    .Include(sm => sm.SystemPowerMeterConfigs)
                    .ThenInclude(com => com.C37Config)
                    .Include(em => em.ExternalPowerMeterConfigs)
                    .ThenInclude(dev => dev.PowerMeterDeviceConfig)
                    .ThenInclude(map => map.PowerMeterMapC37Config)
                    .Include(em => em.ExternalPowerMeterConfigs)
                    .ThenInclude(com => com.C37Config)
                    .Include(am => am.AuxiliaryPowerMeterConfigs)
                    .ThenInclude(dev => dev.PowerMeterDeviceConfig)
                    .ThenInclude(map => map.PowerMeterMapC37Config)
                    .Include(am => am.AuxiliaryPowerMeterConfigs)
                    .ThenInclude(com => com.C37Config)
                    // Meters Modbus
                    .Include(sm => sm.SystemPowerMeterConfigs)
                    .ThenInclude(dev => dev.PowerMeterDeviceConfig)
                    .ThenInclude(map => map.PowerMeterMapModbusConfig)
                    .Include(sm => sm.SystemPowerMeterConfigs)
                    .ThenInclude(com => com.ModbusConfig)
                    .Include(em => em.ExternalPowerMeterConfigs)
                    .ThenInclude(dev => dev.PowerMeterDeviceConfig)
                    .ThenInclude(map => map.PowerMeterMapModbusConfig)
                    .Include(em => em.ExternalPowerMeterConfigs)
                    .ThenInclude(com => com.ModbusConfig)
                    .Include(am => am.AuxiliaryPowerMeterConfigs)
                    .ThenInclude(dev => dev.PowerMeterDeviceConfig)
                    .ThenInclude(map => map.PowerMeterMapModbusConfig)
                    .Include(am => am.AuxiliaryPowerMeterConfigs)
                    .ThenInclude(com => com.ModbusConfig)
                    // Circuit meters
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.CircuitPowerMeterConfig)
                    .ThenInclude(dev => dev!.PowerMeterDeviceConfig)
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.CircuitPowerMeterConfig)
                    .ThenInclude(com => com!.C37Config)
                    .ToList();
            }

            // Do some checks
            Assert.IsNotNull(configs);
            Assert.HasCount(1, configs);
            DerConfig config = configs.First();
            Assert.AreEqual(sample.DerConfig!.Name, config.Name);
            Assert.HasCount(1, config.DerGroupConfigs);
            DerGroupConfig group = config.DerGroupConfigs.First();
            Assert.AreEqual(sample.DerGroupConfig!.Name, group.Name);
            Assert.HasCount(1, group.DerCircuits);
            DerCircuitConfig circuit = group.DerCircuits.First();
            Assert.AreEqual(sample.DerCircuitBessConfig!.Name, circuit.Name);
            // SystemPowerMeter
            Assert.HasCount(1, config.SystemPowerMeterConfigs);
            Assert.AreEqual(sample.SystemPowerMeterConfig!.Name, config.SystemPowerMeterConfigs.First().Name);
            Assert.AreEqual(sample.PowerMeterDeviceC37Config!.Name, config.SystemPowerMeterConfigs.First().PowerMeterDeviceConfig.Name);
            Assert.AreEqual(sample.SystemPowerMeterC37Config!.Name, config.SystemPowerMeterConfigs.First().C37Config!.Name);
            // AuxiliaryPowerMeter
            Assert.HasCount(1, config.AuxiliaryPowerMeterConfigs);
            Assert.AreEqual(sample.AuxiliaryPowerMeterConfig!.Name, config.AuxiliaryPowerMeterConfigs.First().Name);
            Assert.AreEqual(sample.PowerMeterDeviceModbusConfig!.Name, config.AuxiliaryPowerMeterConfigs.First().PowerMeterDeviceConfig.Name);
            Assert.AreEqual(sample.AuxiliaryPowerMeterModbusConfig!.Name, config.AuxiliaryPowerMeterConfigs.First().ModbusConfig!.Name);
            // ExternalPowerMeter
            Assert.HasCount(1, config.ExternalPowerMeterConfigs);
            Assert.AreEqual(sample.ExternalPowerMeterConfig!.Name, config.ExternalPowerMeterConfigs.First().Name);
            Assert.AreEqual(sample.PowerMeterDeviceModbusConfig!.Name, config.ExternalPowerMeterConfigs.First().PowerMeterDeviceConfig.Name);
            Assert.AreEqual(sample.ExternalPowerMeterModbusConfig!.Name, config.ExternalPowerMeterConfigs.First().ModbusConfig!.Name);
            // CircuitPowerMeter
            Assert.AreEqual(sample.CircuitPowerMeterConfig!.Name, circuit.CircuitPowerMeterConfig!.Name);
            Assert.AreEqual(sample.PowerMeterDeviceC37Config!.Name, circuit.CircuitPowerMeterConfig.PowerMeterDeviceConfig.Name);
            Assert.AreEqual(sample.CircuitPowerMeterC37Config!.Name, circuit.CircuitPowerMeterConfig.C37Config!.Name);
        }


        [TestMethod]
        public void CreateGmdsTest()
        {
            SimpleSetBess sample = new SimpleSetBess();
            List<DerConfig> configs = new List<DerConfig>();

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Core
                context.ModbusConnectionConfigs.Add(sample.ModbusConnectionConfig!);
                // Maps
                context.GenericModbusMapConfigs.Add(sample.GenericModbusMapConfig!);
                context.GenericModbusMapConfigs.Add(sample.GenericModbusMapCircuitConfig!);
                context.GenericModbusMapConfigs.Add(sample.GenericModbusMapTransferSwitchConfig!);
                context.GenericModbusCoilPointConfigs.Add(sample.GenericModbusCoilPointConfig!);
                context.GenericModbusDiscreteInputPointConfigs.Add(sample.GenericModbusDiscreteInputPointConfig!);
                context.GenericModbusHoldingRegisterConfigs.Add(sample.GenericModbusHoldingRegisterConfig!);
                context.GenericModbusInputRegisterConfigs.Add(sample.GenericModbusInputRegisterConfig!);
                // Devices
                context.GenericModbusDeviceConfigs.Add(sample.GenericModbusDeviceConfig!);
                context.CircuitBreakerDeviceConfigs.Add(sample.CircuitBreakerDeviceConfig!);
                context.AutomaticTransferSwitchDeviceConfigs.Add(sample.AutomaticTransferSwitchDeviceConfig!);
                // Configurations
                context.DerConfigs.Add(sample.DerConfig!);
                context.DerGroupConfigs.Add(sample.DerGroupConfig!);
                context.DerCircuitConfigs.Add(sample.DerCircuitBessConfig!);
                context.DerBatteryStorageUnitConfigs.Add(sample.DerBatteryStorageUnitConfig!);
                // GMDs
                context.Add(sample.GenericModbusConfig!);
                context.Add(sample.CircuitBreakerConfig!);
                context.Add(sample.AutomaticTransferSwitchConfig!);

                context.SaveChanges();
            }

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                configs = context.DerConfigs
                    // Units
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(circuit => circuit.DerUnitConfigs)
                    // GMDs
                    .Include(gmd => gmd.GenericModbusConfigs)
                    .ThenInclude(dev => dev.GenericModbusDeviceConfig)
                    .ThenInclude(map => map.GenericModbusMapConfig)
                    .Include(gmd => gmd.GenericModbusConfigs)
                    .ThenInclude(mb => mb.ModbusConnectionConfig)
                    .Include(ats => ats.AutomaticTransferSwitchConfigs)
                    .ThenInclude(dev => dev.AutomaticTransferSwitchDeviceConfig)
                    .ThenInclude(map => map.GenericModbusMapConfig)
                    .Include(ats => ats.AutomaticTransferSwitchConfigs)
                    .ThenInclude(mb => mb.ModbusConnectionConfig)
                    // GMD - circuit
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(cb => cb.CircuitBreakerConfig)
                    .ThenInclude(dev => dev!.CircuitBreakerDeviceConfig)
                    .ThenInclude(map => map.GenericModbusMapConfig)
                    .Include(der => der.DerGroupConfigs)
                    .ThenInclude(group => group.DerCircuits)
                    .ThenInclude(cb => cb.CircuitBreakerConfig)
                    .ThenInclude(mb => mb!.ModbusConnectionConfig)
                    .ToList();
            }

            // Do some checks
            Assert.IsNotNull(configs);
            Assert.HasCount(1, configs);
            DerConfig config = configs.First();
            Assert.AreEqual(sample.DerConfig!.Name, config.Name);
            Assert.HasCount(1, config.DerGroupConfigs);
            DerGroupConfig group = config.DerGroupConfigs.First();
            Assert.AreEqual(sample.DerGroupConfig!.Name, group.Name);
            Assert.HasCount(1, group.DerCircuits);
            DerCircuitConfig circuit = group.DerCircuits.First();
            Assert.AreEqual(sample.DerCircuitBessConfig!.Name, circuit.Name);
            Assert.HasCount(1, circuit.DerUnitConfigs.OfType<DerBatteryStorageUnitConfig>());
            // AutomaticTransferSwitchConfig
            Assert.HasCount(1, config.AutomaticTransferSwitchConfigs);
            AutomaticTransferSwitchConfig automatic = config.AutomaticTransferSwitchConfigs.First();
            Assert.AreEqual(sample.AutomaticTransferSwitchConfig!.Name, automatic.Name);
            Assert.AreEqual(sample.AutomaticTransferSwitchDeviceConfig!.Name, automatic.AutomaticTransferSwitchDeviceConfig.Name);
            Assert.AreEqual(sample.AutomaticTransferSwitchDeviceConfig.GenericModbusMapConfig!.Name, automatic.AutomaticTransferSwitchDeviceConfig.GenericModbusMapConfig!.Name);
            Assert.AreEqual(sample.AutomaticTransferSwitchConfig.ModbusConnectionConfig!.Name, automatic.ModbusConnectionConfig.Name);
            // GenericModbusConfig
            Assert.HasCount(1, config.GenericModbusConfigs);
            Assert.AreEqual(sample.GenericModbusConfig!.Name, config.GenericModbusConfigs.First().Name);
            Assert.AreEqual(sample.GenericModbusDeviceConfig!.Name, config.GenericModbusConfigs.First().GenericModbusDeviceConfig.Name);
            GenericModbusConfig generic = config.GenericModbusConfigs.First();
            Assert.AreEqual(sample.GenericModbusDeviceConfig.GenericModbusMapConfig!.Name, generic.GenericModbusDeviceConfig.GenericModbusMapConfig!.Name);
            Assert.AreEqual(sample.GenericModbusConfig.ModbusConnectionConfig!.Name, generic.ModbusConnectionConfig!.Name);
            // CircuitBreakerConfig
            CircuitBreakerConfig? breaker = group.DerCircuits.First().CircuitBreakerConfig;
            Assert.IsNotNull(breaker);
            Assert.AreEqual(sample.CircuitBreakerConfig!.Name, breaker.Name);
            Assert.AreEqual(sample.CircuitBreakerDeviceConfig!.Name, breaker.CircuitBreakerDeviceConfig.Name);
            Assert.AreEqual(sample.CircuitBreakerDeviceConfig.GenericModbusMapConfig!.Name, breaker.CircuitBreakerDeviceConfig.GenericModbusMapConfig!.Name);
            Assert.AreEqual(sample.CircuitBreakerConfig.ModbusConnectionConfig!.Name, breaker.ModbusConnectionConfig!.Name);
        }

    }
}
