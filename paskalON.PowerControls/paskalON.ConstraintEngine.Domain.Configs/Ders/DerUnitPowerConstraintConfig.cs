// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs.Ders
{
    /// <summary>
    /// Configuration for DER unit power constraints.
    /// </summary>
    public class DerUnitPowerConstraintConfig : PowerConstraintConfig
    {
        // Scenario 1: PCS nameplate smaller than Battery
        // PCS NameplateMaximumActivePower = 3630000
        // BB NameplateMaximumChargeRate = 5000000
        // MaximumActivePowerKiloWatt = 3630000

        // Scenario 2: PCS nameplate bigger than Battery
        // PCS NameplateMaximumActivePower = 6630000
        // BB NameplateMaximumChargeRate = 5000000
        // MaximumActivePowerKiloWatt = 5000000

        // Scenario 3: PCS nameplate bigger than Battery but i have two batteries connected
        // PCS NameplateMaximumActivePower = 6630000
        // BB1 NameplateMaximumChargeRate = 5000000
        // BB2 NameplateMaximumChargeRate = 5000000
        // MaximumActivePowerKiloWatt = 6630000
    }
}
