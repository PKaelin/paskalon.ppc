// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Reactive power mode configuration.
    /// </summary>
    public class ReactivePowerModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Proportional gain.
        /// </summary>
        /// <remarks>
        /// Determines how strongly the controller reacts to the current error.
        /// Controller Output = Kp ​× Error (Kp = Proportional Gain)
        /// Value must be between 1 and 0.0001.
        /// </remarks>
        /// <example>
        /// Kp 0.05 to 0.3 is common for plant controls.
        /// </example>
        public double ProportionalGain
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 1);
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0.0001);
                field = value;
            }
        } = 0.25;
    }
}
