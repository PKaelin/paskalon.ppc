// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Alarms.Application.Actions
{
    /// <summary>
    /// Creates the alarm action DTOs of an alarm with the placeholders of the parameter values replaced.
    /// </summary>
    /// <remarks>
    /// Supported placeholders:
    /// - {{DeviceId}}: Device id of the alarm, empty if the alarm is not a device alarm (e.g. system alarm).
    /// - {{PropertyName}}: Property or dictionary entry that raised the alarm, e.g. FaultStates.OverTemperature.
    /// - {{AlarmId}}: Alarm id of the configuration, e.g. PCS-COMM.
    /// - {{PpcAlarmType}}: Alarm type of the configuration, e.g. PcsAlarm.
    /// - {{SeverityLevel}}: Severity level of the alarm, e.g. Error.
    /// The placeholders allow one action to be shared by many alarm configurations.
    /// Other text is not changed.
    /// </remarks>
    public static class AlarmActionDtoFactory
    {
        /// <summary>
        /// Placeholder of the device id.
        /// </summary>
        public const string DeviceIdPlaceholder = "{{DeviceId}}";


        /// <summary>
        /// Placeholder of the property name.
        /// </summary>
        public const string PropertyNamePlaceholder = "{{PropertyName}}";


        /// <summary>
        /// Placeholder of the alarm id of the configuration.
        /// </summary>
        public const string AlarmIdPlaceholder = "{{AlarmId}}";


        /// <summary>
        /// Placeholder of the alarm type of the configuration.
        /// </summary>
        public const string PpcAlarmTypePlaceholder = "{{PpcAlarmType}}";


        /// <summary>
        /// Placeholder of the severity level of the alarm.
        /// </summary>
        public const string SeverityLevelPlaceholder = "{{SeverityLevel}}";


        /// <summary>
        /// Creates the action DTOs of the alarm configuration for the trigger.
        /// </summary>
        /// <param name="alarm">Raised or normalized alarm.</param>
        /// <param name="trigger">Raise for a raised alarm, normalize for a normalized alarm.</param>
        /// <param name="alarmSummary">Alarm summary after the alarm was raised or normalized, sent with every action.</param>
        /// <returns>Action DTOs, each with a new message id, in the configured order.</returns>
        public static List<AlarmActionDto> Create(PpcAlarm alarm, AlarmActionTrigger trigger, AlarmSummaryDto alarmSummary)
        {
            ArgumentNullException.ThrowIfNull(alarm);
            ArgumentNullException.ThrowIfNull(alarmSummary);

            return alarm.PpcAlarmConfig.AlarmActions
                .Where(action => action.Trigger == trigger)
                .OrderBy(action => action.Id)
                .Select(action => new AlarmActionDto
                {
                    MessageId = Guid.CreateVersion7(),
                    Name = action.Name,
                    AlarmActionParameters = action.AlarmActionParameters
                        .OrderBy(parameter => parameter.Id)
                        .Select(parameter => new AlarmActionParameterDto
                        {
                            Key = parameter.Key,
                            Value = ReplacePlaceholders(parameter.Value, alarm)
                        })
                        .ToList(),
                    AlarmSummary = alarmSummary
                })
                .ToList();
        }


        /// <summary>
        /// Replaces the placeholders of a parameter value with the values of the alarm.
        /// </summary>
        private static string ReplacePlaceholders(string value, PpcAlarm alarm)
        {
            return value
                .Replace(DeviceIdPlaceholder, alarm.DeviceId?.ToString() ?? string.Empty, StringComparison.Ordinal)
                .Replace(PropertyNamePlaceholder, alarm.PropertyName, StringComparison.Ordinal)
                .Replace(AlarmIdPlaceholder, alarm.PpcAlarmConfig.AlarmId, StringComparison.Ordinal)
                .Replace(PpcAlarmTypePlaceholder, alarm.PpcAlarmConfig.PpcAlarmType.ToString(), StringComparison.Ordinal)
                .Replace(SeverityLevelPlaceholder, alarm.SeverityLevel.ToString(), StringComparison.Ordinal);
        }
    }
}
