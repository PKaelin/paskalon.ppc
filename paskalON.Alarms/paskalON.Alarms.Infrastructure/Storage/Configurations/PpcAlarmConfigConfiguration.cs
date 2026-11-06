// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Alarms.Domain.Configs.Ppcs;

namespace paskalON.Alarms.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PpcAlarmConfigConfiguration : IEntityTypeConfiguration<PpcAlarmConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PpcAlarmConfig> builder)
        {
            builder.Property(x => x.PpcAlarmType).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired(false);
            builder.Property(x => x.PropertyName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.PropertyOperator).IsRequired();
            builder.Property(x => x.PropertyValue).IsRequired();

            // The evaluator loads the configurations per publisher (alarm type)
            builder.HasIndex(x => new { x.PpcAlarmType, x.DeviceId, x.PropertyName });
        }
    }
}
