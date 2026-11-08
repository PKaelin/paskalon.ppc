// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Alarms.Client
{
    /// <summary>
    /// Client of the alarm service API.
    /// </summary>
    public interface IAlarmServer
    {
        /// <summary>
        /// Gets the number of alarms in the sequence state Alarm per severity level.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The current alarm summary.</returns>
        /// <exception cref="HttpRequestException">Thrown when the alarm service is not reachable or answers with an error (e.g. 503).</exception>
        Task<AlarmSummaryDto> GetAlarmSummary(CancellationToken cancellationToken = default);
    }
}
