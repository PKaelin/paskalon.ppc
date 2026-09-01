// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Power factor mode configuration.
    /// </summary>
    public class PowerFactorModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Power factor target.
        /// </summary>
        public float PowerFactorTarget { get; set; } = 1;
    }
}
