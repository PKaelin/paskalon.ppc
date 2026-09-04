// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Dataface.C37s;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.GenericModbusDevices;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.C37;
using paskalON.Devices.Equipments.Modbus;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.Protocols.C37118;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;
using System.Reflection;

namespace paskalON.Devices.Application
{
    /// <summary>
    /// The device manager gets the device service configuration and
    /// creates a domain structure accordingly.
    /// </summary>
    public class DeviceManager : IDeviceManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DeviceManager> _logger;


        /// <summary>
        /// Application wide shutdown token.
        /// </summary>
        private CancellationToken _shutdownToken;


        /// <summary>
        /// Service provider used to resolve dependencies of configured equipment.
        /// </summary>
        private readonly IServiceProvider _services;


        /// <summary>
        /// Metrics publisher factory interface.
        /// </summary>
        private readonly IMetricsPublisherFactory _publisherFactory;


        /// <summary>
        /// Modbus device factory interface.
        /// </summary>
        private readonly IModbusDeviceFactory _deviceFactoryModbus;


        /// <summary>
        /// C37 device factory interface.
        /// </summary>
        private readonly IC37DeviceFactory _deviceFactoryC37;


        /// <summary>
        /// Dictionary with device id and PCS. 
        /// </summary>
        protected Dictionary<int, PowerConversionSystemBase> _powerConversionSystems = new Dictionary<int, PowerConversionSystemBase>();


        /// <summary>
        /// Dictionary with device id and BB.
        /// </summary>
        protected Dictionary<int, BatteryBankBase> _batteryBanks = new Dictionary<int, BatteryBankBase>();


        /// <summary>
        /// Dictionary with device id and Solar Panel.
        /// </summary>
        protected Dictionary<int, SolarPanelBase> _solarPanels = new Dictionary<int, SolarPanelBase>();


        /// <summary>
        /// Dictionary with device id and External Meter.
        /// </summary>
        protected Dictionary<int, ExternalPowerMeter> _externalPowerMeters = new Dictionary<int, ExternalPowerMeter>();


        /// <summary>
        /// Dictionary with device id and Auxiliary Meter.
        /// </summary>
        protected Dictionary<int, AuxiliaryPowerMeter> _auxiliaryPowerMeters = new Dictionary<int, AuxiliaryPowerMeter>();


        /// <summary>
        /// Dictionary with device id and System Meter.
        /// </summary>
        protected Dictionary<int, SystemPowerMeter> _systemPowerMeters = new Dictionary<int, SystemPowerMeter>();


