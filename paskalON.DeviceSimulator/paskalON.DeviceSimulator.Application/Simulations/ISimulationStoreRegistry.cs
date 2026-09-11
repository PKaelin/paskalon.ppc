// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Application.Simulations
{
    /// <summary>
    /// Provides the memory store associated with a configured Modbus endpoint.
    /// </summary>
    public interface ISimulationStoreRegistry
    {
        /// <summary>
        /// Gets an existing store or creates one for the configuration.
        /// </summary>
        /// <param name="config">Modbus configuration.</param>
        /// <returns>The endpoint data store.</returns>
        IModbusDataStore GetOrCreate(ModbusConfig config);


        /// <summary>
        /// All registered endpoint stores.
        /// </summary>
        /// <returns>The registered stores.</returns>
        IReadOnlyDictionary<ModbusDataMemoryStoreKey, IModbusDataStore> Stores { get; }
    }
}