using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Base class for generic Modbus point base and register base configurations.
    /// </summary>
    public abstract class GenericModbusEntryBase : NameBase
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
        /// Indicates whether the point is writable via Modbus (e.g., for coils and holding registers).
        /// </summary>
        public abstract bool ModbusWritable { get; }


        /// <summary>
        /// Indicates the Modbus value type of the point (e.g., Coil, Discrete Input, Input Register, Holding Register).
        /// </summary>
        public abstract ModbusValueType ModbusValueType { get; }


        /// <summary>
        /// Modbus point register format.
        /// </summary>
        public ModbusRegisterFormat? ModbusRegisterFormat { get; set; }


        /// <summary>
        /// The Modbus address number of the point (e.g., 0-65535 for coils and registers).
        /// </summary>
        public ushort ModbusNumber { get; set; }


        /// <summary>
        /// Indicates the polling class for the point, which can be used to group points for different polling intervals or priorities.
        /// </summary>
        public ModbusPollingClass PollingClass { get; set; } = ModbusPollingClass.Default;
    }
}
