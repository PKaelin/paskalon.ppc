// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    public interface IOperatingClosedMode : IOperatingMode
    {
        /// <summary>
        /// Error adjustment calculated from input and used to make adjustments in real time.
        /// </summary>
        ActivePower ErrorAdjustmentActive { get; }


        /// <summary>
        /// Error adjustment calculated from input and used to make adjustments in real time.
        /// </summary>
        ReactivePower ErrorAdjustmentReactive { get; }


        /// <summary>
        /// Calculates the operating modes power target.
        /// </summary>
        Task CalculateAsync(CancellationToken cancellationToken = default);
    }
}
