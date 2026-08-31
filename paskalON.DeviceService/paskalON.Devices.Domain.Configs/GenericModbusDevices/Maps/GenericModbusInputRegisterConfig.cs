// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Input registers for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read-OnlyData
    /// Type: 16-bit word (Integer)
    /// Typical Use: Analog inputs measuring physical properties (e.g. real-time temperature, pressure, or flow rate readings from a sensor).
    /// </remarks>
    public class GenericModbusInputRegisterConfig : GenericModbusRegisterBaseConfig
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ModbusRegistryType ModbusRegistryType { get => ModbusRegistryType.InputRegister; }

    }
}
