// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs.Ppcs
{
    /// <summary>
    /// Alarm configuration of a PPC alarm.
    /// </summary>
    /// <remarks>
    /// The property name is either the name of a boolean/numeric property of the published data (e.g. IsACBreakerClosed)
    /// or the name of a published dictionary (e.g. FaultStates).
    /// A dictionary configuration applies to every key of the dictionary, so that every key that turns true raises its own alarm
    /// without a configuration per key.
    /// Boolean values are evaluated as numeric values, true = 1 and false = 0.
    /// </remarks>
    /// <example>
    /// PropertyName = "FaultStates", PropertyOperator = Equal, PropertyValue = 1 and SeverityLevel = Error
    /// raises an error alarm for every fault state key that is true.
    /// </example>
    public class PpcAlarmConfig : AlarmConfigBase
    {
        /// <summary>
        /// Type of the PPC alarm (the publishing device type).
        /// </summary>
        public required PpcAlarmType PpcAlarmType { get; set; }


        /// <summary>
        /// Unique identifier of the device.
        /// </summary>
        /// <remarks>
        /// Null means the configuration applies to all devices of the PPC alarm type,
        /// or to an alarm type without devices, e.g. a system alarm.
        /// The raised device alarm always contains the device id of the publishing device.
        /// </remarks>
        public int? DeviceId { get; set; }


        /// <summary>
        /// Name of the property or dictionary of the published data that is evaluated.
        /// </summary>
        public required string PropertyName
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Operator used to condition the value associated with the property name against the property value.
        /// </summary>
        public required PropertyOperator PropertyOperator { get; set; }


        /// <summary>
        /// Value used to condition the value associated with the property name.
        /// </summary>
        /// <remarks>
        /// Boolean values are represented as true = 1 and false = 0.
        /// </remarks>
        public required double PropertyValue { get; set; }
    }
}
