// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Application;

namespace paskalON.Devices.Service.WebApi.Controllers.V1
{
    /// <summary>
    /// API controller for power meter (System, External, Auxiliary, Circuit) operations.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class PowerMeterController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<PowerMeterController> _logger;


        /// <summary>
        /// Device manager for managing DER (Distributed Energy Resources) devices and their operations.
        /// </summary>
        private readonly IDeviceManager _deviceManager;


        /// <summary>
        /// Constructor of <see cref="PowerMeterController"/>
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager for managing DER (Distributed Energy Resources).</param>
        public PowerMeterController(ILogger<PowerMeterController> logger, IDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        /// <summary>
        /// Connects all power meters that are not already connected.
        /// </summary>
        /// <returns>Action result.</returns>
        [HttpPost]
        public IActionResult ConnectAll()
        {
            // Dont wait for all of them to complete
            _ = Task.Run(() => _deviceManager.ConnectAllMetersAsync());

            return Ok();
        }
    }
}
