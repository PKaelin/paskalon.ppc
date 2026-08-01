// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Curves;

namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Frequency droop mode configuration.
    /// </summary>
    public class FrequencyDroopModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Amount of time (in milliseconds) to wait before responding to a healthy condition.
        /// </summary>
        /// <remarks>
        /// Health Response Delay (often called "debounce time" or "stabilization time") acts as a validation timer.
        /// It requires a system, meter, or plant component to prove it is consistently healthy.
        /// </remarks> 
        public int HealthyResponseDelayMilliseconds { get; set; }


        /// <summary>
        /// Curve configuration of this operating mode.
        /// </summary>
        /// <remarks>
        /// This is a droop configuration and therefore should be configured to require a curve configuration.
        /// </remarks>
        public override required CurveBaseConfig? CurveConfig
        {
            get { return field; }
            set
            {
                if (value is not FrequencyWattCurveConfig)
                {
                    throw new ArgumentException($"{nameof(CurveConfig)} must be of type {nameof(FrequencyWattCurveConfig)}");
                }

                field = value ?? throw new ArgumentNullException(nameof(CurveConfig));
            }
        }
    }
}
