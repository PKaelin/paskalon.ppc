// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.EnergyStorages.Batteries
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class BatteryBankDeviceConfiguration : IEntityTypeConfiguration<BatteryBankDeviceConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<BatteryBankDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();

            builder.HasMany(x => x.Customs)
                .WithOne(x => x.BatteryBankDeviceConfig)
                .HasForeignKey(x => x.BatteryBankDeviceConfigId)
                .IsRequired();
        }
    }
}
