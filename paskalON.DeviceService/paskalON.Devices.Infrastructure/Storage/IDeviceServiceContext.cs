using Microsoft.EntityFrameworkCore;
using paskalON.Communication.Protocols.Modbus.Configurations;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Domains;

namespace paskalON.Devices.Infrastructure.Storage
{
    public interface IDeviceServiceContext
    {
        // Core DbSet
        DbSet<Configuration> Configurations { get; set; }            // General configuration class for the microservice
        DbSet<History> Histories { get; set; }                       // For DB migration history.


        // Communications
        DbSet<C37Config> C37Configs { get; set; }
        DbSet<ModbusConfig> ModbusConfigs { get; set; }
        DbSet<ModbusConnectionConfig> ModbusConnectionConfigs { get; set; }


        // Maps DbSet
        DbSet<PowerMeterMapC37Config> PowerMeterMapC37Configs { get; set; }
        DbSet<PowerMeterMapModbusConfig> PowerMeterMapModbusConfigs { get; set; }
        DbSet<GenericModbusMapConfig> GenericModbusMapConfigs { get; set; }
        DbSet<ModbusRegisterMapEntryConfig> ModbusRegisterMapEntryConfigs { get; set; }
        DbSet<ModbusRegisterMapPollingRangeConfig> ModbusRegisterMapPollingRangeConfigs { get; set; }
        DbSet<GenericModbusCoilPointConfig> GenericModbusCoilPointConfigs { get; set; }
        DbSet<GenericModbusDiscreteInputPointConfig> GenericModbusDiscreteInputPointConfigs { get; set; }
        DbSet<GenericModbusHoldingRegisterConfig> GenericModbusHoldingRegisterConfigs { get; set; }
        DbSet<GenericModbusInputRegisterConfig> GenericModbusInputRegisterConfigs { get; set; }


        // Devices DbSet
        DbSet<PowerConversionSystemDeviceConfig> PowerConversionSystemDeviceConfigs { get; set; }
        DbSet<BatteryBankDeviceConfig> BatteryBankDeviceConfigs { get; set; }
        DbSet<SolarPanelDeviceConfig> SolarPanelDeviceConfigs { get; set; }
        DbSet<GenericModbusDeviceConfig> GenericModbusDeviceConfigs { get; set; }
        DbSet<CircuitBreakerDeviceConfig> CircuitBreakerDeviceConfigs { get; set; }
        DbSet<AutomaticTransferSwitchDeviceConfig> AutomaticTransferSwitchDeviceConfigs { get; set; }
        DbSet<PowerMeterDeviceConfig> PowerMeterDeviceConfigs { get; set; }


        // Configurations DbSet        
        DbSet<DerConfig> DerConfigs { get; set; }
        DbSet<DerGroupConfig> DerGroupConfigs { get; set; }
        DbSet<DerCircuitConfig> DerCircuitConfigs { get; set; }
        DbSet<DerBatteryStorageUnitConfig> DerBatteryStorageUnitConfigs { get; set; }
        DbSet<DerSolarUnitConfig> DerSolarUnitConfigs { get; set; }
        DbSet<DerContainerConfig> DerContainerConfigs { get; set; }
        DbSet<BatteryBankConfig> BatteryBankConfigs { get; set; }
        DbSet<PowerConversionSystemConfig> PowerConversionSystemConfigs { get; set; }
        DbSet<SolarPanelConfig> SolarPanelConfigs { get; set; }


        // Meters DbSet
        DbSet<SystemPowerMeterConfig> SystemPowerMeterConfigs { get; set; }
        DbSet<CircuitPowerMeterConfig> CircuitPowerMeterConfigs { get; set; }
        DbSet<AuxiliaryPowerMeterConfig> AuxiliaryPowerMeterConfigs { get; set; }
        DbSet<ExternalPowerMeterConfig> ExternalPowerMeterConfigs { get; set; }


        // GMDs DbSet
        DbSet<GenericModbusConfig> GenericModbusConfigs { get; set; }
        DbSet<CircuitBreakerConfig> CircuitBreakerConfigs { get; set; }
        DbSet<AutomaticTransferSwitchConfig> AutomaticTransferSwitchConfigs { get; set; }

        /// <summary>
        /// Save changes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task result contains the number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
