// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Input mapping class for active power mode.
    /// </summary>
    public class ActivePowerModeMap : OperatingModeBaseMap
    {
        /// <summary>
        /// Active power at the POI map.
        /// </summary>
        public required Func<ActivePower?> ActivePowerAtPoi { get; set; }
    }
}
