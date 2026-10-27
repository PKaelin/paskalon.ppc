// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.OperatingModes.Client;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.OperatingModes.Dto.Stacks;
using paskalON.OperatingModes.Service.Dto.V1.Requests;

namespace paskalON.Luna.Service.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller forwarding the operating mode requests to the operating mode service API.
    /// </summary>
    /// <remarks>
    /// A request rejected by the operating mode service is answered with its status code.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class OperatingModeController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<OperatingModeController> _logger;


        /// <summary>
        /// Operating mode server for calling the operating mode service API.
        /// </summary>
        private readonly IOperatingModeServer _operatingModeServer;


        /// <summary>
        /// Constructor of <see cref="OperatingModeController"/>.
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="operatingModeServer">Operating mode server for calling the operating mode service API.</param>
        public OperatingModeController(ILogger<OperatingModeController> logger, IOperatingModeServer operatingModeServer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(operatingModeServer);

            _logger = logger;
            _operatingModeServer = operatingModeServer;
        }


        /// <summary>
        /// Gets all stackable operating modes that can be added to the stack.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The stack DTOs of the available operating modes.</returns>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OperatingModeStackDto>>> GetAvailableOperatingModes(CancellationToken cancellationToken)
        {
            return await ForwardResult(() => _operatingModeServer.GetAvailableOperatingModes(cancellationToken));
        }


        /// <summary>
        /// Gets the operating modes of the stack in ascending priority order.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The stack DTOs of the active operating modes with their priorities.</returns>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OperatingModeStackDto>>> GetActiveOperatingModes(CancellationToken cancellationToken)
        {
            return await ForwardResult(() => _operatingModeServer.GetActiveOperatingModes(cancellationToken));
        }


        /// <summary>
        /// Adds a new operating mode to the stack with the specified name and priority.
        /// </summary>
        /// <param name="request">The request containing the operating mode name and priority.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> AddOperatingMode(AddOperatingModeStackRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _operatingModeServer.AddOperatingMode(request, cancellationToken));
        }


        /// <summary>
        /// Moves an existing operating mode in the stack to a new priority.
        /// </summary>
        /// <param name="request">The request containing the operating mode name and the new priority.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> MoveOperatingMode(MoveOperatingModeStackRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _operatingModeServer.MoveOperatingMode(request, cancellationToken));
        }


        /// <summary>
        /// Removes an existing operating mode from the stack based on the specified name.
        /// </summary>
        /// <param name="request">The request containing the operating mode name to be removed.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveOperatingMode(RemoveOperatingModeStackRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _operatingModeServer.RemoveOperatingMode(request, cancellationToken));
        }


        /// <summary>
        /// Replaces an operating mode of the stack with another operating mode at the same priority in one atomic change.
        /// </summary>
        /// <param name="request">The request containing the replaced and the replacing operating mode names.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> ReplaceOperatingMode(ReplaceOperatingModeStackRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _operatingModeServer.ReplaceOperatingMode(request, cancellationToken));
        }


        /// <summary>
        /// Sets the whole operating mode stack in one atomic change. The operating mode service validates the resulting stack only.
        /// </summary>
        /// <param name="request">The request containing the operating mode names in ascending priority order.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Ok, or the status code of the operating mode service with its reason as problem details if it rejected the stack.</returns>
        [HttpPost]
        public async Task<IActionResult> SetOperatingModes(SetOperatingModeStackRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _operatingModeServer.SetOperatingModes(request, cancellationToken);

                return Ok();
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                _logger.LogWarning(ex, "Operating mode service rejected the stack with status code {StatusCode}.", ex.StatusCode);

                return Problem(detail: ex.Message, statusCode: (int)ex.StatusCode.Value, title: "Operating mode stack rejected");
            }
        }


        /// <summary>
        /// Gets the supervisory setpoints of the system.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The supervisory setpoints of the system in watts and vars.</returns>
        [HttpGet]
        public async Task<ActionResult<SystemSetpointDto>> GetSetpoint(CancellationToken cancellationToken)
        {
            return await ForwardResult(() => _operatingModeServer.GetSystemSetpoint(cancellationToken));
        }


        /// <summary>
        /// Sets the supervisory setpoints of the system.
        /// </summary>
        /// <param name="request">The request containing the setpoints.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> SetSetpoint(SetSystemSetpointRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _operatingModeServer.SetSystemSetpoint(request, cancellationToken));
        }


        /// <summary>
        /// Activates an override operating mode, e.g. a shutdown or the maintenance of a DER unit.
        /// </summary>
        /// <param name="request">The request containing the name of the override operating mode.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> ActivateOverrideOperatingMode(ActivateOverrideRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _operatingModeServer.ActivateOverrideOperatingMode(request, cancellationToken));
        }


        /// <summary>
        /// Deactivates an override operating mode.
        /// </summary>
        /// <param name="request">The request containing the name of the override operating mode.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> DeactivateOverrideOperatingMode(DeactivateOverrideRequest request, CancellationToken cancellationToken)
        {
            return await Forward(() => _operatingModeServer.DeactivateOverrideOperatingMode(request, cancellationToken));
        }


        /// <summary>
        /// Forwards a request without result to the operating mode service.
        /// </summary>
        /// <param name="request">The request to the operating mode service.</param>
        /// <returns>Ok or the status code of the operating mode service if it rejected the request.</returns>
        private async Task<IActionResult> Forward(Func<Task> request)
        {
            try
            {
                await request.Invoke();

                return Ok();
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                _logger.LogWarning(ex, "Operating mode service rejected the request with status code {StatusCode}.", ex.StatusCode);

                return StatusCode((int)ex.StatusCode.Value);
            }
        }


        /// <summary>
        /// Forwards a request with result to the operating mode service.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="request">The request to the operating mode service.</param>
        /// <returns>The result or the status code of the operating mode service if it rejected the request.</returns>
        private async Task<ActionResult<TResult>> ForwardResult<TResult>(Func<Task<TResult>> request)
        {
            try
            {
                TResult result = await request.Invoke();

                return Ok(result);
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                _logger.LogWarning(ex, "Operating mode service rejected the request with status code {StatusCode}.", ex.StatusCode);

                return StatusCode((int)ex.StatusCode.Value);
            }
        }
    }
}
