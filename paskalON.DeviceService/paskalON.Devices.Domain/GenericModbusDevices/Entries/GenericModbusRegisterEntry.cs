using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;

namespace paskalON.Devices.Domain.GenericModbusDevices.Entries
{
    /// <summary>
    /// Class to hold value and configuration for a single Modbus register.
    /// </summary>
    public class GenericModbusRegisterEntry : GenericModbusEntryBase
    {
        /// <summary>
        /// The generic Modbus register base configuration.
        /// </summary>
        private readonly GenericModbusRegisterBaseConfig _config;

        /// <summary>
        /// Holds the value of the Modbus register.
        /// </summary>
        public Int16 Value { get; set; }


        /// <summary>
        /// Scaling factor applied to the raw Modbus register value to convert it into a meaningful engineering unit.
        /// </summary>
        public double ModbusScale { get => _config.ModbusScale; }


        /// <summary>
        /// Offset applied to a specific Modbus point.
        /// </summary>
        public int IndividualOffset { get => _config.IndividualOffset; }


        /// <summary>
        /// The index of the bit within a 16-bit register (0-15) for points that are part of a register.
        /// If not applicable, it can be set to -1.
        /// </summary>
        public short BitIndex { get => _config.BitIndex; }


        /// <summary>
        /// Indicates whether the sign of the Modbus point value should be reversed.
        /// </summary>
        public bool ReverseSign { get => _config.ReverseSign; }



        /// <summary>
        /// Constructor of <see cref="GenericModbusRegisterEntry"/>.
        /// </summary>
        /// <param name="config">The generic Modbus register base configuration.</param>
        public GenericModbusRegisterEntry(GenericModbusRegisterBaseConfig config) : base(config)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}
