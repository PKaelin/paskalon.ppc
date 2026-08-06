// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Generic Modbus map configuration for a device, defining the mapping of coils,
    /// discrete inputs, input registers, and holding registers.
    /// </summary>
    public class GenericModbusMapConfig : NameBase
    {
        /// <summary>
        /// The maximum number of analog values that can be configured.
        /// </summary>
        public const int MaxAnalogPointCount = 100;


        /// <summary>
        /// The maximum number of binary values that can be configured.
        /// </summary>
        /// <remarks>
        /// Minus 1 the normal count because a non-configurable Communication Error consumes the
        /// first available index.
        /// </remarks>
        public const int MaxBinaryPointCount = 99;


        /// <summary>
        /// The maximum number of counter values that can be configured.
        /// </summary>
        public const int MaxCounterPointCount = 100;


        /// <summary>
        /// The collection of coil map entries for the Modbus device.
        /// </summary>
        public ICollection<GenericModbusCoilPointConfig> Coils { get; set; } = null!;


        /// <summary>
        /// The collection of discrete input map entries for the Modbus device.
        /// </summary>
        public ICollection<GenericModbusDiscreteInputPointConfig> DiscreteInputs { get; set; } = null!;


        /// <summary>
        /// The collection of input register map entries for the Modbus device.
        /// </summary>
        public ICollection<GenericModbusInputRegisterConfig> InputRegisters { get; set; } = null!;


        /// <summary>
        /// The collection of holding register map entries for the Modbus device.
        /// </summary>
        public ICollection<GenericModbusHoldingRegisterConfig> HoldingRegisters { get; set; } = null!;


        /// <summary>
        /// The slave heartbeat register address for the Modbus device.
        /// </summary>

        public ushort? SlaveHeartbeatRegister { get; set; }


        /// <summary>
        /// The master heartbeat register address for the Modbus device.
        /// </summary>
        public ushort? MasterHeartbeatRegister { get; set; }


        /// <summary>
        /// The interval for the slave heartbeat register, which determines how frequently the slave heartbeat is polled.
        /// </summary>
        public int MasterHeartbeatPollingInterval { get; set; } = 1;

    }
}
