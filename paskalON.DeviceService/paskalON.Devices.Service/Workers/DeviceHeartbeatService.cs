// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain;
using System.Diagnostics;

namespace paskalON.Devices.Service.Workers
{
    public class DeviceHeartbeatService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DeviceHeartbeatService> _logger;


        /// <summary>
        /// Time based interval for heartbeat calls.
        /// </summary>
        /// <remarks>
        /// Time based interval means that the heartbeat is called periodically with this time interval.
        /// </remarks>
        private int _intervalMilliseconds;


        /// <summary>
        /// List of heartbeat devices.
        /// </summary>
        private IEnumerable<IDeviceHeartbeat> _deviceHeartbeats = new List<IDeviceHeartbeat>();


        /// <summary>
        /// Constructor of <see cref="DeviceHeartbeatService"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public DeviceHeartbeatService(ILogger<DeviceHeartbeatService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <summary>
        /// Initializes the heartbeat service.
        /// </summary>
        /// <param name="deviceHeartbeats">List of heartbeat devices.</param>
        /// <param name="intervalMilliseconds">Time based interval for heartbeat calls.</param>
        public void Initialize(IEnumerable<IDeviceHeartbeat> deviceHeartbeats, int intervalMilliseconds)
        {
            _logger.LogInformation("Initializing DeviceHeartbeatService with {Interval}ms interval", intervalMilliseconds);
            ArgumentNullException.ThrowIfNull(deviceHeartbeats);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMilliseconds);

            _deviceHeartbeats = deviceHeartbeats;
            _intervalMilliseconds = intervalMilliseconds;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task[] tasks = _deviceHeartbeats.Select(heart => RunEngineAsync(heart, stoppingToken)).ToArray();

            Task.WaitAll(tasks);
        }


        /// <summary>
        /// Run all the heartbeats in a separate threat.
        /// </summary>
        /// <param name="heartbeat">The device heartbeat.</param>
        /// <param name="stoppingToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task RunEngineAsync(IDeviceHeartbeat heartbeat, CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long nextRun = 0;

            while (stoppingToken.IsCancellationRequested == false)
            {
                long now = stopwatch.ElapsedMilliseconds;
                long delay = nextRun - now;

                if (delay > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(delay), stoppingToken);
                }

                nextRun += _intervalMilliseconds;

                try
                {
                    await heartbeat.HeartbeatAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected error heartbeat. Error: {Error}", ex);
                }
            }
        }
    }
}
