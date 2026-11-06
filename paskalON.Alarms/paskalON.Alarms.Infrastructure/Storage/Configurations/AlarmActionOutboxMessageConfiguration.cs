// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;

namespace paskalON.Alarms.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class AlarmActionOutboxMessageConfiguration : IEntityTypeConfiguration<AlarmActionOutboxMessage>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AlarmActionOutboxMessage> builder)
        {
            builder.Property(x => x.MessageId).IsRequired();
            builder.Property(x => x.ActionName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Payload).HasColumnType("jsonb").IsRequired();
            builder.Property(x => x.CreatedOn).IsRequired();
            builder.Property(x => x.SentOn).IsRequired(false);
            builder.Property(x => x.FailedAttempts).IsRequired();
            builder.Property(x => x.LastError).HasMaxLength(2000);

            // The outbox messages are a technical delivery log of the alarm,
            // they are deleted with their alarm (e.g. archival after AlarmArchivalMonths)
            builder.HasOne(x => x.PpcAlarm)
                .WithMany()
                .HasForeignKey(x => x.PpcAlarmId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.MessageId).IsUnique();
            // The sender reads the pending messages in creation order
            builder.HasIndex(x => x.CreatedOn)
                .HasFilter($"\"{nameof(AlarmActionOutboxMessage.SentOn)}\" IS NULL")
                .HasDatabaseName("IX_AlarmActionOutboxMessage_Pending");
        }
    }
}
