// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Ppcs;

namespace paskalON.Alarms.Application.Queries
{
    /// <summary>
    /// Reads the PPC alarms, e.g. for the alarm list of the user interface.
    /// </summary>
    public interface IPpcAlarmQueryHandler
    {
        /// <summary>
        /// Gets the active and normalized alarms that match the filter.
        /// </summary>
        /// <param name="filter">Filter of the alarms.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching alarms, the latest created alarm first.</returns>
        Task<List<PpcAlarmDto>> GetAlarmsAsync(PpcAlarmFilterDto filter, CancellationToken cancellationToken = default);
    }
}
