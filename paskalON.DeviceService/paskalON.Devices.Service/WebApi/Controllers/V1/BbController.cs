// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Application;
using paskalON.Devices.Service.Dto.V1.Requests;

namespace paskalON.Devices.Service.WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class BbController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<BbController> _logger;


        /// <summary>
        /// Device manager for managing DER (Distributed Energy Resources) devices and their operations.
        /// </summary>
        private readonly IDeviceManager _deviceManager;


        /// <summary>
        /// Constructor of <see cref="BbController"/>
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager for managing DER (Distributed Energy Resources).</param>
        public BbController(ILogger<BbController> logger, IDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        /// <summary>
        /// Connects a battery bank.
        /// </summary>
        /// <param name="request">Connect BB request.</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> Connect(ConnectBbRequest request)
        {
            await _deviceManager.ConnectBatteryBankAsync(request.DeviceId);

            return Ok();
        }


        /// <summary>
        /// Disconnects a battery bank.
        /// </summary>
        /// <param name="request">Disconnect BB request.</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> Disconnect(DisconnectBbRequest request)
        {
            await _deviceManager.DisconnectBatteryBankAsync(request.DeviceId);

            return Ok();
        }
    }
}
