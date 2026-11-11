// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.Json;
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Application.Actions;
using paskalON.Alarms.Application.Caches;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Application.Stores;
using paskalON.Alarms.Application.Values;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Devices.Client;

namespace paskalON.Alarms.Application.Handlers
{
    /// <summary>
    /// Generic handler of published data with boolean, numeric and boolean dictionary properties (e.g. PcsDetailDto).
    /// </summary>
    /// <remarks>
    /// The handler is scoped, the repositories share the same alarm context so that the alarms and
    /// their alarm action messages (outbox) are saved in one transaction.
    /// </remarks>
    public class PpcAlarmHandler : IPpcAlarmHandler
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<PpcAlarmHandler> _logger;


        /// <summary>
        /// Stateless store of the latest values per publisher.
        /// </summary>
        private readonly IPpcAlarmValueStore _valueStore;


        /// <summary>
        /// Evaluator of the changed values.
        /// </summary>
        private readonly IPpcAlarmEvaluator _evaluator;


        /// <summary>
        /// Cache of the enabled alarm configurations.
        /// </summary>
        private readonly IPpcAlarmConfigCache _configCache;


        /// <summary>
        /// Cache of the active alarms.
        /// </summary>
        private readonly IPpcAlarmActiveCache _activeCache;


        /// <summary>
        /// Repository of the alarms.
        /// </summary>
        private readonly IPpcAlarmRepository _alarmRepository;


        /// <summary>
        /// Repository of the alarm action outbox.
        /// </summary>
        private readonly IAlarmActionOutboxRepository _outboxRepository;


        /// <summary>
        /// Signal that sends the saved alarm actions immediately.
        /// </summary>
        private readonly IAlarmActionOutboxSignal _outboxSignal;


        /// <summary>
        /// Lock of the alarm writers.
        /// </summary>
        private readonly IPpcAlarmWriteLock _writeLock;


        /// <summary>
        /// Time provider of the normalization of orphaned alarms and of the alarm summaries.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Delayed raises and normalizations of the alarms.
        /// </summary>
        private readonly PpcAlarmPendingTransitions _pendingTransitions;


        /// <summary>
        /// Device client with the devices of the DER, the raised alarms get the name of their device from it.
        /// </summary>
        private readonly IDeviceClient _deviceClient;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmHandler"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="valueStore">Stateless store of the latest values per publisher.</param>
        /// <param name="evaluator">Evaluator of the changed values.</param>
        /// <param name="configCache">Cache of the enabled alarm configurations.</param>
        /// <param name="activeCache">Cache of the active alarms.</param>
        /// <param name="alarmRepository">Repository of the alarms.</param>
        /// <param name="outboxRepository">Repository of the alarm action outbox.</param>
        /// <param name="outboxSignal">Signal that sends the saved alarm actions immediately.</param>
        /// <param name="writeLock">Lock of the alarm writers.</param>
        /// <param name="timeProvider">Time provider of the normalization of orphaned alarms, the delayed alarms and the alarm summaries.</param>
        /// <param name="pendingTransitions">Delayed raises and normalizations of the alarms.</param>
        /// <param name="deviceClient">Device client with the devices of the DER, the raised alarms get the name of their device from it.</param>
        public PpcAlarmHandler(ILogger<PpcAlarmHandler> logger, IPpcAlarmValueStore valueStore, IPpcAlarmEvaluator evaluator,
            IPpcAlarmConfigCache configCache, IPpcAlarmActiveCache activeCache, IPpcAlarmRepository alarmRepository,
            IAlarmActionOutboxRepository outboxRepository, IAlarmActionOutboxSignal outboxSignal, IPpcAlarmWriteLock writeLock,
            TimeProvider timeProvider, PpcAlarmPendingTransitions pendingTransitions, IDeviceClient deviceClient)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(valueStore);
            ArgumentNullException.ThrowIfNull(evaluator);
            ArgumentNullException.ThrowIfNull(configCache);
            ArgumentNullException.ThrowIfNull(activeCache);
            ArgumentNullException.ThrowIfNull(alarmRepository);
            ArgumentNullException.ThrowIfNull(outboxRepository);
            ArgumentNullException.ThrowIfNull(outboxSignal);
            ArgumentNullException.ThrowIfNull(writeLock);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(pendingTransitions);
            ArgumentNullException.ThrowIfNull(deviceClient);

