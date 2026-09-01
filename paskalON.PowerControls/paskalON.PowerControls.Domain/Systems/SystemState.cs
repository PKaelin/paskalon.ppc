// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Systems
{
    public enum SystemState
    {
        /// <summary>
        /// The system is stopped and cannot produce or consume power.
        /// </summary>
        Stopped = 0,
        /// <summary>
        /// The system is started and can actively produce or consume power.
        /// </summary>
        Started = 1,
    }
}