        /// <summary>
        /// Dictionary with device id and Circuit Meter.
        /// </summary>
        protected readonly Dictionary<int, CircuitPowerMeter> _circuitPowerMeters = new Dictionary<int, CircuitPowerMeter>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Der Der { get; protected set; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<IMetricsPublisher> MetricsPublishers { get; protected set; } = new List<IMetricsPublisher>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<IModbusPollingEngine> ModbusPollingEngines { get; protected set; } = new List<IModbusPollingEngine>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<IC37TransmissionEngine> C37TransmissionEngines { get; protected set; } = new List<IC37TransmissionEngine>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PowerConversionSystemBase> PowerConversionSystems { get => _powerConversionSystems.Values; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<BatteryBankBase> BatteryBanks { get => _batteryBanks.Values; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<SolarPanelBase> SolarPanels { get => _solarPanels.Values; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<ExternalPowerMeter> ExternalPowerMeters { get => _externalPowerMeters.Values; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<AuxiliaryPowerMeter> AuxiliaryPowerMeters { get => _auxiliaryPowerMeters.Values; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<SystemPowerMeter> SystemPowerMeters { get => _systemPowerMeters.Values; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<CircuitPowerMeter> CircuitPowerMeters { get => _circuitPowerMeters.Values; }


        /// <summary>        
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="services">Service provider used to resolve dependencies of configured equipment.</param>
        /// <param name="publisherFactory">Metrics publisher factory interface.</param>
        /// <param name="deviceFactoryModbus">Modbus device factory interface.</param>
        /// <param name="deviceFactoryC37">C37 device factory interface.</param>
        public DeviceManager(ILogger<DeviceManager> logger, IServiceProvider services,
            IMetricsPublisherFactory publisherFactory, IModbusDeviceFactory deviceFactoryModbus, IC37DeviceFactory deviceFactoryC37)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(publisherFactory);
            ArgumentNullException.ThrowIfNull(deviceFactoryModbus);
            ArgumentNullException.ThrowIfNull(deviceFactoryC37);

            _logger = logger;
            _services = services;
            _publisherFactory = publisherFactory;
            _deviceFactoryModbus = deviceFactoryModbus;
            _deviceFactoryC37 = deviceFactoryC37;
            IHostApplicationLifetime lifetime = services.GetRequiredService<IHostApplicationLifetime>();
            _shutdownToken = lifetime.ApplicationStopping;
            // Initialized DER with a place holder
            Der = new Der(logger, new DerConfig { ChangedBy = "System", Name = "Uninitialized DER" });
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task LoadDerAsync(IDerRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            _logger.LogInformation("Load DER during startup");
            DerConfig config = await LoadDerConfig(repository);
            Der der = new Der(_logger, config);

            foreach (DerGroupConfig groupConfig in config.DerGroupConfigs)
            {
                DerGroup group = new(_logger, groupConfig, der);
                der.DerGroups.Add(group);

                foreach (DerCircuitConfig circuitConfig in groupConfig.DerCircuits)
                {
                    DerCircuit circuit = new(_logger, circuitConfig, group);
                    group.DerCircuits.Add(circuit);

                    if (circuitConfig.CircuitBreakerConfig is { } breakerConfig)
                    {
                        // TODO: Add dataface factory and parameter.
                        IMetricsPublisher gmdMetrics = _publisherFactory.Create();
                        MetricsPublishers.Add(gmdMetrics);
                        circuit.CircuitBreaker = Create<CircuitBreaker>(
                            breakerConfig.CircuitBreakerDeviceConfig.ClassName, breakerConfig,
                            CreateGenericModbusEntries(breakerConfig.CircuitBreakerDeviceConfig.GenericModbusMapConfig), gmdMetrics);
                    }

                    if (circuitConfig.CircuitPowerMeterConfig is { } meterConfig)
                    {
                        IMetricsPublisher meterMetrics = _publisherFactory.Create();
                        MetricsPublishers.Add(meterMetrics);

                        if (meterConfig.C37Config != null)
                        {
                            (IC37Dataface dataface, IC37Client client) = _deviceFactoryC37.Create(meterConfig.C37Config);
                            C37TransmissionEngines.Add(new C37TransmissionEngine(_logger, client, dataface, meterConfig.C37Config.StationName, meterConfig.C37Config.StreamId));
                            circuit.CircuitPowerMeter = Create<CircuitPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                                meterConfig, meterMetrics, dataface, client);
                        }
                        else if (meterConfig.ModbusConfig != null)
                        {
                            (IModbusDataface dataface, IModbusClient client) = _deviceFactoryModbus.Create(meterConfig.ModbusConfig);
                            ModbusPollingEngines.Add(new ModbusPollingEngine(_logger, client, dataface));
                            circuit.CircuitPowerMeter = Create<CircuitPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                                meterConfig, meterMetrics, dataface, client);
                        }
                    }

                    foreach (DerUnitConfig unitConfig in circuitConfig.DerUnitConfigs)
                    {
                        DerUnit unit;

                        switch (unitConfig)
                        {
                            case DerBatteryStorageUnitConfig batteryConfig:
                                unit = CreateBatteryUnit(batteryConfig, circuit);
                                break;

                            case DerSolarUnitConfig solarConfig:
                                unit = CreateSolarUnit(solarConfig, circuit);
                                break;

                            default:
                                throw new InvalidOperationException($"Unsupported DER unit configuration type '{unitConfig.GetType().FullName}'.");
                        }

                        circuit.DerUnits.Add(unit);
                    }
                }
            }

            LoadDevicesToCollections(der);
            LoadRootMeters(der, config);
            LoadRootGenericModbusDevices(der, config);
            Der = der;
            ConnectDevices();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task StartAllPcsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start all Power Conversion Systems");

            try
            {
                await Task.WhenAll(PowerConversionSystems.Where(p => p.IsInMaintenanceMode == false).Select(d => d.StartAsync()));
            }
            catch (AggregateException ex)
            {
                // Handle exceptions thrown by one or more parallel tasks
                foreach (Exception innerEx in ex.Flatten().InnerExceptions)
                {
                    _logger.LogError("System failed to start {Error}: ", innerEx.Message);
                }

                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task StartPcsAsync(int deviceId)
        {
            _logger.LogInformation("Start Power Conversion System with {DeviceId}", deviceId);

            try
            {
                if (_powerConversionSystems.TryGetValue(deviceId, out var pcs) == false)
                {
                    _logger.LogError("Device Manager cannot find PCS with device id: {DeviceId}", deviceId);
                    throw new InvalidOperationException($"Device Manager cannot find PCS with device id: {deviceId}");
                }

                await pcs.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Start PCS failed error: {Error}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task StopAllPcsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Stop all Power Conversion Systems");

            try
            {
                await Task.WhenAll(PowerConversionSystems.Where(p => p.IsInMaintenanceMode == false).Select(d => d.StopAsync()));
            }
            catch (AggregateException ex)
            {
                // Handle exceptions thrown by one or more parallel tasks
                foreach (Exception innerEx in ex.Flatten().InnerExceptions)
                {
                    _logger.LogError("System failed to stop {Error}: ", innerEx.Message);
                }

                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task StopPcsAsync(int deviceId)
        {
            _logger.LogInformation("Stop Power Conversion System with {DeviceId}", deviceId);

            try
            {
                if (_powerConversionSystems.TryGetValue(deviceId, out var pcs) == false)
                {
                    _logger.LogError("Device Manager cannot find PCS with device id: {DeviceId}", deviceId);
                    throw new InvalidOperationException($"Device Manager cannot find PCS with device id: {deviceId}");
                }

                await pcs.StopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Stop PCS failed error: {Error}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task StandbyAllPcsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Standby all Power Conversion Systems");

            try
            {
                await Task.WhenAll(PowerConversionSystems.Where(p => p.IsInMaintenanceMode == false).Select(d => d.StandbyAsync()));
            }
            catch (AggregateException ex)
            {
                // Handle exceptions thrown by one or more parallel tasks
                foreach (Exception innerEx in ex.Flatten().InnerExceptions)
                {
                    _logger.LogError("System failed to go into standby {Error}: ", innerEx.Message);
                }

                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task StandbyPcsAsync(int deviceId)
        {
            _logger.LogInformation("Standby Power Conversion System with {DeviceId}", deviceId);

            try
            {
                if (_powerConversionSystems.TryGetValue(deviceId, out var pcs) == false)
                {
                    _logger.LogError("Device Manager cannot find PCS with device id: {DeviceId}", deviceId);
                    throw new InvalidOperationException($"Device Manager cannot find PCS with device id: {deviceId}");
                }

                await pcs.StandbyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Standby PCS failed error: {Error}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task ConnectBatteryBankAsync(int deviceId)
        {
            _logger.LogInformation("Connect Battery Bank with {DeviceId}", deviceId);

            try
            {
                if (_batteryBanks.TryGetValue(deviceId, out var bb) == false)
                {
                    _logger.LogError("Device Manager cannot find Battery Bank with device id: {DeviceId}", deviceId);
                    throw new InvalidOperationException($"Device Manager cannot find Battery Bank with device id: {deviceId}");
                }

                await bb.ConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Connect battery bank failed error: {Error}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task DisconnectBatteryBankAsync(int deviceId)
        {
            _logger.LogInformation("Disconnect Battery Bank with {DeviceId}", deviceId);

            try
            {
                if (_batteryBanks.TryGetValue(deviceId, out var bb) == false)
                {
                    _logger.LogError("Device Manager cannot find Battery Bank with device id: {DeviceId}", deviceId);
                    throw new InvalidOperationException($"Device Manager cannot find Battery Bank with device id: {deviceId}");
                }

                await bb.DisconnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Disconnect battery bank failed error: {Error}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void PutIntoMaintenance(string unitName)
        {
            _logger.LogInformation("Put unit into maintenance. Unit name: {UnitName}", unitName);
            DerUnit? unit = Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).FirstOrDefault(u => u.Name == unitName);

            if (unit == null)
            {
                _logger.LogError("Device Manager cannot find Unit with name: {UnitName}", unitName);
                throw new InvalidOperationException($"Device Manager cannot find Unit with name: {unitName}");
            }

            unit.IsInMaintenanceMode = true;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task SetPcsPowerTarget(int deviceId, double activePowerWatt, double reactivePowerVar)
        {
            try
            {
                if (_powerConversionSystems.TryGetValue(deviceId, out var pcs) == false)
                {
                    _logger.LogError("Device Manager cannot find PCS with device id: {DeviceId}", deviceId);
                    throw new InvalidOperationException($"Device Manager cannot find PCS with device id: {deviceId}");
                }

                await pcs.SetActivePowerTargetAsync(activePowerWatt);
                await pcs.SetReactivePowerTargetAsync(reactivePowerVar);
            }
            catch (Exception ex)
            {
                _logger.LogError("Standby PCS failed error: {Error}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Loads the Distributed Energy Resource (DER) root configuration object with all its content.
        /// </summary>
        /// <returns>Distributed Energy Resource (DER) root configuration object with all its content</returns>
        private async Task<DerConfig> LoadDerConfig(IDerRepository repository)
        {
            try
            {
                // Get DER with all active configurations.
                return await repository.GetDer(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error in loading DER configuration in Device Manager. Error: {Error}", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Assign the devices to the device specific collections.
        /// </summary>
        /// <param name="der">Der domain root instance.</param>
        private void LoadDevicesToCollections(Der der)
        {
            // 
            IEnumerable<DerUnit> units = der.DerGroups
                .SelectMany(g => g.DerCircuits)
                .SelectMany(c => c.DerUnits);

            _powerConversionSystems = units
                .SelectMany(unit => new PowerConversionSystemBase?[]
                {
                    unit switch
                    {
                        DerBatteryStorageUnit battery => battery.PowerConversionSystem,
                        DerSolarUnit solar => solar.PowerConversionSystem,
                        _ => null
                    }
                }).Where(p => p is not null).Cast<PowerConversionSystemBase>().ToDictionary(d => d.DeviceId);

            _batteryBanks = units.OfType<DerBatteryStorageUnit>().SelectMany(u => u.BatteryBanks).ToDictionary(d => d.DeviceId);
            _solarPanels = units.OfType<DerSolarUnit>().SelectMany(u => u.SolarPanels).ToDictionary(d => d.DeviceId);
            _systemPowerMeters = der.SystemPowerMeters.ToDictionary(d => d.DeviceId); ;
            _auxiliaryPowerMeters = der.AuxiliaryPowerMeters.ToDictionary(d => d.DeviceId); ;
            _externalPowerMeters = der.ExternalPowerMeters.ToDictionary(d => d.DeviceId); ;
        }


        /// <summary>
        /// Load the root meters.
        /// </summary>
        /// <param name="der">Der domain root instance.</param>
        /// <param name="config">DerConfig root configuration instance.</param>
        private void LoadRootMeters(Der der, DerConfig config)
        {
            foreach (SystemPowerMeterConfig meterConfig in config.SystemPowerMeterConfigs)
            {
                IMetricsPublisher meterMetrics = _publisherFactory.Create();
                MetricsPublishers.Add(meterMetrics);

                if (meterConfig.C37Config != null)
                {
                    (IC37Dataface dataface, IC37Client client) = _deviceFactoryC37.Create(meterConfig.C37Config);
                    C37TransmissionEngines.Add(new C37TransmissionEngine(_logger, client, dataface, meterConfig.C37Config.StationName, meterConfig.C37Config.StreamId));
                    der.SystemPowerMeters.Add(Create<SystemPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                        meterConfig, meterMetrics, dataface, client));
                }
                else if (meterConfig.ModbusConfig != null)
                {
                    (IModbusDataface dataface, IModbusClient client) = _deviceFactoryModbus.Create(meterConfig.ModbusConfig);
                    ModbusPollingEngines.Add(new ModbusPollingEngine(_logger, client, dataface));
                    der.SystemPowerMeters.Add(Create<SystemPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                        meterConfig, meterMetrics, dataface, client));
                }
            }

            foreach (AuxiliaryPowerMeterConfig meterConfig in config.AuxiliaryPowerMeterConfigs)
            {
                IMetricsPublisher meterMetrics = _publisherFactory.Create();
                MetricsPublishers.Add(meterMetrics);

                if (meterConfig.C37Config != null)
                {
                    (IC37Dataface dataface, IC37Client client) = _deviceFactoryC37.Create(meterConfig.C37Config);
                    C37TransmissionEngines.Add(new C37TransmissionEngine(_logger, client, dataface, meterConfig.C37Config.StationName, meterConfig.C37Config.StreamId));
                    der.AuxiliaryPowerMeters.Add(Create<AuxiliaryPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                        meterConfig, meterMetrics, dataface, client));
                }
                else if (meterConfig.ModbusConfig != null)
                {
                    (IModbusDataface dataface, IModbusClient client) = _deviceFactoryModbus.Create(meterConfig.ModbusConfig);
                    ModbusPollingEngines.Add(new ModbusPollingEngine(_logger, client, dataface));
                    der.AuxiliaryPowerMeters.Add(Create<AuxiliaryPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                        meterConfig, meterMetrics, dataface, client));
                }
            }

            foreach (ExternalPowerMeterConfig meterConfig in config.ExternalPowerMeterConfigs)
            {
                IMetricsPublisher meterMetrics = _publisherFactory.Create();
                MetricsPublishers.Add(meterMetrics);

                if (meterConfig.C37Config != null)
                {
                    (IC37Dataface dataface, IC37Client client) = _deviceFactoryC37.Create(meterConfig.C37Config);
                    C37TransmissionEngines.Add(new C37TransmissionEngine(_logger, client, dataface, meterConfig.C37Config.StationName, meterConfig.C37Config.StreamId));
                    der.ExternalPowerMeters.Add(Create<ExternalPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                        meterConfig, meterMetrics, dataface, client));
                }
                else if (meterConfig.ModbusConfig != null)
                {
                    (IModbusDataface dataface, IModbusClient client) = _deviceFactoryModbus.Create(meterConfig.ModbusConfig);
                    ModbusPollingEngines.Add(new ModbusPollingEngine(_logger, client, dataface));
                    der.ExternalPowerMeters.Add(Create<ExternalPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, _logger,
                        meterConfig, meterMetrics, dataface, client));
                }
            }
        }


        /// <summary>
        /// Load the root Generic Modbus Devices.
        /// </summary>
        /// <param name="der">Der domain root instance.</param>
        /// <param name="config">DerConfig root configuration instance.</param>
        private void LoadRootGenericModbusDevices(Der der, DerConfig config)
        {
            foreach (GenericModbusConfig genericConfig in config.GenericModbusConfigs)
            {
                // TODO: Add dataface factory and parameter.
                IMetricsPublisher gmdMetrics = _publisherFactory.Create();
                MetricsPublishers.Add(gmdMetrics);
                der.GenericModbusDevices.Add(Create<GenericModbusDevice>(
                    genericConfig.GenericModbusDeviceConfig.ClassName, genericConfig,
                    CreateGenericModbusEntries(genericConfig.GenericModbusDeviceConfig.GenericModbusMapConfig), gmdMetrics));
            }

            foreach (AutomaticTransferSwitchConfig atsConfig in config.AutomaticTransferSwitchConfigs)
            {
                // TODO: Add dataface factory and parameter.
                IMetricsPublisher gmdMetrics = _publisherFactory.Create();
                MetricsPublishers.Add(gmdMetrics);
                der.AutomaticTransferSwitches.Add(Create<AutomaticTransferSwitch>(
                    atsConfig.AutomaticTransferSwitchDeviceConfig.ClassName, atsConfig,
                    CreateGenericModbusEntries(atsConfig.AutomaticTransferSwitchDeviceConfig.GenericModbusMapConfig), gmdMetrics));
            }
        }


        /// <summary>
        /// Creates a Battery Unit and its dependencies from a configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="circuit">The parent instance.</param>
        /// <returns>Return a battery unit and its dependencies.</returns>
        private DerBatteryStorageUnit CreateBatteryUnit(DerBatteryStorageUnitConfig config, DerCircuit circuit)
        {
            DerBatteryStorageUnit unit = new(_logger, config, circuit);
            IMetricsPublisher pcsMetrics = _publisherFactory.Create();
            MetricsPublishers.Add(pcsMetrics);
            (IModbusDataface pcsDataface, IModbusClient pcsClient) = _deviceFactoryModbus.Create(config.PowerConversionSystemConfig.ModbusConfig);
            ModbusPollingEngines.Add(new ModbusPollingEngine(_logger, pcsClient, pcsDataface));

            unit.PowerConversionSystem = Create<PowerConversionSystemBase>(
                config.PowerConversionSystemConfig.PowerConversionSystemDeviceConfig.ClassName, _logger,
                config.PowerConversionSystemConfig, unit, pcsMetrics, pcsDataface, pcsClient);

            foreach (BatteryBankConfig batteryConfig in config.BatteryBankConfigs)
            {
                IMetricsPublisher batteryMetrics = _publisherFactory.Create();
                MetricsPublishers.Add(batteryMetrics);
                (IModbusDataface batteryDataface, IModbusClient batteryClient) = _deviceFactoryModbus.Create(batteryConfig.ModbusConfig);
                ModbusPollingEngines.Add(new ModbusPollingEngine(_logger, batteryClient, batteryDataface));
                unit.BatteryBanks.Add(Create<BatteryBankBase>(batteryConfig.BatteryBankDeviceConfig.ClassName, _logger, batteryConfig,
                    unit, batteryMetrics, batteryDataface, batteryClient));
            }

            return unit;
        }


        /// <summary>
        /// Creates a Solar Unit and its dependencies from a configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="circuit">The parent instance.</param>
        /// <returns>Return a solar unit and its dependencies.</returns>
        private DerSolarUnit CreateSolarUnit(DerSolarUnitConfig config, DerCircuit circuit)
        {
            DerSolarUnit unit = new(_logger, config, circuit);

            IMetricsPublisher pcsMetrics = _publisherFactory.Create();
            MetricsPublishers.Add(pcsMetrics);
            (IModbusDataface pcsDataface, IModbusClient pcsClient) = _deviceFactoryModbus.Create(config.PowerConversionSystemConfig.ModbusConfig);
            ModbusPollingEngines.Add(new ModbusPollingEngine(_logger, pcsClient, pcsDataface));

            unit.PowerConversionSystem = Create<PowerConversionSystemBase>(
                config.PowerConversionSystemConfig.PowerConversionSystemDeviceConfig.ClassName, _logger,
                config.PowerConversionSystemConfig, unit, pcsMetrics, pcsDataface, pcsClient);

            IMetricsPublisher solarMetrics = _publisherFactory.Create();
            MetricsPublishers.Add(solarMetrics);
            IDataface solarDataface = _services.GetRequiredService<IDataface>();
            unit.SolarPanels.Add(Create<SolarPanelBase>(config.SolarPanelConfig.SolarPanelDeviceConfig.ClassName, _logger,
                config.SolarPanelConfig, unit, solarMetrics, solarDataface));

            return unit;
        }


        /// <summary>
        /// Create a list of generic Modbus entries from a Modbus map configuration
        /// </summary>
        /// <param name="mapConfig">Generic Modbus map configuration.</param>
        /// <returns>Returns a list of generic Modbus entries.</returns>
        private List<GenericModbusEntryBase> CreateGenericModbusEntries(GenericModbusMapConfig? mapConfig)
        {
            List<GenericModbusEntryBase> entries = [];

            if (mapConfig != null)
            {
                foreach (GenericModbusPointBaseConfig entryConfig in mapConfig.Coils.Cast<GenericModbusPointBaseConfig>()
                    .Concat(mapConfig.DiscreteInputs))
                {
                    entries.Add(new GenericModbusPointEntry(entryConfig));
                }

                foreach (GenericModbusRegisterBaseConfig entryConfig in mapConfig.InputRegisters.Cast<GenericModbusRegisterBaseConfig>()
                    .Concat(mapConfig.HoldingRegisters))
                {
                    entries.Add(new GenericModbusRegisterEntry(entryConfig));
                }
            }

            return entries;
        }


        /// <summary>
        /// Create a class instance.
        /// </summary>
        /// <typeparam name="T">Type of the instance.</typeparam>
        /// <param name="className">Full class name of the instance to create.</param>
        /// <param name="arguments">Arguments for the constructor.</param>
        /// <returns>A new instance of type T</returns>
        /// <exception cref="InvalidOperationException">Throw invalid operation exception if class name could not be found.</exception>
        private T Create<T>(string className, params object[] arguments) where T : class
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(className);

            Type? type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(className, false, true))
                .FirstOrDefault(candidate => candidate is not null);

            if (type is null)
            {
                Assembly equipmentAssembly = Assembly.Load("paskalON.Devices.Equipments");
                type = equipmentAssembly.GetType(className, false, true);
            }

            if (type is null)
            {
                throw new InvalidOperationException($"Configured type '{className}' could not be found.");
            }

            if (typeof(T).IsAssignableFrom(type) == false)
            {
                throw new InvalidOperationException($"Configured type '{className}' is not assignable to '{typeof(T).FullName}'.");
            }

            return (T)ActivatorUtilities.CreateInstance(_services, type, arguments);
        }


        /// <summary>
        /// Connect all devices.
        /// </summary>
        private void ConnectDevices()
        {
            _logger.LogInformation("Connect all device during startup");

            foreach (IC37TransmissionEngine engine in C37TransmissionEngines)
            {
                _ = Task.Run(() => engine.StartStreaming(_shutdownToken));
            }

            foreach (IModbusPollingEngine engine in ModbusPollingEngines)
            {
                _ = Task.Run(() => engine.ConnectAsync(_shutdownToken));
            }
        }
    }
}
