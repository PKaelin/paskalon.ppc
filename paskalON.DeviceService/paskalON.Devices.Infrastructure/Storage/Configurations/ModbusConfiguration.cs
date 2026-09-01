// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs;

namespace paskalON.Devices.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class ModbusConfiguration : IEntityTypeConfiguration<ModbusConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ModbusConfig> builder)
        {
            builder.HasOne(x => x.ModbusConnectionConfig)
                .WithMany()
                .HasForeignKey(x => x.ModbusConnectionConfigId)
                .IsRequired();

            builder.Property(x => x.Address).IsRequired();
            builder.Property(x => x.Port).IsRequired();
            builder.Property(x => x.AddressFamily).IsRequired();
            builder.Property(x => x.StationId).IsRequired();
        }
    }
}
