// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.OperatingModes.Application;
using paskalON.OperatingModes.Service.Dto.V1.Requests;

namespace paskalON.OperatingModes.Service.Controllers.V1
{
    /// <summary>
    /// Controller for managing the stack of operating modes in the system.
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class StackController : ControllerBase
    {
        /// <summary>
        /// The logger instance for logging information and errors.
        /// </summary>
        private readonly ILogger<StackController> _logger;


        /// <summary>
        /// Operating mode manager instance for managing operating modes.
        /// </summary>
        private readonly IOperatingModeManager _manager;


        /// <summary>
        /// Constroctor of <see cref="StackController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="manager">The operating mode manager instance for managing operating modes.</param>
        public StackController(ILogger<StackController> logger, IOperatingModeManager manager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);

            _logger = logger;
            _manager = manager;
        }


        /// <summary>
        /// Adds a new operating mode to the stack with the specified name and priority.
        /// </summary>
        /// <param name="request">The request containing the operating mode name and priority.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult AddOperatingMode(AddOperatingModeStackRequest request)
        {
            _manager.AddOperatingMode(request.OperatingModeName, request.Priority);

            return Ok();
        }


        /// <summary>
        /// Moves an existing operating mode in the stack to a new priority.
        /// </summary>
        /// <param name="request">The request containing the operating mode name and the new priority.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult MoveOperatingMode(MoveOperatingModeStackRequest request)
        {
            _manager.MoveOperatingMode(request.OperatingModeName, request.Priority);

            return Ok();
        }


        /// <summary>
        /// Removes an existing operating mode from the stack based on the specified name.
        /// </summary>
        /// <param name="request">The request containing the operating mode name to be removed.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult RemoveOperatingMode(RemoveOperatingModeStackRequest request)
        {
            _manager.RemoveOperatingMode(request.OperatingModeName);

            return Ok();
        }
    }
}
