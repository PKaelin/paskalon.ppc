// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Transitions
{
    /// <summary>
    /// Kind of transition of a power axis caused by a change of the operating modes.
    /// </summary>
    public enum TransitionKind
    {
        /// <summary>
        /// No transition is running, the applied target is the objective.
        /// </summary>
        None = 0,

        /// <summary>
        /// Operating modes present from startup. The transition starts from the measured power at the POI.
        /// </summary>
        InitialActivation = 1,

        /// <summary>
        /// An operating mode was added while running.
        /// </summary>
        ModeAdded = 2,

        /// <summary>
        /// A base or exclusive operating mode was replaced by another one while running.
        /// </summary>
        ModeReplacement = 3,

        /// <summary>
        /// An operating mode was removed while running.
        /// </summary>
        ModeRemoved = 4,

        /// <summary>
        /// The target setpoint of an operating mode changed while running (setpoint or available power change).
        /// </summary>
        TargetChanged = 5,

        /// <summary>
        /// The operating mode control resumes after holding. The applied target ramps from the held setpoint to the objective.
        /// </summary>
        Resume = 6,
    }
}
