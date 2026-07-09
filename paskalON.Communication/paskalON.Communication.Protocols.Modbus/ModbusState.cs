// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Communication.Protocols.Modbus
{
    /// <summary>
    /// Modbus state.
    /// </summary>
    public enum ModbusState
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting,
    }
}
