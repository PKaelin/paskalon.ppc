// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;
using paskalON.Luna.Service.WebApi.Hubs;
using paskalON.OperatingModes.Dto.Powers;

namespace paskalON.Luna.Service.Publishers
{
    /// <summary>
    /// Periodically pushes the applied setpoints of the operating mode service to the clients of the <see cref="SystemHub"/>,
    /// so that the operator sees why the applied setpoints differ from the supervisory setpoints.
    /// </summary>
    /// <remarks>
    /// Nothing is published while no client is connected.
    /// </remarks>
    public class SetpointStatusPublisher : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<SetpointStatusPublisher> _logger;


        /// <summary>
        /// Operating mode manager with the last received setpoints.
        /// </summary>
        private readonly ILunaOperatingModeManager _operatingModeManager;


        /// <summary>
        /// Hub context to send the setpoints to the clients.
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
        /// Constructor of <see cref="SetpointStatusPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="operatingModeManager">Operating mode manager with the last received setpoints.</param>
        /// <param name="hubContext">Hub context to send the setpoints to the clients.</param>
        /// <param name="connectionCounter">Counter of the clients connected to the <see cref="SystemHub"/>.</param>
        /// <param name="interval">Time based interval for publishing.</param>
        public SetpointStatusPublisher(ILogger<SetpointStatusPublisher> logger, ILunaOperatingModeManager operatingModeManager,
            IHubContext<SystemHub, ISystemClient> hubContext, HubConnectionCounter connectionCounter, TimeSpan interval)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(operatingModeManager);
            ArgumentNullException.ThrowIfNull(hubContext);
            ArgumentNullException.ThrowIfNull(connectionCounter);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

            _logger = logger;
            _operatingModeManager = operatingModeManager;
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
                        CurrentSetpointsDto setpoints = _operatingModeManager.GetSetpoints();
                        await _hubContext.Clients.All.SetpointsUpdated(setpoints);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Publishing the setpoints failed.");
                    }
                }
            }
        }
    }
}
