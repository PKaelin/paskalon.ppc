// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.PowerConversionSystems
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PowerConversionSystemDeviceConfiguration : IEntityTypeConfiguration<PowerConversionSystemDeviceConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PowerConversionSystemDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
            builder.Property(x => x.NameplateMaximumActivePower).IsRequired();
            builder.Property(x => x.NameplateMaximumReactivePower).IsRequired();
            builder.Property(x => x.NameplateMaximumApparentPower).IsRequired();

            builder.HasMany(x => x.Customs)
                .WithOne(x => x.PowerConversionSystemDeviceConfig)
                .HasForeignKey(x => x.PowerConversionSystemDeviceConfigId)
                .IsRequired();
        }
    }
}
