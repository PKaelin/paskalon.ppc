// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.EnergyStorages.Batteries
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class BatteryBankConfiguration : IEntityTypeConfiguration<BatteryBankConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<BatteryBankConfig> builder)
        {
            builder.HasOne(x => x.BatteryBankDeviceConfig)
                .WithMany()
                .HasForeignKey(x => x.BatteryBankDeviceConfigId)
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