            _logger = logger;
            _valueStore = valueStore;
            _evaluator = evaluator;
            _configCache = configCache;
            _activeCache = activeCache;
            _alarmRepository = alarmRepository;
            _outboxRepository = outboxRepository;
            _outboxSignal = outboxSignal;
            _writeLock = writeLock;
            _timeProvider = timeProvider;
            _pendingTransitions = pendingTransitions;
            _deviceClient = deviceClient;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// - The configurations and the active alarms are taken from the caches, the database is only used to save changes.
        /// - When the evaluation or persistence fails the values of the publisher are removed from the stateless store and
        ///   its active alarms are invalidated, so that the next received data of the publisher is evaluated completely
        ///   again against the saved alarms and no change gets lost.
        /// </remarks>
        public async Task<PpcAlarmEvaluation> HandleAsync(PpcAlarmKey key, object data, DateTimeOffset receivedOn, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);

            // Acquired before the stateless store is updated, so that a cancelled wait does not lose the changed values
            using IDisposable writeLock = await _writeLock.AcquireAsync(cancellationToken);
            Dictionary<string, double> values = PpcAlarmValueReader.Read(data);
            List<PpcAlarmKeyValue> changedValues = _valueStore.Update(key, values, receivedOn);

            if (changedValues.Count == 0)
            {
                return new PpcAlarmEvaluation();
            }

