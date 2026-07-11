// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Coil map entry for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read/WriteData
    /// Type: 1-bit (Boolean: 1 or 0 / ON or OFF)
    /// Typical Use: Digital outputs used to trigger external actions (e.g. turning a motor on, opening a valve).
    /// </remarks>
    public class GenericModbusCoilPointConfig : GenericModbusPointBaseConfig
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ModbusRegistryType ModbusRegistryType { get => ModbusRegistryType.Coil; }

    }
}
