// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Infrastructure.Storage.Outboxes;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Alarm action outbox repository interface definition.
    /// </summary>
    /// <remarks>
    /// Lifecycle of an outbox message (see <see cref="AlarmActionOutboxMessage"/>):
    /// 1. Created: added with its raised or normalized alarm and saved in the same transaction (<see cref="Add"/>).
    /// 2. Sent: the sender publishes the pending messages to the message broker and sets SentOn once the broker confirmed them.
    /// 3. Deleted, there is intentionally no delete of a single message:
    ///    - Purge: sent messages are deleted after the configured outbox retention (cleanup job, not implemented yet).
    ///    - Cascade: the messages of an alarm are deleted by the database when the alarm is deleted (archival after AlarmArchivalMonths).
    /// Deleting or changing an alarm action configuration does not affect the messages, the payload is a snapshot of the action.
    /// </remarks>
    public interface IAlarmActionOutboxRepository
    {
        /// <summary>
        /// Adds alarm action messages to the outbox.
        /// </summary>
        /// <remarks>
        /// The messages are saved with <see cref="IPpcAlarmRepository.SaveChangesAsync"/> in the same transaction as their alarms,
        /// because both repositories share the same (scoped) alarm context.
        /// </remarks>
        /// <param name="messages">Alarm action messages.</param>
        void Add(IEnumerable<AlarmActionOutboxMessage> messages);


        /// <summary>
        /// Gets the oldest pending (not sent) messages in creation order.
        /// </summary>
        /// <param name="take">Maximum number of messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tracked pending messages, changes (sent or failed) are saved with <see cref="SaveChangesAsync"/>.</returns>
        Task<List<AlarmActionOutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken = default);


        /// <summary>
        /// Saves the changes of the tracked messages (sent or failed attempts).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
