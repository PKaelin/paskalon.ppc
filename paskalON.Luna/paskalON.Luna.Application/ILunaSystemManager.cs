// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna system manager interface definition.
    /// </summary>
    public interface ILunaSystemManager
    {
        /// <summary>
        /// Starts all power conversion systems (PCS) unless a shutdown is active.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>
        /// <see cref="SystemCommandResult.Executed"/> if the PCS were started,
        /// <see cref="SystemCommandResult.ShutdownActive"/> if a shutdown is active and nothing was started.
        /// </returns>
        Task<SystemCommandResult> StartAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Shuts the system down by activating the shutdown override operating mode unless it is already active.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>
        /// <see cref="SystemCommandResult.Executed"/> if the shutdown was activated,
        /// <see cref="SystemCommandResult.ShutdownActive"/> if the shutdown was already active.
        /// </returns>
        Task<SystemCommandResult> StopAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Clears the shutdown by deactivating the shutdown override operating mode if it is active.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>
        /// <see cref="SystemCommandResult.Executed"/> if the shutdown was deactivated,
        /// <see cref="SystemCommandResult.ShutdownNotActive"/> if no shutdown was active.
        /// </returns>
        Task<SystemCommandResult> ClearShutdownAsync(CancellationToken cancellationToken = default);
    }
}
