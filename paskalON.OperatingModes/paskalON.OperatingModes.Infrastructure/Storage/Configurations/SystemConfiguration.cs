// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class SystemConfiguration : IEntityTypeConfiguration<SystemConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<SystemConfig> builder)
        {
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.ReferenceFrequency).IsRequired();
        }
    }
}
