// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Application.Publishers;

namespace paskalON.Devices.Service.Publishers
{
    public class DevicePublisherService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DevicePublisherService> _logger;


        /// <summary>
        /// Device publisher interface that publishes the data.
        /// </summary>
        private IDevicePublisher? _devicePublisher;


        /// <summary>
        /// Time based interval for data publisher.
        /// </summary>
        /// <remarks>
        /// Time based interval means that the publisher is called periodically with this time interval.
        /// </remarks>
        private int _intervalMilliseconds;


        /// <summary>
        /// Constructor of <see cref="DevicePublisherService"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>        
        public DevicePublisherService(ILogger<DevicePublisherService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="devicePublisher">Device publisher interface that publishes the data.</param>
        /// <param name="intervalMilliseconds">Time based interval for data publisher.</param>
        public void Initialize(IDevicePublisher devicePublisher, int intervalMilliseconds)
        {
            ArgumentNullException.ThrowIfNull(devicePublisher);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMilliseconds);

            _devicePublisher = devicePublisher;
            _intervalMilliseconds = intervalMilliseconds;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int interval = 0;
            using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(_intervalMilliseconds));

            // awaits make sure that no overload should occur
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                if (++interval == int.MaxValue)
                {
                    interval = 1;
                }

                try
                {
                    await (_devicePublisher?.Publish(interval) ?? Task.CompletedTask);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected error while publishing device data. {Error}", ex);
                }
            }
        }
    }
}
