// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs.Curves;
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Domain.Configs
{
    /// <summary>
    /// Base class for all operating mode configurations.
    /// </summary>
    /// <remarks>
    /// Some operating modes just need basic ramp and curve configuration.
    /// Inherit from this base class for specific configurations like:
    /// class FrequencyWattCurveModeConfig : OperatingModeConfig
    /// </remarks>
    public abstract class OperatingModeBaseConfig : NameBase
    {
        /// <summary>
        /// Operating mode type as a flag representation.
        /// </summary>
        public required OperatingModeType Type { get; set; }


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
        public required RampBaseConfig RampConfig { get; set; }


        /// <summary>
        /// Curve configuration of this operating mode.
        /// </summary>
        public CurveBaseConfig? CurveConfig { get; set; }

    }
}
