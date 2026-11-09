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
    public interface IPpcAlarmEvaluator
    {
        /// <summary>
        /// Evaluates the changed values of a publisher and raises or normalizes the alarms.
        /// </summary>
        /// <param name="key">Key of the publisher of the values.</param>
        /// <param name="changedValues">Values that changed with the latest update.</param>
        /// <param name="configs">All PPC alarm configurations.</param>
        /// <param name="activeAlarms">All active (not normalized) PPC alarms.</param>
        /// <param name="pendingTransitions">Delayed raises and normalizations, started and cancelled by the evaluation.</param>
        /// <returns>Newly raised alarms and the normalized active alarms, without the delayed ones.</returns>
        PpcAlarmEvaluation Evaluate(PpcAlarmKey key, IEnumerable<PpcAlarmKeyValue> changedValues,
            IEnumerable<PpcAlarmConfig> configs, IEnumerable<PpcAlarm> activeAlarms, PpcAlarmPendingTransitions pendingTransitions);


        /// <summary>
        /// Raises and normalizes the alarms of a publisher whose delay elapsed, with the time the value changed.
        /// The due transitions are removed from the pending transitions.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="now">Current time.</param>
        /// <param name="configs">All PPC alarm configurations, a transition of a configuration that is no longer enabled is dropped.</param>
        /// <param name="activeAlarms">All active (not normalized) PPC alarms.</param>
        /// <param name="pendingTransitions">Delayed raises and normalizations.</param>
        /// <returns>Raised alarms and normalized active alarms whose delay elapsed.</returns>
        PpcAlarmEvaluation EvaluateDue(PpcAlarmKey key, DateTimeOffset now, IEnumerable<PpcAlarmConfig> configs,
            IEnumerable<PpcAlarm> activeAlarms, PpcAlarmPendingTransitions pendingTransitions);
    }
}
