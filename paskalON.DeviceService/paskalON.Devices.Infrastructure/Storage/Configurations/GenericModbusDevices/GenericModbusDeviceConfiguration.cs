// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.GenericModbusDevices
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class GenericModbusDeviceConfiguration : IEntityTypeConfiguration<GenericModbusDeviceConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<GenericModbusDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}
