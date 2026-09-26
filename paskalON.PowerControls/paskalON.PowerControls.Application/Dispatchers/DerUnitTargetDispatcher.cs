// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Application.Dispatchers
{
    /// <summary>
    /// Dispatches the power targets of DER units to the device service.
    /// Each registered unit gets its own worker, so a unit that is starting does not delay the other units.
    /// </summary>
    public sealed class DerUnitTargetDispatcher : IDerUnitTargetDispatcher
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DerUnitTargetDispatcher> _logger;


        /// <summary>
        /// Device server to send commands to the device service.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Timing options for starting, polling and retrying.
        /// </summary>
        private readonly DerUnitTargetDispatcherOptions _options;


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
        private readonly object _dataLock = new();


        /// <summary>
        /// Workers and their running tasks by PCS device identifier.
        /// </summary>
        private readonly Dictionary<int, (DerUnitTargetWorker Worker, Task Run)> _workers = new Dictionary<int, (DerUnitTargetWorker Worker, Task Run)>();


        /// <summary>
        /// Constructor of <see cref="DerUnitTargetDispatcher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceServer">Device server to send commands to the device service.</param>
        /// <param name="options">Timing options for starting, polling and retrying.</param>
        /// <param name="timeProvider">Time provider for delays and timeouts.</param>
        public DerUnitTargetDispatcher(ILogger<DerUnitTargetDispatcher> logger, IDeviceServer deviceServer,
            DerUnitTargetDispatcherOptions options, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceServer);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.StartTimeout, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.StatePollInterval, TimeSpan.Zero);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.RetryDelay, TimeSpan.Zero);

            _logger = logger;
            _deviceServer = deviceServer;
            _options = options;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        public void Register(IDerUnitPowerControl unit)
        {
            ArgumentNullException.ThrowIfNull(unit);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unit.PcsDeviceId);

            // Create a worker for the unit and start it in a separate task.
            DerUnitTargetWorker worker = new DerUnitTargetWorker(_logger, unit, _deviceServer, _options, _timeProvider);

            lock (_dataLock)
            {
                if (_workers.ContainsKey(unit.PcsDeviceId))
                {
                    throw new InvalidOperationException($"A DER unit with PCS device id {unit.PcsDeviceId} is already registered");
                }

                _workers.Add(unit.PcsDeviceId, (worker, worker.RunAsync(_cancellation.Token)));
            }

            // Link the unit's TargetPowerChanged event to the worker's Notify method, so that the worker is notified when the target power changes.
            unit.TargetPowerChanged += (sender, args) => worker.Notify();
            _logger.LogInformation("Registered DER unit with PCS {PcsDeviceId} for target dispatching", unit.PcsDeviceId);
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
