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
        ComplexPower ErrorAdjustment { get; }


        /// <summary>
        /// Gets the adjusted complex power target for the operating mode.
        /// </summary>
        ComplexPower TargetAdjusted { get; }


        /// <summary>
        /// Calculates the operating modes power target.
        /// </summary>
        Task CalculateAsync<TInput>(TInput input, CancellationToken cancellationToken = default) where TInput : class;
    }
}
