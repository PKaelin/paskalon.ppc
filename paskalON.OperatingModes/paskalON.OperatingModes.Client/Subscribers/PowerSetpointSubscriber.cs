// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Messaging;
using paskalON.OperatingModes.Dto.Powers;
using System.Text.Json;
using System.Threading.Channels;

namespace paskalON.OperatingModes.Client.Subscribers
{
    /// <summary>
    /// Receives the system power setpoints as <see cref="CurrentSetpointsDto"/> and the DER unit power setpoints
    /// as <see cref="CurrentDerUnitSetpointsDto"/> from a message broker and passes them to the <see cref="IPowerSetpointHandler"/>.
    /// </summary>
    /// <remarks>
    /// Received messages are queued and handled by a single reader, so the setpoints are handled in the order they were received
    /// and the message broker callback is not blocked by the handler.
    /// </remarks>
    public sealed class PowerSetpointSubscriber : IPowerSetpointSubscriber
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<PowerSetpointSubscriber> _logger;


        /// <summary>
        /// Handler of the received power setpoints.
        /// </summary>
        private readonly IPowerSetpointHandler _powerSetpointHandler;


        /// <summary>
        /// Interface for subscribing to message brokers.
        /// </summary>
        private readonly IMessageSubscriber _messageSubscriber;


        /// <summary>
        /// Queue of received power setpoints to handle, either <see cref="CurrentSetpointsDto"/> or <see cref="CurrentDerUnitSetpointsDto"/>.
        /// </summary>
        /// <remarks>
        /// Thread-safe FIFO (First-In, First-Out) queue.
        /// </remarks>
        private readonly Channel<object> _setpoints = Channel.CreateBounded<object>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
        });


        /// <summary>
        /// Constructor of <see cref="PowerSetpointSubscriber"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="powerSetpointHandler">Handler of the received power setpoints.</param>
        /// <param name="messageSubscriber">Interface for subscribing to message brokers.</param>
        public PowerSetpointSubscriber(ILogger<PowerSetpointSubscriber> logger, IPowerSetpointHandler powerSetpointHandler, IMessageSubscriber messageSubscriber)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(powerSetpointHandler);
            ArgumentNullException.ThrowIfNull(messageSubscriber);

            _logger = logger;
            _powerSetpointHandler = powerSetpointHandler;
            _messageSubscriber = messageSubscriber;
        }


        /// <inheritdoc/>
        public void Subscribe(string? topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                _logger.LogWarning("No power setpoint topic is configured, power setpoints are not received from the message broker");

                return;
            }

            _messageSubscriber.Subscribe(topic, Enqueue<CurrentSetpointsDto>);
            _logger.LogInformation("Subscribed to power setpoints on topic {Topic}", topic);
        }


        /// <inheritdoc/>
        public void SubscribeDerUnit(string? topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                _logger.LogWarning("No DER unit power setpoint topic is configured, DER unit power setpoints are not received from the message broker");

                return;
            }

            _messageSubscriber.Subscribe(topic, Enqueue<CurrentDerUnitSetpointsDto>);
            _logger.LogInformation("Subscribed to DER unit power setpoints on topic {Topic}", topic);
        }


        /// <inheritdoc/>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Reads from the channel and handles the power setpoints in the order they were received.
                await foreach (object setpoint in _setpoints.Reader.ReadAllAsync(cancellationToken))
                {
                    await HandleAsync(setpoint);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Handling power setpoints stopped");
            }
        }


        /// <summary>
        /// Passes the power setpoints to the handler. A failure is logged, so the following setpoints are still handled.
        /// </summary>
        /// <param name="setpoint">The power setpoints to handle.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        private async Task HandleAsync(object setpoint)
        {
            try
            {
                switch (setpoint)
                {
                    case CurrentSetpointsDto system:
                        await _powerSetpointHandler.HandleSetpointsAsync(system);
                        break;

                    case CurrentDerUnitSetpointsDto unit:
                        await _powerSetpointHandler.HandleDerUnitSetpointsAsync(unit);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handling power setpoints failed");
            }
        }


        /// <summary>
        /// Handles a received power setpoints message and queues it to be handled.
        /// </summary>
        /// <typeparam name="T">The DTO type of the power setpoints message.</typeparam>
        /// <param name="json">The json message with the power setpoints.</param>
        private void Enqueue<T>(string json) where T : class
        {
            try
            {
                T? setpoint = JsonSerializer.Deserialize<T>(json);

                if (setpoint is null)
                {
                    _logger.LogWarning("Received an empty {Type} message", typeof(T).Name);

                    return;
                }

                // Writes the received power setpoints to the channel (queue) to be handled by the single reader.
                _setpoints.Writer.TryWrite(setpoint);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Received an invalid {Type} message", typeof(T).Name);
            }
        }
    }
}
