// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Discrete input registers for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read-OnlyData
    /// Type: 1-bit (Boolean)
    /// Typical Use: Digital inputs originating from the field (e.g. a limit switch, door alarm, or emergency stop status).
    /// </remarks>
    public class GenericModbusDiscreteInputPointConfig : GenericModbusPointBaseConfig
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ModbusRegistryType ModbusRegistryType { get => ModbusRegistryType.DiscreteInput; }

    }
}
