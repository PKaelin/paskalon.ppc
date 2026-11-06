// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PpcAlarmConfiguration : IEntityTypeConfiguration<PpcAlarm>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PpcAlarm> builder)
        {
            builder.Property(x => x.DeviceId).IsRequired(false);
            builder.Property(x => x.DeviceName).HasMaxLength(250);
            builder.Property(x => x.PropertyName).HasMaxLength(250).IsRequired();

            // Keep the alarm history, a configuration with alarms cannot be deleted (disable it instead)
            builder.HasOne(x => x.PpcAlarmConfig)
                .WithMany()
                .HasForeignKey(x => x.PpcAlarmConfigId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Only one active (not normalized) alarm per device, property and configuration.
            // NULLS NOT DISTINCT so that also system alarms (without device) are unique.
            builder.HasIndex(x => new { x.DeviceId, x.PropertyName, x.PpcAlarmConfigId })
                .IsUnique()
                .AreNullsDistinct(false)
                .HasFilter($"\"{nameof(PpcAlarm.AlarmSequenceState)}\" <> {(int)AlarmSequenceState.Normal}")
                .HasDatabaseName("IX_PpcAlarm_Active");
            // Get the active alarms
            builder.HasIndex(x => x.AlarmSequenceState);
        }
    }
}
