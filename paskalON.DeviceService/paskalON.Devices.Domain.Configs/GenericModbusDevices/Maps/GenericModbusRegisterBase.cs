namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Base class for point map entries that are 16-bits values (sensor data, setpoints, diagnostic, etc.).
    /// </summary>
    public abstract class GenericModbusRegisterBase : GenericModbusEntryBase
    {
        /// <summary>
        /// Scaling factor applied to the raw Modbus register value to convert it into a meaningful engineering unit.
        /// </summary>
        public double ModbusScale { get; set; } = 1;


        /// <summary>
        /// Offset applied to a specific Modbus point.
        /// </summary>
        public int IndividualOffset { get; set; } = 0;


        private short? _bitIndex;
        /// <summary>
        /// The index of the bit within a 16-bit register (0-15) for points that are part of a register. If not applicable, it can be set to -1.
        /// </summary>
        public short BitIndex
        {
            get
            {
                if (_bitIndex.HasValue)
                {
                    return _bitIndex.Value;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (value > 31 || value < 0)
                {
                    _bitIndex = -1;
                }
                else
                {
                    _bitIndex = value;
                }
            }
        }


        /// <summary>
        /// Indicates whether the sign of the Modbus point value should be reversed.
        /// </summary>
        public bool ReverseSign { get; set; } = false;
    }
}
