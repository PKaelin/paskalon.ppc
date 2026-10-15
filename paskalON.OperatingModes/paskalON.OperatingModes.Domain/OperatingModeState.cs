// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Operating mode states to track the state of the operating mode.
    /// </summary>
    public enum OperatingModeState
    {
        /// <summary>
        /// Operating mode is disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Operating mode is enabling.
        /// </summary>
        Enabling = 1,

        /// <summary>
        /// Operating mode is ramping to target and therefore not fully enabled.
        /// </summary>
        RampingToEnabled = 2,

        /// <summary>
        /// Operating mode is at the expected set point(s) and is therefore fully enabled.
        /// </summary>
        Enabled = 3,

        /// <summary>
        /// Operating mode is removed and no longer activated, but its contribution is still ramping to zero.
        /// </summary>
        /// <remarks>
        /// Set while the transition segment of the removed operating mode is running in the system operating mode control.
        /// </remarks>
        RampingToDisabled = 4
    }
}
