// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.Messaging;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Application.Publishers
{
    /// <summary>
    /// Publishes the power targets of DER units to a message broker.
    /// Each registered unit gets its own worker, so a unit that is starting does not delay the other units.
    /// </summary>
    /// <remarks>
    /// Units are started through the device service, while their power targets are published to the configured power target topic.
    /// If no power target topic is configured, units are not registered and their targets are not published.
    /// </remarks>
    public sealed class DerUnitTargetPublisher : IDerUnitTargetPublisher
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DerUnitTargetPublisher> _logger;


        /// <summary>
        /// Device server to send commands to the device service.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Interface for publishing to message brokers.
        /// </summary>
        private readonly IMessagePublisher _messagePublisher;


        /// <summary>
        /// Timing options for starting, polling and retrying.
        /// </summary>
        private readonly DerUnitTargetPublisherOptions _options;


        /// <summary>
        /// Time provider for delays and timeouts.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Cancellation source to stop all workers.
        /// </summary>
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();


        /// <summary>
        /// Lock object for synchronizing access to shared data.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Workers and their running tasks by PCS device identifier.
        /// </summary>
        private readonly Dictionary<int, (DerUnitTargetWorker Worker, Task Run)> _workers = new Dictionary<int, (DerUnitTargetWorker Worker, Task Run)>();


        /// <summary>
        /// Message publisher topics, set by <see cref="Initialize(PublisherTopic)"/>.
        /// </summary>
        private PublisherTopic? _topics;


        /// <summary>
        /// Constructor of <see cref="DerUnitTargetPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceServer">Device server to send commands to the device service.</param>
        /// <param name="messagePublisher">Interface for publishing to message brokers.</param>
        /// <param name="options">Timing options for starting, polling and retrying.</param>
        /// <param name="timeProvider">Time provider for delays and timeouts.</param>
        public DerUnitTargetPublisher(ILogger<DerUnitTargetPublisher> logger, IDeviceServer deviceServer, IMessagePublisher messagePublisher,
            DerUnitTargetPublisherOptions options, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceServer);
            ArgumentNullException.ThrowIfNull(messagePublisher);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.StartTimeout, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.StatePollInterval, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.RetryDelay, TimeSpan.Zero);

            _logger = logger;
            _deviceServer = deviceServer;
            _messagePublisher = messagePublisher;
            _options = options;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        public void Initialize(PublisherTopic topics)
        {
            ArgumentNullException.ThrowIfNull(topics);

            lock (_dataLock)
            {
                _topics = topics;
            }

            if (string.IsNullOrWhiteSpace(topics.PowerTargetTopic))
            {
                _logger.LogWarning("No power target topic is configured, power targets of DER units will not be published");
            }
        }


        /// <inheritdoc/>
        public void Register(IDerUnitPowerControl unit)
        {
            ArgumentNullException.ThrowIfNull(unit);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unit.PcsDeviceId);

            lock (_dataLock)
            {
                if (_topics is null)
                {
                    throw new InvalidOperationException($"{nameof(DerUnitTargetPublisher)} must be initialized before a DER unit is registered");
                }

                string? topic = _topics.PowerTargetTopic;

                if (string.IsNullOrWhiteSpace(topic))
                {
                    _logger.LogWarning("DER unit with PCS {PcsDeviceId} is not registered, because no power target topic is configured", unit.PcsDeviceId);

                    return;
                }

                if (_workers.ContainsKey(unit.PcsDeviceId))
                {
                    throw new InvalidOperationException($"A DER unit with PCS device id {unit.PcsDeviceId} is already registered");
                }

                // Create a worker for the unit and start it in a separate task.
                DerUnitTargetWorker worker = new DerUnitTargetWorker(_logger, unit, _deviceServer, _messagePublisher, topic, _options, _timeProvider);
                _workers.Add(unit.PcsDeviceId, (worker, worker.RunAsync(_cancellation.Token)));
                // Link the unit's TargetPowerChanged event to the worker's Notify method, so that the worker is notified when the target power changes.
                unit.TargetPowerChanged += (sender, args) => worker.Notify();
            }

            _logger.LogInformation("Registered DER unit with PCS {PcsDeviceId} for target publishing", unit.PcsDeviceId);
        }


        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            List<(DerUnitTargetWorker Worker, Task Run)> workers;

            lock (_dataLock)
            {
                workers = _workers.Values.ToList();
            }

            workers.ForEach(w => w.Worker.Complete());
            await _cancellation.CancelAsync();
            await Task.WhenAll(workers.Select(w => w.Run));
            _cancellation.Dispose();
        }
    }
}
