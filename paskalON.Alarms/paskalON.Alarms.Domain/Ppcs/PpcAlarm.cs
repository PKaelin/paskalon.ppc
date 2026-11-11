// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;

namespace paskalON.Alarms.Domain.Ppcs
{
    /// <summary>
    /// PPC alarm, either of a device or not device related (e.g. a system alarm).
    /// </summary>
    /// <remarks>
    /// The property name identifies the alarm within the device, as one dictionary configuration can raise multiple alarms.
    /// A dictionary entry is named [Dictionary].[Key], e.g. FaultStates.OverTemperature.
    /// </remarks>
    public class PpcAlarm : AlarmBase
    {
        /// <summary>
        /// Unique identifier of the device.
        /// </summary>
        /// <remarks>
        /// Contains the device id of the publishing device, also when the configuration applies to all devices.
        /// Null means the alarm is not a device alarm, e.g. a system alarm.
        /// </remarks>
        public int? DeviceId { get; set; }


        /// <summary>
        /// Name of the device associated with the device id.
        /// </summary>
        public string? DeviceName { get; set; }


        /// <summary>
        /// Name of the property, or dictionary entry, that raised the alarm.
        /// </summary>
        public required string PropertyName
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Associated alarm configuration Id.
        /// </summary>
        public int PpcAlarmConfigId { get; set; }


        /// <summary>
        /// Associated alarm configuration.
        /// </summary>
        public required PpcAlarmConfig PpcAlarmConfig { get; set; }


        /// <inheritdoc/>
        public override AlarmSeverity SeverityLevel
        {
            get => PpcAlarmConfig.SeverityLevel;
        }


        /// <inheritdoc/>
        public override bool IsAcknowledgeable
        {
            get => PpcAlarmConfig.IsAcknowledgeable;
        }
    }
}
