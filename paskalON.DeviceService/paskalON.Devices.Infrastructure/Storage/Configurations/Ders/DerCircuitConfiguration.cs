// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Ders
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class DerCircuitConfiguration : IEntityTypeConfiguration<DerCircuitConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<DerCircuitConfig> builder)
        {
            builder.HasMany(x => x.DerUnitConfigs)
                .WithOne(x => x.DerCircuitConfig)
                .HasForeignKey(x => x.DerCircuitConfigId)
                .IsRequired();

            builder.HasOne(x => x.CircuitBreakerConfig)
                .WithOne(x => x.DerCircuitConfig)
                .HasForeignKey<CircuitBreakerConfig>(x => x.DerCircuitConfigId)
                .IsRequired();

            builder.HasOne(x => x.CircuitPowerMeterConfig)
                .WithOne(x => x.DerCircuitConfig)
                .HasForeignKey<CircuitPowerMeterConfig>(x => x.DerCircuitConfigId)
                .IsRequired();
        }
    }
}
