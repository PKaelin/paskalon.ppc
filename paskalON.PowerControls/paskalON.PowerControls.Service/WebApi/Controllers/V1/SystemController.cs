// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Client;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Application;
using paskalON.PowerControls.Service.Dto.V1.Requests;

namespace paskalON.PowerControls.Service.WebApi.Controllers.V1
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// Logger instance for logging messages.
        /// </summary>
        private readonly ILogger<SystemController> _logger;

        /// <summary>
        /// Manager for handling power control operations.
        /// </summary>
        private readonly IPowerControlManager _manager;


        /// <summary>
        /// Device server for calling the device service web API.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Constructor of <see cref="SystemController"/>.
        /// </summary>
        /// <param name="logger">Logger instance for logging messages.</param>
        /// <param name="manager">Manager for handling power control operations.</param>
        /// <param name="deviceServer">Device server for calling the device service web API.</param>
        public SystemController(ILogger<SystemController> logger, IPowerControlManager manager, IDeviceServer deviceServer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(deviceServer);

            _logger = logger;
            _manager = manager;
            _deviceServer = deviceServer;
        }


        /// <summary>
        /// Sets the power setpoint for the system.
        /// </summary>
        /// <param name="request">The request containing the active and reactive power setpoints.</param>
        /// <returns>Task.</returns>
        [HttpPost]
        public async Task<IActionResult> SetPowerSetpoints(SetPowerSetpointRequest request)
        {
            await _manager.SetSystemPowerTarget(new ActivePower(request.ActivePowerWatt), new ReactivePower(request.ReactivePowerVar));

            return Ok();
        }


        /// <summary>
        /// Starts a specific power conversion system (PCS) identified by the provided device ID.
        /// </summary>
        /// <param name="deviceId">The ID of the device to start.</param>
        /// <returns>Task.</returns>
        [HttpPost]
        public async Task<IActionResult> StartPcs(int deviceId)
        {
            await _deviceServer.StartPcs(deviceId);

            return Ok();
        }


        /// <summary>
        /// Starts all power conversion systems (PCS) that are not in maintenance mode.
        /// </summary>
        /// <returns>Task.</returns>
        [HttpPost]
        public async Task<IActionResult> StartAllPcs()
        {
            await _deviceServer.StartAllPcs();

            return Ok();
        }


        /// <summary>
        /// Puts all power conversion systems (PCS) that are not in maintenance mode into standby mode.
        /// </summary>
        /// <returns>Task.</returns>
        [HttpPost]
        public async Task<IActionResult> StandbyAllPcs()
        {
            await _deviceServer.StandbyAllPcs();

            return Ok();
        }


        /// <summary>
        /// Stops all power conversion systems (PCS) that are not in maintenance mode.
        /// </summary>
        /// <returns>Task.</returns>
        [HttpPost]
        public async Task<IActionResult> StopAllPcs()
        {
            await _deviceServer.StopAllPcs();

            return Ok();
        }
    }
}
