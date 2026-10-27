// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Dto.Overrides;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.OperatingModes.Dto.Stacks;
using paskalON.OperatingModes.Service.Dto.V1.Requests;

namespace paskalON.OperatingModes.Client
{
    /// <summary>
    /// Operating mode server for calling the operating mode service web API.
    /// </summary>
    /// <remarks>
    /// A request rejected by the operating mode service throws a <see cref="HttpRequestException"/> with its status code.
    /// </remarks>
    public interface IOperatingModeServer
    {
        /// <summary>
        /// Gets all stackable operating modes that can be added to the stack.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The stack DTOs of the available operating modes.</returns>
        Task<IReadOnlyList<OperatingModeStackDto>> GetAvailableOperatingModes(CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the operating modes of the stack in ascending priority order.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The stack DTOs of the active operating modes with their priorities.</returns>
        Task<IReadOnlyList<OperatingModeStackDto>> GetActiveOperatingModes(CancellationToken cancellationToken = default);


        /// <summary>
        /// Adds an operating mode to the stack.
        /// </summary>
        /// <param name="request">The request containing the operating mode name and priority.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task AddOperatingMode(AddOperatingModeStackRequest request, CancellationToken cancellationToken = default);


        /// <summary>
        /// Moves an operating mode of the stack to a new priority.
        /// </summary>
        /// <param name="request">The request containing the operating mode name and the new priority.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task MoveOperatingMode(MoveOperatingModeStackRequest request, CancellationToken cancellationToken = default);


        /// <summary>
        /// Removes an operating mode from the stack.
        /// </summary>
        /// <param name="request">The request containing the operating mode name.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task RemoveOperatingMode(RemoveOperatingModeStackRequest request, CancellationToken cancellationToken = default);


        /// <summary>
        /// Replaces an operating mode of the stack with another operating mode at the same priority in one atomic change.
        /// </summary>
        /// <param name="request">The request containing the replaced and the replacing operating mode names.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task ReplaceOperatingMode(ReplaceOperatingModeStackRequest request, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the supervisory setpoints of the system.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The supervisory setpoints of the system.</returns>
        Task<SystemSetpointDto> GetSystemSetpoint(CancellationToken cancellationToken = default);


        /// <summary>
        /// Sets the supervisory setpoints of the system.
        /// </summary>
        /// <param name="request">The request containing the active and reactive power setpoints.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task SetSystemSetpoint(SetSystemSetpointRequest request, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the active override operating modes.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>The DTOs of the active override operating modes.</returns>
        Task<IReadOnlyList<OperatingModeOverrideDto>> GetActiveOverrideOperatingModes(CancellationToken cancellationToken = default);


        /// <summary>
        /// Activates an override operating mode.
        /// </summary>
        /// <param name="request">The request containing the name of the override operating mode.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task ActivateOverrideOperatingMode(ActivateOverrideRequest request, CancellationToken cancellationToken = default);


        /// <summary>
        /// Deactivates an override operating mode.
        /// </summary>
        /// <param name="request">The request containing the name of the override operating mode.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task DeactivateOverrideOperatingMode(DeactivateOverrideRequest request, CancellationToken cancellationToken = default);
    }
}
