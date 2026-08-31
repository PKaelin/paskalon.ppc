// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Holding registers for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read/WriteData
    /// Type: 16-bit word (Integer)
    /// Typical Use: General storage and system configurations. Often stores setpoints, scaling factors, and calibration parameters.
    /// </remarks>
    public class GenericModbusHoldingRegisterConfig : GenericModbusRegisterBaseConfig
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override ModbusRegistryType ModbusRegistryType { get => ModbusRegistryType.HoldingRegister; }

    }
}
