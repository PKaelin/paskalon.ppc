// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Input mapping class for reactive power mode.
    /// </summary>
    public class ReactivePowerModeMap : OperatingModeBaseMap
    {
        /// <summary>
        /// Reactive power at the POI map.
        /// </summary>
        public required Func<ReactivePower?> ReactivePowerAtPoi { get; set; }
    }
}
