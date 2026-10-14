// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Defines how a power control establishes its target when it is activated.
    /// </summary>
    public enum PowerControlStart
    {
        /// <summary>
        /// The target is set to the target setpoint immediately.
        /// </summary>
        /// <remarks>
        /// Used by stack operating modes, the system operating mode control transitions the applied target.
        /// </remarks>
        AtTargetSetpoint,


        /// <summary>
        /// The target ramps from the current target to the target setpoint.
        /// </summary>
        /// <remarks>
        /// Used by override operating modes whose targets are applied directly to a DER unit.
        /// </remarks>
        RampFromCurrentTarget,
    }
}
