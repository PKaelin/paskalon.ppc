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
    public class AlarmActionConfigConfiguration : IEntityTypeConfiguration<AlarmActionConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AlarmActionConfig> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Trigger).IsRequired();

            builder.HasMany(x => x.AlarmActionParameters)
                .WithOne(x => x.AlarmActionConfig)
                .HasForeignKey(x => x.AlarmActionConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
