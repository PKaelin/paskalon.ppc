// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Alarms.Dto.Actions
{
    /// <summary>
    /// Alarm action that contains enough information for the recipient to execute the action without making another API call.
    /// </summary>
    /// <remarks>
    /// Recipients implement a command design pattern where the command maps to the name of the action
    /// and takes the key/value parameters.
    /// The action is delivered at least once, recipients use the message id to ignore duplicated deliveries.
    /// </remarks>
    public record AlarmActionDto
    {
        /// <summary>
        /// Unique identifier of the action message.
        /// </summary>
        public required Guid MessageId { get; init; }


        /// <summary>
        /// Unique identifiable action name.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Key/Value parameters needed to execute the action.
        /// </summary>
        public List<AlarmActionParameterDto> AlarmActionParameters { get; init; } = new List<AlarmActionParameterDto>();


        /// <summary>
        /// Alarm summary of the alarm service after the alarm of the action was raised or normalized.
        /// </summary>
        public required AlarmSummaryDto AlarmSummary { get; init; }
    }
}
