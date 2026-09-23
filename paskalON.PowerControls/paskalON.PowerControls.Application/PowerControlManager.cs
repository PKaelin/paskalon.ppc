// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Application
{
    public class PowerControlManager : IPowerControlManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<PowerControlManager> _logger;


        /// <summary>
        /// Device client to receive device DTOs from pub/sub.
        /// </summary>
        private readonly IDeviceClient _deviceClient;

        public ICollection<IMetricsPublisher> MetricsPublishers { get; protected set; } = new List<IMetricsPublisher>();


        public PowerControlManager(ILogger<PowerControlManager> logger, IDeviceClient deviceClient)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);

            _logger = logger;
            _deviceClient = deviceClient;
        }


    }
}
