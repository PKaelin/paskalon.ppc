// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Frequency watt mode configuration.
    /// </summary>
    public class FrequencyWattModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Amount of time (in milliseconds) to wait before responding to a healthy condition.
        /// </summary>
        /// <remarks>
        /// Health Response Delay (often called "debounce time" or "stabilization time") acts as a validation timer.
        /// It requires a system, meter, or plant component to prove it is consistently healthy.
        /// </remarks> 
        public int HealthyResponseDelayMilliseconds { get; set; }
    }
}
