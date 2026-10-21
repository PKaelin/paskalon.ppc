// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Thread-safe registry of simulator Modbus stores (Modbus registers).
    /// </summary>
    /// <remarks>
    /// Every simulated device has its own IP:Port endpoint and therefore its own store.
    /// </remarks>
    public class SimulationStoreRegistry : ISimulationStoreRegistry
    {
        /// <summary>
        /// Dictionary of Modbus data key and their stores.
        /// </summary>
        private readonly Dictionary<ModbusDataMemoryStoreKey, SimulationModbusDataStore> _stores = new Dictionary<ModbusDataMemoryStoreKey, SimulationModbusDataStore>();


        /// <summary>
        /// Data lock object
        /// </summary>
        private readonly object _dataLock = new object();


        /// <inheritdoc/>
        public IReadOnlyDictionary<ModbusDataMemoryStoreKey, SimulationModbusDataStore> Stores
        {
            get
            {
                lock (_dataLock)
                {
                    return new Dictionary<ModbusDataMemoryStoreKey, SimulationModbusDataStore>(_stores);
                }
            }
        }


        /// <inheritdoc/>
        public SimulationModbusDataStore GetOrCreate(ModbusConfig config)
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
                if (_stores.TryGetValue(key, out SimulationModbusDataStore? store) == true)
                {
                    return store;
                }

                store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
                _stores.Add(key, store);

                return store;
            }
        }


        /// <inheritdoc/>
        public SimulationModbusDataStore Find(string address, int port)
        {
            ArgumentException.ThrowIfNullOrEmpty(address);

            lock (_dataLock)
            {
                return _stores.Where(entry => entry.Key.Address == address && entry.Key.Port == port)
                    .Select(entry => entry.Value)
                    .FirstOrDefault()
                    ?? throw new InvalidOperationException($"No Modbus simulation store exists for endpoint '{address}:{port}'.");
            }
        }
    }
}
