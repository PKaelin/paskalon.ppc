// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
