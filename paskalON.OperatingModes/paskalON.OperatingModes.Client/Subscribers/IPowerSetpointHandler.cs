// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Dto.Powers;

namespace paskalON.OperatingModes.Client.Subscribers
{
    /// <summary>
    /// Handles the power setpoints received by the <see cref="IPowerSetpointSubscriber"/>. Implemented by each service consuming the power setpoints.
    /// </summary>
    public interface IPowerSetpointHandler
    {
        /// <summary>
        /// Handles the received system power setpoints.
        /// </summary>
        /// <param name="setpoints">The received system power setpoints.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task HandleSetpointsAsync(CurrentSetpointsDto setpoints);


        /// <summary>
        /// Handles the received DER unit power setpoints.
        /// </summary>
        /// <param name="setpoints">The received DER unit power setpoints.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task HandleDerUnitSetpointsAsync(CurrentDerUnitSetpointsDto setpoints);
    }
}
