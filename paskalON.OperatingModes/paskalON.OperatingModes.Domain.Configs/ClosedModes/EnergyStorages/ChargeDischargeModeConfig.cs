// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes.EnergyStorages
{
    /// <summary>
    /// Charge discharge mode configuration.
    /// </summary>
    public class ChargeDischargeModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Maximum storage reserve deadband.
        /// </summary>
        public double MaximumStorageReserveDeadband { get; set; }


        /// <summary>
        /// Minimum storage reserve deadband.
        /// </summary>
        public double MinimumStorageReserveDeadband { get; set; }
    }
}
