// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Application.Publishers;

namespace paskalON.Devices.Service.Publishers
{
    public class DevicePublisherService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Device publisher interface that publishes the data.
        /// </summary>
        private readonly IDevicePublisher _devicePublisher;


        /// <summary>
        /// Time based interval for data publisher.
        /// </summary>
        /// <remarks>
        /// Time based interval means that the publisher is called periodically with this time interval.
        /// </remarks>
        private readonly int _intervalMilliseconds;


        /// <summary>
        /// Constructor of <see cref="DevicePublisherService"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="devicePublisher">Device publisher interface that publishes the data.</param>
        /// <param name="intervalMilliseconds">Time based interval for data publisher.</param>
        public DevicePublisherService(ILogger<DevicePublisherService> logger, IDevicePublisher devicePublisher, int intervalMilliseconds)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(devicePublisher);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMilliseconds);

            _logger = logger;
            _devicePublisher = devicePublisher;
            _intervalMilliseconds = intervalMilliseconds;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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
                    await _devicePublisher.Publish(interval);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected error while publishing device data. {Error}", ex);
                }
            }
        }
    }
}
