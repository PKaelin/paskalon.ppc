// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Domains;

namespace paskalON.Alarms.Infrastructure.Storage
{
    /// <summary>
    /// Alarm service database context.
    /// </summary>
    public class AlarmContext : DbContext, IAlarmContext
    {
        // Core DbSet
        /// <inheritdoc/>
        public DbSet<Configuration> Configurations { get; set; }    // General configuration class for the microservice
        /// <inheritdoc/>
        public DbSet<History> Histories { get; set; }               // For DB migration history.


        /// <inheritdoc/>
        public DbSet<PpcAlarmConfig> PpcAlarmConfigs { get; set; }


        /// <inheritdoc/>
        public DbSet<AlarmActionConfig> AlarmActionConfigs { get; set; }


        /// <inheritdoc/>
        public DbSet<AlarmActionParameterConfig> AlarmActionParameterConfigs { get; set; }


        /// <inheritdoc/>
        public DbSet<PpcAlarm> PpcAlarms { get; set; }


        /// <inheritdoc/>
        public DbSet<AlarmActionOutboxMessage> AlarmActionOutboxMessages { get; set; }


        /// <summary>
        /// Constructor of <see cref="AlarmContext"/>.
        /// </summary>
        /// <param name="options">DbContextOptions</param>
        public AlarmContext(DbContextOptions<AlarmContext> options) : base(options)
        {
        }


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
            // Use a non-deterministic ICU collation to be case-insensitive text
            modelBuilder.HasCollation("case_insensitive_collation", locale: "und-u-ks-level2", provider: "icu", deterministic: false);
            // Configure inheritance mapping in the model configurations (see: DomainBaseConfiguration)
            // Table-per-Hierarchy (TPH), Table-per-Type (TPT), Table-per-Concrete-type (TPC)
            modelBuilder.Entity<History>().ToTable(t => t.ExcludeFromMigrations(true));
            base.OnModelCreating(modelBuilder);
            // Automatically pulls all individual configuration classes
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainBase).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AlarmContext).Assembly);
            // No alarm entity derives from NameBase (names are not unique), remove it after the common configurations added it
            modelBuilder.Ignore<NameBase>();
        }
    }
}
