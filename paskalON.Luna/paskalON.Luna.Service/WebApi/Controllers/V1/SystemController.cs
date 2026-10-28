// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Luna.Application;
using paskalON.Luna.Dto.Systems;

namespace paskalON.Luna.Service.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller for the system commands start, stop (shutdown) and clear shutdown.
    /// </summary>
    /// <remarks>
    /// A request rejected by the operating mode or device service is answered with its status code.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<SystemController> _logger;


        /// <summary>
        /// System manager executing the system commands.
        /// </summary>
        private readonly ILunaSystemManager _systemManager;


        /// <summary>
        /// System nameplate ratings of the system configuration.
        /// </summary>
        private readonly SystemNameplateDto _nameplate;


        /// <summary>
        /// Constructor of <see cref="SystemController"/>.
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="systemManager">System manager executing the system commands.</param>
        /// <param name="nameplate">System nameplate ratings of the system configuration.</param>
        public SystemController(ILogger<SystemController> logger, ILunaSystemManager systemManager, SystemNameplateDto nameplate)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(systemManager);
            ArgumentNullException.ThrowIfNull(nameplate);

            _logger = logger;
            _systemManager = systemManager;
            _nameplate = nameplate;
        }


        /// <summary>
        /// Gets the system nameplate ratings (minimum and maximum active and reactive power).
        /// </summary>
        /// <returns>The system nameplate ratings.</returns>
        [HttpGet]
        public ActionResult<SystemNameplateDto> GetNameplate()
        {
            return Ok(_nameplate);
        }


        /// <summary>
        /// Starts all power conversion systems (PCS). Rejected with 409 Conflict while a shutdown is active.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The outcome of the command.</returns>
        [HttpPost]
        public async Task<ActionResult<SystemCommandResponseDto>> SystemStartCommand(CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                SystemCommandResult result = await _systemManager.StartAsync(cancellationToken);

                return result switch
                {
                    SystemCommandResult.ShutdownActive => Conflict(CreateResponse("A shutdown is in progress. Clear the shutdown before starting the system.")),
                    _ => Ok(CreateResponse("Start command sent to all PCS."))
                };
            });
        }


        /// <summary>
        /// Shuts the system down by activating the shutdown override operating mode. Succeeds as well if it is already active.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The outcome of the command.</returns>
        [HttpPost]
        public async Task<ActionResult<SystemCommandResponseDto>> SystemStopCommand(CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                SystemCommandResult result = await _systemManager.StopAsync(cancellationToken);

                return result switch
                {
                    SystemCommandResult.ShutdownActive => Ok(CreateResponse("The system is already shutting down.")),
                    _ => Ok(CreateResponse("System shutdown started."))
                };
            });
        }


        /// <summary>
        /// Clears the shutdown by deactivating the shutdown override operating mode. Succeeds as well if no shutdown is active.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The outcome of the command.</returns>
        [HttpPost]
        public async Task<ActionResult<SystemCommandResponseDto>> ClearShutdownCommand(CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                SystemCommandResult result = await _systemManager.ClearShutdownAsync(cancellationToken);

                return result switch
                {
                    SystemCommandResult.ShutdownNotActive => Ok(CreateResponse("No shutdown is active.")),
                    _ => Ok(CreateResponse("Shutdown cleared."))
                };
            });
        }


        /// <summary>
        /// Creates the response of a system command.
        /// </summary>
        /// <param name="message">Message describing the outcome of the system command.</param>
        /// <returns>The response DTO.</returns>
        private static SystemCommandResponseDto CreateResponse(string message)
        {
            return new SystemCommandResponseDto { Message = message };
        }


        /// <summary>
        /// Executes a system command.
        /// </summary>
        /// <param name="command">The system command.</param>
        /// <returns>The result of the command or the status code of the service that rejected the request.</returns>
        private async Task<ActionResult<SystemCommandResponseDto>> Execute(Func<Task<ActionResult<SystemCommandResponseDto>>> command)
        {
            try
            {
                return await command.Invoke();
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                _logger.LogWarning(ex, "A service rejected the system command with status code {StatusCode}.", ex.StatusCode);

                return StatusCode((int)ex.StatusCode.Value, CreateResponse($"The system command was rejected with status code {(int)ex.StatusCode.Value}."));
            }
        }
    }
}
