// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.Sequences
{
    /// <summary>
    /// Creates the <see cref="MessageStamp"/> with an increasing sequence number for the messages of one service instance.
    /// </summary>
    /// <remarks>
    /// Thread safe. Use one sequencer per published message stream (topic), so that the receiver sees a gapless sequence.
    /// </remarks>
    public sealed class MessageSequencer
    {
        /// <summary>
        /// Time provider to get the creation time of the messages.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Sequence number of the last created stamp.
        /// </summary>
        private long _sequence;


        /// <summary>
        /// Unique identifier of the publishing service instance.
        /// </summary>
        public string SourceInstance { get; }


        /// <summary>
        /// Constructor of <see cref="MessageSequencer"/>.
        /// </summary>
        /// <param name="sourceInstance">Unique identifier of the publishing service instance. See <see cref="CreateSourceInstance(string)"/>.</param>
        /// <param name="timeProvider">Time provider to get the creation time of the messages.</param>
        public MessageSequencer(string sourceInstance, TimeProvider timeProvider)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceInstance);
            ArgumentNullException.ThrowIfNull(timeProvider);

            SourceInstance = sourceInstance;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// Creates a unique identifier for a service instance.
        /// </summary>
        /// <remarks>
        /// Contains a new GUID, so that every process start results in a new source instance.
        /// </remarks>
        /// <param name="serviceName">Name of the service, e.g. PowerControl.</param>
        /// <returns>The unique identifier of the service instance.</returns>
        public static string CreateSourceInstance(string serviceName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

            return $"{serviceName}-{Environment.MachineName}-{Guid.NewGuid():N}";
        }


        /// <summary>
        /// Creates the stamp for the next message.
        /// </summary>
        /// <returns>The stamp with the next sequence number and the current time.</returns>
        public MessageStamp Next()
        {
            long sequence = Interlocked.Increment(ref _sequence);

            return new MessageStamp(SourceInstance, sequence, _timeProvider.GetUtcNow());
        }
    }
}
