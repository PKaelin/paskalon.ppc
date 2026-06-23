using paskalON.Domains;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using System.ComponentModel.DataAnnotations;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Base class for all operating mode configurations.
    /// </summary>
    /// <remarks>
    /// Some operating modes just need basic ramp and curve configuration.
    /// Inherit from this base class for specific configurations like:
    /// class FrequencyWattCurveModeConfig : OperatingModeConfig
    /// </remarks>
    public class OperatingModeConfig : DomainBase
    {
        /// <summary>
        /// Operating mode type as a flag representation.
        /// </summary>
        [Required]
        public OperatingModeType Type { get; set; }


        /// <summary>
        /// Name of the operating mode.
        /// </summary>
        [Required, StringLength(150)]
        public required string Name { get; set; }


        /// <summary>
        /// Timeout period (in seconds) between enabling the operating mode and the automatic disablement of the mode.
        /// </summary>
        /// <remarks>
        /// Value of 0 means it will never be automatically disabled.
        /// </remarks>
        public int TimeoutSeconds
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }


        /// <summary>
        /// Ramp configuration of this operating mode.
        /// </summary>
        [Required]
        public required RampBaseConfig RampConfig { get; set; }


        /// <summary>
        /// Curve configuration of this operating mode.
        /// </summary>
        public CurveBaseConfig? CurveConfig { get; set; }

    }
}
