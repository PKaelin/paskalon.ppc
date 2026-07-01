namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Base class for point map entries that are 1-bit boolean values (ON/OFF statuses, alarms, relay commands, etc.).
    /// </summary>
    public abstract class GenericModbusPointBase : GenericModbusEntryBase
    {
        /// <summary>
        /// Indicates whether the point is considered an alarm point, which can be used for monitoring and alerting purposes.
        /// </summary>
        public bool IsAlarm { get; set; } = true;


        /// <summary>
        /// Indicates whether the point is used to reset an alarm condition, which can be used to acknowledge or clear alarms in the system.
        /// </summary>
        public bool IsAlarmReset { get; set; } = false;
    }
}
