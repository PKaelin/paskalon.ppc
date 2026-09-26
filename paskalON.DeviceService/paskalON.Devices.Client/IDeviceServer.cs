// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;

namespace paskalON.Devices.Client
{
    /// <summary>
    /// Device server for calling the device service web API.
    /// </summary>
    public interface IDeviceServer
    {
        /// <summary>
        /// Gets the DER DTO root object and all its content.
        /// </summary>
        /// <returns>The DER DTO root object and all its content.</returns>
        Task<DerDto> GetDer();


        /// <summary>
        /// Sends a start command to a specific power conversion system (PCS).
        /// </summary>
        /// <param name="deviceId">Device identifier of the PCS to start.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task StartPcs(int deviceId, CancellationToken cancellationToken = default);


        /// <summary>
        /// Sends a start command to all power conversion systems (PCS) that are not in maintenance mode.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task StartAllPcs(CancellationToken cancellationToken = default);


        /// <summary>
        /// Sends a standby command to all power conversion systems (PCS) that are not in maintenance mode.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task StandbyAllPcs(CancellationToken cancellationToken = default);


        /// <summary>
        /// Sends a stop command to all power conversion systems (PCS) that are not in maintenance mode.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>

        Task StopAllPcs(CancellationToken cancellationToken = default);


        /// <summary>
        /// Sends active and reactive power targets to a specific power conversion system (PCS).
        /// </summary>
        /// <param name="deviceId">Device identifier of the PCS to set the targets for.</param>
        /// <param name="activePowerWatt">Active power target in watts.</param>
        /// <param name="reactivePowerVar">Reactive power target in var.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>Task.</returns>
        Task SetPcsPowerTarget(int deviceId, double activePowerWatt, double reactivePowerVar, CancellationToken cancellationToken = default);
    }
}
