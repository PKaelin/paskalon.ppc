// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Input mapping class for active power mode.
    /// </summary>
    public class ActivePowerModeMap
    {
        /// <summary>
        /// Gets the active power.
        /// </summary>
        public required Func<ActivePower?> ActivePower { get; set; }
    }
}
