// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Actions;

namespace paskalON.Alarms.Dto.Ppcs
{
    /// <summary>
    /// Alarm configuration of a published PPC alarm.
    /// </summary>
    /// <remarks>
    /// Name, Description, PossibleCauses, Response and Consequences are string identifiers (e.g. {{PCS.OverTemp.Name}}),
    /// that the user interface maps to a language dependent text.
    /// </remarks>
    public record PpcAlarmConfigDto
    {
        /// <summary>
        /// Unique identifier (error code) of the alarm.
        /// </summary>
        public required string AlarmId { get; init; }


        /// <summary>
        /// String identifier of the name of the alarm.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Severity level of the alarm.
        /// </summary>
        public required AlarmSeverityDto SeverityLevel { get; init; }


        /// <summary>
        /// String identifier of the short description of the alarm.
        /// </summary>
        public required string Description { get; init; }


        /// <summary>
        /// String identifier of the indication of possible causes for the alarm.
        /// </summary>
        public string? PossibleCauses { get; init; }


        /// <summary>
        /// String identifier that describes the appropriate response by the operator.
        /// </summary>
        public string? Response { get; init; }


        /// <summary>
        /// String identifier of the indication of what may occur if the alarm is ignored.
        /// </summary>
        public string? Consequences { get; init; }


        /// <summary>
        /// Actions of the published alarm sequence state.
        /// </summary>
        /// <remarks>
        /// Sequence state Alarm contains the raise alarm actions.
        /// Sequence state Normal contains the normalize alarm actions.
        /// </remarks>
        public List<AlarmActionDto> AlarmActions { get; init; } = new List<AlarmActionDto>();
    }
}
