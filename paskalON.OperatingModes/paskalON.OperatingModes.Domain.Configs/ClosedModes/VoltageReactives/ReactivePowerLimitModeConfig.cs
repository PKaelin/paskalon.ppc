// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Reactive power limit mode configuration.
    /// </summary>
    /// <remarks>
    /// The limits are <see cref="OperatingModeBaseConfig.MinimumReactivePowerLimitVars"/> and <see cref="OperatingModeBaseConfig.MaximumReactivePowerLimitVars"/>.
    /// If a limit is not set the systems nameplate for reactive power is used.
    /// </remarks>
    public class ReactivePowerLimitModeConfig : OperatingClosedModeBaseConfig
    {
    }
}
