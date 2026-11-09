// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Alarms.Client
{
    /// <summary>
    /// Client of the alarm service API.
    /// </summary>
    public interface IAlarmServer
    {
        /// <summary>
        /// Gets the number of active alarms per severity level, unacknowledged and acknowledged separately.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The current alarm summary.</returns>
        /// <exception cref="HttpRequestException">Thrown when the alarm service is not reachable or answers with an error (e.g. 503).</exception>
        Task<AlarmSummaryDto> GetAlarmSummary(CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the active and normalized PPC alarms that match the filter.
        /// </summary>
        /// <param name="filter">Filter of the alarms.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching alarms, the latest created alarm first.</returns>
        /// <exception cref="HttpRequestException">Thrown when the alarm service is not reachable or answers with an error (e.g. 503).</exception>
        Task<List<PpcAlarmDto>> GetPpcAlarms(PpcAlarmFilterDto filter, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the texts of the localization keys of the alarm configurations in the language,
        /// a key without a text in the language has the text of the default language (en).
        /// </summary>
        /// <param name="language">Language of the texts, e.g. en or de-CH.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The texts by their localization key.</returns>
        /// <exception cref="HttpRequestException">Thrown when the alarm service is not reachable or answers with an error (e.g. 400 or 503).</exception>
        Task<LocalizedTextsDto> GetLocalizedTexts(string language, CancellationToken cancellationToken = default);
    }
}
