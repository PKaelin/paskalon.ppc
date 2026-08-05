// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using paskalON.Domains;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.EnergyStorages;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Curves;
using paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Infrastructure.Storage
{
    public class OperatingModeContext : DbContext, IOperatingModeContext
    {
        // Core DbSet
        public DbSet<Configuration> Configurations { get; set; }            // General configuration class for the microservice
        public DbSet<History> Histories { get; set; }                       // For DB migration history.
        public DbSet<SystemConfig> SystemConfigs { get; set; }

        // Ramps
        public DbSet<RampRateConfig> RampRateConfigs { get; set; }
        public DbSet<RampRatePercentageConfig> RampRatePercentageConfigs { get; set; }
        public DbSet<RampTimeConfig> RampTimeConfigs { get; set; }
        public DbSet<RampTimeConstantConfig> RampTimeConstantConfigs { get; set; }

        // Curves
        public DbSet<CurvePointConfig> CurvePointConfigs { get; set; }
        public DbSet<VoltWattCurveConfig> VoltWattCurveConfigs { get; set; }
        public DbSet<VoltVarCurveConfig> VoltVarCurveConfigs { get; set; }
        public DbSet<FrequencyWattCurveConfig> FrequencyWattCurveConfigs { get; set; }

        // Operating closed modes
        public DbSet<MaintenanceSocModeConfig> MaintenanceSocModeConfigs { get; set; }
        // Operating closed modes Energy Storages
        public DbSet<ChargeDischargeModeConfig> ChargeDischargeModeConfigs { get; set; }
        public DbSet<CoordinatedChargeDischargeModeConfig> CoordinatedChargeDischargeModeConfigs { get; set; }
        // Operating closed modes Frequency Actives
        public DbSet<ActivePowerModeConfig> ActivePowerModeConfigs { get; set; }
        public DbSet<FrequencyDroopModeConfig> FrequencyDroopModeConfigs { get; set; }
        public DbSet<FrequencyWattModeConfig> FrequencyWattModeConfigs { get; set; }
        public DbSet<MaximumActivePowerLimitModeConfig> MaximumActivePowerLimitModeConfigs { get; set; }
        // Operating closed modes Voltage Actives
        public DbSet<VoltageWattDroopModeConfig> VoltageWattDroopModeConfigs { get; set; }
        // Operating closed modes Voltage Reactives
        public DbSet<PowerFactorModeConfig> PowerFactorModeConfigs { get; set; }
        public DbSet<ReactivePowerModeConfig> ReactivePowerModeConfigs { get; set; }
        public DbSet<VoltageModeConfig> VoltageModeConfigs { get; set; }
        public DbSet<VoltageVarDroopModeConfig> VoltageVarDroopModeConfigs { get; set; }

        // Operating open modes
        public DbSet<MaintenanceModeConfig> MaintenanceModeConfigs { get; set; }
        // Operating open modes Energy Storages
        public DbSet<MaximumPowerPointTrackingModeConfig> MaximumPowerPointTrackingModeConfigs { get; set; }
        // Operating open modes Frequency Actives
        public DbSet<ActivePowerFixedModeConfig> ActivePowerFixedModeConfigs { get; set; }
        // Operating open modes Voltage Reactives
        public DbSet<ReactivePowerFixedModeConfig> ReactivePowerFixedModeConfigs { get; set; }


        /// <summary>
        /// Set defaults and configure conventions before they run.
        /// </summary>
        /// <param name="configurationBuilder">Configuration builder instance.</param>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
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
            // Configure inheritance mapping in the model configurations (see: ConfigurationBaseConfiguration)
            // Table-per-Hierarchy (TPH), Table-per-Type (TPT), Table-per-Concrete-type (TPC)
            modelBuilder.Entity<History>().ToTable(t => t.ExcludeFromMigrations(true));
            base.OnModelCreating(modelBuilder);
            // Automatically pulls all individual configuration classes
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainBase).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OperatingModeContext).Assembly);
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
