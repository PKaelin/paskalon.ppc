// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Configs;
using paskalON.DeviceSimulator.Application.Simulations;
using paskalON.Protocols.Modbus;

namespace paskalON.DeviceSimulator.Application.Factories
{
    /// <summary>
    /// Creates Modbus dependencies backed by simulator memory stores.
    /// </summary>
    public sealed class SimulationModbusDeviceFactory : IModbusDeviceFactory
    {
        private readonly ISimulationStoreRegistry _stores;


        /// <summary>
        /// Constructor of <see cref="SimulationModbusDeviceFactory"/>.
        /// </summary>
        /// <param name="stores">Simulation store registry.</param>
        public SimulationModbusDeviceFactory(ISimulationStoreRegistry stores)
        {
            ArgumentNullException.ThrowIfNull(stores);
            _stores = stores;
        }


        /// <inheritdoc/>
        public (IModbusDataface Dataface, IModbusClient Client) Create(ModbusConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            IModbusDataface dataface = new ModbusRegister(config.Name);
            IModbusClient client = new MemoryModbusClient(_stores.GetOrCreate(config), config.Address, config.Port, config.UnitId);

            return (dataface, client);
        }
    }
}