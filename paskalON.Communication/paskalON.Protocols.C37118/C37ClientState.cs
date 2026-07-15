// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118
{
    /// <summary>
    /// C37 client state.
    /// </summary>
    public enum C37ClientState
    {
        /// <summary>
        /// C37 client is disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// C37 client is connecting.
        /// </summary>
        Connecting,
        /// <summary>
        /// C37 client is connected.
        /// </summary>
        Connected,
        /// <summary>
        /// C37 client is disconnecting.
        /// </summary>
        Disconnecting,
    }
}
