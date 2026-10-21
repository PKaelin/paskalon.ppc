// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Application.Repositories
{
    /// <summary>
    /// Repository of the simulator custom configurations of the devices.
    /// </summary>
    public interface ISimulationCustomRepository
    {
        /// <summary>
        /// Gets the simulator custom configurations of all active devices.
        /// </summary>
        /// <returns>The custom configurations keyed by device ID per device type.</returns>
        Task<SimulationCustomConfigs> GetCustomConfigsAsync();
    }
}
