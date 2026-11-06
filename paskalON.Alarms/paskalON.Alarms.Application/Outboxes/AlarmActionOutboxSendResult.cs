// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Application.Outboxes
{
    /// <summary>
    /// Result of sending the pending alarm action messages.
    /// </summary>
    public record AlarmActionOutboxSendResult
    {
        /// <summary>
        /// Number of messages sent and confirmed by the message broker.
        /// </summary>
        public required int SentCount { get; init; }


        /// <summary>
        /// Indicates whether sending stopped because a message failed, the caller should wait before sending again.
        /// </summary>
        public required bool IsFailed { get; init; }


        /// <summary>
        /// Indicates whether more pending messages might exist (the batch was full), the caller can send again immediately.
        /// </summary>
        public required bool HasMore { get; init; }
    }
}
