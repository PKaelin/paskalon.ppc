// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Devices.Client;
using paskalON.OperatingModes.Application;
using paskalON.OperatingModes.Service.Dto.V1.Requests;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Service.Controllers.V1
{
    /// <summary>
    /// Controller for managing the system setpoints of the operating modes.
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// The logger instance for logging information and errors.
        /// </summary>
        private readonly ILogger<SystemController> _logger;


        /// <summary>
        /// Operating mode manager instance for managing operating modes.
        /// </summary>
        private readonly IOperatingModeManager _manager;


        /// <summary>
        /// Device server for calling the device service web API.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Constructor of <see cref="SystemController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="manager">Operating mode manager instance for managing operating modes.</param>
        /// <param name="deviceServer">Device server for calling the device service web API.</param>
        public SystemController(ILogger<SystemController> logger, IOperatingModeManager manager, IDeviceServer deviceServer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(deviceServer);

            _logger = logger;
            _manager = manager;
            _deviceServer = deviceServer;
        }


        /// <summary>
        /// Sets the operator setpoints of the system.
        /// </summary>
        /// <param name="request">The request containing the setpoints.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult SetSetpoint(SetSystemSetpointRequest request)
        {
            _manager.SetSystemPowerSetpoints(new ActivePower(request.ActivePowerWatts), new ReactivePower(request.ReactivePowerVars));

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
