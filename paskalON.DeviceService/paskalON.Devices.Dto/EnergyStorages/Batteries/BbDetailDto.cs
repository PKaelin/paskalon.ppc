// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    /// <summary>
    /// Data Transfer Object for a Battery Bank Detail.
    /// </summary>
    /// <remarks>
    /// Used as low frequency DTO update.
    /// </remarks>
    public record BbDetailDto : IDevice
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
        /// State of health of the battery bank as a percentage of capacity / nameplate capacity.
        /// </summary>
        public double? StateOfHealth { get; init; }


        /// <summary>
        /// Minimum measured cell voltage of the battery bank.
        /// </summary>
        public double? MinimumCellVoltage { get; init; }


        /// <summary>
        /// Maximum measured cell voltage of the battery bank.
        /// </summary>
        public double? MaximumCellVoltage { get; init; }


        /// <summary>
        /// Minimum rack temperature of the battery bank.
        /// </summary>
        public double? MinimumRackTemperature { get; init; }


        /// <summary>
        /// Maximum rack temperature of the battery bank.
        /// </summary>
        public double? MaximumRackTemperature { get; init; }


        /// <summary>
        /// Average rack temperature.
        /// </summary>
        public double? AverageRackTemperature { get; init; }


        /// <summary>
        /// Minimum string temperature of the battery bank.
        /// </summary>
        public double? MinimumStringTemperature { get; init; }


        /// <summary>
        /// Maximum string temperature of the battery bank.
        /// </summary>
        public double? MaximumStringTemperature { get; init; }


        /// <summary>
        /// Average string temperature.
        /// </summary>
        public double? AverageStringTemperature { get; init; }


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
