// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Devices.Application;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.C37;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.DeviceSimulator.Application.Repositories;
using paskalON.DeviceSimulator.Dto.Simulations;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Protocols.Modbus.NModbus;
using paskalON.Telemetry.Factories;

namespace paskalON.DeviceSimulator.Application
{
    /// <summary>
    /// Device manager that hosts simulated communication endpoints.
    /// </summary>
    public class DeviceManagerSimulator : DeviceManager, IDeviceManagerSimulator, IDisposable
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
        /// Factories for supported simulator models.
        /// </summary>
        private readonly IEnumerable<ISimulationModelFactory> _simulationFactories;


        /// <summary>
        /// Shared simulated grid.
        /// </summary>
        private readonly SimulatedGrid _grid;


        /// <summary>
        /// Simulator custom configurations of the devices.
        /// </summary>
        private SimulationCustomConfigs _customConfigs = SimulationCustomConfigs.Empty;


        /// <summary>
        /// C37 servers hosted by the simulator.
        /// </summary>
        private readonly List<C37Server> _c37Servers = new List<C37Server>();


        /// <summary>
        /// Modbus servers hosted by the simulator keyed by their endpoint.
        /// </summary>
        private readonly Dictionary<(string Address, int Port), NModbusServer> _modbusServers = new Dictionary<(string Address, int Port), NModbusServer>();


        /// <summary>
        /// Endpoints of the Modbus servers that simulate a communication loss.
        /// </summary>
        private readonly HashSet<(string Address, int Port)> _notResponding = new HashSet<(string Address, int Port)>();


        /// <summary>
        /// Data lock object
        /// </summary>
        protected readonly object _dataLock = new object();


        /// <inheritdoc/>
        public ISimulationStoreRegistry StoreRegisters { get; init; }


        /// <inheritdoc/>
        public ISimulationStreamRegistry StreamRegisters { get; init; }


        /// <inheritdoc/>
        public SimulationDeviceRegistry SimulationDevices { get; init; }


        /// <inheritdoc/>
        public List<string> ExpandedUnits
        {
            get { lock (_dataLock) { return field; } }
            set { lock (_dataLock) { field = value; } }
        } = new List<string>();


        /// <inheritdoc/>
        public List<string> ExpandedDevices
        {
            get { lock (_dataLock) { return field; } }
            set { lock (_dataLock) { field = value; } }
        } = new List<string>();


        /// <summary>
        /// Constructor of <see cref="DeviceManagerSimulator"/>.
        /// </summary>
        /// <param name="logger">Application logger.</param>
        /// <param name="services">Application service provider.</param>
        /// <param name="publisherFactory">Metrics publisher factory.</param>
        /// <param name="deviceFactoryModbus">Modbus device factory.</param>
        /// <param name="deviceFactoryC37">C37 device factory.</param>
        /// <param name="stores">Simulation store registry.</param>
        /// <param name="streams">Simulation stream registry.</param>
        /// <param name="simulationFactories">Factories for supported simulator models.</param>
        /// <param name="simulationDevices">Registry of simulator models.</param>
        /// <param name="grid">Shared simulated grid.</param>
        public DeviceManagerSimulator(ILogger<DeviceManagerSimulator> logger, IServiceProvider services, IMetricsPublisherFactory publisherFactory,
            IModbusDeviceFactory deviceFactoryModbus, IC37DeviceFactory deviceFactoryC37, ISimulationStoreRegistry stores, ISimulationStreamRegistry streams,
            IEnumerable<ISimulationModelFactory> simulationFactories, SimulationDeviceRegistry simulationDevices, SimulatedGrid grid)
            : base(logger, services, publisherFactory, deviceFactoryModbus, deviceFactoryC37)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(stores);
            ArgumentNullException.ThrowIfNull(streams);
            ArgumentNullException.ThrowIfNull(simulationFactories);
            ArgumentNullException.ThrowIfNull(simulationDevices);
            ArgumentNullException.ThrowIfNull(grid);

            _logger = logger;
            StoreRegisters = stores;
            StreamRegisters = streams;
            SimulationDevices = simulationDevices;
            _simulationFactories = simulationFactories;
            _grid = grid;
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


        /// <summary>
        /// Loads the simulator custom configurations of the devices.
        /// </summary>
        /// <param name="repository">Simulator custom configuration repository.</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// Must be called before <see cref="LoadDerAsync(IDerRepository)"/>.
        /// </remarks>
        public async Task LoadCustomConfigsAsync(ISimulationCustomRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            _customConfigs = await repository.GetCustomConfigsAsync();
        }


        /// <inheritdoc/>
        public override async Task LoadDerAsync(IDerRepository repository)
        {
            await base.LoadDerAsync(repository);
            ConfigureGrid();
            RegisterSimulationModels();
            CreateServers();
        }


        /// <inheritdoc/>
        public override void ConnectDevices()
        {
            // Dont connect to the devices as the device simulator simulates the devices
        }


