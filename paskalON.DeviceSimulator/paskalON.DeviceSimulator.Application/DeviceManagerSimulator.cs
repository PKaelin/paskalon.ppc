// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.C37;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Protocols.Modbus.NModbus;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Application
{
    /// <summary>
    /// Device manager that hosts simulated communication endpoints.
    /// </summary>
    public class DeviceManagerSimulator : DeviceManager, IDisposable
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
        /// Registry of simulator C37 streams.
        /// </summary>
        private readonly ISimulationStreamRegistry _streams;


        /// <summary>
        /// Factories for supported simulator models.
        /// </summary>
        private readonly IEnumerable<ISimulationModelFactory> _simulationFactories;


        /// <summary>
        /// Registered simulator models.
        /// </summary>
        private readonly SimulationDeviceRegistry _simulationDevices;


        /// <summary>
        /// C37 servers hosted by the simulator.
        /// </summary>
        private readonly List<C37Server> _c37Servers = new List<C37Server>();


        /// <summary>
        /// Modbus servers hosted by the simulator.
        /// </summary>
        private readonly List<NModbusServer> _modbusServers = new List<NModbusServer>();


        /// <summary>
        /// Constructor of <see cref="DeviceManagerSimulator"/>.
        /// </summary>
        /// <param name="logger">Application logger.</param>
        /// <param name="services">Application service provider.</param>
        /// <param name="publisherFactory">Metrics publisher factory.</param>
        /// <param name="deviceFactoryModbus">Modbus device factory.</param>
        /// <param name="deviceFactoryC37">C37 device factory.</param>
        /// <param name="stores">Simulation store registry.</param>
        /// <param name="simulationFactories">Factories for supported simulator models.</param>
        /// <param name="simulationDevices">Registry of simulator models.</param>
        /// <param name="streams">Simulation stream registry.</param>
        public DeviceManagerSimulator(ILogger<DeviceManagerSimulator> logger, IServiceProvider services, IMetricsPublisherFactory publisherFactory,
            IModbusDeviceFactory deviceFactoryModbus, IC37DeviceFactory deviceFactoryC37, ISimulationStoreRegistry stores, ISimulationStreamRegistry streams,
            IEnumerable<ISimulationModelFactory> simulationFactories, SimulationDeviceRegistry simulationDevices)
            : base(logger, services, publisherFactory, deviceFactoryModbus, deviceFactoryC37)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(stores);
            ArgumentNullException.ThrowIfNull(streams);
            ArgumentNullException.ThrowIfNull(simulationFactories);
            ArgumentNullException.ThrowIfNull(simulationDevices);

            _logger = logger;
            _stores = stores;
            _streams = streams;
            _simulationFactories = simulationFactories;
            _simulationDevices = simulationDevices;
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
        public override void ConnectDevices()
        {
            // Dont connect to the devices as the device simulator simulates the devices
        }


        private void CreateServers()
        {
            _logger.LogInformation("Create all device servers during startup");
            ILogger<NModbusServer> modbusLogger = _services.GetRequiredService<ILogger<NModbusServer>>();
            ILogger<C37Server> c37Logger = _services.GetRequiredService<ILogger<C37Server>>();

            foreach (IGrouping<(string Address, int Port), IC37TransmissionEngine> engineGroup in C37TransmissionEngines
                .GroupBy(engine => (engine.DestinationAddress.ToUpperInvariant(), engine.DestinationPort)))
            {
                IReadOnlyList<IPmuDataSimulation> simulations = _streams.Streams
                    .Where(entry => entry.Key.Address.ToUpperInvariant() == engineGroup.Key.Address && entry.Key.Port == engineGroup.Key.Port)
                    .Select(entry => (IPmuDataSimulation)entry.Value)
                    .ToArray();
                C37Server server = new C37Server(c37Logger, simulations, engineGroup.Key.Port, _dataRate);
                _c37Servers.Add(server);
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
                _modbusServers.Add(server);
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

            foreach (BatteryBankBase device in BatteryBanks)
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

            IEnumerable<PowerMeterBase> powerMeters = SystemPowerMeters.Cast<PowerMeterBase>()
                .Concat(AuxiliaryPowerMeters)
                .Concat(ExternalPowerMeters)
                .Concat(CircuitPowerMeters);

            foreach (PowerMeterBase device in powerMeters.Where(device => device.TargetStreamId is not 0))
            {
                PmuDataSimulation stream = _streams.GetOrCreate(device);
                ISimulatedDevice? simulation = _simulationFactories
                    .Select(factory => factory.Create(device, stream, PowerConversionSystems))
                    .FirstOrDefault(candidate => candidate is not null);

                if (simulation is not null)
                {
                    _simulationDevices.Add(simulation);
                }
            }
        }


        /// <summary>
        /// Disposes device manager simulator.
        /// Managed by Dependency Injection and therefore called when application shuts down.
        /// </summary>
        public override async void Dispose()
        {
            foreach (C37Server server in _c37Servers)
            {
                await server.StopAsync();
                await server.DisposeAsync();
            }

            foreach (NModbusServer server in _modbusServers)
            {
                await server.StopAsync();
                await server.DisposeAsync();
            }
        }
    }
}
