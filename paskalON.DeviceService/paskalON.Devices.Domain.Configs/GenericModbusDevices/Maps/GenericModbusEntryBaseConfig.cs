// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Base class for generic Modbus point base and register base configurations.
    /// </summary>
    public abstract class GenericModbusEntryBaseConfig : NameBase
    {
        /// <summary>
        /// Parent relationship to GenericModbusMapConfig Id.
        /// </summary>
        public int GenericModbusMapConfigId { get; set; }

        /// <summary>
        /// Parent relationship to GenericModbusMapConfig.
        /// </summary>
        public required GenericModbusMapConfig GenericModbusMapConfig { get; set; }


        /// <summary>
        /// Indicates the Modbus value type of the point (e.g., Coil, Discrete Input, Input Register, Holding Register).
        /// </summary>
        public abstract ModbusRegistryType ModbusRegistryType { get; }


        /// <summary>
        /// Modbus point register format.
        /// </summary>
        public ModbusDataType ModbusDataType { get; set; }


        /// <summary>
        /// The Modbus address number of the point (e.g., 0-65535 for coils and registers).
        /// </summary>
        public ushort ModbusNumber { get; set; }


        /// <summary>
        /// Indicates the interval for the point, which can be used to group points for different polling intervals or priorities.
        /// </summary>
        public int PollingInterval { get; set; } = 3;
    }
}
