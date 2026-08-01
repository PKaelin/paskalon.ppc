// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
