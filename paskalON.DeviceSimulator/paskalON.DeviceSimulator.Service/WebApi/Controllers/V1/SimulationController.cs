// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.DeviceSimulator.Service.Dto.Pcs;

namespace paskalON.DeviceSimulator.Service.WebApi.Controllers.V1
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class SimulationController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<SimulationController> _logger;


        public SimulationController(ILogger<SimulationController> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> SlowDownPower(SlowDownPowerRequest request)
        {
            return Ok();
        }
    }
}
