// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.GenericModbusDevices
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class GenericModbusConfiguration : IEntityTypeConfiguration<GenericModbusConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<GenericModbusConfig> builder)
        {
            builder.HasOne(x => x.GenericModbusDeviceConfig)
                .WithMany()
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}
