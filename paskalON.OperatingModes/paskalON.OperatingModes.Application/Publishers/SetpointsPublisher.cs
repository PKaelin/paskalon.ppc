// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Messaging;
using paskalON.Messaging.Sequences;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.PhysicalUnits.Electricals.Powers;
using System.Text.Json;

namespace paskalON.OperatingModes.Application.Publishers
{
    /// <summary>
    /// Publishes the applied setpoints of the system operating mode control as <see cref="CurrentSetpointsDto"/> and the DER unit
    /// setpoints as <see cref="CurrentDerUnitSetpointsDto"/> to a message broker.
    /// </summary>
    /// <remarks>
    /// The messages are handed over to the message publisher without waiting for the broker, so the calculation cycle is not blocked.
    /// The Redis message publisher queues the messages in the order of the calls. A message is not published if its topic is not configured.
    /// </remarks>
    public class SetpointsPublisher : ISetpointsPublisher
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<SetpointsPublisher> _logger;


        /// <summary>
        /// Interface for publishing to message brokers.
        /// </summary>
        private readonly IMessagePublisher _publisher;


        /// <summary>
        /// Message publisher topic.
        /// </summary>
        private readonly PublisherTopic _publisherTopics;


        /// <summary>
        /// Sequencer to stamp the published setpoints.
        /// </summary>
        private readonly MessageSequencer _sequencer;


        /// <summary>
        /// Constructor of <see cref="SetpointsPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="publisher">Interface for publishing to message brokers.</param>
        /// <param name="topics">Message publisher topic.</param>
        /// <param name="sequencer">Sequencer to stamp the published setpoints, so that the receivers drop outdated setpoints.</param>
        public SetpointsPublisher(ILogger<SetpointsPublisher> logger, IMessagePublisher publisher, PublisherTopic topics, MessageSequencer sequencer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(topics);
            ArgumentNullException.ThrowIfNull(sequencer);

            _logger = logger;
            _publisher = publisher;
            _publisherTopics = topics;
            _sequencer = sequencer;
        }


        /// <inheritdoc/>
        public void Publish(ActivePower setpointActivePower, ReactivePower setpointReactivePower)
        {
            MessageStamp stamp = _sequencer.Next();
            CurrentSetpointsDto dto = new CurrentSetpointsDto
            {
                SetpointActivePowerWatts = setpointActivePower.Watts,
                SetpointReactivePowerVars = setpointReactivePower.VoltAmperesReactive,
                SourceInstance = stamp.SourceInstance,
                Sequence = stamp.Sequence,
                Timestamp = stamp.Timestamp
            };
            string json = JsonSerializer.Serialize(dto);

            _ = PublishAsync(json);
        }


        /// <inheritdoc/>
        public void PublishUnit(string derUnitName, ActivePower setpointActivePower, ReactivePower setpointReactivePower)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(derUnitName);

            MessageStamp stamp = _sequencer.Next();
            CurrentDerUnitSetpointsDto dto = new CurrentDerUnitSetpointsDto
            {
                DerUnitName = derUnitName,
                SetpointActivePowerWatts = setpointActivePower.Watts,
                SetpointReactivePowerVars = setpointReactivePower.VoltAmperesReactive,
                SourceInstance = stamp.SourceInstance,
                Sequence = stamp.Sequence,
                Timestamp = stamp.Timestamp
            };
            string json = JsonSerializer.Serialize(dto);

            _ = PublishUnitAsync(json);
        }


        /// <summary>
        /// Publishes the current setpoints message and logs a failure, since nobody awaits the publish operation.
        /// </summary>
        /// <param name="json">The json message to publish.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        private async Task PublishAsync(string json)
        {
            string? topic = _publisherTopics.CurrentSetpointTopic;

            if (string.IsNullOrWhiteSpace(topic))
            {
                return;
            }

            try
            {
                await _publisher.PublishAsync(topic, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publishing the current setpoints to topic {Topic} failed.", topic);
            }
        }


        /// <summary>
        /// Publishes the current DER unit setpoints message and logs a failure, since nobody awaits the publish operation.
        /// </summary>
        /// <param name="json">The json message to publish.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        private async Task PublishUnitAsync(string json)
        {
            string? topic = _publisherTopics.CurrentDerUnitSetpointTopic;

            if (string.IsNullOrWhiteSpace(topic))
            {
                return;
            }

            try
            {
                await _publisher.PublishAsync(topic, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publishing the current DER unit setpoints to topic {Topic} failed.", topic);
            }
        }
    }
}
