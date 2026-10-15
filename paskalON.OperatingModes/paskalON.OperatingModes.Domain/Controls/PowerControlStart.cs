// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Defines how a power control establishes its target when it is activated and how it follows target setpoint changes.
    /// </summary>
    public enum PowerControlStart
    {
        /// <summary>
        /// The target is set to the target setpoint immediately, at the activation and at every target setpoint change.
        /// </summary>
        /// <remarks>
        /// Used by stack operating modes. The power control does not ramp, the system operating mode control transitions the
        /// applied target with the ramp of the operating mode (one ramp layer).
        /// </remarks>
        AtTargetSetpoint,


        /// <summary>
        /// The target ramps from the current target to the target setpoint, at the activation and at every target setpoint change.
        /// </summary>
        /// <remarks>
        /// Used by override operating modes whose targets are applied directly to a DER unit without a system transition.
        /// </remarks>
        RampFromCurrentTarget,
    }
}
