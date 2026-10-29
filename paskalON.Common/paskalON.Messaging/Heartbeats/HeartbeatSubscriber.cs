// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Messaging.Health;
using paskalON.Messaging.Sequences;
using System.Text.Json;

namespace paskalON.Messaging.Heartbeats
{
    /// <summary>
    /// Subscribes to the <see cref="HeartbeatDto"/> of a dependency and signals the <see cref="IDependencyHealthMonitor"/> that it is alive.
    /// </summary>
    /// <remarks>
    /// Outdated or duplicated heartbeats are dropped and do not signal the dependency.
    /// </remarks>
    public sealed class HeartbeatSubscriber
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<HeartbeatSubscriber> _logger;


        /// <summary>
        /// Interface for subscribing to message brokers.
        /// </summary>
        private readonly IMessageSubscriber _subscriber;


        /// <summary>
        /// Monitor to signal that the dependency is alive.
        /// </summary>
        private readonly IDependencyHealthMonitor _monitor;


        /// <summary>
        /// Topic to subscribe the heartbeats from.
        /// </summary>
        private readonly string _topic;


        /// <summary>
        /// Name of the dependency registered in the monitor.
        /// </summary>
        private readonly string _dependencyName;


        /// <summary>
        /// Guard to drop outdated or duplicated heartbeats.
        /// </summary>
        private readonly MessageOrderGuard _orderGuard = new MessageOrderGuard();


        /// <summary>
        /// Constructor of <see cref="HeartbeatSubscriber"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="subscriber">Interface for subscribing to message brokers.</param>
        /// <param name="monitor">Monitor to signal that the dependency is alive.</param>
        /// <param name="topic">Topic to subscribe the heartbeats from.</param>
        /// <param name="dependencyName">Name of the dependency registered in the monitor.</param>
        public HeartbeatSubscriber(ILogger<HeartbeatSubscriber> logger, IMessageSubscriber subscriber, IDependencyHealthMonitor monitor, string topic, string dependencyName)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(subscriber);
            ArgumentNullException.ThrowIfNull(monitor);
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
            ArgumentException.ThrowIfNullOrWhiteSpace(dependencyName);

            _logger = logger;
            _subscriber = subscriber;
            _monitor = monitor;
            _topic = topic;
            _dependencyName = dependencyName;
        }


        /// <summary>
        /// Subscribes to the heartbeat topic.
        /// </summary>
        public void Subscribe()
        {
            _subscriber.Subscribe(_topic, Handle);
        }


        /// <summary>
        /// Handles a received heartbeat message.
        /// </summary>
        /// <param name="json">The received json message.</param>
        public void Handle(string json)
        {
            HeartbeatDto? dto;

            try
            {
                dto = JsonSerializer.Deserialize<HeartbeatDto>(json);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Deserializing the heartbeat of {Dependency} from topic {Topic} failed.", _dependencyName, _topic);

                return;
            }

            if (dto is null)
            {
                return;
            }

            if (_orderGuard.TryAccept(dto) is false)
            {
                _logger.LogDebug("Dropped outdated heartbeat {Sequence} of {SourceInstance}.", dto.Sequence, dto.SourceInstance);

                return;
            }

            _monitor.Signal(_dependencyName);
        }
    }
}
