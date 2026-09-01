// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Ders
{
    /// <summary>
    /// State of a distributed energy resource (DER).
    /// </summary>
    public enum DerState
    {
        /// <summary>
        /// The DER is stopped and cannot produce or consume power.
        /// </summary>
        Stopped = 0,
        /// <summary>
        /// The DER is started and can actively produce or consume power.
        /// </summary>
        Started = 1,
        /// <summary>
        /// The DER is in standby mode, ready to be started but not actively produce or consume power.
        /// </summary>
        Standby = 3,
        /// <summary>
        /// The DER is in maintenance mode and temporarily unavailable for operation.
        /// </summary>
        Maintenance = 4
    }
}
