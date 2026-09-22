// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.Domains;
using paskalON.PowerControls.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;

namespace paskalON.PowerControls.Infrastructure.Storage
{
    public class PowerControlContext : DbContext, IPowerControlContext
    {
        // Core DbSet
        public DbSet<Configuration> Configurations { get; set; }            // General configuration class for the microservice
        public DbSet<History> Histories { get; set; }                       // For DB migration history.
        public DbSet<SystemConfig> SystemConfigs { get; set; }

        //-----------------------------------------------------------------------------------------
        // Constraints
        //-----------------------------------------------------------------------------------------
        // System constraints
        public DbSet<SystemPowerConstraintConfig> SystemPowerConstraintConfigs { get; set; }
        public DbSet<SystemRampConstraintConfig> SystemRampConstraintConfigs { get; set; }

        // DER constraints
        public DbSet<DerUnitPowerConstraintConfig> DerUnitPowerConstraintConfigs { get; set; }
        public DbSet<DerUnitRampConstraintConfig> DerUnitRampConstraintConfig { get; set; }


        //-----------------------------------------------------------------------------------------
        // Power Control
        //-----------------------------------------------------------------------------------------
        // System power control
        public DbSet<SystemPowerControlConfig> SystemPowerControlConfigs { get; set; }

        // DER power control
        public DbSet<DerUnitPowerControlConfig> DerUnitPowerControlConfigs { get; set; }
        public DbSet<DerUnitEnergyStoragePowerControlConfig> DerUnitEnergyStoragePowerControlConfigs { get; set; }


        /// <summary>
        /// Constructor of <see cref="PowerControlContext"/>.
        /// </summary>
        /// <param name="options">DbContextOptions</param>
        public PowerControlContext(DbContextOptions<PowerControlContext> options) : base(options)
        {
        }


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
            // Configure inheritance mapping in the model configurations (see: ConfigurationBaseConfiguration)
            // Table-per-Hierarchy (TPH), Table-per-Type (TPT), Table-per-Concrete-type (TPC)
            modelBuilder.Entity<History>().ToTable(t => t.ExcludeFromMigrations(true));
            base.OnModelCreating(modelBuilder);
            // Automatically pulls all individual configuration classes
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainBase).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PowerControlContext).Assembly);
        }
    }
}
