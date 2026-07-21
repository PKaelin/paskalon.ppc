// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources
{
    /// <summary>
    /// Configuration class for maximum power point tracking node.
    /// </summary>
    public class MaximumPowerPointTrackingModeConfig : OperatingModeBaseConfig
    {
        // Things to consider:
        // Power smoothing -> e.g. Solar: 500 kW, 800 kW, 700 kW, 420 kW / Smoothing: 500, 600, 700, 420
    }
}
