// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Messaging;
using paskalON.PowerControls.Dto.Powers;
using System.Text.Json;
using System.Threading.Channels;

namespace paskalON.Devices.Application.Subscribers
{
    /// <summary>
    /// Receives the power targets as <see cref="CurrentTargetsDto"/> from a message broker and applies them to the devices.
    /// </summary>
    /// <remarks>
    /// Received messages are queued and applied by a single reader, so the targets are applied in the order they were received
    /// and the message broker callback is not blocked by the device communication.
    /// </remarks>
    public sealed class PowerTargetSubscriber : IPowerTargetSubscriber
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<PowerTargetSubscriber> _logger;


        /// <summary>
        /// Device manager to apply the power targets to the devices.
        /// </summary>
        private readonly IDeviceManager _deviceManager;


        /// <summary>
        /// Interface for subscribing to message brokers.
        /// </summary>
        private readonly IMessageSubscriber _messageSubscriber;


        /// <summary>
        /// Queue of received power targets to apply.
        /// </summary>
        /// <remarks>
        /// Thread-safe FIFO (First-In, First-Out) queue.
        /// </remarks>
        /// 
        private readonly Channel<CurrentTargetsDto> _targets = Channel.CreateBounded<CurrentTargetsDto>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
        });


        /// <summary>
        /// Constructor of <see cref="PowerTargetSubscriber"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager to apply the power targets to the devices.</param>
        /// <param name="messageSubscriber">Interface for subscribing to message brokers.</param>
        public PowerTargetSubscriber(ILogger<PowerTargetSubscriber> logger, IDeviceManager deviceManager, IMessageSubscriber messageSubscriber)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(messageSubscriber);

            _logger = logger;
            _deviceManager = deviceManager;
            _messageSubscriber = messageSubscriber;
        }


        /// <inheritdoc/>
        public void Subscribe(string? topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                _logger.LogWarning("No power target topic is configured, power targets are not received from the message broker");

                return;
            }

            _messageSubscriber.Subscribe(topic, UpdatePowerTargets);
            _logger.LogInformation("Subscribed to power targets on topic {Topic}", topic);
        }


        /// <inheritdoc/>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Reads from the channel and applies the power targets to the devices in the order they were received.
                await foreach (CurrentTargetsDto targets in _targets.Reader.ReadAllAsync(cancellationToken))
                {
                    await ApplyAsync(targets);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Applying power targets stopped");
            }
        }


        /// <summary>
        /// Handles a received power target message and queues it to be applied.
        /// </summary>
        /// <param name="json">The json message with the power targets.</param>
        private void UpdatePowerTargets(string json)
        {
            try
            {
                CurrentTargetsDto? targets = JsonSerializer.Deserialize<CurrentTargetsDto>(json);

                if (targets is null)
                {
                    _logger.LogWarning("Received an empty power target message");

                    return;
                }

                // Writes the received power targets to the channel to be applied by the channel (queue).
                _targets.Writer.TryWrite(targets);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Received an invalid power target message");
            }
        }


        /// <summary>
        /// Applies the power targets to the power conversion system. A failure is logged, so the following targets are still applied.
        /// </summary>
        /// <param name="targets">The power targets to apply.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        private async Task ApplyAsync(CurrentTargetsDto targets)
        {
            try
            {
                await _deviceManager.SetPcsPowerTarget(targets.DeviceId, targets.ActivePowerWatt, targets.ReactivePowerVar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Applying power targets to PCS {DeviceId} failed", targets.DeviceId);
            }
        }
    }
}
