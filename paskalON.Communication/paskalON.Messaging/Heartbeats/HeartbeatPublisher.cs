// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Messaging.Sequences;
using System.Text.Json;

namespace paskalON.Messaging.Heartbeats
{
    /// <summary>
    /// Publishes a <see cref="HeartbeatDto"/> periodically to signal the subscribers that the service is alive.
    /// </summary>
    /// <remarks>
    /// A failing publish is logged once until the publish succeeds again, so that a message broker outage does not flood the log.
    /// </remarks>
    public sealed class HeartbeatPublisher
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<HeartbeatPublisher> _logger;


        /// <summary>
        /// Interface for publishing to message brokers.
        /// </summary>
        private readonly IMessagePublisher _publisher;


        /// <summary>
        /// Sequencer to stamp the heartbeats.
        /// </summary>
        private readonly MessageSequencer _sequencer;


        /// <summary>
        /// Name of the publishing service.
        /// </summary>
        private readonly string _serviceName;


        /// <summary>
        /// Topic to publish the heartbeats to.
        /// </summary>
        private readonly string _topic;


        /// <summary>
        /// Interval between two heartbeats.
        /// </summary>
        private readonly TimeSpan _interval;


        /// <summary>
        /// Time provider for the heartbeat interval.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Whether the last publish failed.
        /// </summary>
        private bool _isFailing;


        /// <summary>
        /// Constructor of <see cref="HeartbeatPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="publisher">Interface for publishing to message brokers.</param>
        /// <param name="sequencer">Sequencer to stamp the heartbeats.</param>
        /// <param name="serviceName">Name of the publishing service, e.g. PowerControl.</param>
        /// <param name="topic">Topic to publish the heartbeats to.</param>
        /// <param name="interval">Interval between two heartbeats.</param>
        /// <param name="timeProvider">Time provider for the heartbeat interval.</param>
        public HeartbeatPublisher(ILogger<HeartbeatPublisher> logger, IMessagePublisher publisher, MessageSequencer sequencer, string serviceName, string topic,
            TimeSpan interval, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(sequencer);
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _publisher = publisher;
            _sequencer = sequencer;
            _serviceName = serviceName;
            _topic = topic;
            _interval = interval;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// Publishes a heartbeat immediately and then periodically until the cancellation is requested.
        /// </summary>
        /// <param name="cancellationToken">Token to stop publishing.</param>
        /// <returns>A <see cref="Task"/> that completes when publishing is stopped.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_interval, _timeProvider);

            try
            {
                do
                {
                    await PublishAsync();
                }
                while (await timer.WaitForNextTickAsync(cancellationToken));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Expected when normal shutdown.
            }
        }


        /// <summary>
        /// Publishes one heartbeat.
        /// </summary>
        /// <returns>True if the heartbeat was published, false if the publish failed.</returns>
        public async Task<bool> PublishAsync()
        {
            MessageStamp stamp = _sequencer.Next();
            HeartbeatDto dto = new HeartbeatDto
            {
                ServiceName = _serviceName,
                SourceInstance = stamp.SourceInstance,
                Sequence = stamp.Sequence,
                Timestamp = stamp.Timestamp
            };
            string json = JsonSerializer.Serialize(dto);

            try
            {
                await _publisher.PublishAsync(_topic, json);
            }
            catch (Exception ex)
            {
                if (_isFailing is false)
                {
                    _isFailing = true;
                    _logger.LogWarning(ex, "Publishing the heartbeat of {ServiceName} to topic {Topic} failed.", _serviceName, _topic);
                }

                return false;
            }

            if (_isFailing)
            {
                _isFailing = false;
                _logger.LogInformation("Publishing the heartbeat of {ServiceName} to topic {Topic} recovered.", _serviceName, _topic);
            }

            return true;
        }
    }
}
