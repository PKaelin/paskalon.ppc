using paskalON.Domains;
using System.ComponentModel.DataAnnotations;

namespace paskalON.OperatingModes.Domain.Curves
{
    public abstract class CurveBaseConfig : DomainBase
    {
        /// <summary>
        /// Name of the curve
        /// </summary>
        [Required, StringLength(200)]
        public required string Name { get; set; }


        /// <summary>
        /// List of configured points (X, Y) for this curve.
        /// </summary>
        [Required]
        public required List<CurvePointConfig> Points { get; set; }


        /// <summary>
        /// Curve unit of the X axis.
        /// </summary>
        [Required]
        public abstract CurveUnit XUnit { get; init; }


        /// <summary>
        /// Curve unit of the Y axis.
        /// </summary>
        [Required]
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