            try
            {
                IReadOnlyList<PpcAlarmConfig> configs = _configCache.GetEnabled(key.PpcAlarmType);
                List<PpcAlarm> activeAlarms = await _activeCache.GetActiveAsync(key, _alarmRepository, cancellationToken);
                PpcAlarmEvaluation evaluation = _evaluator.Evaluate(key, changedValues, configs, activeAlarms, _pendingTransitions);

                if (evaluation.HasChanges is false)
                {
                    return evaluation;
                }

                await SaveEvaluationAsync(key, evaluation, cancellationToken);

                return evaluation;
            }
            catch
            {
                // The evaluator might have normalized cached alarms that are not saved, they are loaded again with the next data
                _valueStore.Remove(key);
                _activeCache.Invalidate(key);
                // The delays start again when the values are evaluated again
                _pendingTransitions.CancelAll(key);

                throw;
            }
        }


        /// <inheritdoc/>
        /// <remarks>
        /// - Each publisher is saved separately like a received data, a failed publisher is logged and evaluated
        ///   completely again with its next data (its delays start again), the other publishers are still saved.
        /// - The raised alarms are created and the normalized alarms are normalized at the time their value changed.
        /// </remarks>
        public async Task<int> HandleDueAsync(CancellationToken cancellationToken = default)
        {
            using IDisposable writeLock = await _writeLock.AcquireAsync(cancellationToken);
            DateTimeOffset now = _timeProvider.GetUtcNow();
            int changedCount = 0;

            foreach (PpcAlarmKey key in _pendingTransitions.GetDueKeys(now))
            {
                try
                {
                    IReadOnlyList<PpcAlarmConfig> configs = _configCache.GetEnabled(key.PpcAlarmType);
                    List<PpcAlarm> activeAlarms = await _activeCache.GetActiveAsync(key, _alarmRepository, cancellationToken);
                    PpcAlarmEvaluation evaluation = _evaluator.EvaluateDue(key, now, configs, activeAlarms, _pendingTransitions);

                    if (evaluation.HasChanges)
                    {
                        await SaveEvaluationAsync(key, evaluation, cancellationToken);
                        changedCount += evaluation.Raised.Count + evaluation.Normalized.Count;
                    }
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogError(exception, "Raising or normalizing the delayed alarms of {PpcAlarmType} device {DeviceId} failed.",
                        key.PpcAlarmType, key.DeviceId);
                    _valueStore.Remove(key);
                    _activeCache.Invalidate(key);
                    _pendingTransitions.CancelAll(key);
                }
            }

            return changedCount;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// - The orphaned alarms are normalized like by the evaluator, their normalize actions are sent.
        /// - Each publisher is saved separately. When saving fails the publisher is invalidated and the exception is
        ///   thrown, the already saved publishers stay normalized and the others are normalized with the next call or by
        ///   the evaluator with the next changed data of their publisher.
        /// </remarks>
        public async Task<int> NormalizeOrphanedAsync(CancellationToken cancellationToken = default)
        {
            using IDisposable writeLock = await _writeLock.AcquireAsync(cancellationToken);
            int normalizedCount = 0;

            foreach (PpcAlarmKey key in _activeCache.GetKeys())
            {
                try
                {
                    List<PpcAlarm> activeAlarms = await _activeCache.GetActiveAsync(key, _alarmRepository, cancellationToken);
                    IReadOnlyList<PpcAlarmConfig> configs = _configCache.GetEnabled(key.PpcAlarmType);
                    List<PpcAlarm> orphanedAlarms = activeAlarms
                        .Where(alarm => configs.Any(config => config.Id == alarm.PpcAlarmConfigId) is false)
                        .ToList();

                    if (orphanedAlarms.Count == 0)
                    {
                        continue;
                    }

                    DateTimeOffset now = _timeProvider.GetUtcNow();
                    PpcAlarmEvaluation evaluation = new PpcAlarmEvaluation();

                    foreach (PpcAlarm alarm in orphanedAlarms)
                    {
                        alarm.Normalize(PpcAlarmEvaluator.SystemUser, now);
                        evaluation.Normalized.Add(alarm);
                    }

                    await SaveEvaluationAsync(key, evaluation, cancellationToken);
                    normalizedCount += orphanedAlarms.Count;
                }
                catch
                {
                    _activeCache.Invalidate(key);

                    throw;
                }
            }

            _logger.LogInformation("Normalized {NormalizedCount} active alarms of removed or disabled configurations.", normalizedCount);

            return normalizedCount;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// Calculated under the write lock from the active alarm cache, like the summary sent with the alarm actions.
        /// </remarks>
        public async Task<AlarmSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            using IDisposable writeLock = await _writeLock.AcquireAsync(cancellationToken);

            return await _activeCache.GetSummaryAsync(_alarmRepository, _timeProvider.GetUtcNow(), cancellationToken);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// - An active alarm is acknowledged on its cached instance, so that the active alarm cache and the alarm summary
        ///   include the acknowledgement. A normal alarm is acknowledged on the instance loaded from the database.
        /// - The acknowledge actions (Trigger=Acknowledge) carry the alarm summary after the acknowledgement.
        /// - When saving fails the publishers of the acknowledged active alarms are invalidated and loaded again.
        /// </remarks>
        public async Task<int> AcknowledgeAsync(IReadOnlyCollection<int> alarmIds, string acknowledgedBy, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(alarmIds);
            ArgumentException.ThrowIfNullOrWhiteSpace(acknowledgedBy);

            using IDisposable writeLock = await _writeLock.AcquireAsync(cancellationToken);
            List<PpcAlarm> storedAlarms = await _alarmRepository.GetByIdsAsync(alarmIds, cancellationToken);
            List<PpcAlarm> acknowledgedAlarms = new List<PpcAlarm>();
            HashSet<PpcAlarmKey> activeKeys = new HashSet<PpcAlarmKey>();
            DateTimeOffset now = _timeProvider.GetUtcNow();

            try
            {
                foreach (PpcAlarm storedAlarm in storedAlarms)
                {
                    PpcAlarm alarm = await GetCurrentInstanceAsync(storedAlarm, activeKeys, cancellationToken);

                    if (alarm.CanBeAcknowledged is false)
                    {
                        continue;
                    }

                    alarm.Acknowledge(acknowledgedBy, now);
                    acknowledgedAlarms.Add(alarm);
                }

                if (acknowledgedAlarms.Count == 0)
                {
                    return 0;
                }

                AlarmSummaryDto alarmSummary = await _activeCache.GetSummaryAsync(_alarmRepository, now, cancellationToken);
                List<AlarmActionOutboxMessage> messages = acknowledgedAlarms
                    .SelectMany(alarm => CreateOutboxMessages(alarm, AlarmActionTrigger.Acknowledge, alarmSummary))
                    .ToList();
                // The alarms are detached instances, they are updated before their outbox messages reference them
                _alarmRepository.Update(acknowledgedAlarms);
                _outboxRepository.Add(messages);
                await _alarmRepository.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                // The cached instances might be acknowledged without being saved, they are loaded again
                foreach (PpcAlarmKey key in activeKeys)
                {
                    _activeCache.Invalidate(key);
                }

                throw;
            }

            _outboxSignal.Notify();
            _logger.LogInformation("{AcknowledgedCount} of {RequestedCount} alarms acknowledged by {AcknowledgedBy}.",
                acknowledgedAlarms.Count, alarmIds.Count, acknowledgedBy);

            return acknowledgedAlarms.Count;
        }


        /// <summary>
        /// Gets the cached instance of an active alarm, or the stored instance of a normal alarm.
        /// </summary>
        /// <param name="storedAlarm">Alarm loaded from the database.</param>
        /// <param name="activeKeys">Collects the publishers of the active alarms.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The instance to change.</returns>
        private async Task<PpcAlarm> GetCurrentInstanceAsync(PpcAlarm storedAlarm, HashSet<PpcAlarmKey> activeKeys, CancellationToken cancellationToken)
        {
            if (storedAlarm.IsActive is false)
            {
                return storedAlarm;
            }

            PpcAlarmKey key = new PpcAlarmKey { PpcAlarmType = storedAlarm.PpcAlarmConfig.PpcAlarmType, DeviceId = storedAlarm.DeviceId };
            activeKeys.Add(key);
            List<PpcAlarm> cachedAlarms = await _activeCache.GetActiveAsync(key, _alarmRepository, cancellationToken);

            return cachedAlarms.FirstOrDefault(alarm => alarm.Id == storedAlarm.Id) ?? storedAlarm;
        }


        /// <summary>
        /// Saves the raised and normalized alarms with their alarm actions in one transaction and applies them to the cache.
        /// </summary>
        /// <remarks>
        /// The evaluation is applied to the cache before the save, so that the alarm summary sent with the actions
        /// contains it. When the save fails the callers invalidate the publisher, its alarms are loaded again from the database.
        /// The raised alarms get the name of their device.
        /// </remarks>
        private async Task SaveEvaluationAsync(PpcAlarmKey key, PpcAlarmEvaluation evaluation, CancellationToken cancellationToken)
        {
            string? deviceName = GetDeviceName(key);

            foreach (PpcAlarm alarm in evaluation.Raised)
            {
                alarm.DeviceName = deviceName;
            }

            _activeCache.Apply(key, evaluation);
            AlarmSummaryDto alarmSummary = await _activeCache.GetSummaryAsync(_alarmRepository, _timeProvider.GetUtcNow(), cancellationToken);
            List<AlarmActionOutboxMessage> messages = CreateOutboxMessages(evaluation, alarmSummary);
            _alarmRepository.Add(evaluation.Raised);
            // The normalized alarms are cached instances, they are updated before their outbox messages reference them
            _alarmRepository.Update(evaluation.Normalized);
            _outboxRepository.Add(messages);
            await _alarmRepository.SaveChangesAsync(cancellationToken);

            if (messages.Count > 0)
            {
                _outboxSignal.Notify();
            }

            _logger.LogInformation("Alarms of {PpcAlarmType} device {DeviceId}: {RaisedCount} raised, {NormalizedCount} normalized.",
                key.PpcAlarmType, key.DeviceId, evaluation.Raised.Count, evaluation.Normalized.Count);
        }


        /// <summary>
        /// Gets the name of the device of the publisher from the DER of the device client.
        /// </summary>
        /// <remarks>
        /// The device ids are only unique per device type (e.g. PCS 21 and battery bank 21), therefore the device is searched
        /// in the devices of the type of the publisher. The power control publishes the targets with the device id of the PCS.
        /// </remarks>
        /// <param name="key">Publisher of the data: the PPC alarm type and the device id.</param>
        /// <returns>The name of the device, null if the publisher is not a device or the device is not in the DER.</returns>
        private string? GetDeviceName(PpcAlarmKey key)
        {
            if (key.DeviceId is not int deviceId)
            {
                return null;
            }

            return key.PpcAlarmType switch
            {
                PpcAlarmType.PcsAlarm or PpcAlarmType.PowerControlAlarm => _deviceClient.PowerConversionSystems.FirstOrDefault(device => device.DeviceId == deviceId)?.Name,
                PpcAlarmType.BatteryAlarm => _deviceClient.BatteryBanks.FirstOrDefault(device => device.DeviceId == deviceId)?.Name,
                PpcAlarmType.SolarPanelAlarm => _deviceClient.SolarPanels.FirstOrDefault(device => device.DeviceId == deviceId)?.Name,
                PpcAlarmType.ExternalPowerMeterAlarm => _deviceClient.ExternalPowerMeters.FirstOrDefault(device => device.DeviceId == deviceId)?.Name,
                PpcAlarmType.AuxiliaryPowerMeterAlarm => _deviceClient.AuxiliaryPowerMeters.FirstOrDefault(device => device.DeviceId == deviceId)?.Name,
                PpcAlarmType.CircuitPowerMeterAlarm => _deviceClient.CircuitPowerMeters.FirstOrDefault(device => device.DeviceId == deviceId)?.Name,
                PpcAlarmType.SystemPowerMeterAlarm => _deviceClient.SystemPowerMeters.FirstOrDefault(device => device.DeviceId == deviceId)?.Name,
                _ => null
            };
        }


        /// <summary>
        /// Creates the outbox messages of the raise actions of the raised alarms and the normalize actions of the normalized alarms.
        /// </summary>
        private static List<AlarmActionOutboxMessage> CreateOutboxMessages(PpcAlarmEvaluation evaluation, AlarmSummaryDto alarmSummary)
        {
            List<AlarmActionOutboxMessage> messages = new List<AlarmActionOutboxMessage>();

            foreach (PpcAlarm alarm in evaluation.Raised)
            {
                messages.AddRange(CreateOutboxMessages(alarm, AlarmActionTrigger.Raise, alarmSummary));
            }

            foreach (PpcAlarm alarm in evaluation.Normalized)
            {
                messages.AddRange(CreateOutboxMessages(alarm, AlarmActionTrigger.Normalize, alarmSummary));
            }

            return messages;
        }


        /// <summary>
        /// Creates the outbox messages of the actions of an alarm for the trigger.
        /// </summary>
        private static IEnumerable<AlarmActionOutboxMessage> CreateOutboxMessages(PpcAlarm alarm, AlarmActionTrigger trigger, AlarmSummaryDto alarmSummary)
        {
            foreach (AlarmActionDto action in AlarmActionDtoFactory.Create(alarm, trigger, alarmSummary))
            {
                yield return new AlarmActionOutboxMessage
                {
                    ChangedBy = PpcAlarmEvaluator.SystemUser,
                    ChangedDate = alarm.ChangedDate,
                    MessageId = action.MessageId,
                    PpcAlarm = alarm,
                    ActionName = action.Name,
                    Payload = JsonSerializer.Serialize(action),
                    CreatedOn = alarm.ChangedDate
                };
            }
        }
    }
}
