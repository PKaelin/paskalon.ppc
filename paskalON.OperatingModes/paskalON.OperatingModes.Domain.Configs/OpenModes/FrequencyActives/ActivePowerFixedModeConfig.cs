// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives
{
    /// <summary>
    /// Configuration class for active power fixed mode.
    /// </summary>
    public class ActivePowerFixedModeConfig : OperatingModeBaseConfig
    {
        /// <summary>
        /// Deadband threshold used to filter minor noise from setpoint signals.
        /// </summary>
        public double DeadbandKiloWatt { get; set; } = 100;


        /// <summary>
        /// Configurable maximum active power limit in kilo watt.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// </remarks>
        public required double MaximumActivePowerLimitKiloWatt { get; set; }


        /// <summary>
        /// Configurable minimum active power limit in kilo watt.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// </remarks>
        public required double MinimumActivePowerLimitKiloWatt { get; set; }
    }
}
