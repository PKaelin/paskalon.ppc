// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.OperatingModes.Domain.Configs.Curves
{
    public abstract class CurveBaseConfig : NameBase
    {
        /// <summary>
        /// List of configured points (X, Y) for this curve.
        /// </summary>
        public ICollection<CurvePointConfig> Points { get; set; } = [];


        /// <summary>
        /// Curve unit of the X axis.
        /// </summary>
        public abstract CurveUnit XUnit { get; init; }


        /// <summary>
        /// Curve unit of the Y axis.
        /// </summary>
        public abstract CurveUnit YUnit { get; init; }


        /// <summary>
        /// Indicator whether a ramp should be used to transition between values.
        /// </summary>
        public bool UseRamp { get; set; } = false;


        /// <summary>
        /// Ramp up time in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 0, Target: 60, RampUpTimeSeconds: 30 = Ramp reaches target of 60 in 30 seconds.
        /// </example>
        public int RampUpTimeSeconds { get; set; }


        /// <summary>
        /// Ramp down time in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 60, Target: 0, RampDownTimeSeconds: 30 = Ramp reaches target of 0 in 30 seconds.
        /// </example>
        public int RampDownTimeSeconds { get; set; }
    }
}
