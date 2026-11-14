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
        /// C37 servers hosted by the simulator keyed by their endpoint (address in upper case).
        /// </summary>
        private readonly Dictionary<(string Address, int Port), C37Server> _c37Servers = new Dictionary<(string Address, int Port), C37Server>();


        /// <summary>
        /// Modbus servers hosted by the simulator keyed by their endpoint.
        /// </summary>
        private readonly Dictionary<(string Address, int Port), NModbusServer> _modbusServers = new Dictionary<(string Address, int Port), NModbusServer>();


        /// <summary>
        /// Endpoints (address in upper case) of the Modbus and C37 servers that simulate a communication loss.
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
            ArgumentNullException.ThrowIfNull(address);

            lock (_dataLock)
            {
                return _notResponding.Contains(Normalize((address, port))) is false;
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

            if (deviceType == SimulationDeviceType.PowerMeter)
            {
                await SetC37CommunicationAsync(deviceType, deviceId, endpoint, isResponding);
            }
            else
            {
                await SetModbusCommunicationAsync(deviceType, deviceId, endpoint, isResponding);
            }

            lock (_dataLock)
            {
                if (isResponding)
                {
                    _notResponding.Remove(Normalize(endpoint));
                }
                else
                {
                    _notResponding.Add(Normalize(endpoint));
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


        /// <inheritdoc/>
        public void SetResponseDynamics(int deviceId, double deadTimeMilliseconds, double timeConstantMilliseconds)
        {
            PowerConversionSystemSimulationBase device = SimulationDevices.Find<PowerConversionSystemSimulationBase>(deviceId)
                ?? throw new KeyNotFoundException($"No simulated {SimulationDeviceType.PowerConversionSystem} with device ID {deviceId} exists.");

            device.SetResponseDynamics(TimeSpan.FromMilliseconds(deadTimeMilliseconds), TimeSpan.FromMilliseconds(timeConstantMilliseconds));
            _logger.LogInformation("Response of PCS {DeviceId} changed. Dead time: {DeadTime} ms Time constant: {TimeConstant} ms",
                deviceId, deadTimeMilliseconds, timeConstantMilliseconds);
        }


        /// <inheritdoc/>
        public void SetStateOfCharge(int deviceId, double preferredStateOfCharge)
        {
            BatteryBankSimulationBase device = FindBatteryBank(deviceId);

            device.SetPreferredStateOfCharge(preferredStateOfCharge);
            _logger.LogInformation("State of charge of battery bank {DeviceId} set to {PreferredStateOfCharge}% preferred.",
                deviceId, preferredStateOfCharge);
        }


        /// <inheritdoc/>
        public void SetChargeSpeed(int deviceId, double chargeSpeedFactor)
        {
            BatteryBankSimulationBase device = FindBatteryBank(deviceId);

            device.SetChargeSpeedFactor(chargeSpeedFactor);
            _logger.LogInformation("Charge speed of battery bank {DeviceId} set to factor {ChargeSpeedFactor}.", deviceId, chargeSpeedFactor);
        }


        /// <inheritdoc/>
        public void SetPowerTaper(int deviceId, bool isEnabled)
        {
            BatteryBankSimulationBase device = FindBatteryBank(deviceId);

            device.SetPowerTaper(isEnabled);
            _logger.LogInformation("Power taper of battery bank {DeviceId} enabled: {IsEnabled}.", deviceId, isEnabled);
        }


        /// <summary>
        /// Finds a simulated battery bank.
        /// </summary>
        /// <param name="deviceId">Device ID of the simulated battery bank.</param>
        /// <returns>The simulated battery bank.</returns>
        /// <exception cref="KeyNotFoundException">The simulated battery bank does not exist.</exception>
        private BatteryBankSimulationBase FindBatteryBank(int deviceId)
        {
            return SimulationDevices.Find<BatteryBankSimulationBase>(deviceId)
                ?? throw new KeyNotFoundException($"No simulated {SimulationDeviceType.BatteryBank} with device ID {deviceId} exists.");
        }


        /// <summary>
        /// Starts or stops the Modbus server of a PCS or battery bank.
        /// </summary>
        /// <param name="deviceType">Type of the simulated device.</param>
        /// <param name="deviceId">Device ID of the simulated device.</param>
        /// <param name="endpoint">Modbus endpoint of the device.</param>
        /// <param name="isResponding">True to start, false to stop the server.</param>
        /// <returns>Task</returns>
        /// <exception cref="KeyNotFoundException">No Modbus server exists for the endpoint.</exception>
        private async Task SetModbusCommunicationAsync(SimulationDeviceType deviceType, int deviceId, (string Address, int Port) endpoint, bool isResponding)
        {
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
        }


        /// <summary>
        /// Starts or stops the C37 server of a power meter.
        /// </summary>
        /// <param name="deviceType">Type of the simulated device.</param>
        /// <param name="deviceId">Device ID of the simulated device.</param>
        /// <param name="endpoint">C37 endpoint of the device.</param>
        /// <param name="isResponding">True to start, false to stop the server.</param>
        /// <returns>Task</returns>
        /// <exception cref="KeyNotFoundException">No C37 server exists for the endpoint.</exception>
        /// <remarks>
        /// A C37 server streams all PMU streams of its port, so all power meters on the same port lose the communication.
        /// </remarks>
        private async Task SetC37CommunicationAsync(SimulationDeviceType deviceType, int deviceId, (string Address, int Port) endpoint, bool isResponding)
        {
            C37Server? server;

            lock (_dataLock)
            {
                _c37Servers.TryGetValue(Normalize(endpoint), out server);
            }

            if (server is null)
            {
                throw new KeyNotFoundException($"No C37 server exists for {deviceType} with device ID {deviceId}.");
            }

            if (isResponding)
            {
                await server.StartAsync(_cancellationToken);
            }
            else
            {
                await server.StopAsync();
            }
        }


        /// <summary>
        /// Normalizes an endpoint so that the address is compared case insensitive.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>The endpoint with the address in upper case.</returns>
        private static (string Address, int Port) Normalize((string Address, int Port) endpoint)
        {
            return (endpoint.Address.ToUpperInvariant(), endpoint.Port);
        }


        /// <summary>
        /// Finds the Modbus or C37 endpoint of a device.
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
                case SimulationDeviceType.PowerMeter:
                    PowerMeterBase? meter = AllPowerMeters().FirstOrDefault(device => device.DeviceId == deviceId);

                    if (meter is not null)
                    {
                        return (meter.TargetAddress, meter.TargetPort);
                    }

                    break;
            }

            throw new KeyNotFoundException($"No {deviceType} with device ID {deviceId} exists.");
        }


        /// <summary>
        /// Gets all system, auxiliary, external and circuit power meters.
        /// </summary>
        /// <returns>All power meters.</returns>
        private IEnumerable<PowerMeterBase> AllPowerMeters()
        {
            return SystemPowerMeters.Cast<PowerMeterBase>()
                .Concat(AuxiliaryPowerMeters)
                .Concat(ExternalPowerMeters)
                .Concat(CircuitPowerMeters);
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

                lock (_dataLock)
                {
                    _c37Servers[engineGroup.Key] = server;
                }

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

            IEnumerable<PowerMeterBase> powerMeters = AllPowerMeters();
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
            List<C37Server> c37Servers;
            List<NModbusServer> modbusServers;

            lock (_dataLock)
            {
                c37Servers = _c37Servers.Values.ToList();
                modbusServers = _modbusServers.Values.ToList();
            }

            foreach (C37Server server in c37Servers)
            {
                await server.StopAsync();
                await server.DisposeAsync();
            }

            foreach (NModbusServer server in modbusServers)
            {
                await server.StopAsync();
                await server.DisposeAsync();
            }
        }
    }
}
