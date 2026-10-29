// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.Sequences
{
    /// <summary>
    /// Defines a message that carries its source instance, sequence number and creation time.
    /// </summary>
    /// <remarks>
    /// Receivers use the source instance and sequence to drop outdated or duplicated messages, e.g. after the message broker reconnected.
    /// </remarks>
    public interface ISequencedMessage
    {
        /// <summary>
        /// Unique identifier of the publishing service instance.
        /// </summary>
        /// <remarks>
        /// A restarted service instance gets a new identifier, so that its sequence starts again without being dropped.
        /// </remarks>
        string SourceInstance { get; }


        /// <summary>
        /// Sequence number of the message, increasing with every message of the source instance.
        /// </summary>
        long Sequence { get; }


        /// <summary>
        /// Creation time of the message in UTC.
        /// </summary>
        /// <remarks>
        /// Informational only, since the clocks of different nodes are not guaranteed to be in sync. Liveness is based on the receive time.
        /// </remarks>
        DateTimeOffset Timestamp { get; }
    }
}
