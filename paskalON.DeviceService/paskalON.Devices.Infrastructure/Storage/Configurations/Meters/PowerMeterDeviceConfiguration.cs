// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Meters
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PowerMeterDeviceConfiguration : IEntityTypeConfiguration<PowerMeterDeviceConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PowerMeterDeviceConfig> builder)
        {
            builder.HasOne(x => x.PowerMeterMapC37Config)
                .WithMany()
                .HasForeignKey(x => x.PowerMeterMapC37ConfigId)
                .IsRequired(false);

            builder.HasOne(x => x.PowerMeterMapModbusConfig)
                .WithMany()
                .HasForeignKey(x => x.PowerMeterMapModbusConfigId)
                .IsRequired(false);

            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}
