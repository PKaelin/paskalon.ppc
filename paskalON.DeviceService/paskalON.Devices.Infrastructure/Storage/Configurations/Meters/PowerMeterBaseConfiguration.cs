// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Meters
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PowerMeterBaseConfiguration : IEntityTypeConfiguration<PowerMeterBaseConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PowerMeterBaseConfig> builder)
        {
            builder.HasOne(x => x.PowerMeterDeviceConfig)
                .WithMany()
                .HasForeignKey(x => x.PowerMeterDeviceConfigId)
                .IsRequired();

            builder.HasOne(x => x.ModbusConfig)
                .WithMany()
                .HasForeignKey(x => x.ModbusConfigId)
                .IsRequired(false);

            builder.HasOne(x => x.C37Config)
                .WithMany()
                .HasForeignKey(x => x.C37ConfigId)
                .IsRequired(false);

            builder.Property(x => x.PowerFactorStandard).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}
