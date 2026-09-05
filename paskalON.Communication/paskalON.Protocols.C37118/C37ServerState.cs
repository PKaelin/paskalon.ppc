// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118
{
    /// <summary>
    /// C37 server state.
    /// </summary>
    public enum C37ServerState
    {
        /// <summary>
        /// C37 server is disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// C37 server is connected.
        /// </summary>
        Connected,
        /// <summary>
        /// C37 server is streaming.
        /// </summary>
        Streaming,
        /// <summary>
        /// C37 server is paused.
        /// </summary>
        Paused
    }
}
