// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Modbus server state
    /// </summary>
    public enum ModbusServerState
    {
        /// <summary>
        /// Stopped state.
        /// </summary>
        Stopped,
        /// <summary>
        /// Begin listen.
        /// </summary>
        BeginListen,
        /// <summary>
        /// Listening state.
        /// </summary>
        Listening,
        /// <summary>
        /// Stop listen.
        /// </summary>
        StopListen,
        /// <summary>
        /// Modbus server is faulted.
        /// </summary>
        Faulted
    }
}
