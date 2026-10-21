// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries
{
    /// <summary>
    /// Vendor independent telemetry of a simulated battery bank that a vendor simulation writes to its registers.
    /// </summary>
    public sealed record BatteryBankSimulationTelemetry
    {
        /// <summary>
        /// Simulated state.
        /// </summary>
        public required BatteryBankSimulationState State { get; init; }


        /// <summary>
        /// Active power in watts (positive discharging, negative charging).
        /// </summary>
        public required double ActivePower { get; init; }


        /// <summary>
        /// State of charge in percent of the window the vendor reports (see <see cref="StateOfChargeType"/>).
        /// </summary>
        public required double StateOfCharge { get; init; }


        /// <summary>
        /// State of health in percent.
        /// </summary>
        public required double StateOfHealth { get; init; }


        /// <summary>
        /// Total DC voltage in volts.
        /// </summary>
        public required double DcVoltage { get; init; }


        /// <summary>
        /// Total DC current in amperes (positive discharging, negative charging).
        /// </summary>
        public required double DcCurrent { get; init; }
    }
}
