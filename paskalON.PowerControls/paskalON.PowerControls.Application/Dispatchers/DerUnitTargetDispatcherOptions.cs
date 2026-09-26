// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Application.Dispatchers
{
    /// <summary>
    /// Timing options of the <see cref="DerUnitTargetDispatcher"/>.
    /// </summary>
    public class DerUnitTargetDispatcherOptions
    {
        /// <summary>
        /// Maximum time to wait for a DER unit to reach the started state after a start command was sent.
        /// </summary>
        public TimeSpan StartTimeout { get; init; } = TimeSpan.FromSeconds(90);


        /// <summary>
        /// Interval in which the DER unit state is checked while waiting for the started state.
        /// </summary>
        public TimeSpan StatePollInterval { get; init; } = TimeSpan.FromMilliseconds(500);


        /// <summary>
        /// Delay before a failed delivery of the targets is retried.
        /// </summary>
        public TimeSpan RetryDelay { get; init; } = TimeSpan.FromSeconds(5);
    }
}
