// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Messaging.Sequences;

namespace paskalON.Alarms.Dto.Ppcs
{
    /// <summary>
    /// PPC alarm DTO published when an alarm is raised, acknowledged or normalized.
    /// </summary>
    /// <remarks>
    /// The alarm service stamps the published alarms with its source instance and sequence, so that the receivers drop outdated alarms.
    /// </remarks>
    public record PpcAlarmDto : ISequencedMessage
    {
        /// <summary>
        /// Unique identifier of the alarm, used to acknowledge the alarm.
        /// </summary>
        public required int Id { get; init; }


        /// <summary>
        /// Type of the PPC alarm.
        /// </summary>
        public required PpcAlarmTypeDto PpcAlarmType { get; init; }


        /// <summary>
        /// Unique identifier of the device.
        /// </summary>
        /// <remarks>
        /// Contains the device id of the publishing device, also when the configuration applies to all devices.
        /// Null means the alarm is not a device alarm, e.g. a system alarm.
        /// </remarks>
        public int? DeviceId { get; init; }


        /// <summary>
        /// Name of the device associated with the device id.
        /// </summary>
        public string? DeviceName { get; init; }


        /// <summary>
        /// Name of the property, or dictionary entry ([Dictionary].[Key]), that raised the alarm.
        /// </summary>
        public required string PropertyName { get; init; }


        /// <summary>
        /// State of the alarm in the alarm sequence.
        /// </summary>
        public required AlarmSequenceStateDto AlarmSequenceState { get; init; }


        /// <summary>
        /// Condition of the process that caused the alarm event.
        /// </summary>
        public required AlarmProcessConditionDto AlarmProcessCondition { get; init; }


        /// <summary>
        /// Timestamp when the alarm was created.
        /// </summary>
        public required DateTimeOffset CreatedOn { get; init; }


        /// <summary>
        /// User or system that last changed the alarm.
        /// </summary>
        public required string ChangedBy { get; init; }


        /// <summary>
        /// Timestamp when the alarm was last changed.
        /// </summary>
        public required DateTimeOffset ChangedDate { get; init; }


        /// <summary>
        /// Associated alarm configuration.
        /// </summary>
        public required PpcAlarmConfigDto PpcAlarmConfig { get; init; }


        /// <inheritdoc/>
        public string SourceInstance { get; init; } = string.Empty;


        /// <inheritdoc/>
        public long Sequence { get; init; }


        /// <inheritdoc/>
        public DateTimeOffset Timestamp { get; init; }
    }
}
