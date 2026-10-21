// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems
{
    /// <summary>
    /// Vendor independent telemetry of a simulated PCS that a vendor simulation writes to its registers.
    /// </summary>
    public sealed record PowerConversionSystemSimulationTelemetry
    {
        /// <summary>
        /// Simulated state.
        /// </summary>
        public required PowerConversionSystemSimulationState State { get; init; }


        /// <summary>
        /// Active power in watts (positive discharging, negative charging).
        /// </summary>
        public required double ActivePower { get; init; }


        /// <summary>
        /// Reactive power in volt-amperes reactive.
        /// </summary>
        public required double ReactivePower { get; init; }


        /// <summary>
        /// Available active power in watts. Positive discharge capability while discharging or idle,
        /// negative charge capability while charging. 0 when no battery bank is connected.
        /// </summary>
        public required double ActiveAvailablePower { get; init; }


        /// <summary>
        /// Available reactive power in volt-amperes reactive. 0 when no battery bank is connected.
        /// </summary>
        public required double ReactiveAvailablePower { get; init; }


        /// <summary>
        /// Flag whether the AC breaker is closed.
        /// </summary>
        public required bool IsACBreakerClosed { get; init; }


        /// <summary>
        /// Flag whether the DC contactor is closed (at least one battery bank is connected).
        /// </summary>
        public required bool IsDcContactorClosed { get; init; }


        /// <summary>
        /// Grid frequency in hertz.
        /// </summary>
        public required double Frequency { get; init; }


        /// <summary>
        /// DC voltage in volts.
        /// </summary>
        public required double DcVoltage { get; init; }


        /// <summary>
        /// DC current in amperes (positive discharging, negative charging).
        /// </summary>
        public required double DcCurrent { get; init; }


        /// <summary>
        /// AC line to line voltage in volts.
        /// </summary>
        public required double AcVoltage { get; init; }


        /// <summary>
        /// AC current in amperes.
        /// </summary>
        public required double AcCurrent { get; init; }
    }
}
