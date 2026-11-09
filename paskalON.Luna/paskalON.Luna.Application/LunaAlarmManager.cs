// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Client;
using paskalON.Alarms.Client.Subscribers;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Keeps the alarm summary of the alarm service, received with the alarm actions and loaded from the alarm service API.
    /// </summary>
    /// <remarks>
    /// - Every Luna instance receives every action (own queue per instance), so every instance has the summary.
    /// - The summary with the latest calculation time wins: an older summary (e.g. a delayed or duplicated message,
    ///   or a summary loaded before a newer action arrived) is ignored, so the order of arrival does not matter.
    /// </remarks>
    public class LunaAlarmManager : ILunaAlarmManager, IAlarmActionHandler
    {
        /// <summary>
        /// Action sent by the alarm service when an alarm is raised.
        /// </summary>
        public const string RaiseAlarmActionName = "RaiseAlarm";


        /// <summary>
        /// Action sent by the alarm service when an alarm is normalized.
        /// </summary>
        public const string ClearAlarmActionName = "ClearAlarm";


        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<LunaAlarmManager> _logger;


        /// <summary>
        /// Client of the alarm service API.
        /// </summary>
        private readonly IAlarmServer _alarmServer;


        /// <summary>
        /// Time provider of the period of the alarm list.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Lock of the summary.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Latest alarm summary, null until the first summary was received.
        /// </summary>
        private AlarmSummaryDto? _summary;


        /// <summary>
        /// Constructor of <see cref="LunaAlarmManager"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="alarmServer">Client of the alarm service API.</param>
        /// <param name="timeProvider">Time provider of the period of the alarm list.</param>
        public LunaAlarmManager(ILogger<LunaAlarmManager> logger, IAlarmServer alarmServer, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(alarmServer);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _alarmServer = alarmServer;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        public event EventHandler<AlarmSummaryDto>? SummaryChanged;


        /// <inheritdoc/>
        public IReadOnlyCollection<string> ActionNames { get; } = new string[] { RaiseAlarmActionName, ClearAlarmActionName };


        /// <inheritdoc/>
        public AlarmSummaryDto? GetSummary()
        {
            lock (_dataLock)
            {
                return _summary;
            }
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The period is calculated with the clock of the service, not of the browser.
        /// </remarks>
        public async Task<List<PpcAlarmDto>> GetPpcAlarmsAsync(TimeSpan triggeredWithin, AlarmSeverityDto? severityLevel,
            AlarmProcessConditionDto? processCondition, AlarmSequenceStateDto? sequenceState, string? deviceName, CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(triggeredWithin, TimeSpan.Zero);

            PpcAlarmFilterDto filter = new PpcAlarmFilterDto
            {
                CreatedFrom = _timeProvider.GetUtcNow() - triggeredWithin,
                SeverityLevel = severityLevel,
                AlarmProcessCondition = processCondition,
                AlarmSequenceState = sequenceState,
                DeviceName = string.IsNullOrWhiteSpace(deviceName) ? null : deviceName.Trim()
            };

            return await _alarmServer.GetPpcAlarms(filter, cancellationToken);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// Every action carries the alarm summary, a duplicated action has the same summary and does not change it.
        /// </remarks>
        public Task HandleAsync(AlarmActionDto action, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(action);

            UpdateSummary(action.AlarmSummary, action.Name);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public async Task SynchronizeAsync(CancellationToken cancellationToken = default)
        {
            AlarmSummaryDto summary = await _alarmServer.GetAlarmSummary(cancellationToken);
            UpdateSummary(summary, "alarm summary API");
        }


        /// <summary>
        /// Keeps the summary if it was calculated after the current summary.
        /// </summary>
        /// <param name="summary">Received summary.</param>
        /// <param name="source">Source of the summary for the log.</param>
        private void UpdateSummary(AlarmSummaryDto summary, string source)
        {
            ArgumentNullException.ThrowIfNull(summary);

            lock (_dataLock)
            {
                if (_summary != null && summary.CalculatedOn <= _summary.CalculatedOn)
                {
                    _logger.LogDebug("Ignored the alarm summary of {Source} calculated on {CalculatedOn}, it is not newer.", source, summary.CalculatedOn);

                    return;
                }

                _summary = summary;
            }

            _logger.LogInformation("Alarm summary of {Source}: unacknowledged {CriticalCount} critical, {ErrorCount} error, {WarningCount} warning, " +
                "{InfoCount} info, acknowledged {AcknowledgedCriticalCount} critical, {AcknowledgedErrorCount} error, {AcknowledgedWarningCount} warning, " +
                "{AcknowledgedInfoCount} info alarms.", source, summary.CriticalCount, summary.ErrorCount, summary.WarningCount, summary.InfoCount,
                summary.AcknowledgedCriticalCount, summary.AcknowledgedErrorCount, summary.AcknowledgedWarningCount, summary.AcknowledgedInfoCount);
            // Raised outside the lock, the subscribers might call GetSummary
            SummaryChanged?.Invoke(this, summary);
        }
    }
}
