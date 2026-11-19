// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.Sequences
{
    /// <summary>
    /// Source instance, sequence number and creation time to stamp a <see cref="ISequencedMessage"/> with.
    /// </summary>
    public readonly struct MessageStamp
    {
        /// <summary>
        /// Unique identifier of the publishing service instance.
        /// </summary>
        public string SourceInstance { get; }


        /// <summary>
        /// Sequence number of the message.
        /// </summary>
        public long Sequence { get; }


        /// <summary>
        /// Creation time of the message in UTC.
        /// </summary>
        public DateTimeOffset Timestamp { get; }


        /// <summary>
        /// Constructor of <see cref="MessageStamp"/>.
        /// </summary>
        /// <param name="sourceInstance">Unique identifier of the publishing service instance.</param>
        /// <param name="sequence">Sequence number of the message.</param>
        /// <param name="timestamp">Creation time of the message in UTC.</param>
        public MessageStamp(string sourceInstance, long sequence, DateTimeOffset timestamp)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceInstance);

            SourceInstance = sourceInstance;
            Sequence = sequence;
            Timestamp = timestamp;
        }
    }
}
