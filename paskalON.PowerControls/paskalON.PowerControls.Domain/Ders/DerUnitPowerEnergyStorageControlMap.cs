// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Ders
{
    public class DerUnitPowerEnergyStorageControlMap : DerUnitPowerControlMap
    {
        /// <summary>
        /// Absolute state of charge in percent, on the same scale as <see cref="StateOfChargeMaximum"/> and <see cref="StateOfChargeMinimum"/>.
        /// Null if the state of charge is unknown.
        /// </summary>
        public required Func<double?> StateOfCharge { get; init; }


        /// <summary>
        /// Preferred state of charge maximum in absolute percent.
        /// </summary>
        public required Func<double> StateOfChargeMaximum { get; init; }


        /// <summary>
        /// Preferred state of charge minimum in absolute percent.
        /// </summary>
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
