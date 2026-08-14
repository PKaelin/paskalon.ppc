// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Ders
{
    public class DerUnitPowerEnergyStorageControlMap : DerUnitPowerControlMap
    {
        public required Func<double> StateOfCharge { get; init; }

        public required Func<double> StateOfChargeMaximum { get; init; }

        public required Func<double> StateOfChargeMinimum { get; init; }

        public Func<double>? StateOfHealth { get; init; }

        public Func<double>? Thermal { get; init; }

        public Func<double>? NameplateThermalMaximum { get; init; }

        public Func<double>? Voltage { get; init; }

        public Func<double>? NameplateVoltageMaximum { get; init; }

        public Func<double>? NameplateVoltageMinimum { get; init; }

        public Func<double>? Current { get; init; }

        public Func<double>? NameplateCurrentMaximum { get; init; }

        public Func<double>? NameplateCurrentMinimum { get; init; }
    }
}
