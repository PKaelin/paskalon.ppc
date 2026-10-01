// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.OpenModes
{
    /// <summary>
    /// Maintenance mode configuration.
    /// </summary>
    /// <remarks>
    /// This is the open loop configuration.
    /// There is a closed loop for SOC maintenance.
    /// </remarks>
    public class MaintenanceModeConfig : OperatingModeBaseConfig
    {
        /// <summary>
        /// Gets or sets the name of the unit associated with this maintenance mode configuration.
        /// </summary>
        public required string DerUnitName { get; set; }
    }
}