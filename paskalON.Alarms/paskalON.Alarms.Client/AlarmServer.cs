// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Net.Http.Json;
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Alarms.Client
{
    /// <summary>
    /// Client of the alarm service API with a long living HTTP client.
    /// </summary>
    public class AlarmServer : IAlarmServer
    {
        /// <summary>
        /// Path of the alarm summary, relative to the base address of the API (e.g. http://paskalon.alarms.service:8080/api/v1/).
        /// </summary>
        private const string GetAlarmSummaryPath = "alarms/summary";


        /// <summary>
        /// HTTP client with the base address of the alarm service API.
        /// </summary>
        private readonly HttpClient _client;


        /// <summary>
        /// Constructor of <see cref="AlarmServer"/>.
        /// </summary>
        /// <param name="client">HTTP client with the base address of the alarm service API.</param>
        public AlarmServer(HttpClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(client.BaseAddress);

            _client = client;
        }


        /// <inheritdoc/>
        public async Task<AlarmSummaryDto> GetAlarmSummary(CancellationToken cancellationToken = default)
        {
            using HttpResponseMessage response = await _client.GetAsync(GetAlarmSummaryPath, cancellationToken);
            response.EnsureSuccessStatusCode();

            AlarmSummaryDto? summary = await response.Content.ReadFromJsonAsync<AlarmSummaryDto>(cancellationToken);

            if (summary == null)
            {
                throw new InvalidOperationException("Alarm server returned an empty alarm summary.");
            }

            return summary;
        }
    }
}
