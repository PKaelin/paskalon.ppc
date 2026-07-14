// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Communication.Protocols.Modbus
{
    /// <summary>
    /// Modbus server state
    /// </summary>
    public enum ModbusServerState
    {
        /// <summary>
        /// Idle state
        /// </summary>
        Idle,
        /// <summary>
        /// Begin listen
        /// </summary>
        BeginListen,
        /// <summary>
        /// Listening state
        /// </summary>
        Listening,
        /// <summary>
        /// Stop listen
        /// </summary>
        StopListen,
    }
}