        /// <inheritdoc/>
        public bool IsResponding(string address, int port)
        {
            lock (_dataLock)
            {
                return _notResponding.Contains((address, port)) is false;
            }
        }


        /// <inheritdoc/>
        public void SetEvents(SimulationDeviceType deviceType, int deviceId, ushort? warning, ushort? fault, ushort? vendorEvent)
        {
            IEventInjectable? found = deviceType switch
            {
                SimulationDeviceType.PowerConversionSystem => SimulationDevices.Find<PowerConversionSystemSimulationBase>(deviceId),
                SimulationDeviceType.BatteryBank => SimulationDevices.Find<BatteryBankSimulationBase>(deviceId),
                _ => null
            };
            IEventInjectable device = found ?? throw new KeyNotFoundException($"No simulated {deviceType} with device ID {deviceId} exists.");

            device.SetEvents(warning, fault, vendorEvent);
            _logger.LogInformation("Events of {DeviceType} {DeviceId} set. Warning: {Warning} Fault: {Fault} VendorEvent: {VendorEvent}",
                deviceType, deviceId, warning, fault, vendorEvent);
        }


        /// <inheritdoc/>
        public async Task SetCommunicationAsync(SimulationDeviceType deviceType, int deviceId, bool isResponding)
        {
            (string Address, int Port) endpoint = FindEndpoint(deviceType, deviceId);
            NModbusServer? server;

            lock (_dataLock)
            {
                _modbusServers.TryGetValue(endpoint, out server);
            }

            if (server is null)
            {
                throw new KeyNotFoundException($"No Modbus server exists for {deviceType} with device ID {deviceId}.");
            }

            if (isResponding)
            {
                await server.StartAsync(_cancellationToken);
            }
            else
            {
                await server.StopAsync();
            }

            lock (_dataLock)
            {
                if (isResponding)
                {
                    _notResponding.Remove(endpoint);
                }
                else
                {
                    _notResponding.Add(endpoint);
                }
            }

            _logger.LogInformation("Communication of {DeviceType} {DeviceId} ({Address}:{Port}) is responding: {IsResponding}",
                deviceType, deviceId, endpoint.Address, endpoint.Port, isResponding);
        }


        /// <inheritdoc/>
        public void SlowDownPower(int deviceId, double slowActivePowerByPercent, double slowReactivePowerByPercent)
        {
            PowerConversionSystemSimulationBase device = SimulationDevices.Find<PowerConversionSystemSimulationBase>(deviceId)
                ?? throw new KeyNotFoundException($"No simulated {SimulationDeviceType.PowerConversionSystem} with device ID {deviceId} exists.");

            device.SlowDownPower(slowActivePowerByPercent, slowReactivePowerByPercent);
            _logger.LogInformation("Power of PCS {DeviceId} slowed down. Active: {Active}%/s Reactive: {Reactive}%/s",
                deviceId, slowActivePowerByPercent, slowReactivePowerByPercent);
        }


        /// <summary>
        /// Finds the Modbus endpoint of a device.
        /// </summary>
        private (string Address, int Port) FindEndpoint(SimulationDeviceType deviceType, int deviceId)
        {
            switch (deviceType)
            {
                case SimulationDeviceType.PowerConversionSystem:
                    PowerConversionSystemBase? pcs = PowerConversionSystems.FirstOrDefault(device => device.DeviceId == deviceId);

                    if (pcs is not null)
                    {
                        return (pcs.TargetAddress, pcs.TargetPort);
                    }

                    break;
                case SimulationDeviceType.BatteryBank:
                    BatteryBankBase? bank = BatteryBanks.FirstOrDefault(device => device.DeviceId == deviceId);

                    if (bank is not null)
                    {
                        return (bank.TargetAddress, bank.TargetPort);
                    }

                    break;
            }

            throw new KeyNotFoundException($"No {deviceType} with device ID {deviceId} exists.");
        }


        /// <summary>
        /// Configures the shared grid from the custom configurations of the first system power meter that has them.
        /// </summary>
        private void ConfigureGrid()
        {
            IReadOnlyDictionary<string, string> customs = SystemPowerMeters
                .Select(meter => _customConfigs.GetSystemPowerMeter(meter.DeviceId))
                .FirstOrDefault(entries => entries.ContainsKey(SimulatedGrid.NominalFrequencyKey) || entries.ContainsKey(SimulatedGrid.NominalVoltageLineToLineKey))
                ?? SimulationCustoms.Empty;

            double frequency = SimulationCustoms.GetDouble(customs, SimulatedGrid.NominalFrequencyKey, SimulatedGrid.DefaultNominalFrequency);
            double voltage = SimulationCustoms.GetDouble(customs, SimulatedGrid.NominalVoltageLineToLineKey, SimulatedGrid.DefaultNominalVoltageLineToLine);

            _grid.Configure(frequency, voltage);
            _logger.LogInformation("Simulated grid configured with {Frequency}Hz and {Voltage}V line to line", frequency, voltage);
        }


