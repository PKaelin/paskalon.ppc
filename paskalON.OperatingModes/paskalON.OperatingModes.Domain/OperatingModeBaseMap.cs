// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Input mapping class for maintenance mode.
    /// </summary>
    public class OperatingModeBaseMap
    {

        /// <summary>
        /// Register function that get the available active power.
        /// </summary>
        public required Func<ActivePower?> AvailableActivePower { get; set; }


        /// <summary>
        /// Register function that gets the available reactive power.
        /// </summary>
        public required Func<ReactivePower?> AvailableReactivePower { get; set; }
    }
}
