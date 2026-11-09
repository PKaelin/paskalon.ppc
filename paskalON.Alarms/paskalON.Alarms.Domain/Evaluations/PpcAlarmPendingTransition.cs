// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Evaluations
{
    /// <summary>
    /// Raise or normalization of an alarm that waits for the delay of its configuration to elapse.
    /// </summary>
    public record PpcAlarmPendingTransition
    {
        /// <summary>
        /// Key of the publisher.
        /// </summary>
        public required PpcAlarmKey Key { get; init; }


        /// <summary>
        /// Id of the alarm configuration.
        /// </summary>
        public required int PpcAlarmConfigId { get; init; }


        /// <summary>
        /// Property or dictionary entry of the alarm, e.g. CommunicationError or FaultStates.OverTemperature.
        /// </summary>
        public required string PropertyName { get; init; }


        /// <summary>
        /// Raise or normalization.
        /// </summary>
        public required PpcAlarmTransition Transition { get; init; }


        /// <summary>
        /// Value at the time it became abnormal (raise) or normal (normalization), its change time is the time of the alarm change.
        /// </summary>
        public required PpcAlarmKeyValue Value { get; init; }


        /// <summary>
        /// Time the delay elapses: change time of the value plus the delay of the configuration.
        /// </summary>
        public required DateTimeOffset DueOn { get; init; }
    }
}
