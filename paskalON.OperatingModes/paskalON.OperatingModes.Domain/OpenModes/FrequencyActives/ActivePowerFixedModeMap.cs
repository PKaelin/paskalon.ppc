// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.OpenModes.FrequencyActives
{
    /// <summary>
    /// Input mapping class for active power fixed mode.
    /// </summary>
    public class ActivePowerFixedModeMap
    {
        /// <summary>
        /// Gets the active available power.
        /// </summary>
        public required Func<ActivePower?> ActiveAvailablePower { get; set; }
    }
}
