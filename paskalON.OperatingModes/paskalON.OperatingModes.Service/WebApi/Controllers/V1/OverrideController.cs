// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.OperatingModes.Application;
using paskalON.OperatingModes.Service.Dto.V1.Requests;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Service.Controllers.V1
{
    /// <summary>
    /// Controller for managing the override modes (e.g. maintenance) of the system.
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class OverrideController : ControllerBase
    {
        /// <summary>
        /// The logger instance for logging information and errors.
        /// </summary>
        private readonly ILogger<OverrideController> _logger;


        /// <summary>
        /// Operating mode manager instance for managing operating modes.
        /// </summary>
        private readonly IOperatingModeManager _manager;


        /// <summary>
        /// Constructor of <see cref="OverrideController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="manager">The operating mode manager instance for managing operating modes.</param>
        public OverrideController(ILogger<OverrideController> logger, IOperatingModeManager manager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);

            _logger = logger;
            _manager = manager;
        }


        /// <summary>
        /// Activates an override mode.
        /// </summary>
        /// <param name="request">The request containing the override mode name.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult ActivateOperatingMode(ActivateOverrideRequest request)
        {
            _manager.OverrideManager.ActivateOperatingMode(request.OperatingModeName);

            return Ok();
        }


        /// <summary>
        /// Deactivates an override mode.
        /// </summary>
        /// <param name="request">The request containing the override mode name.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult DeactivateOperatingMode(DeactivateOverrideRequest request)
        {
            _manager.OverrideManager.DeactivateOperatingMode(request.OperatingModeName);

            return Ok();
        }


        /// <summary>
        /// Sets the operator setpoints of a maintenance override mode.
        /// </summary>
        /// <param name="request">The request containing the override mode name and the setpoints.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult SetSetpoint(SetOverrideSetpointRequest request)
        {
            _manager.OverrideManager.SetSetpoint(request.OperatingModeName, new ActivePower(request.ActivePowerWatts), new ReactivePower(request.ReactivePowerVars));

            return Ok();
        }
    }
}
