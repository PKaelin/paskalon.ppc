// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Domain.Evaluations
{
    /// <summary>
    /// Evaluates changed values of the stateless store against the PPC alarm configurations.
    /// </summary>
    /// <remarks>
    /// Only enabled configurations of the publisher's alarm type and device (or all devices) are evaluated.
    /// When "multiple configurations" match a value, the most specific ones win:
    /// 1. Configuration of the property or dictionary entry (FaultStates.GridLoss) before the dictionary (FaultStates).
    /// 2. Configuration of the device before the configuration of all devices.
    /// All configurations of the winning level are evaluated, so that e.g. warning and error thresholds of the same value raise their own alarms.
    /// Per configuration and value:
    /// - Abnormal without an active alarm raises a new alarm or after TimeBeforeRaisingAlarmMilliseconds.
    /// - Normal with an active alarm normalizes the alarm or after TimeBeforeNormalizingAlarmMilliseconds.
    /// - Otherwise a pending raise or normalization is cancelled: the value is normal again before the delayed raise
    ///   or abnormal again before the delayed normalization (no action is sent).
    /// A delayed raise or normalization waits in <see cref="PpcAlarmPendingTransitions"/> until <see cref="EvaluateDue"/>
    /// applies it with the time the value changed.
    /// Active alarms of a value whose configuration no longer wins (overridden or disabled) are normalized without delay.
    /// </remarks>
    public class PpcAlarmEvaluator : IPpcAlarmEvaluator
    {
        /// <summary>
        /// Name of the system that changes the alarms.
        /// </summary>
        public const string SystemUser = "System";


        /// <summary>
        /// Specificity of a configuration of the property or dictionary entry itself.
        /// </summary>
        private const int PropertySpecificity = 2;


        /// <summary>
        /// Specificity of a configuration of the device.
        /// </summary>
        private const int DeviceSpecificity = 1;


        /// <inheritdoc/>
        public PpcAlarmEvaluation Evaluate(PpcAlarmKey key, IEnumerable<PpcAlarmKeyValue> changedValues,
            IEnumerable<PpcAlarmConfig> configs, IEnumerable<PpcAlarm> activeAlarms, PpcAlarmPendingTransitions pendingTransitions)
        {
            ArgumentNullException.ThrowIfNull(changedValues);
            ArgumentNullException.ThrowIfNull(configs);
            ArgumentNullException.ThrowIfNull(activeAlarms);
            ArgumentNullException.ThrowIfNull(pendingTransitions);

            List<PpcAlarmConfig> keyConfigs = GetKeyConfigs(key, configs);
            List<PpcAlarm> keyAlarms = GetKeyAlarms(key, activeAlarms);
            PpcAlarmEvaluation evaluation = new PpcAlarmEvaluation();

            foreach (PpcAlarmKeyValue value in changedValues)
            {
                List<PpcAlarmConfig> winningConfigs = SelectMostSpecificConfigs(keyConfigs, value.Name);
                List<PpcAlarm> valueAlarms = keyAlarms.Where(alarm => alarm.PropertyName == value.Name).ToList();
                // Normalize the alarms of configurations that no longer apply to the value (overridden or disabled), without delay
                List<PpcAlarm> orphanedAlarms = valueAlarms
                    .Where(alarm => winningConfigs.Any(config => config.Id == alarm.PpcAlarmConfigId) is false)
                    .ToList();
                pendingTransitions.CancelOthers(key, value.Name, winningConfigs.Select(config => config.Id).ToList());

                foreach (PpcAlarm alarm in orphanedAlarms)
                {
                    alarm.Normalize(SystemUser, value.ValueChangedOn);
                    evaluation.Normalized.Add(alarm);
                }

                foreach (PpcAlarmConfig config in winningConfigs)
                {
                    bool isAbnormal = IsAbnormal(config, value.Value);
                    PpcAlarm? activeAlarm = valueAlarms.FirstOrDefault(alarm => alarm.PpcAlarmConfigId == config.Id);

                    if (isAbnormal && activeAlarm == null)
                    {
                        RaiseOrStartDelay(key, value, config, pendingTransitions, evaluation);
                    }
                    else if (isAbnormal is false && activeAlarm != null)
                    {
                        NormalizeOrStartDelay(key, value, config, activeAlarm, pendingTransitions, evaluation);
                    }
                    else
                    {
                        // Abnormal again before a delayed normalization, or normal again before a delayed raise
                        pendingTransitions.Cancel(key, config.Id, value.Name);
                    }
                }
            }

            return evaluation;
        }


        /// <inheritdoc/>
        public PpcAlarmEvaluation EvaluateDue(PpcAlarmKey key, DateTimeOffset now, IEnumerable<PpcAlarmConfig> configs,
            IEnumerable<PpcAlarm> activeAlarms, PpcAlarmPendingTransitions pendingTransitions)
        {
            ArgumentNullException.ThrowIfNull(configs);
            ArgumentNullException.ThrowIfNull(activeAlarms);
            ArgumentNullException.ThrowIfNull(pendingTransitions);

            List<PpcAlarmConfig> keyConfigs = GetKeyConfigs(key, configs);
            List<PpcAlarm> keyAlarms = GetKeyAlarms(key, activeAlarms);
            PpcAlarmEvaluation evaluation = new PpcAlarmEvaluation();

            foreach (PpcAlarmPendingTransition transition in pendingTransitions.TakeDue(key, now))
            {
                // The configuration might have been disabled or removed by a configuration reload meanwhile
                PpcAlarmConfig? config = keyConfigs.FirstOrDefault(keyConfig => keyConfig.Id == transition.PpcAlarmConfigId);
                PpcAlarm? activeAlarm = keyAlarms.FirstOrDefault(alarm =>
                    alarm.PpcAlarmConfigId == transition.PpcAlarmConfigId && alarm.PropertyName == transition.PropertyName);

                if (config == null)
                {
                    continue;
                }

                if (transition.Transition == PpcAlarmTransition.Raise && activeAlarm == null)
                {
                    evaluation.Raised.Add(CreateAlarm(key, transition.Value, config));
                }
                else if (transition.Transition == PpcAlarmTransition.Normalize && activeAlarm != null)
                {
                    activeAlarm.Normalize(SystemUser, transition.Value.ValueChangedOn);
                    evaluation.Normalized.Add(activeAlarm);
                }
            }

            return evaluation;
        }


        /// <summary>
        /// Raises the alarm, or starts the delay of the raise when the configuration has a delay.
        /// </summary>
        private static void RaiseOrStartDelay(PpcAlarmKey key, PpcAlarmKeyValue value, PpcAlarmConfig config,
            PpcAlarmPendingTransitions pendingTransitions, PpcAlarmEvaluation evaluation)
        {
            if (config.TimeBeforeRaisingAlarmMilliseconds == 0)
            {
                // Raise the alarm immediately
                evaluation.Raised.Add(CreateAlarm(key, value, config));

                return;
            }

            // Start the delay of the raise, it will be raised when the delay elapses
            pendingTransitions.Start(CreateTransition(key, value, config, PpcAlarmTransition.Raise, config.TimeBeforeRaisingAlarmMilliseconds));
        }


        /// <summary>
        /// Normalizes the active alarm, or starts the delay of the normalization when the configuration has a delay.
        /// </summary>
        private static void NormalizeOrStartDelay(PpcAlarmKey key, PpcAlarmKeyValue value, PpcAlarmConfig config, PpcAlarm activeAlarm,
            PpcAlarmPendingTransitions pendingTransitions, PpcAlarmEvaluation evaluation)
        {
            if (config.TimeBeforeNormalizingAlarmMilliseconds == 0)
            {
                activeAlarm.Normalize(SystemUser, value.ValueChangedOn);
                evaluation.Normalized.Add(activeAlarm);

                return;
            }

            pendingTransitions.Start(CreateTransition(key, value, config, PpcAlarmTransition.Normalize, config.TimeBeforeNormalizingAlarmMilliseconds));
        }


        /// <summary>
        /// Creates the pending transition that is due when the delay after the value change elapsed.
        /// </summary>
        private static PpcAlarmPendingTransition CreateTransition(PpcAlarmKey key, PpcAlarmKeyValue value, PpcAlarmConfig config,
            PpcAlarmTransition transition, long delayMilliseconds)
        {
            return new PpcAlarmPendingTransition
            {
                Key = key,
                PpcAlarmConfigId = config.Id,
                PropertyName = value.Name,
                Transition = transition,
                Value = value,
                DueOn = value.ValueChangedOn.AddMilliseconds(delayMilliseconds)
            };
        }


        /// <summary>
        /// Gets the enabled configurations of the publisher's alarm type and device (or all devices).
        /// </summary>
        private static List<PpcAlarmConfig> GetKeyConfigs(PpcAlarmKey key, IEnumerable<PpcAlarmConfig> configs)
        {
            return configs
                .Where(config => config.IsEnabled && config.PpcAlarmType == key.PpcAlarmType)
                .Where(config => config.DeviceId == null || config.DeviceId == key.DeviceId)
                .ToList();
        }


        /// <summary>
        /// Gets the active alarms of the publisher.
        /// </summary>
        private static List<PpcAlarm> GetKeyAlarms(PpcAlarmKey key, IEnumerable<PpcAlarm> activeAlarms)
        {
            return activeAlarms
                .Where(alarm => alarm.IsActive && alarm.DeviceId == key.DeviceId && alarm.PpcAlarmConfig.PpcAlarmType == key.PpcAlarmType)
                .ToList();
        }


        /// <summary>
        /// Selects the most specific configurations of a value.
        /// </summary>
        /// <param name="configs">Enabled configurations of the publisher.</param>
        /// <param name="propertyName">Name of the property or dictionary entry.</param>
        /// <returns>All configurations of the most specific level, or empty if no configuration matches.</returns>
        private static List<PpcAlarmConfig> SelectMostSpecificConfigs(List<PpcAlarmConfig> configs, string propertyName)
        {
            List<(PpcAlarmConfig Config, int Specificity)> matches = new List<(PpcAlarmConfig Config, int Specificity)>();

            foreach (PpcAlarmConfig config in configs)
            {
                int? specificity = GetSpecificity(config, propertyName);

                if (specificity != null)
                {
                    matches.Add((config, specificity.Value));
                }
            }

            if (matches.Count == 0)
            {
                return new List<PpcAlarmConfig>();
            }

            int mostSpecific = matches.Max(match => match.Specificity);

            return matches
                .Where(match => match.Specificity == mostSpecific)
                .Select(match => match.Config)
                .ToList();
        }


        /// <summary>
        /// Gets the specificity of a configuration for a value.
        /// </summary>
        /// <param name="config">Configuration of the publisher.</param>
        /// <param name="propertyName">Name of the property or dictionary entry.</param>
        /// <returns>Specificity of the configuration, higher is more specific, or null if the configuration does not match the value.</returns>
        private static int? GetSpecificity(PpcAlarmConfig config, string propertyName)
        {
            int deviceSpecificity = config.DeviceId == null ? 0 : DeviceSpecificity;

            if (string.Equals(config.PropertyName, propertyName, StringComparison.Ordinal))
            {
                return PropertySpecificity + deviceSpecificity;
            }

            if (propertyName.StartsWith(config.PropertyName + PpcAlarmKeyValue.DictionarySeparator, StringComparison.Ordinal))
            {
                return deviceSpecificity;
            }

            return null;
        }


        /// <summary>
        /// Conditions the value with the operator and value of the configuration.
        /// </summary>
        /// <param name="config">Configuration of the value.</param>
        /// <param name="value">Value of the property or dictionary entry.</param>
        /// <returns>True if the condition of the configuration is fulfilled (abnormal).</returns>
        private static bool IsAbnormal(PpcAlarmConfig config, double value)
        {
            return config.PropertyOperator switch
            {
                PropertyOperator.Equal => value == config.PropertyValue,
                PropertyOperator.NotEqual => value != config.PropertyValue,
                PropertyOperator.GreaterThan => value > config.PropertyValue,
                PropertyOperator.SmallerThan => value < config.PropertyValue,
                PropertyOperator.GreaterThanOrEqual => value >= config.PropertyValue,
                PropertyOperator.SmallerThanOrEqual => value <= config.PropertyValue,
                _ => throw new ArgumentOutOfRangeException(nameof(config), config.PropertyOperator, "Unknown property operator.")
            };
        }


        /// <summary>
        /// Creates a newly raised alarm.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="value">Abnormal value.</param>
        /// <param name="config">Configuration of the alarm.</param>
        /// <returns>Raised alarm.</returns>
        private static PpcAlarm CreateAlarm(PpcAlarmKey key, PpcAlarmKeyValue value, PpcAlarmConfig config)
        {
            return new PpcAlarm
            {
                ChangedBy = SystemUser,
                ChangedDate = value.ValueChangedOn,
                CreatedOn = value.ValueChangedOn,
                DeviceId = key.DeviceId,
                PropertyName = value.Name,
                PpcAlarmConfigId = config.Id,
                PpcAlarmConfig = config
            };
        }
    }
}
