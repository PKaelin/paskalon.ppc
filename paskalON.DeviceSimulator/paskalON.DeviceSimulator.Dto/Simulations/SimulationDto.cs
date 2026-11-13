// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Dto.Simulations
{
    /// <summary>
    /// Simulation state of a simulated Modbus or C37 device.
    /// </summary>
    public record SimulationDto
    {
        /// <summary>
        /// Type of the simulated device.
        /// </summary>
        public required SimulationDeviceType DeviceType { get; init; }


        /// <summary>
        /// Vendor independent simulated state (e.g. Off, On, Standby, Fault, Disconnected, PreCharging, Connected, Streaming, Stopped).
        /// </summary>
        public required string State { get; init; }


        /// <summary>
        /// Flag whether the device answers Modbus requests or streams C37 data (false simulates a communication loss).
        /// </summary>
        public required bool IsResponding { get; init; }


        /// <summary>
        /// Flag whether the device has a warning register.
        /// </summary>
        public required bool HasWarningRegister { get; init; }


        /// <summary>
        /// Flag whether the device has a fault register.
        /// </summary>
        public required bool HasFaultRegister { get; init; }


        /// <summary>
        /// Flag whether the device has a vendor event register.
        /// </summary>
        public required bool HasVendorEventRegister { get; init; }


        /// <summary>
        /// Current warning bitmask or null when the device has no warning register.
        /// </summary>
        public ushort? Warning { get; init; }


        /// <summary>
        /// Current fault bitmask or null when the device has no fault register.
        /// </summary>
        public ushort? Fault { get; init; }


        /// <summary>
        /// Current vendor event bitmask or null when the device has no vendor event register.
        /// </summary>
        public ushort? VendorEvent { get; init; }


        /// <summary>
        /// PCS only: maximum active power change per second in percent of the nameplate. 0 means instant.
        /// </summary>
        public double? SlowActivePowerByPercent { get; init; }


        /// <summary>
        /// PCS only: maximum reactive power change per second in percent of the nameplate. 0 means instant.
        /// </summary>
        public double? SlowReactivePowerByPercent { get; init; }


        /// <summary>
        /// Battery bank only: usable state of charge in percent.
        /// </summary>
        public double? UsableStateOfCharge { get; init; }


        /// <summary>
        /// Battery bank only: preferred (operator window) state of charge in percent. Can be below 0% or above 100%.
        /// </summary>
        public double? PreferredStateOfCharge { get; init; }


        /// <summary>
        /// Battery bank only: factor by which the state of charge changes faster than the physical definition
        /// (1 = real time, 100 = one real second charges or discharges like 100 seconds).
        /// </summary>
        public double? ChargeSpeedFactor { get; init; }
    }
}
