

namespace paskalON.Communication.Protocols.Modbus.Configurations
{
    /// <summary>
    /// Stores a single PollingRange entry in the ModbusRegisterMap. Contains the starting index, 
    /// ending index and the PollingClass.
    /// </summary>
    public class ModbusRegisterMapPollingRangeConfig
    {
        /// <summary>
        /// Primary Id
        /// </summary>
        public int ModbusRegisterMapPollingRangeConfigId { get; set; }


        /// <summary>
        /// Relationship to PowerMeterMapModbusConfig whom has a list of polling ranges
        /// </summary>
        public int? PowerMeterMapModbusConfigId { get; set; }




        public ushort Start { get; set; }

        public ushort End { get; set; }

        public string? PollingClass { get; set; }

        public bool IsInputRegisterRange { get; set; }




        public void Validate()
        {
            if (Start > End)
            {
                throw new InvalidOperationException(string.Format("Start {0} is greater than End {1}.", Start, End));
            }
        }


        public bool Includes(int index)
        {
            return (Start <= index) && (index <= End);
        }
    }
}
