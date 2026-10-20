// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Application;

namespace paskalON.OperatingModes.Service.Workers
{
    public class OperatingModeService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<OperatingModeService> _logger;


        /// <summary>
        /// Interface for managing operating mode operations.
        /// </summary>
        private readonly IOperatingModeManager _operatingModeManager;


        /// <summary>
        /// Time based interval for operating mode calculations.
        /// </summary>
        /// <remarks>
        /// Time based interval means that the calculations are performed periodically with this time interval.
        /// </remarks>
        private int _intervalMilliseconds;


        /// <summary>
        /// Initializes a new instance of the <c>OperatingModeService</c> class.
        /// </summary>
        /// <param name="logger">Logger used to write diagnostic and operational messages.</param>
        /// <param name="operatingModeManager">Manager responsible for operating mode operations.</param>
        public OperatingModeService(ILogger<OperatingModeService> logger, IOperatingModeManager operatingModeManager)
        {
            _logger = logger;
            _operatingModeManager = operatingModeManager;
        }


        /// <summary>
        /// Initializes the service with the configured execution interval.
        /// </summary>
        /// <param name="intervalMilliseconds">Interval, in milliseconds, used by the service; must be greater than zero.</param>
        public void Initialize(int intervalMilliseconds)
        {
            _logger.LogInformation("Initializing OperatingModeService with {Interval}ms interval", intervalMilliseconds);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMilliseconds);

            _intervalMilliseconds = intervalMilliseconds;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int interval = 0;
            using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(_intervalMilliseconds));

            try
            {
                // awaits make sure that no overload should occur
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if (++interval == int.MaxValue)
                    {
                        interval = 1;
                    }

                    try
                    {
                        _operatingModeManager.SystemOperatingModeControl?.Calculate(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Unexpected error while calculating operating mode. {Error}", ex);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected when normal shutdown.
            }
        }
    }
}
