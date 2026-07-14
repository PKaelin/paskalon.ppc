// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Modbus client state.
    /// </summary>
    public enum ModbusClientState
    {
        /// <summary>
        /// Modbus client is disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// Modbus client is connecting.
        /// </summary>
        Connecting,
        /// <summary>
        /// Modbus client is connected.
        /// </summary>
        Connected,
        /// <summary>
        /// Modbus client is disconnecting.
        /// </summary>
        Disconnecting,
    }
}
