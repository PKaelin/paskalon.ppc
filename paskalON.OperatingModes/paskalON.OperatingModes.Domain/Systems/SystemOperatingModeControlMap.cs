// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Systems
{
    /// <summary>
    /// Input mapping class for the system operating mode control.
    /// </summary>
    public class SystemOperatingModeControlMap
    {
        /// <summary>
        /// Register function that gets the measured active power at the point of interconnection (POI).
        /// </summary>
        /// <remarks>
        /// Used as transition origin at startup when there is no previous applied target. Return null if not available.
        /// </remarks>
        public required Func<ActivePower?> ActivePowerAtPoi { get; set; }


        /// <summary>
        /// Register function that gets the measured reactive power at the point of interconnection (POI).
        /// </summary>
        /// <remarks>
        /// Used as transition origin at startup when there is no previous applied target. Return null if not available.
        /// </remarks>
        public required Func<ReactivePower?> ReactivePowerAtPoi { get; set; }


        /// <summary>
        /// Register function that gets the reason why the operating mode control must hold the last setpoints, e.g. a lost dependency.
        /// </summary>
        /// <remarks>
        /// Return null if the operating mode control may calculate. The default never holds.
        /// </remarks>
        public Func<string?> HoldReason { get; set; } = () => null;
    }
}
