// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.OperatingModes.Client;
using paskalON.OperatingModes.Dto.Overrides;
using paskalON.OperatingModes.Service.Dto.V1.Requests;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna system manager to start, stop (shut down) and clear the shutdown of the system.
    /// </summary>
    /// <remarks>
    /// The shutdown is the shutdown override operating mode of the operating mode service. A request rejected by the
    /// operating mode or device service throws a <see cref="HttpRequestException"/> with its status code.
    /// </remarks>
    public class LunaSystemManager : ILunaSystemManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<LunaSystemManager> _logger;


        /// <summary>
        /// Operating mode server for calling the operating mode service API.
        /// </summary>
        private readonly IOperatingModeServer _operatingModeServer;


        /// <summary>
        /// Device server for calling the device service API.
        /// </summary>
        private readonly IDeviceServer _deviceServer;


        /// <summary>
        /// Name of the shutdown override operating mode.
        /// </summary>
        private readonly string _shutdownOperatingModeName;


        /// <summary>
        /// Constructor of <see cref="LunaSystemManager"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="operatingModeServer">Operating mode server for calling the operating mode service API.</param>
        /// <param name="deviceServer">Device server for calling the device service API.</param>
        /// <param name="shutdownOperatingModeName">Name of the shutdown override operating mode.</param>
        public LunaSystemManager(ILogger<LunaSystemManager> logger, IOperatingModeServer operatingModeServer, IDeviceServer deviceServer, string shutdownOperatingModeName)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(operatingModeServer);
            ArgumentNullException.ThrowIfNull(deviceServer);
            ArgumentException.ThrowIfNullOrWhiteSpace(shutdownOperatingModeName);

            _logger = logger;
            _operatingModeServer = operatingModeServer;
            _deviceServer = deviceServer;
            _shutdownOperatingModeName = shutdownOperatingModeName;
        }


        /// <inheritdoc/>
        public async Task<SystemCommandResult> StartAsync(CancellationToken cancellationToken = default)
        {
            if (await IsShutdownActive(cancellationToken))
            {
                _logger.LogWarning("System start rejected, shutdown '{ShutdownOperatingModeName}' is active.", _shutdownOperatingModeName);

                return SystemCommandResult.ShutdownActive;
            }

            await _deviceServer.ConnectAllMeters(cancellationToken);
            _logger.LogInformation("System connect command sent to all power meters.");
            await _deviceServer.StartAllPcs(cancellationToken);
            _logger.LogInformation("System start command sent to all PCS.");

            return SystemCommandResult.Executed;
        }


        /// <inheritdoc/>
        public async Task<SystemCommandResult> StopAsync(CancellationToken cancellationToken = default)
        {
            if (await IsShutdownActive(cancellationToken))
            {
                _logger.LogInformation("System stop not required, shutdown '{ShutdownOperatingModeName}' is already active.", _shutdownOperatingModeName);

                return SystemCommandResult.ShutdownActive;
            }

            await _operatingModeServer.ActivateOverrideOperatingMode(new ActivateOverrideRequest { OperatingModeName = _shutdownOperatingModeName }, cancellationToken);
            _logger.LogInformation("System shutdown '{ShutdownOperatingModeName}' activated.", _shutdownOperatingModeName);

            return SystemCommandResult.Executed;
        }


        /// <inheritdoc/>
        public async Task<SystemCommandResult> ClearShutdownAsync(CancellationToken cancellationToken = default)
        {
            if (await IsShutdownActive(cancellationToken) is false)
            {
                _logger.LogInformation("Clear shutdown not required, shutdown '{ShutdownOperatingModeName}' is not active.", _shutdownOperatingModeName);

                return SystemCommandResult.ShutdownNotActive;
            }

            await _operatingModeServer.DeactivateOverrideOperatingMode(new DeactivateOverrideRequest { OperatingModeName = _shutdownOperatingModeName }, cancellationToken);
            _logger.LogInformation("System shutdown '{ShutdownOperatingModeName}' deactivated.", _shutdownOperatingModeName);

            return SystemCommandResult.Executed;
        }


        /// <summary>
        /// Checks whether the shutdown override operating mode is active.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>True if the shutdown override operating mode is active.</returns>
        private async Task<bool> IsShutdownActive(CancellationToken cancellationToken)
        {
            IReadOnlyList<OperatingModeOverrideDto> overrides = await _operatingModeServer.GetActiveOverrideOperatingModes(cancellationToken);

            return overrides.Any(dto => string.Equals(dto.OperatingModeName, _shutdownOperatingModeName, StringComparison.Ordinal));
        }
    }
}
