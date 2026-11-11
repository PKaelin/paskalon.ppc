// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Ppcs;

namespace paskalON.Alarms.Application.Queries
{
    /// <summary>
    /// Maps the PPC alarm filter DTO to the domain filter and the PPC alarms to their DTOs.
    /// </summary>
    /// <remarks>
    /// The enumerations of the DTOs are analog to the enumerations of the domain, they share names and values.
    /// </remarks>
    public static class PpcAlarmDtoFactory
    {
        /// <summary>
        /// Creates the domain filter of a filter DTO.
        /// </summary>
        /// <param name="filter">Filter DTO of the request.</param>
        /// <returns>The domain filter.</returns>
        public static PpcAlarmFilter CreateFilter(PpcAlarmFilterDto filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            return new PpcAlarmFilter
            {
                CreatedFrom = filter.CreatedFrom,
                SeverityLevel = (AlarmSeverity?)filter.SeverityLevel,
                AlarmSequenceState = (AlarmSequenceState?)filter.AlarmSequenceState,
                DeviceName = filter.DeviceName
            };
        }


        /// <summary>
        /// Creates the DTO of an alarm with its configuration.
        /// </summary>
        /// <remarks>
        /// - The actions of the configuration are not mapped, a listed alarm is not published with its actions.
        /// - The sequence of the DTO is not set, a listed alarm is a query result and not a published message.
        /// </remarks>
        /// <param name="alarm">Alarm including its configuration.</param>
        /// <returns>The alarm DTO.</returns>
        public static PpcAlarmDto Create(PpcAlarm alarm)
        {
            ArgumentNullException.ThrowIfNull(alarm);

            return new PpcAlarmDto
            {
                Id = alarm.Id,
                PpcAlarmType = (PpcAlarmTypeDto)alarm.PpcAlarmConfig.PpcAlarmType,
                DeviceId = alarm.DeviceId,
                DeviceName = alarm.DeviceName,
                PropertyName = alarm.PropertyName,
                AlarmSequenceState = (AlarmSequenceStateDto)alarm.AlarmSequenceState,
                AcknowledgedBy = alarm.AcknowledgedBy,
                AcknowledgedOn = alarm.AcknowledgedOn,
                CanBeAcknowledged = alarm.CanBeAcknowledged,
                CreatedOn = alarm.CreatedOn,
                ChangedBy = alarm.ChangedBy,
                ChangedDate = alarm.ChangedDate,
                PpcAlarmConfig = CreateConfig(alarm.PpcAlarmConfig)
            };
        }


        /// <summary>
        /// Creates the DTO of an alarm configuration without its actions.
        /// </summary>
        private static PpcAlarmConfigDto CreateConfig(PpcAlarmConfig config)
        {
            return new PpcAlarmConfigDto
            {
                AlarmId = config.AlarmId,
                Name = config.Name,
                SeverityLevel = (AlarmSeverityDto)config.SeverityLevel,
                IsAcknowledgeable = config.IsAcknowledgeable,
                Description = config.Description,
                PossibleCauses = config.PossibleCauses,
                Response = config.Response,
                Consequences = config.Consequences
            };
        }
    }
}
