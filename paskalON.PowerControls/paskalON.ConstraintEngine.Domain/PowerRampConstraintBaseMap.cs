// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain
{
    /// <summary>
    /// Power ramp constraint base map.
    /// </summary>
    public abstract class PowerRampConstraintBaseMap : ConstraintBaseMap
    {
        /// <summary>
        /// Register function that gets the active power target currently applied by the devices.
        /// </summary>
        /// <remarks>
        /// The first ramp after a (re)start of the power control starts from this target instead of zero. Return null if not available.
        /// </remarks>
        public Func<ActivePower?> CurrentActivePowerTarget { get; init; } = () => null;


        /// <summary>
        /// Register function that gets the reactive power target currently applied by the devices.
        /// </summary>
        /// <remarks>
        /// The first ramp after a (re)start of the power control starts from this target instead of zero. Return null if not available.
        /// </remarks>
        public Func<ReactivePower?> CurrentReactivePowerTarget { get; init; } = () => null;
    }
}
