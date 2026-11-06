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
        /// Constructor of <see cref="PpcAlarmHandler"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="valueStore">Stateless store of the latest values per publisher.</param>
        /// <param name="evaluator">Evaluator of the changed values.</param>
        /// <param name="configCache">Cache of the enabled alarm configurations.</param>
        /// <param name="alarmRepository">Repository of the alarms.</param>
        /// <param name="outboxRepository">Repository of the alarm action outbox.</param>
        /// <param name="outboxSignal">Signal that sends the saved alarm actions immediately.</param>
        public PpcAlarmHandler(ILogger<PpcAlarmHandler> logger, IPpcAlarmValueStore valueStore, IPpcAlarmEvaluator evaluator,
            IPpcAlarmConfigCache configCache, IPpcAlarmRepository alarmRepository, IAlarmActionOutboxRepository outboxRepository,
            IAlarmActionOutboxSignal outboxSignal)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(valueStore);
            ArgumentNullException.ThrowIfNull(evaluator);
            ArgumentNullException.ThrowIfNull(configCache);
            ArgumentNullException.ThrowIfNull(alarmRepository);
            ArgumentNullException.ThrowIfNull(outboxRepository);
            ArgumentNullException.ThrowIfNull(outboxSignal);

            _logger = logger;
            _valueStore = valueStore;
            _evaluator = evaluator;
            _configCache = configCache;
            _alarmRepository = alarmRepository;
            _outboxRepository = outboxRepository;
            _outboxSignal = outboxSignal;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// When the evaluation or persistence fails the values of the publisher are removed from the stateless store,
        /// so that the next received data of the publisher is evaluated completely again and no change gets lost.
        /// </remarks>
        public async Task<PpcAlarmEvaluation> HandleAsync(PpcAlarmKey key, object data, DateTimeOffset receivedOn, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);

            Dictionary<string, double> values = PpcAlarmValueReader.Read(data);
            List<PpcAlarmKeyValue> changedValues = _valueStore.Update(key, values, receivedOn);

            if (changedValues.Count == 0)
            {
                return new PpcAlarmEvaluation();
            }

            try
            {
                IReadOnlyList<PpcAlarmConfig> configs = _configCache.GetEnabled(key.PpcAlarmType);
                List<PpcAlarm> activeAlarms = await _alarmRepository.GetActiveAsync(key, cancellationToken);
                PpcAlarmEvaluation evaluation = _evaluator.Evaluate(key, changedValues, configs, activeAlarms);

                if (evaluation.HasChanges is false)
                {
                    return evaluation;
                }

                List<AlarmActionOutboxMessage> messages = CreateOutboxMessages(evaluation);
                _alarmRepository.Add(evaluation.Raised);
                _outboxRepository.Add(messages);
                await _alarmRepository.SaveChangesAsync(cancellationToken);

                // Only after the commit, otherwise the publisher service might not see the messages yet
                if (messages.Count > 0)
                {
                    _outboxSignal.Notify();
                }

                _logger.LogInformation("Alarms of {PpcAlarmType} device {DeviceId}: {RaisedCount} raised, {NormalizedCount} normalized.",
                    key.PpcAlarmType, key.DeviceId, evaluation.Raised.Count, evaluation.Normalized.Count);

                return evaluation;
            }
            catch
            {
                _valueStore.Remove(key);

                throw;
            }
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
