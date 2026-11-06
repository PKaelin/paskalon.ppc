// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Domains;

namespace paskalON.Alarms.Infrastructure.Storage.Outboxes
{
    /// <summary>
    /// Alarm action message of the transactional outbox.
    /// </summary>
    /// <remarks>
    /// The message is saved in the same transaction as the alarm that triggered the action,
    /// so that no action gets lost when the message broker is not available.
    /// A sender sends the pending messages to the message broker and marks them as sent once the broker confirmed them.
    /// The payload is a snapshot of the action (placeholders replaced) at the time the alarm changed,
    /// later changes or deletes of the alarm action configuration do not affect the message.
    /// The message is a technical delivery log, it is deleted:
    /// - after the outbox retention when it was sent (cleanup job, not implemented yet), or
    /// - by the database together with its alarm (cascade, e.g. archival after AlarmArchivalMonths).
    /// </remarks>
    public class AlarmActionOutboxMessage : DomainBase
    {
        /// <summary>
        /// Unique identifier of the message, also contained in the payload so that recipients can ignore duplicated deliveries.
        /// </summary>
        public required Guid MessageId { get; set; }


        /// <summary>
        /// Id of the alarm that triggered the action.
        /// </summary>
        public int PpcAlarmId { get; set; }


        /// <summary>
        /// Alarm that triggered the action.
        /// </summary>
        public required PpcAlarm PpcAlarm { get; set; }


        /// <summary>
        /// Name of the action, e.g. ReducePower.
        /// </summary>
        public required string ActionName
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Serialized JSON alarm action that is sent to the message broker.
        /// </summary>
        public required string Payload
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Timestamp when the message was created.
        /// </summary>
        public required DateTimeOffset CreatedOn
        {
            get => field;
            set => field = value.ToUniversalTime();
        }


        /// <summary>
        /// Timestamp when the message broker confirmed the message, null while the message is pending.
        /// </summary>
        public DateTimeOffset? SentOn
        {
            get => field;
            set => field = value?.ToUniversalTime();
        }


        /// <summary>
        /// Number of failed attempts to send the message.
        /// </summary>
        public int FailedAttempts
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }


        /// <summary>
        /// Error of the last failed attempt.
        /// </summary>
        public string? LastError { get; set; }
    }
}
