// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Equipments.C37;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Protocols.Modbus.NModbus;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Application
{
    public class DeviceManagerSimulator : DeviceManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DeviceManagerSimulator> _logger;

        private ushort _dataRate = 1;
        private CancellationToken _cancellationToken;

        Dictionary<ModbusDataMemoryStoreKey, IModbusDataStore> modbusDataStores = new Dictionary<ModbusDataMemoryStoreKey, IModbusDataStore>();
        Dictionary<PmuDataSimulationKey, IPmuDataSimulation> c37Simulations = new Dictionary<PmuDataSimulationKey, IPmuDataSimulation>();



        public DeviceManagerSimulator(ILogger<DeviceManagerSimulator> logger, IServiceProvider services, IMetricsPublisherFactory publisherFactory,
            IModbusDeviceFactory deviceFactoryModbus, IC37DeviceFactory deviceFactoryC37)
            : base(logger, services, publisherFactory, deviceFactoryModbus, deviceFactoryC37)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        public void Initialize(ushort dataRate, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dataRate);

            _dataRate = dataRate;
            _cancellationToken = cancellationToken;
        }


        public override async Task LoadDerAsync(IDerRepository repository)
        {
            await base.LoadDerAsync(repository);
            CreateServers();
        }


        protected override void ConnectDevices()
        {
            // Dont connect to the devices as the device simulator simulates the devices
        }


        private void CreateServers()
        {
            _logger.LogInformation("Create all device servers during startup");
            ILogger<NModbusServer> modbusLogger = _services.GetRequiredService<ILogger<NModbusServer>>();
            ILogger<C37Server> c37Logger = _services.GetRequiredService<ILogger<C37Server>>();

            foreach (IC37TransmissionEngine engine in C37TransmissionEngines)
            {
                PmuDataSimulation simulation = new PmuDataSimulation();
                List<PmuDataSimulation> simulations = new List<PmuDataSimulation> { simulation };
                C37Server server = new C37Server(c37Logger, simulations, engine.DestinationPort, _dataRate);
                c37Simulations.Add(new PmuDataSimulationKey { Port = engine.DestinationPort, StreamId = engine.StreamId }, simulation);
                _ = Task.Run(() => server.StartAsync(_cancellationToken));
            }

            foreach (IModbusPollingEngine engine in ModbusPollingEngines)
            {
                ModbusDataMemoryStore store = new ModbusDataMemoryStore();
                NModbusServer server = new NModbusServer(modbusLogger, store, engine.DestinationAddress, engine.DestinationPort);
                modbusDataStores.Add(new ModbusDataMemoryStoreKey { Port = engine.DestinationPort }, store);
                _ = Task.Run(() => server.StartAsync(_cancellationToken));
            }
        }
    }
}
