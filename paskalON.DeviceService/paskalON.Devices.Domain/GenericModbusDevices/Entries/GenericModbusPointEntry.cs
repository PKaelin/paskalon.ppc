using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;

namespace paskalON.Devices.Domain.GenericModbusDevices.Entries
{
    /// <summary>
    /// Class to hold value and configuration for a single Modbus point.
    /// </summary>
    public class GenericModbusPointEntry : GenericModbusEntryBase
    {
        /// <summary>
        /// The generic Modbus point base configuration.
        /// </summary>
        private readonly GenericModbusPointBaseConfig _config;


        /// <summary>
        /// Holds the value of the Modbus point.
        /// </summary>
        public byte Value
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(1, value);
                ArgumentOutOfRangeException.ThrowIfLessThan(0, value);
                field = value;
            }
        }


        /// <summary>
        /// Indicates whether the point is considered an alarm point, which can be used for monitoring and alerting purposes.
        /// </summary>
        public bool IsAlarm { get => _config.IsAlarm; }


        /// <summary>
        /// Indicates whether the point is used to reset an alarm condition, which can be used to acknowledge or clear alarms in the system.
        /// </summary>
        public bool IsAlarmReset { get => _config.IsAlarmReset; }



        /// <summary>
        /// Constructor of <see cref="GenericModbusPointEntry"/>.
        /// </summary>
        /// <param name="config">The generic Modbus point base configuration.</param>
        public GenericModbusPointEntry(GenericModbusPointBaseConfig config) : base(config)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}
