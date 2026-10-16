// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.SafetyFunctions
{
    /// <summary>
    /// Shutdown mode configuration.
    /// </summary>
    public class ShutdownModeConfig : OperatingModeBaseConfig
    {
        /// <summary>
        /// Active power target the shutdown ramps to in Watts.
        /// </summary>
        public double ShutdownTargetActivePowerWatts { get; set; } = 0;


        /// <summary>
        /// Reactive power target the shutdown ramps to in Vars.
        /// </summary>
        public double ShutdownTargetReactivePowerVars { get; set; } = 0;
    }
}
