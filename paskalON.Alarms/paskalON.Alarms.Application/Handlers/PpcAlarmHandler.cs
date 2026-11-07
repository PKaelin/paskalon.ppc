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
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

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
        /// Time provider of the normalization of orphaned alarms.
        /// </summary>
        private readonly TimeProvider _timeProvider;


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
        /// <param name="timeProvider">Time provider of the normalization of orphaned alarms.</param>
        public PpcAlarmHandler(ILogger<PpcAlarmHandler> logger, IPpcAlarmValueStore valueStore, IPpcAlarmEvaluator evaluator,
            IPpcAlarmConfigCache configCache, IPpcAlarmActiveCache activeCache, IPpcAlarmRepository alarmRepository,
            IAlarmActionOutboxRepository outboxRepository, IAlarmActionOutboxSignal outboxSignal, IPpcAlarmWriteLock writeLock,
            TimeProvider timeProvider)
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
                PpcAlarmEvaluation evaluation = _evaluator.Evaluate(key, changedValues, configs, activeAlarms);

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

                throw;
            }
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


        /// <summary>
        /// Saves the raised and normalized alarms with their alarm actions in one transaction and applies them to the cache.
        /// </summary>
        private async Task SaveEvaluationAsync(PpcAlarmKey key, PpcAlarmEvaluation evaluation, CancellationToken cancellationToken)
        {
            List<AlarmActionOutboxMessage> messages = CreateOutboxMessages(evaluation);
            _alarmRepository.Add(evaluation.Raised);
            // The normalized alarms are cached instances, they are updated before their outbox messages reference them
            _alarmRepository.Update(evaluation.Normalized);
            _outboxRepository.Add(messages);
            await _alarmRepository.SaveChangesAsync(cancellationToken);
            // Only after the commit, so that the cache and the outbox publisher service see the saved state
            _activeCache.Apply(key, evaluation);

            if (messages.Count > 0)
            {
                _outboxSignal.Notify();
            }

            _logger.LogInformation("Alarms of {PpcAlarmType} device {DeviceId}: {RaisedCount} raised, {NormalizedCount} normalized.",
                key.PpcAlarmType, key.DeviceId, evaluation.Raised.Count, evaluation.Normalized.Count);
        }


        /// <summary>
        /// Creates the outbox messages of the raise actions of the raised alarms and the normalize actions of the normalized alarms.
        /// </summary>
        private static List<AlarmActionOutboxMessage> CreateOutboxMessages(PpcAlarmEvaluation evaluation)
        {
            List<AlarmActionOutboxMessage> messages = new List<AlarmActionOutboxMessage>();

            foreach (PpcAlarm alarm in evaluation.Raised)
            {
                messages.AddRange(CreateOutboxMessages(alarm, AlarmActionTrigger.Raise));
            }

            foreach (PpcAlarm alarm in evaluation.Normalized)
            {
                messages.AddRange(CreateOutboxMessages(alarm, AlarmActionTrigger.Normalize));
            }

            return messages;
        }


        /// <summary>
        /// Creates the outbox messages of the actions of an alarm for the trigger.
        /// </summary>
        private static IEnumerable<AlarmActionOutboxMessage> CreateOutboxMessages(PpcAlarm alarm, AlarmActionTrigger trigger)
        {
            foreach (AlarmActionDto action in AlarmActionDtoFactory.Create(alarm, trigger))
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
