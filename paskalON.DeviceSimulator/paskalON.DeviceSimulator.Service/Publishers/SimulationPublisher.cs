// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.DeviceSimulator.Application;
using paskalON.DeviceSimulator.Dto;
using paskalON.DeviceSimulator.Dto.Ders;
using paskalON.DeviceSimulator.Service.WebApi.Hubs;

namespace paskalON.DeviceSimulator.Service.Publishers
{
    /// <summary>
    /// Periodically pushes the values of the units and devices expanded in the UI to the SignalR clients.
    /// </summary>
    /// <remarks>
    /// For performance reasons only what the UI reported as expanded (POST v1/der/setexpanded) is published.
    /// </remarks>
    public class SimulationPublisher : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<SimulationPublisher> _logger;


        /// <summary>
        /// Device manager of the simulated devices.
        /// </summary>
        private readonly IDeviceManagerSimulator _deviceManager;


        /// <summary>
        /// Hub context to send the updates to the clients.
        /// </summary>
        private readonly IHubContext<DerHub, IDerClient> _hubContext;


        /// <summary>
        /// Time based interval for publishing.
        /// </summary>
        private int _intervalTimerMilliseconds;


        /// <summary>
        /// Constructor of <see cref="SimulationPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager of the simulated devices.</param>
        /// <param name="hubContext">Hub context to send the updates to the clients.</param>
        public SimulationPublisher(ILogger<SimulationPublisher> logger, IDeviceManagerSimulator deviceManager, IHubContext<DerHub, IDerClient> hubContext)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(hubContext);

            _logger = logger;
            _deviceManager = deviceManager;
            _hubContext = hubContext;
        }


        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="intervalTimerMilliseconds">Time based interval for publishing.</param>
        public void Initialize(int intervalTimerMilliseconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalTimerMilliseconds);

            _intervalTimerMilliseconds = intervalTimerMilliseconds;
        }


        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_intervalTimerMilliseconds));

            using (timer)
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        await PublishAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Publishing the simulation values failed. Error: {Error}", ex);
                    }
                }
            }
        }


        /// <summary>
        /// Publishes the expanded units and power meters.
        /// </summary>
        private async Task PublishAsync()
        {
            List<string> expandedUnits = _deviceManager.ExpandedUnits;
            List<string> expandedDevices = _deviceManager.ExpandedDevices;

            if (expandedUnits.Count == 0 && expandedDevices.Count == 0)
            {
                return;
            }

            DeviceMapperSimulator mapper = new DeviceMapperSimulator(_deviceManager.StoreRegisters, _deviceManager.StreamRegisters,
                _deviceManager.SimulationDevices, _deviceManager.IsResponding);
            List<DerUnitDto> units = mapper.MapUnits(_deviceManager.Der, expandedUnits);
            List<DeviceUpdateDto> meters = mapper.MapPowerMeterUpdates(_deviceManager.Der, expandedDevices);

            if (units.Count > 0)
            {
                await _hubContext.Clients.All.UnitsUpdated(units);
            }

            if (meters.Count > 0)
            {
                await _hubContext.Clients.All.DevicesUpdated(meters);
            }
        }
    }
}
