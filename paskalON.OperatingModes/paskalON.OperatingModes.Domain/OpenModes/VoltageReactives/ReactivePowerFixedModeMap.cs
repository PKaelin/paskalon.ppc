// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.OpenModes.VoltageReactives
{
    /// <summary>
    /// Input mapping class for reactive power fixed mode.
    /// </summary>
    public class ReactivePowerFixedModeMap
    {
        /// <summary>
        /// Gets the reactive available power.
        /// </summary>
        public required Func<ReactivePower?> ReactiveAvailablePower { get; set; }
    }
}
