// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Active power limit mode configuration.
    /// </summary>
    /// <remarks>
    /// The limits are <see cref="OperatingModeBaseConfig.MinimumActivePowerLimitWatt"/> and <see cref="OperatingModeBaseConfig.MaximumActivePowerLimitWatt"/>.
    /// If a limit is not set the systems nameplate for active power is used.
    /// </remarks>
    public class ActivePowerLimitModeConfig : OperatingClosedModeBaseConfig
    {
    }
}
