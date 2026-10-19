// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.Messaging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Dto.Powers;
using System.Text.Json;
using System.Threading.Channels;

namespace paskalON.PowerControls.Application.Publishers
{
    /// <summary>
    /// Delivers the power targets of a single DER unit by publishing them as <see cref="CurrentTargetsDto"/> to a message broker.
    /// The worker is notified about new targets and always publishes the latest targets of the unit,
    /// so targets that arrive while the unit is starting are merged into one delivery.
    /// A failed delivery is retried until it succeeds or a newer notification replaces it.
    /// </summary>
    internal sealed class DerUnitTargetWorker
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// DER unit whose targets are delivered.
        /// </summary>
        private readonly IDerUnitPowerControl _unit;


        /// <summary>
        /// Device server to send commands to the device service.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Interface for publishing to message brokers.
        /// </summary>
        private readonly IMessagePublisher _messagePublisher;


        /// <summary>
        /// Topic to publish the power targets to.
        /// </summary>
        private readonly string _topic;


        /// <summary>
        /// Timing options for starting, polling and retrying.
        /// </summary>
        private readonly DerUnitTargetPublisherOptions _options;


        /// <summary>
        /// Time provider for delays and timeouts.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Signal holding at most one pending notification. Multiple notifications are merged
        /// because the latest targets are read from the unit at the time of delivery.
        /// </summary>
        private readonly Channel<bool> _signal = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true
        });


        /// <summary>
        /// Constructor of <see cref="DerUnitTargetWorker"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="unit">DER unit whose targets are delivered.</param>
        /// <param name="deviceServer">Device server to send commands to the device service.</param>
        /// <param name="messagePublisher">Interface for publishing to message brokers.</param>
        /// <param name="topic">Topic to publish the power targets to.</param>
        /// <param name="options">Timing options for starting, polling and retrying.</param>
        /// <param name="timeProvider">Time provider for delays and timeouts.</param>
        public DerUnitTargetWorker(ILogger logger, IDerUnitPowerControl unit, IDeviceServer deviceServer, IMessagePublisher messagePublisher,
            string topic, DerUnitTargetPublisherOptions options, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(unit);
            ArgumentNullException.ThrowIfNull(deviceServer);
            ArgumentNullException.ThrowIfNull(messagePublisher);
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _unit = unit;
            _deviceServer = deviceServer;
            _messagePublisher = messagePublisher;
            _topic = topic;
            _options = options;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// Notifies the worker that the unit has new targets to deliver.
        /// </summary>
        public void Notify()
        {
            _signal.Writer.TryWrite(true);
        }


        /// <summary>
        /// Stops accepting notifications.
        /// </summary>
        public void Complete()
        {
            _signal.Writer.TryComplete();
        }


        /// <summary>
        /// Processes notifications until cancellation is requested or the worker is completed.
        /// </summary>
        /// <param name="cancellationToken">Token to stop processing.</param>
        /// <returns>Task that completes when processing has stopped.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (await _signal.Reader.WaitToReadAsync(cancellationToken))
                {
                    _signal.Reader.TryRead(out bool _);
                    bool isDelivered = await DeliverAsync(cancellationToken);

                    while (isDelivered is false)
                    {
                        await Task.Delay(_options.RetryDelay, _timeProvider, cancellationToken);
                        // Consume a pending notification because the retry delivers the latest targets anyway.
                        _signal.Reader.TryRead(out bool _);
                        isDelivered = await DeliverAsync(cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Target delivery for PCS {PcsDeviceId} stopped", _unit.PcsDeviceId);
            }
        }


        /// <summary>
        /// Delivers the latest targets of the unit. A stopped or standby unit is started first
        /// and the targets are published once it reached the started state.
        /// For a unit in maintenance only its maintenance target is published and the unit is never started,
        /// so the targets of the system distribution and of the maintenance are kept separate.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the delivery.</param>
        /// <returns>True if the delivery is completed, false if it failed and has to be retried.</returns>
        public async Task<bool> DeliverAsync(CancellationToken cancellationToken)
        {
            try
            {
                DerState state = _unit.State;

                if (state is DerState.Maintenance)
                {
                    if (_unit.IsMaintenanceTargetSet is false)
                    {
                        _logger.LogWarning("PCS {PcsDeviceId} is in maintenance without maintenance target, targets are not published", _unit.PcsDeviceId);

                        return true;
                    }

                    await PublishAsync(_unit.MaintenanceTargetActivePower, _unit.MaintenanceTargetReactivePower);

                    return true;
                }

                if (state is not DerState.Started)
                {
                    bool isStarted = await StartAsync(state, cancellationToken);

                    if (isStarted is false)
                    {
                        return false;
                    }
                }

                await PublishAsync(_unit.TargetActivePower, _unit.TargetReactivePower);

                return true;
            }
            catch (Exception ex) when (cancellationToken.IsCancellationRequested is false)
            {
                _logger.LogError(ex, "Publishing targets of PCS {PcsDeviceId} to topic {Topic} failed", _unit.PcsDeviceId, _topic);

                return false;
            }
        }


        /// <summary>
        /// Publishes the targets as <see cref="CurrentTargetsDto"/> for the PCS of the unit.
        /// </summary>
        /// <param name="activePower">The active power target to publish.</param>
        /// <param name="reactivePower">The reactive power target to publish.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous publish operation.</returns>
        private async Task PublishAsync(ActivePower activePower, ReactivePower reactivePower)
        {
            CurrentTargetsDto dto = new CurrentTargetsDto
            {
                DeviceId = _unit.PcsDeviceId,
                ActivePowerWatt = activePower.Watts,
                ReactivePowerVar = reactivePower.VoltAmperesReactive
            };
            string json = JsonSerializer.Serialize(dto);
            await _messagePublisher.PublishAsync(_topic, json);

            _logger.LogDebug("Published targets of PCS {PcsDeviceId} to topic {Topic}. Active Power {ActivePower}, Reactive Power {ReactivePower}",
                _unit.PcsDeviceId, _topic, activePower.Watts, reactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Sends a start command and waits until the unit reached the started state or the start timeout elapsed.
        /// </summary>
        /// <param name="state">The current state of the unit.</param>
        /// <param name="cancellationToken">Token to cancel the start.</param>
        /// <returns>True if the unit reached the started state, false if the start timed out.</returns>
        private async Task<bool> StartAsync(DerState state, CancellationToken cancellationToken)
        {
            _logger.LogInformation("PCS {PcsDeviceId} is in state {State}, sending start command", _unit.PcsDeviceId, state);

            await _deviceServer.StartPcs(_unit.PcsDeviceId, cancellationToken);
            long startTimestamp = _timeProvider.GetTimestamp();

            while (_unit.State is not DerState.Started)
            {
                if (_timeProvider.GetElapsedTime(startTimestamp) >= _options.StartTimeout)
                {
                    _logger.LogWarning("PCS {PcsDeviceId} did not reach the started state within {StartTimeout}", _unit.PcsDeviceId, _options.StartTimeout);

                    return false;
                }

                await Task.Delay(_options.StatePollInterval, _timeProvider, cancellationToken);
            }

            return true;
        }
    }
}
