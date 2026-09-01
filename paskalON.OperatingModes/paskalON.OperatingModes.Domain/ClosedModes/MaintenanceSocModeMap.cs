// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
