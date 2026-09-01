// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Maximum active power limit mode configuration.
    /// </summary>
    public class MaximumActivePowerLimitModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Maximum active power limit.
        /// </summary>
        /// <remarks>
        /// If this value is not set the systems nameplate for active power is used.
        /// </remarks>
        public double? MaximumActivePowerLimitKiloWatts { get; set; }
    }
}
