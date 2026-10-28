// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;
using paskalON.Luna.Dto.Devices;
using paskalON.Luna.Service.WebApi.Hubs;

namespace paskalON.Luna.Service.Publishers
{
    /// <summary>
    /// Periodically pushes the system meter summary to the clients of the <see cref="SystemHub"/>.
    /// </summary>
    /// <remarks>
    /// Nothing is published while no client is connected. A missing system power meter is published as null,
    /// so the UI knows that no data can be retrieved.
    /// </remarks>
    public class SystemMeterSummaryPublisher : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<SystemMeterSummaryPublisher> _logger;


        /// <summary>
        /// Device manager to get the system meter summary.
        /// </summary>
        private readonly ILunaDeviceManager _deviceManager;


        /// <summary>
        /// Hub context to send the summary to the clients.
        /// </summary>
        private readonly IHubContext<SystemHub, ISystemClient> _hubContext;


        /// <summary>
        /// Counter of the clients connected to the <see cref="SystemHub"/>.
        /// </summary>
        private readonly HubConnectionCounter _connectionCounter;


        /// <summary>
        /// Time based interval for publishing.
        /// </summary>
        private readonly TimeSpan _interval;


        /// <summary>
        /// Constructor of <see cref="SystemMeterSummaryPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager to get the system meter summary.</param>
        /// <param name="hubContext">Hub context to send the summary to the clients.</param>
        /// <param name="connectionCounter">Counter of the clients connected to the <see cref="SystemHub"/>.</param>
        /// <param name="interval">Time based interval for publishing.</param>
        public SystemMeterSummaryPublisher(ILogger<SystemMeterSummaryPublisher> logger, ILunaDeviceManager deviceManager,
            IHubContext<SystemHub, ISystemClient> hubContext, HubConnectionCounter connectionCounter, TimeSpan interval)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(hubContext);
            ArgumentNullException.ThrowIfNull(connectionCounter);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

            _logger = logger;
            _deviceManager = deviceManager;
            _hubContext = hubContext;
            _connectionCounter = connectionCounter;
            _interval = interval;
        }


        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PeriodicTimer timer = new PeriodicTimer(_interval);

            using (timer)
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if (_connectionCounter.HasConnections is false)
                    {
                        continue;
                    }

                    try
                    {
                        SystemMeterSummaryDto? summary = _deviceManager.GetSystemMeterSummary();
                        await _hubContext.Clients.All.SystemMeterSummaryUpdated(summary);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Publishing the system meter summary failed.");
                    }
                }
            }
        }
    }
}
