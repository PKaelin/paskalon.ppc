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
    public class PcsController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<PcsController> _logger;


        /// <summary>
        /// Device manager for managing DER (Distributed Energy Resources) devices and their operations.
        /// </summary>
        private readonly IDeviceManager _deviceManager;


        /// <summary>
        /// Constructor of <see cref="PcsController"/>
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager for managing DER (Distributed Energy Resources).</param>
        public PcsController(ILogger<PcsController> logger, IDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);

            _logger = logger;
            _deviceManager = deviceManager;
        }


        /// <summary>
        /// Starts all PCS that are not in maintenance mode.
        /// </summary>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> StartAll()
        {
            // Dont wait for them all
            _ = Task.Run(() => _deviceManager.StartAllPcsAsync());

            return Ok();
        }


        /// <summary>
        /// Starts a specific PCS.
        /// </summary>
        /// <param name="request">Request containing the device ID of the PCS to action on.</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> Start(StartPcsRequestDto request)
        {
            await _deviceManager.StartPcsAsync(request.DeviceId);

            return Ok();
        }


        /// <summary>
        /// Stops all PCS that are not in maintenance mode.
        /// </summary>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> StopAll()
        {
            // Dont wait for them all
            _ = Task.Run(() => _deviceManager.StopAllPcsAsync());

            return Ok();
        }


        /// <summary>
        /// Stops a specific PCS.
        /// </summary>
        /// <param name="request">Request containing the device ID of the PCS to action on.</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> Stop(StopPcsRequestDto request)
        {
            await _deviceManager.StopPcsAsync(request.DeviceId);

            return Ok();
        }


        /// <summary>
        /// Standbys all PCS that are not in maintenance mode.
        /// </summary>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> StandbyAll()
        {
            _ = Task.Run(() => _deviceManager.StandbyAllPcsAsync());

            return Ok();
        }


        /// <summary>
        /// Standbys a specific PCS.
        /// </summary>
        /// <param name="request">Request containing the device ID of the PCS to action on.</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> Standby(StandbyPcsRequestDto request)
        {
            await _deviceManager.StandbyPcsAsync(request.DeviceId);

            return Ok();
        }


        /// <summary>
        /// Sets a power targets to a specific PCS.
        /// </summary>
        /// <param name="request">Request containing the device ID of the PCS to action on.</param>
        /// <returns>Task</returns>
        [HttpPost]
        public async Task<IActionResult> SetPowerTarget(SetPowerTargetRequest request)
        {
            await _deviceManager.SetPcsPowerTarget(request.DeviceId, request.ActivePowerWatt, request.ReactivePowerVar);

            return Ok();
        }
    }
}
