// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.C37;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.DeviceSimulator.Application.Simulations;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Protocols.Modbus.NModbus;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Application
{
    /// <summary>
    /// Device manager that hosts simulated communication endpoints.
    /// </summary>
    public class DeviceManagerSimulator : DeviceManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DeviceManagerSimulator> _logger;

        /// <summary>
        /// Configured C37 data rate.
        /// </summary>
        private ushort _dataRate = 1;


        /// <summary>
        /// Cancellation token for simulator servers.
        /// </summary>
        private CancellationToken _cancellationToken;


        /// <summary>
        /// Registry of simulator Modbus stores.
        /// </summary>
        private readonly ISimulationStoreRegistry _stores;


        /// <summary>
        /// Factories for supported simulator models.
        /// </summary>
        private readonly IEnumerable<ISimulationModelFactory> _simulationFactories;


        /// <summary>
        /// Registered simulator models.
        /// </summary>
        private readonly SimulationDeviceRegistry _simulationDevices;


        /// <summary>
        /// C37 simulation instances by endpoint.
        /// </summary>
        Dictionary<PmuDataSimulationKey, IPmuDataSimulation> c37Simulations = new Dictionary<PmuDataSimulationKey, IPmuDataSimulation>();


        /// <summary>
        /// Constructor of <see cref="DeviceManagerSimulator"/>.
        /// </summary>
        /// <param name="logger">Application logger.</param>
        /// <param name="services">Application service provider.</param>
        /// <param name="publisherFactory">Metrics publisher factory.</param>
        /// <param name="deviceFactoryModbus">Modbus device factory.</param>
        /// <param name="deviceFactoryC37">C37 device factory.</param>
        /// <param name="stores">Simulation store registry.</param>
        public DeviceManagerSimulator(ILogger<DeviceManagerSimulator> logger, IServiceProvider services, IMetricsPublisherFactory publisherFactory,
            IModbusDeviceFactory deviceFactoryModbus, IC37DeviceFactory deviceFactoryC37,
            ISimulationStoreRegistry? stores = null,
            IEnumerable<ISimulationModelFactory>? simulationFactories = null,
            SimulationDeviceRegistry? simulationDevices = null)
            : base(logger, services, publisherFactory, deviceFactoryModbus, deviceFactoryC37)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(stores);

            _logger = logger;
            _stores = stores;
            _simulationFactories = simulationFactories ?? [];
            _simulationDevices = simulationDevices ?? new SimulationDeviceRegistry();
        }


        /// <summary>
        /// Initializes simulator timing and cancellation.
        /// </summary>
        /// <param name="dataRate">C37 data rate.</param>
        /// <param name="cancellationToken">Shutdown cancellation token.</param>
        public void Initialize(ushort dataRate, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dataRate);

            _dataRate = dataRate;
            _cancellationToken = cancellationToken;
        }


        /// <inheritdoc/>
        public override async Task LoadDerAsync(IDerRepository repository)
        {
            await base.LoadDerAsync(repository);
            RegisterSimulationModels();
            CreateServers();
        }


        /// <inheritdoc/>
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
                ModbusDataMemoryStoreKey key = new ModbusDataMemoryStoreKey
                {
                    Address = engine.DestinationAddress,
                    Port = engine.DestinationPort
                };

                IModbusDataStore store = _stores.Stores
                    .Where(entry => entry.Key.Address == key.Address && entry.Key.Port == key.Port)
                    .Select(entry => entry.Value)
                    .First();
                NModbusServer server = new NModbusServer(modbusLogger, store, engine.DestinationAddress, engine.DestinationPort);
                _ = Task.Run(() => server.StartAsync(_cancellationToken));
            }
        }


        private void RegisterSimulationModels()
        {
            foreach (PowerConversionSystemBase device in PowerConversionSystems)
            {
                IModbusDataStore store = _stores.Stores
                    .Where(entry => entry.Key.Address == device.TargetAddress && entry.Key.Port == device.TargetPort)
                    .Select(entry => entry.Value)
                    .First();

                ISimulatedDevice? simulation = _simulationFactories
                    .Select(factory => factory.Create(device, store))
                    .FirstOrDefault(candidate => candidate is not null);

                if (simulation is not null)
                {
                    _simulationDevices.Add(simulation);
                }
            }
        }
    }
}
