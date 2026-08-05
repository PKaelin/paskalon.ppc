// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.EnergyResources.Solars
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class SolarPanelDeviceConfiguration : IEntityTypeConfiguration<SolarPanelDeviceConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<SolarPanelDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}
