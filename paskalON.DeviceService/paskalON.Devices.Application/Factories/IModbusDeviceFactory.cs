// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus;

namespace paskalON.Devices.Application.Factories
{
    /// <summary>
    /// Modbus device factory interface definition.
    /// </summary>
    public interface IModbusDeviceFactory
    {
        /// <summary>
        /// Create an IModbusDataface and IModbusClient.
        /// </summary>
        /// <returns>The IModbusDataface and IModbusClient implementation.</returns>
        (IModbusDataface Dataface, IModbusClient Client) Create(ModbusConfig config);
    }
}
