// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Ders
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class DerSolarUnitConfiguration : IEntityTypeConfiguration<DerSolarUnitConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<DerSolarUnitConfig> builder)
        {
            builder.HasOne(x => x.PowerConversionSystemConfig)
                .WithOne(x => (DerSolarUnitConfig)x.DerUnitConfig)
                .HasForeignKey<PowerConversionSystemConfig>(b => b.DerUnitConfigId)
                .IsRequired();

            builder.HasOne(x => x.SolarPanelConfig)
                .WithOne(x => (DerSolarUnitConfig)x.DerUnitConfig)
                .HasForeignKey<SolarPanelConfig>(x => x.DerUnitConfigId)
                .IsRequired();

            builder.HasOne(x => x.DerContainerConfig)
                .WithOne(x => (DerSolarUnitConfig?)x.DerUnitConfig)
                .HasForeignKey<DerContainerConfig>(x => x.DerUnitConfigId)
                .IsRequired(false);

            builder.HasOne(x => x.GenericModbusUnitConfig)
                .WithOne(x => (DerSolarUnitConfig?)x.DerUnitConfig)
                .HasForeignKey<GenericModbusUnitConfig>(x => x.DerUnitConfigId)
                .IsRequired(false);
        }
    }
}
