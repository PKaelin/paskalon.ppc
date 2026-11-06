// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Application.Outboxes
{
    /// <summary>
    /// Sends the pending alarm action messages of the outbox to the message broker.
    /// </summary>
    public interface IAlarmActionOutboxSender
    {
        /// <summary>
        /// Sends a batch of the oldest pending messages in order and marks them as sent once the broker confirmed them.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the batch.</returns>
        Task<AlarmActionOutboxSendResult> SendPendingAsync(CancellationToken cancellationToken = default);
    }
}
