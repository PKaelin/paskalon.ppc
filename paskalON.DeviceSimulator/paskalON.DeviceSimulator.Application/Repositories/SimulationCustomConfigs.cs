// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Application.Repositories
{
    /// <summary>
    /// Simulator custom configurations (key/value) of the devices, keyed by device ID per device type.
    /// </summary>
    /// <remarks>
    /// The custom configurations belong to the device configuration (device model), so all devices of the same
    /// model share them.
    /// </remarks>
    public sealed record SimulationCustomConfigs
    {
        /// <summary>
        /// Empty custom configurations.
        /// </summary>
        public static SimulationCustomConfigs Empty { get; } = new SimulationCustomConfigs();


        /// <summary>
        /// Custom configurations of the power conversion systems by device ID.
        /// </summary>
        public IReadOnlyDictionary<int, IReadOnlyDictionary<string, string>> PowerConversionSystems { get; init; } =
            new Dictionary<int, IReadOnlyDictionary<string, string>>();


        /// <summary>
        /// Custom configurations of the battery banks by device ID.
        /// </summary>
        public IReadOnlyDictionary<int, IReadOnlyDictionary<string, string>> BatteryBanks { get; init; } =
            new Dictionary<int, IReadOnlyDictionary<string, string>>();


        /// <summary>
        /// Custom configurations of the system power meters by device ID.
        /// </summary>
        public IReadOnlyDictionary<int, IReadOnlyDictionary<string, string>> SystemPowerMeters { get; init; } =
            new Dictionary<int, IReadOnlyDictionary<string, string>>();


        /// <summary>
        /// Gets the custom configurations of a power conversion system.
        /// </summary>
        /// <param name="deviceId">Device ID of the power conversion system.</param>
        /// <returns>The custom configurations or an empty dictionary.</returns>
        public IReadOnlyDictionary<string, string> GetPowerConversionSystem(int deviceId)
        {
            return PowerConversionSystems.GetValueOrDefault(deviceId) ?? SimulationCustoms.Empty;
        }


        /// <summary>
        /// Gets the custom configurations of a battery bank.
        /// </summary>
        /// <param name="deviceId">Device ID of the battery bank.</param>
        /// <returns>The custom configurations or an empty dictionary.</returns>
        public IReadOnlyDictionary<string, string> GetBatteryBank(int deviceId)
        {
            return BatteryBanks.GetValueOrDefault(deviceId) ?? SimulationCustoms.Empty;
        }


        /// <summary>
        /// Gets the custom configurations of a system power meter.
        /// </summary>
        /// <param name="deviceId">Device ID of the system power meter.</param>
        /// <returns>The custom configurations or an empty dictionary.</returns>
        public IReadOnlyDictionary<string, string> GetSystemPowerMeter(int deviceId)
        {
            return SystemPowerMeters.GetValueOrDefault(deviceId) ?? SimulationCustoms.Empty;
        }
    }
}
