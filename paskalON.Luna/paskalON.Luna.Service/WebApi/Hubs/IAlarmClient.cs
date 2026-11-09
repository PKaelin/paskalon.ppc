// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Methods of the UI clients connected to the <see cref="AlarmHub"/>.
    /// </summary>
    public interface IAlarmClient
    {
        /// <summary>
        /// Receives the alarm summary when a client connects and every time a newer summary was received from the alarm service.
        /// </summary>
        /// <param name="summary">The alarm summary, null while no summary has been received from the alarm service (unknown).</param>
        /// <returns>Task.</returns>
        Task AlarmSummaryUpdated(AlarmSummaryDto? summary);
    }
}