        private void CreateServers()
        {
            _logger.LogInformation("Create all device servers during startup");
            ILogger<NModbusServer> modbusLogger = _services.GetRequiredService<ILogger<NModbusServer>>();
            ILogger<C37Server> c37Logger = _services.GetRequiredService<ILogger<C37Server>>();

            foreach (IGrouping<(string Address, int Port), IC37TransmissionEngine> engineGroup in C37TransmissionEngines
                .GroupBy(engine => (engine.DestinationAddress.ToUpperInvariant(), engine.DestinationPort)))
            {
                IReadOnlyList<IPmuDataSimulation> simulations = StreamRegisters.Streams
                    .Where(entry => entry.Key.Address.ToUpperInvariant() == engineGroup.Key.Address && entry.Key.Port == engineGroup.Key.Port)
                    .Select(entry => (IPmuDataSimulation)entry.Value)
                    .ToArray();
                C37Server server = new C37Server(c37Logger, simulations, engineGroup.Key.Port, _dataRate);
                _c37Servers.Add(server);
                _ = Task.Run(() => server.StartAsync(_cancellationToken));
            }

            foreach (IModbusPollingEngine engine in ModbusPollingEngines)
            {
                (string Address, int Port) endpoint = (engine.DestinationAddress, engine.DestinationPort);
                SimulationModbusDataStore store = StoreRegisters.Find(endpoint.Address, endpoint.Port);
                NModbusServer server = new NModbusServer(modbusLogger, store, endpoint.Address, endpoint.Port);

                lock (_dataLock)
                {
                    _modbusServers[endpoint] = server;
                }

                _ = Task.Run(() => server.StartAsync(_cancellationToken));
            }
        }


        private void RegisterSimulationModels()
        {
            // Registration order is the tick order: the PCS allocates the power to its battery banks before they integrate it.
            Dictionary<PowerConversionSystemBase, PowerConversionSystemSimulationBase> pcsSimulations =
                new Dictionary<PowerConversionSystemBase, PowerConversionSystemSimulationBase>();
            Dictionary<BatteryBankBase, BatteryBankSimulationBase> bankSimulations = new Dictionary<BatteryBankBase, BatteryBankSimulationBase>();

            foreach (PowerConversionSystemBase device in PowerConversionSystems)
            {
                SimulationModbusDataStore store = StoreRegisters.Find(device.TargetAddress, device.TargetPort);
                PowerConversionSystemSimulationBase? simulation = _simulationFactories
                    .Select(factory => factory.Create(device, store, _grid))
                    .FirstOrDefault(candidate => candidate is not null);

                if (simulation is not null)
                {
                    pcsSimulations.Add(device, simulation);
                    SimulationDevices.Add(simulation);
                }
            }

            foreach (BatteryBankBase device in BatteryBanks)
            {
                SimulationModbusDataStore store = StoreRegisters.Find(device.TargetAddress, device.TargetPort);
                IReadOnlyDictionary<string, string> customs = _customConfigs.GetBatteryBank(device.DeviceId);
                BatteryBankSimulationBase? simulation = _simulationFactories
                    .Select(factory => factory.Create(device, store, customs))
                    .FirstOrDefault(candidate => candidate is not null);

                if (simulation is not null)
                {
                    bankSimulations.Add(device, simulation);
                    SimulationDevices.Add(simulation);
                }
            }

            foreach ((PowerConversionSystemBase device, PowerConversionSystemSimulationBase simulation) in pcsSimulations)
            {
                if (device.DerUnit is DerBatteryStorageUnit unit)
                {
                    simulation.AttachBatteryBanks(unit.BatteryBanks
                        .Select(bank => bankSimulations.GetValueOrDefault(bank))
                        .OfType<BatteryBankSimulationBase>());
                }
            }

            IEnumerable<PowerMeterBase> powerMeters = SystemPowerMeters.Cast<PowerMeterBase>()
                .Concat(AuxiliaryPowerMeters)
                .Concat(ExternalPowerMeters)
                .Concat(CircuitPowerMeters);
            IReadOnlyCollection<PowerConversionSystemSimulationBase> allPcsSimulations = pcsSimulations.Values.ToArray();

            foreach (PowerMeterBase device in powerMeters.Where(device => device.TargetStreamId is not 0))
            {
                PmuDataSimulation stream = StreamRegisters.GetOrCreate(device);
                ISimulatedDevice? simulation = _simulationFactories
                    .Select(factory => factory.Create(device, stream, allPcsSimulations, _grid))
                    .FirstOrDefault(candidate => candidate is not null);

                if (simulation is not null)
                {
                    SimulationDevices.Add(simulation);
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

            List<NModbusServer> modbusServers;

            lock (_dataLock)
            {
                modbusServers = _modbusServers.Values.ToList();
            }

            foreach (NModbusServer server in modbusServers)
            {
                await server.StopAsync();
                await server.DisposeAsync();
            }
        }
    }
}
