// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Alarms.Domain.Configs;

namespace paskalON.Alarms.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class AlarmConfigBaseConfiguration : IEntityTypeConfiguration<AlarmConfigBase>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AlarmConfigBase> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
            builder.Property(x => x.IsEnabled).IsRequired();
            builder.Property(x => x.AlarmId).HasMaxLength(100).IsRequired();
            builder.Property(x => x.SeverityLevel).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(250).IsRequired();
            builder.Property(x => x.PossibleCauses).HasMaxLength(250);
            builder.Property(x => x.Response).HasMaxLength(250);
            builder.Property(x => x.Consequences).HasMaxLength(250);
            builder.Property(x => x.TimeBeforeRaisingAlarmMilliseconds).IsRequired();

            builder.HasMany(x => x.AlarmActions)
                .WithOne(x => x.AlarmConfigBase)
                .HasForeignKey(x => x.AlarmConfigBaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
