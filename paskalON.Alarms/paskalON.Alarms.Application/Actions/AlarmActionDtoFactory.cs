// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Actions;

namespace paskalON.Alarms.Application.Actions
{
    /// <summary>
    /// Creates the alarm action DTOs of an alarm with the placeholders of the parameter values replaced.
    /// </summary>
    /// <remarks>
    /// Supported placeholders:
    /// - {{DeviceId}}: Device id of the alarm, empty if the alarm is not a device alarm (e.g. system alarm).
    /// - {{PropertyName}}: Property or dictionary entry that raised the alarm, e.g. FaultStates.OverTemperature.
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
        /// Creates the action DTOs of the alarm configuration for the trigger.
        /// </summary>
        /// <param name="alarm">Raised or normalized alarm.</param>
        /// <param name="trigger">Raise for a raised alarm, normalize for a normalized alarm.</param>
        /// <returns>Action DTOs, each with a new message id, in the configured order.</returns>
        public static List<AlarmActionDto> Create(PpcAlarm alarm, AlarmActionTrigger trigger)
        {
            ArgumentNullException.ThrowIfNull(alarm);

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
                        .ToList()
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
                .Replace(PropertyNamePlaceholder, alarm.PropertyName, StringComparison.Ordinal);
        }
    }
}
