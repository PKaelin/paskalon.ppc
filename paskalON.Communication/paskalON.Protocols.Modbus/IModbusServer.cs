// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Protocols.Modbus.Converters;

namespace paskalON.Protocols.Modbus
{
    public interface IModbusServer : IModbusDataConverter
    {
        // TODO: Implement IModbusServer
        public ModbusServerState State { get; }
    }
}
