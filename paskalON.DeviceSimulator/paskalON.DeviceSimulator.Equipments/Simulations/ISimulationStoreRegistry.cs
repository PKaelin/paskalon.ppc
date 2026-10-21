// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.Simulations
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
        SimulationModbusDataStore GetOrCreate(ModbusConfig config);


        /// <summary>
        /// Finds the store of an endpoint.
        /// </summary>
        /// <param name="address">Endpoint address.</param>
        /// <param name="port">Endpoint port.</param>
        /// <returns>The endpoint data store.</returns>
        /// <exception cref="InvalidOperationException">No store exists for the endpoint.</exception>
        SimulationModbusDataStore Find(string address, int port);


        /// <summary>
        /// All registered endpoint stores.
        /// </summary>
        /// <returns>The registered stores.</returns>
        IReadOnlyDictionary<ModbusDataMemoryStoreKey, SimulationModbusDataStore> Stores { get; }
    }
}
