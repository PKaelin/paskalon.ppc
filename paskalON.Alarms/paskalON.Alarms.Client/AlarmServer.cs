// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Globalization;
using System.Net.Http.Json;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Dto.Ppcs;
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
        /// Path of the PPC alarms, relative to the base address of the API.
        /// </summary>
        private const string GetPpcAlarmsPath = "alarms/ppcalarms";


        /// <summary>
        /// Path of the localized texts, relative to the base address of the API.
        /// </summary>
        private const string GetLocalizedTextsPath = "localizations/texts";


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


        /// <inheritdoc/>
        public async Task<List<PpcAlarmDto>> GetPpcAlarms(PpcAlarmFilterDto filter, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);

            using HttpResponseMessage response = await _client.GetAsync(CreatePpcAlarmsUri(filter), cancellationToken);
            response.EnsureSuccessStatusCode();
            List<PpcAlarmDto>? alarms = await response.Content.ReadFromJsonAsync<List<PpcAlarmDto>>(cancellationToken);

            return alarms ?? new List<PpcAlarmDto>();
        }


        /// <inheritdoc/>
        public async Task<LocalizedTextsDto> GetLocalizedTexts(string language, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(language);

            using HttpResponseMessage response = await _client.GetAsync($"{GetLocalizedTextsPath}?language={Uri.EscapeDataString(language.Trim())}", cancellationToken);
            response.EnsureSuccessStatusCode();
            LocalizedTextsDto? texts = await response.Content.ReadFromJsonAsync<LocalizedTextsDto>(cancellationToken);

            if (texts == null)
            {
                throw new InvalidOperationException("Alarm server returned empty localized texts.");
            }

            return texts;
        }


        /// <summary>
        /// Creates the relative URI of the PPC alarms with the set criteria of the filter as query parameters.
        /// </summary>
        /// <remarks>
        /// The enumerations are sent by name, the creation timestamp in the round-trip format.
        /// </remarks>
        private static string CreatePpcAlarmsUri(PpcAlarmFilterDto filter)
        {
            List<string> parameters = new List<string>
            {
                $"createdFrom={Uri.EscapeDataString(filter.CreatedFrom.ToString("O", CultureInfo.InvariantCulture))}"
            };

            if (filter.SeverityLevel.HasValue)
            {
                parameters.Add($"severityLevel={filter.SeverityLevel.Value}");
            }

            if (filter.AlarmSequenceState.HasValue)
            {
                parameters.Add($"alarmSequenceState={filter.AlarmSequenceState.Value}");
            }

            if (string.IsNullOrWhiteSpace(filter.DeviceName) is false)
            {
                parameters.Add($"deviceName={Uri.EscapeDataString(filter.DeviceName.Trim())}");
            }

            return $"{GetPpcAlarmsPath}?{string.Join('&', parameters)}";
        }
    }
}
