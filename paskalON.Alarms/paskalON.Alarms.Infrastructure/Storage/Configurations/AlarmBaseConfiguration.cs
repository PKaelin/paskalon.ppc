// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Alarms.Domain;

namespace paskalON.Alarms.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class AlarmBaseConfiguration : IEntityTypeConfiguration<AlarmBase>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AlarmBase> builder)
        {
            builder.Property(x => x.CreatedOn).IsRequired();
            builder.Property(x => x.AlarmSequenceState).IsRequired();
            builder.Property(x => x.AcknowledgedBy).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.AcknowledgedOn).IsRequired(false);

            // Calculated by the domain, not persisted
            builder.Ignore(x => x.SeverityLevel);
            builder.Ignore(x => x.IsAcknowledgeable);
            builder.Ignore(x => x.CanBeAcknowledged);
            builder.Ignore(x => x.IsActive);
        }
    }
}
