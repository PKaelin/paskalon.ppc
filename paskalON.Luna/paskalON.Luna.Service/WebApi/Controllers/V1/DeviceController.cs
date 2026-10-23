// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Dto.Ders;
using paskalON.Luna.Application;

namespace paskalON.Luna.Service.WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class DeviceController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        public readonly ILogger<DeviceController> _logger;


        /// <summary>
        /// Device manager for managing DER (Distributed Energy Resources) devices and their operations.
        /// </summary>
        public readonly ILunaDeviceManager _deviceManager;


        /// <summary>
        /// Constructor of <see cref="DeviceController"/>.
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager for managing DER (Distributed Energy Resources) devices and their operations.</param>
        public DeviceController(ILogger<DeviceController> logger, ILunaDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        /// <summary>
        /// Gets the DER root with all its contents.
        /// </summary>
        /// <returns>The DER root object.</returns>
        [HttpGet]
        public DerDto GetDer()
        {
            return _deviceManager.GetDer();
        }
    }
}
