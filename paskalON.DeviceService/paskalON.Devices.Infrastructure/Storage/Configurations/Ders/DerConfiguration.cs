// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Ders
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class DerConfiguration : IEntityTypeConfiguration<DerConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<DerConfig> builder)
        {
            builder.HasMany(x => x.DerGroupConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.GenericModbusConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.AutomaticTransferSwitchConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.SystemPowerMeterConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.AuxiliaryPowerMeterConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();

            builder.HasMany(x => x.ExternalPowerMeterConfigs)
                .WithOne(x => x.DerConfig)
                .HasForeignKey(x => x.DerConfigId)
                .IsRequired();
        }
    }
}
