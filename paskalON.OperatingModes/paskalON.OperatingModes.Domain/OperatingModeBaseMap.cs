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
        /// <remarks>
        /// If not needed for the operating mode set it to: AvailableActivePower = () => null;
        /// If PCS need minimum power to start before the operating mode should start subtract them in the function.
        /// </remarks>
        public required Func<ActivePower?> AvailableActivePower { get; set; }


        /// <summary>
        /// Register function that gets the available reactive power.
        /// </summary>
        /// <remarks>
        /// If not needed for the operating mode set it to: AvailableReactivePower = () => null;
        /// </remarks>
        public required Func<ReactivePower?> AvailableReactivePower { get; set; }
    }
}
