// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
        /// Register function that gets the available active power range.
        /// </summary>
        /// <remarks>
        /// The minimum (&lt;= 0) is the charge/consumption limit, the maximum (&gt;= 0) the discharge/production limit.
        /// The target is limited to this range, e.g. an empty battery has the range [-charge limit, 0] and can still be charged.
        /// If not needed for the operating mode set it to: AvailableActivePower = () => null;
        /// If PCS need minimum power to start before the operating mode should start subtract them in the function.
        /// If ramp should not start before the resources are ready return null or a range of 0.
        /// </remarks>
        public required Func<PowerRange<ActivePower>?> AvailableActivePower { get; set; }


        /// <summary>
        /// Register function that gets the available reactive power.
        /// </summary>
        /// <remarks>
        /// If not needed for the operating mode set it to: AvailableReactivePower = () => null;
        /// If ramp should not start before the resources are ready return null or 0 available power.
        /// </remarks>
        public required Func<ReactivePower?> AvailableReactivePower { get; set; }
    }
}
