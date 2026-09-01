// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;

namespace paskalON.Devices.Domain.GenericModbusDevices.Entries
{
    /// <summary>
    /// Generic Modbus entry base class.
    /// </summary>
    public abstract class GenericModbusEntryBase
    {
        /// <summary>
        /// The generic Modbus entry base configuration.
        /// </summary>
        private readonly GenericModbusEntryBaseConfig _config;


        /// <summary>
        /// Name of the Modbus entry.
        /// </summary>
        public string Name { get => _config.Name; }


        /// <summary>
        /// Indicates the Modbus value type of the point (e.g., Coil, Discrete Input, Input Register, Holding Register).
        /// </summary>
        public ModbusRegistryType ModbusRegistryType { get => _config.ModbusRegistryType; }


        /// <summary>
        /// Modbus point register format.
        /// </summary>
        public ModbusDataType ModbusDataType { get => _config.ModbusDataType; }


        /// <summary>
        /// The Modbus address number of the point (e.g., 0-65535 for coils and registers).
        /// </summary>
        public ushort ModbusNumber { get => _config.ModbusNumber; }


        /// <summary>
        /// Indicates the interval for the point, which can be used to group points for different polling intervals or priorities.
        /// </summary>
        public int PollingInterval { get => _config.PollingInterval; }


        /// <summary>
        /// Constructor of <see cref="GenericModbusEntryBase"/>.
        /// </summary>
        /// <param name="config">The generic Modbus entry base configuration.</param>
        public GenericModbusEntryBase(GenericModbusEntryBaseConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}
