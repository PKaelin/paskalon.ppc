// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;
using paskalON.Luna.Dto.Systems;
using paskalON.Luna.Service.WebApi.Hubs;

namespace paskalON.Luna.Service.Publishers
{
    /// <summary>
    /// Periodically pushes the availability of the dependencies to the clients of the <see cref="DependencyHub"/>.
    /// </summary>
    /// <remarks>
    /// Runs without connected clients too, because the dependency health monitor only evaluates its dependencies
    /// and logs their state changes when the state is requested.
    /// </remarks>
    public class DependencyStatusPublisher : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DependencyStatusPublisher> _logger;


        /// <summary>
        /// Dependency manager to get the availability of the dependencies.
        /// </summary>
        private readonly ILunaDependencyManager _dependencyManager;


        /// <summary>
        /// Hub context to send the availability to the clients.
        /// </summary>
        private readonly IHubContext<DependencyHub, IDependencyClient> _hubContext;


        /// <summary>
        /// Time based interval for publishing.
        /// </summary>
        private readonly TimeSpan _interval;


        /// <summary>
        /// Constructor of <see cref="DependencyStatusPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="dependencyManager">Dependency manager to get the availability of the dependencies.</param>
        /// <param name="hubContext">Hub context to send the availability to the clients.</param>
        /// <param name="interval">Time based interval for publishing.</param>
        public DependencyStatusPublisher(ILogger<DependencyStatusPublisher> logger, ILunaDependencyManager dependencyManager,
            IHubContext<DependencyHub, IDependencyClient> hubContext, TimeSpan interval)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(dependencyManager);
            ArgumentNullException.ThrowIfNull(hubContext);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

            _logger = logger;
            _dependencyManager = dependencyManager;
            _hubContext = hubContext;
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
                    try
                    {
                        IReadOnlyList<DependencyStatusDto> statuses = _dependencyManager.GetDependencyStatus();
                        await _hubContext.Clients.All.DependencyStatusUpdated(statuses);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Publishing the dependency status failed.");
                    }
                }
            }
        }
    }
}
