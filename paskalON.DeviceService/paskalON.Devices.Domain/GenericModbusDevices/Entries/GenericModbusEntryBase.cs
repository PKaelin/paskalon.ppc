using paskalON.Communication.Protocols.Modbus.Enums;
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
        /// Indicates whether the point is writable via Modbus (e.g., for coils and holding registers).
        /// </summary>
        public bool ModbusWritable { get => _config.ModbusWritable; }


        /// <summary>
        /// Indicates the Modbus value type of the point (e.g., Coil, Discrete Input, Input Register, Holding Register).
        /// </summary>
        public ModbusValueType ModbusValueType { get => _config.ModbusValueType; }


        /// <summary>
        /// Modbus point register format.
        /// </summary>
        public ModbusRegisterFormat? ModbusRegisterFormat { get => _config.ModbusRegisterFormat; }


        /// <summary>
        /// The Modbus address number of the point (e.g., 0-65535 for coils and registers).
        /// </summary>
        public ushort ModbusNumber { get => _config.ModbusNumber; }


        /// <summary>
        /// Indicates the polling class for the point, which can be used to group points for different polling intervals or priorities.
        /// </summary>
        public ModbusPollingClass PollingClass { get => _config.PollingClass; }


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
