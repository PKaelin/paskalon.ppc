// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Alarms.Domain.Configs.Actions;

namespace paskalON.Alarms.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class AlarmActionParameterConfigConfiguration : IEntityTypeConfiguration<AlarmActionParameterConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AlarmActionParameterConfig> builder)
        {
            builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Value).HasMaxLength(800).IsRequired();

            // The key is unique per alarm action
            builder.HasIndex(x => new { x.AlarmActionConfigId, x.Key }).IsUnique();
        }
    }
}
