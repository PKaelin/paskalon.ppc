// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.ClosedModes
{
    /// <summary>
    /// Maintenance SOC mode configuration.
    /// </summary>
    public class MaintenanceSocModeConfig : OperatingClosedModeBaseConfig
    {
        /// <summary>
        /// Flag whether to use a target state of charge or not in case the
        /// target is a energy storage.
        /// </summary>
        /// <remarks>
        /// If true the mode will run until the target SOC is hit.
        /// </remarks>
        public bool UseTargetStateOfCharge { get; set; } = false;


        /// <summary>
        /// The target state of charge when <see cref="UseTargetStateOfCharge"/> is set to true.
        /// </summary>
        /// <remarks>
        /// Target state of charge in percent %.
        /// </remarks>
        public double TargetStateOfCharge { get; set; } = 0;
    }
}
