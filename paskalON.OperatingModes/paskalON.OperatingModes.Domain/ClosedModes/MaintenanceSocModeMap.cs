// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.ClosedModes
{
    /// <summary>
    /// Input mapping class for maintenance SOC mode.
    /// </summary>
    public class MaintenanceSocModeMap : OperatingModeBaseMap
    {
        /// <summary>
        /// State of charge map.
        /// </summary>
        public required Func<double> StateOfCharge { get; set; }
    }
}
