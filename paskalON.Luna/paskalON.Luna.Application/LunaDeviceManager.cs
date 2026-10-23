// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.Devices.Dto.Ders;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna device manager to get device information.
    /// </summary>
    public class LunaDeviceManager : ILunaDeviceManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<LunaDeviceManager> _logger;


        /// <summary>
        /// Device client to get DER configuration and to receive device DTOs from pub/sub.
        /// </summary>
        private readonly IDeviceClient _deviceClient;


        /// <summary>
        /// Constructor of <see cref="LunaDeviceManager"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceClient">Device client to get DER configuration and to receive device DTOs from pub/sub.</param>
        public LunaDeviceManager(ILogger<LunaDeviceManager> logger, IDeviceClient deviceClient)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);

            _logger = logger;
            _deviceClient = deviceClient;
        }


        /// <summary>
        /// Get the DER configuration DTOs
        /// </summary>
        /// <returns>The DER configuration DTO</returns>
        public DerDto GetDer()
        {
            return _deviceClient.Der;
        }
    }
}
