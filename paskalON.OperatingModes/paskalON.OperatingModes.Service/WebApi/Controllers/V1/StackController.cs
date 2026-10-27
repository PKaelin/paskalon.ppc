// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.OperatingModes.Application;
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Dto.Stacks;
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
        /// Factory that creates the stack DTOs of the operating modes.
        /// </summary>
        private readonly IOperatingModeStackDtoFactory _dtoFactory;


        /// <summary>
        /// Constroctor of <see cref="StackController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="manager">The operating mode manager instance for managing operating modes.</param>
        /// <param name="dtoFactory">Factory that creates the stack DTOs of the operating modes.</param>
        public StackController(ILogger<StackController> logger, IOperatingModeManager manager, IOperatingModeStackDtoFactory dtoFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(dtoFactory);

            _logger = logger;
            _manager = manager;
            _dtoFactory = dtoFactory;
        }


        /// <summary>
        /// Gets all stackable operating modes that can be added to the stack.
        /// </summary>
        /// <returns>The stack DTOs of the available operating modes, specialized per operating mode or <see cref="OperatingModeStackDto"/> as default.</returns>
        [HttpGet]
        public ActionResult<IReadOnlyList<OperatingModeStackDto>> GetAvailableOperatingModes()
        {
            IReadOnlyList<OperatingModeStackDto> dtos = _dtoFactory.Create(_manager.StackManager.AvailableOperatingModes.OrderBy(m => m.Name));

            return Ok(dtos);
        }


        /// <summary>
        /// Gets the operating modes of the stack in ascending priority order.
        /// </summary>
        /// <returns>The stack DTOs of the active operating modes with their priorities, specialized per operating mode or <see cref="OperatingModeStackDto"/> as default.</returns>
        [HttpGet]
        public ActionResult<IReadOnlyList<OperatingModeStackDto>> GetActiveOperatingModes()
        {
            IReadOnlyList<OperatingModeStackDto> dtos = _dtoFactory.Create(_manager.StackManager.ActiveOperatingModeEntries);

            return Ok(dtos);
        }


        /// <summary>
        /// Adds a new operating mode to the stack with the specified name and priority.
        /// </summary>
        /// <param name="request">The request containing the operating mode name and priority.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult AddOperatingMode(AddOperatingModeStackRequest request)
        {
            _manager.StackManager.AddOperatingMode(request.OperatingModeName, request.Priority);

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
            _manager.StackManager.MoveOperatingMode(request.OperatingModeName, request.Priority);

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
            _manager.StackManager.RemoveOperatingMode(request.OperatingModeName);

            return Ok();
        }


        /// <summary>
        /// Replaces an operating mode of the stack with another operating mode at the same priority in one atomic change.
        /// </summary>
        /// <param name="request">The request containing the replaced and the replacing operating mode names.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public IActionResult ReplaceOperatingMode(ReplaceOperatingModeStackRequest request)
        {
            _manager.StackManager.ReplaceOperatingMode(request.PredecessorName, request.OperatingModeName);

            return Ok();
        }


        /// <summary>
        /// Sets the whole operating mode stack in one atomic change. Only the resulting stack is validated.
        /// </summary>
        /// <param name="request">The request containing the operating mode names in ascending priority order.</param>
        /// <returns>Ok, or 400 Bad Request with the reason if the stack is invalid or contains an unknown operating mode.</returns>
        [HttpPost]
        public IActionResult SetOperatingModes(SetOperatingModeStackRequest request)
        {
            try
            {
                _manager.StackManager.SetOperatingModes(request.OperatingModeNames);

                return Ok();
            }
            catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
            {
                _logger.LogWarning(ex, "Operating mode stack rejected.");

                return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Invalid operating mode stack");
            }
        }
    }
}
