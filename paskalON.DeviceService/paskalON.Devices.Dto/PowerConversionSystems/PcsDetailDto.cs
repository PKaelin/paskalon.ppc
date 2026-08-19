// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Dto.PowerConversionSystems
{
    /// <summary>
    /// Data Transfer Object for a Power Conversion System Detail.
    /// </summary>
    /// <remarks>
    /// Used as low frequency DTO update.
    /// </remarks>
    public record PcsDetailDto : IDevice
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode { get; init; }


        /// <summary>
        /// Current active power target in Watts
        /// </summary>
        public ActivePower? ActivePowerTarget { get; init; }


        /// <summary>
        /// Current reactive power target in Vars
        /// </summary>
        public ReactivePower? ReactivePowerTarget { get; init; }


        /// <summary>
        /// Flag whether the AC breaker is closed.
        /// An AC breaker will open on overcurrent (usually settable) or when voltages are down a minimum.
        /// </summary>
        public bool? IsACBreakerClosed { get; init; }


        /// <summary>
        /// Array of flags whether the DC contactors are closed DC contactor will open/close via external command.
        /// </summary>
        public bool[]? IsDcContactorClosed { get; init; }


        /// <summary>
        /// Contains fault definitions and their states.
        /// </summary>
        public Dictionary<string, bool> FaultStates { get; init; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any active alarms.
        /// </summary>
        public bool HasActiveFaults { get => FaultStates.Any(a => a.Value == true); }


        /// <summary>
        /// Contains warning definitions and their states.
        /// </summary>
        public Dictionary<string, bool> WarningStates { get; init; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any active warnings.
        /// </summary>
        public bool HasActiveWarnings { get => WarningStates.Any(a => a.Value == true); }


        /// <summary>
        /// Contains vendors event definitions and their states.
        /// </summary>
        public Dictionary<string, bool> VendorEvents { get; init; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any vendor events.
        /// </summary>
        public bool HasVendorEvents { get => WarningStates.Any(a => a.Value == true); }
    }
}
