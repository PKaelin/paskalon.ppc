// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs.Systems
{
    /// <summary>
    /// Configuration for system power constraints.
    /// </summary>
    public class SystemPowerConstraintConfig : PowerConstraintConfig
    {
        // Scenario 1: System has only one unit
        // Unit_1 MaximumActivePowerKiloWatt = 3630000
        // System MaximumActivePowerKiloWatt = Unit_1

        // Scenario 2: System has multiple units all the same maximum
        // Unit_1 MaximumActivePowerKiloWatt = 3630000
        // Unit_2 MaximumActivePowerKiloWatt = 3630000
        // System MaximumActivePowerKiloWatt = Unit_X * Amount of Units

        // Scenario 3: System has multiple units have different maximum
        // Unit_1 MaximumActivePowerKiloWatt = 3630000
        // Unit_2 MaximumActivePowerKiloWatt = 4530000
        // System MaximumActivePowerKiloWatt = Sum of all units MaximumActivePowerKiloWatt

        /// <summary>
        /// Indicates whether the system should derate per unit stopped.
        /// </summary>
        public bool DeratePerUnitStopped { get; set; } = true;


        /// <summary>
        /// Indicates whether the system should derate per unit in maintenance.
        /// </summary>
        public bool DeratePerUnitInMaintenance { get; set; } = true;

    }
}
