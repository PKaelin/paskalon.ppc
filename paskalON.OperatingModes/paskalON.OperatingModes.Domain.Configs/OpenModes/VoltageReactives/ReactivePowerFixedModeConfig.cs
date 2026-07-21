// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives
{
    /// <summary>
    /// Configuration class for reactive power fixed mode.
    /// </summary>
    public class ReactivePowerFixedModeConfig : OperatingModeBaseConfig
    {
        /// <summary>
        /// Deadband threshold used to filter minor noise from setpoint signals.
        /// </summary>
        public double DeadbandKiloVars { get; set; } = 100;


        /// <summary>
        /// Configurable maximum reactive power limit in kilo vars.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// </remarks>
        public required double MaximumReactivePowerLimitKiloVars { get; set; }


        /// <summary>
        /// Configurable minimum reactive power limit in kilo vars.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// </remarks>
        public required double MinimumReactivePowerLimitKiloVars { get; set; }
    }
}
