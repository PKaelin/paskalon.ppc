using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    public class DeviceServiceContext : DbContext, IDeviceServiceContext
    {
        // Core DbSet
        public DbSet<Configuration> Configurations { get; set; }            // General configuration class for the microservice
        public DbSet<History> Histories { get; set; }                       // For DB migration history.


        // Communications
        public DbSet<C37Config> C37Configs { get; set; }
        public DbSet<ModbusConfig> ModbusConfigs { get; set; }
        public DbSet<ModbusConnectionConfig> ModbusConnectionConfigs { get; set; }


        // Maps DbSet
        public DbSet<PowerMeterMapC37Config> PowerMeterMapC37Configs { get; set; }
        public DbSet<PowerMeterMapModbusConfig> PowerMeterMapModbusConfigs { get; set; }
        public DbSet<GenericModbusMapConfig> GenericModbusMapConfigs { get; set; }
        public DbSet<ModbusRegisterMapEntryConfig> ModbusRegisterMapEntryConfigs { get; set; }
        public DbSet<ModbusRegisterMapPollingRangeConfig> ModbusRegisterMapPollingRangeConfigs { get; set; }
        public DbSet<GenericModbusCoilMapEntryConfig> GenericModbusCoilInputMapConfigs { get; set; }
        public DbSet<GenericModbusDiscreteInputMapEntryConfig> GenericModbusDiscreteInputMapEntryConfigs { get; set; }
        public DbSet<GenericModbusHoldingRegisterMapEntryConfig> GenericModbusHoldingRegisterMapConfigs { get; set; }
        public DbSet<GenericModbusInputRegisterMapEntryConfig> GenericModbusInputRegisterMapConfigs { get; set; }


        // Devices DbSet
        public DbSet<PowerConversionSystemDeviceConfig> PowerConversionSystemDeviceConfigs { get; set; }
        public DbSet<BatteryBankDeviceConfig> BatteryBankDeviceConfigs { get; set; }
        public DbSet<SolarPanelDeviceConfig> SolarPanelDeviceConfigs { get; set; }
        public DbSet<GenericModbusDeviceConfig> GenericModbusDeviceConfigs { get; set; }
        public DbSet<CircuitBreakerDeviceConfig> CircuitBreakerDeviceConfigs { get; set; }
        public DbSet<AutomaticTransferSwitchDeviceConfig> AutomaticTransferSwitchDeviceConfigs { get; set; }
        public DbSet<PowerMeterDeviceConfig> PowerMeterDeviceConfigs { get; set; }


        // Configurations DbSet        
        public DbSet<DerConfig> DerConfigs { get; set; }
        public DbSet<DerGroupConfig> DerGroupConfigs { get; set; }
        public DbSet<DerCircuitConfig> DerCircuitConfigs { get; set; }
        public DbSet<DerBatteryStorageUnitConfig> DerBatteryStorageUnitConfigs { get; set; }
        public DbSet<DerSolarUnitConfig> DerSolarUnitConfigs { get; set; }
        public DbSet<DerContainerConfig> DerContainerConfigs { get; set; }
        public DbSet<BatteryBankConfig> BatteryBankConfigs { get; set; }
        public DbSet<PowerConversionSystemConfig> PowerConversionSystemConfigs { get; set; }
        public DbSet<SolarPanelConfig> SolarPanelConfigs { get; set; }


        // Meters DbSet
        public DbSet<SystemPowerMeterConfig> SystemPowerMeterConfigs { get; set; }
        public DbSet<CircuitPowerMeterConfig> CircuitPowerMeterConfigs { get; set; }
        public DbSet<AuxiliaryPowerMeterConfig> AuxiliaryPowerMeterConfigs { get; set; }
        public DbSet<ExternalPowerMeterConfig> ExternalPowerMeterConfigs { get; set; }


        // GMDs DbSet
        public DbSet<GenericModbusConfig> GenericModbusConfigs { get; set; }
        public DbSet<CircuitBreakerConfig> CircuitBreakerConfigs { get; set; }
        public DbSet<AutomaticTransferSwitchConfig> AutomaticTransferSwitchConfigs { get; set; }



        /// <summary>
        /// Set defaults and configure conventions before they run.
        /// </summary>
        /// <param name="configurationBuilder">Configuration builder instance.</param>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Use singular table names instead of plural
            configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
            base.ConfigureConventions(configurationBuilder);
        }


        /// <summary>
        /// Configure the model that was discovered by convention from the entity types exposed
        /// in Microsoft.EntityFrameworkCore.DbSet properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">Model builder instance <see cref="ModelBuilder"/>.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<History>().ToTable(t => t.ExcludeFromMigrations(true));
            base.OnModelCreating(modelBuilder);
            // Automatically pulls all individual configuration classes
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainBase).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeviceServiceContext).Assembly);
        }


        /// <summary>
        /// Configure the database (and other options) to be used for this context.
        /// </summary>
        /// <param name="optionsBuilder">Options builder instance.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string variable = "DB_CONNECTION_STRING";
            string? connectionString = Environment.GetEnvironmentVariable(variable);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, variable);
            optionsBuilder.UseNpgsql(connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
