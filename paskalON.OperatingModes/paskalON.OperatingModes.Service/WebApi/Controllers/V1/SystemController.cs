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
        /// Constructor of <see cref="SystemController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="manager">Operating mode manager instance for managing operating modes.</param>
        public SystemController(ILogger<SystemController> logger, IOperatingModeManager manager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);

            _logger = logger;
            _manager = manager;
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
    }
}
