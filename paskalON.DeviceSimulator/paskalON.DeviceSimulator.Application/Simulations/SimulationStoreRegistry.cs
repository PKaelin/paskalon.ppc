// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Application.Simulations
{
    /// <summary>
    /// Thread-safe registry of simulator Modbus stores (Modbus registers).
    /// </summary>
    public class SimulationStoreRegistry : ISimulationStoreRegistry
    {
        /// <summary>
        /// Dictionary of Modbus data key and their stores.
        /// </summary>
        private readonly Dictionary<ModbusDataMemoryStoreKey, IModbusDataStore> _stores = new();


        /// <summary>
        /// Data lock object
        /// </summary>
        private readonly object _dataLock = new();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IReadOnlyDictionary<ModbusDataMemoryStoreKey, IModbusDataStore> Stores
        {
            get
            {
                lock (_dataLock)
                {
                    return new Dictionary<ModbusDataMemoryStoreKey, IModbusDataStore>(_stores);
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IModbusDataStore GetOrCreate(ModbusConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            ModbusDataMemoryStoreKey key = new ModbusDataMemoryStoreKey
            {
                Address = config.Address,
                Port = config.Port,
                UnitId = config.UnitId
            };

            lock (_dataLock)
            {
                if (_stores.TryGetValue(key, out IModbusDataStore? store) == true)
                {
                    return store;
                }

                store = new ModbusDataMemoryStore();
                _stores.Add(key, store);

                return store;
            }
        }
    }
}