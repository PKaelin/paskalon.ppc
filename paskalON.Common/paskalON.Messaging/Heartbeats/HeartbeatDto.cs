// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Messaging.Sequences;

namespace paskalON.Messaging.Heartbeats
{
    /// <summary>
    /// Heartbeat message that a service publishes periodically to signal that it is alive.
    /// </summary>
    public sealed class HeartbeatDto : ISequencedMessage
    {
        /// <summary>
        /// Name of the publishing service, e.g. PowerControl.
        /// </summary>
        public required string ServiceName { get; init; }


        /// <inheritdoc/>
        public required string SourceInstance { get; init; }


        /// <inheritdoc/>
        public long Sequence { get; init; }


        /// <inheritdoc/>
        public DateTimeOffset Timestamp { get; init; }
    }
}
