// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Dto.Summaries
{
    /// <summary>
    /// Number of alarms in the sequence state Alarm (raised, not acknowledged and not normal) per severity level.
    /// </summary>
    /// <remarks>
    /// The summary is calculated by the alarm service, it is sent with every alarm action and returned by the alarm summary API.
    /// Recipients keep the summary with the latest <see cref="CalculatedOn"/>, an older summary (e.g. a delayed message) is ignored.
    /// </remarks>
    public record AlarmSummaryDto
    {
        /// <summary>
        /// Time the alarm service calculated the summary (UTC).
        /// </summary>
        public required DateTimeOffset CalculatedOn { get; init; }


        /// <summary>
        /// Number of alarms with the severity level Info.
        /// </summary>
        public required int InfoCount { get; init; }


        /// <summary>
        /// Number of alarms with the severity level Warning.
        /// </summary>
        public required int WarningCount { get; init; }


        /// <summary>
        /// Number of alarms with the severity level Error.
        /// </summary>
        public required int ErrorCount { get; init; }


        /// <summary>
        /// Number of alarms with the severity level Critical.
        /// </summary>
        public required int CriticalCount { get; init; }
    }
}
