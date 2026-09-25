// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
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
        /// Constructor of <see cref="SystemController"/>.
        /// </summary>
        /// <param name="logger">Logger instance for logging messages.</param>
        /// <param name="manager">Manager for handling power control operations.</param>
        public SystemController(ILogger<SystemController> logger, IPowerControlManager manager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);

            _logger = logger;
            _manager = manager;
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
    }
}
