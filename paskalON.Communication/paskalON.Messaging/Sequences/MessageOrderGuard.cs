// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.Sequences
{
    /// <summary>
    /// Accepts only messages that are newer than the last accepted message of the same source instance.
    /// </summary>
    /// <remarks>
    /// Thread safe. Use one guard per subscribed message stream (topic).
    /// A message of another source instance is always accepted, e.g. after a failover or restart of the publisher.
    /// If two primary instances publish concurrently, messages of both are accepted.
    /// </remarks>
    public sealed class MessageOrderGuard
    {
        /// <summary>
        /// Lock object to protect the last accepted source instance and sequence.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Source instance of the last accepted message.
        /// </summary>
        private string? _lastSourceInstance;


        /// <summary>
        /// Sequence number of the last accepted message.
        /// </summary>
        private long _lastSequence;


        /// <summary>
        /// Checks whether the message is newer than the last accepted message and if so, accepts it.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <returns>True if the message is accepted, false if it is outdated or a duplicate.</returns>
        public bool TryAccept(ISequencedMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);

            lock (_dataLock)
            {
                bool isSameSource = string.Equals(_lastSourceInstance, message.SourceInstance, StringComparison.Ordinal);

                if (isSameSource && message.Sequence <= _lastSequence)
                {
                    return false;
                }

                _lastSourceInstance = message.SourceInstance;
                _lastSequence = message.Sequence;

                return true;
            }
        }
    }
}
