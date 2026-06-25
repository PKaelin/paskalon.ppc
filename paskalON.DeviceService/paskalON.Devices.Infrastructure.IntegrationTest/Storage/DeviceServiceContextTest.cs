using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Infrastructure.IntegrationTest.Storage.SampleData;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.Devices.Infrastructure.IntegrationTest.Storage
{
    [TestClass]
    public class DeviceServiceContextTest
    {
        [TestMethod]
        public void CreateDeviceServiceContext()
        {
            using DeviceServiceContext context = new DeviceServiceContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }


        [TestMethod]
        public void CreateAllTest()
        {
            SimpleSet sample = new SimpleSet();

            using (DeviceServiceContext context = new DeviceServiceContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Core
                context.Configurations.Add(new Configuration { ChangedBy = "Test", Key = "Key", Value = "Value", Description = "Desc" });
                context.ModbusConnectionConfig.Add(sample.ModbusConnectionConfig!);

                // Maps
                context.PowerMeterMapC37Configs.Add(sample.PowerMeterMapC37Config!);
                context.PowerMeterMapModbusConfigs.Add(sample.PowerMeterMapModbusConfig!);
                context.GenericModbusMapConfigs.Add(sample.GenericModbusMapConfig!);

                // Devices
                context.PowerConversionSystemDeviceConfigs.Add(sample.PowerConversionSystemDeviceConfig!);
                context.BatteryBankDeviceConfigs.Add(sample.BatteryBankDeviceConfig!);
                context.SolarPanelDeviceConfigs.Add(sample.SolarPanelDeviceConfig!);
                context.PowerMeterDeviceConfigs.Add(sample.PowerMeterDeviceC37Config!);
                context.PowerMeterDeviceConfigs.Add(sample.PowerMeterDeviceModbusConfig!);
                context.GenericModbusDeviceConfigs.Add(sample.GenericModbusDeviceConfig!);
                context.CircuitBreakerDeviceConfigs.Add(sample.CircuitBreakerDeviceConfig!);
                context.AutomaticTransferSwitchDeviceConfigs.Add(sample.AutomaticTransferSwitchDeviceConfig!);

                // Configurations
                context.DerConfigs.Add(sample.DerConfig!);
                context.DerGroupConfigs.Add(sample.DerGroupConfig!);
                context.DerCircuitConfigs.Add(sample.DerCircuitBessConfig!);
                context.DerCircuitConfigs.Add(sample.DerCircuitPvConfig!);
                context.DerBatteryStorageUnitConfigs.Add(sample.DerBatteryStorageUnitConfig!);
                context.DerSolarUnitConfigs.Add(sample.DerSolarUnitConfig!);
                context.DerContainerConfigs.Add(sample.DerContainerConfig!);
                context.BatteryBankConfigs.Add(sample.BatteryBankConfig!);
                context.PowerConversionSystemConfigs.Add(sample.PowerConversionSystemBessConfig!);
                context.PowerConversionSystemConfigs.Add(sample.PowerConversionSystemPvConfig!);
                context.SolarPanelConfigs.Add(sample.SolarPanelConfig!);

                // Meters                
                context.PowerMeterC37Configs.Add(sample.SystemPowerMeterC37Config!);
                context.SystemPowerMeterConfigs.Add(sample.SystemPowerMeterConfig!);
                context.PowerMeterC37Configs.Add(sample.CircuitPowerMeterC37Config!);
                context.CircuitPowerMeterConfigs.Add(sample.CircuitPowerMeterConfig!);
                context.PowerMeterModbusConfigs.Add(sample.AuxiliaryPowerMeterModbusConfig!);
                context.AuxiliaryPowerMeterConfigs.Add(sample.AuxiliaryPowerMeterConfig!);
                context.PowerMeterModbusConfigs.Add(sample.ExternalPowerMeterModbusConfig!);
                context.ExternalPowerMeterConfigs.Add(sample.ExternalPowerMeterConfig!);

                // GMDs
                context.Add(sample.GenericModbusConfig!);
                context.Add(sample.CircuitBreakerConfig!);
                context.Add(sample.AutomaticTransferSwitchConfig!);

                context.SaveChanges();
            }

        }
    }
}
