// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.OpenModes
{
    /// <summary>
    /// Input mapping class for maintenance mode.
    /// </summary>
    public class MaintenanceModeMap
    {
        /// <summary>
        /// Gets the active available power.
        /// </summary>
        public required Func<ActivePower?> ActiveAvailablePower { get; set; }


        /// <summary>
        /// Gets the reactive available power.
        /// </summary>
        public required Func<ReactivePower?> ReactiveAvailablePower { get; set; }
    }
}
