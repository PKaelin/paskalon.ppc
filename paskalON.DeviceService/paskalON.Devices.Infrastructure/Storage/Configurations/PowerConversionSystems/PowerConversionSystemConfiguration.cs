// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.PowerConversionSystems
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PowerConversionSystemConfiguration : IEntityTypeConfiguration<PowerConversionSystemConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PowerConversionSystemConfig> builder)
        {
            builder.HasOne(x => x.PowerConversionSystemDeviceConfig)
                .WithMany()
                .HasForeignKey(x => x.PowerConversionSystemDeviceConfigId)
                .IsRequired();

            // TODO: Implement clean relationship
            builder.HasOne(x => x.ModbusConfig)
                .WithMany()
                .HasForeignKey(x => x.ModbusConfigId)
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}
