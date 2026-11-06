// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace paskalON.Alarms.Application.Subscribers
{
    /// <summary>
    /// Queue of the received published data between the message subscriber and the alarm handler.
    /// </summary>
    /// <remarks>
    /// - The message subscriber callbacks are synchronous, the alarm handler is asynchronous and uses the database.
    ///   The queue decouples them and the data is evaluated one after another in receive order, so that the data of
    ///   the same publisher is never evaluated concurrently (e.g. the same alarm raised twice).
    /// - The queue is bounded: when the evaluation cannot keep up (e.g. database is slow) the oldest data is dropped.
    ///   The publishers send their complete state periodically, so the latest data still raises and normalizes the alarms.
    /// </remarks>
    public class PpcAlarmMessageQueue : IPpcAlarmMessageQueue
    {
        /// <summary>
        /// Default maximum number of queued messages.
        /// </summary>
        public const int DefaultCapacity = 1000;


        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<PpcAlarmMessageQueue> _logger;


        /// <summary>
        /// Bounded channel of the queued messages.
        /// </summary>
        private readonly Channel<PpcAlarmMessage> _channel;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmMessageQueue"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="capacity">Maximum number of queued messages.</param>
        public PpcAlarmMessageQueue(ILogger<PpcAlarmMessageQueue> logger, int capacity = DefaultCapacity)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);

            _logger = logger;
            BoundedChannelOptions options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true
            };
            _channel = Channel.CreateBounded<PpcAlarmMessage>(options, OnMessageDropped);
        }


        /// <inheritdoc/>
        public void Enqueue(PpcAlarmMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);

            _channel.Writer.TryWrite(message);
        }


        /// <inheritdoc/>
        public IAsyncEnumerable<PpcAlarmMessage> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }


        /// <summary>
        /// Logs the dropped oldest message of a full queue.
        /// </summary>
        private void OnMessageDropped(PpcAlarmMessage message)
        {
            _logger.LogWarning("Alarm message queue is full, dropped the data of {PpcAlarmType} device {DeviceId} received on {ReceivedOn}.",
                message.Key.PpcAlarmType, message.Key.DeviceId, message.ReceivedOn);
        }
    }
}
