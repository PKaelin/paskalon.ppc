// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
using paskalON.Devices.Infrastructure.Storage;
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
        private readonly ILogger _logger;


        /// <summary>
        /// Distributed Energy Resources (DER) repository interface.
        /// </summary>
        private readonly IDerRepository _repository;


        /// <summary>
        /// Service provider used to resolve dependencies of configured equipment.
        /// </summary>
        private readonly IServiceProvider _services;


        /// <summary>
        /// Dictionary with device id and PCS. 
        /// </summary>
        private Dictionary<int, PowerConversionSystemBase> _powerConversionSystems = new Dictionary<int, PowerConversionSystemBase>();


        /// <summary>
        /// Dictionary with device id and BB.
        /// </summary>
        private Dictionary<int, BatteryBankBase> _batteryBanks = new Dictionary<int, BatteryBankBase>();


        /// <summary>
        /// Dictionary with device id and Solar Panel.
        /// </summary>
        private Dictionary<int, SolarPanelBase> _solarPanels = new Dictionary<int, SolarPanelBase>();


        /// <summary>
        /// Dictionary with device id and External Meter.
        /// </summary>
        private Dictionary<int, ExternalPowerMeter> _externalPowerMeters = new Dictionary<int, ExternalPowerMeter>();


        /// <summary>
        /// Dictionary with device id and Auxiliary Meter.
        /// </summary>
        private Dictionary<int, AuxiliaryPowerMeter> _auxiliaryPowerMeters = new Dictionary<int, AuxiliaryPowerMeter>();


        /// <summary>
        /// Dictionary with device id and System Meter.
        /// </summary>
        private Dictionary<int, SystemPowerMeter> _systemPowerMeters = new Dictionary<int, SystemPowerMeter>();


        /// <summary>
        /// Dictionary with device id and Circuit Meter.
        /// </summary>
        private readonly Dictionary<int, CircuitPowerMeter> _circuitPowerMeters = new Dictionary<int, CircuitPowerMeter>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Der Der { get; private set; }


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



        public DeviceManager(ILogger logger, IDerRepository repository, IServiceProvider services)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(services);

            _logger = logger;
            _repository = repository;
            _services = services;
            // Initialized DER with a place holder
            Der = new Der(logger, new DerConfig { ChangedBy = "System", Name = "Uninitialized DER" });
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task LoadDerAsync()
        {
            DerConfig config = await LoadDerConfig();
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
                        circuit.CircuitBreaker = Create<CircuitBreaker>(
                            breakerConfig.CircuitBreakerDeviceConfig.ClassName, breakerConfig,
                            CreateGenericModbusEntries(breakerConfig.CircuitBreakerDeviceConfig.GenericModbusMapConfig));
                    }

                    if (circuitConfig.CircuitPowerMeterConfig is { } circuitMeterConfig)
                    {
                        circuit.CircuitPowerMeter = Create<CircuitPowerMeter>(
                            circuitMeterConfig.PowerMeterDeviceConfig.ClassName, circuitMeterConfig);
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

            foreach (GenericModbusConfig genericConfig in config.GenericModbusConfigs)
            {
                der.GenericModbusDevices.Add(Create<GenericModbusDevice>(
                    genericConfig.GenericModbusDeviceConfig.ClassName, genericConfig,
                    CreateGenericModbusEntries(genericConfig.GenericModbusDeviceConfig.GenericModbusMapConfig)));
            }

            foreach (AutomaticTransferSwitchConfig atsConfig in config.AutomaticTransferSwitchConfigs)
            {
                der.AutomaticTransferSwitches.Add(Create<AutomaticTransferSwitch>(
                    atsConfig.AutomaticTransferSwitchDeviceConfig.ClassName, atsConfig,
                    CreateGenericModbusEntries(atsConfig.AutomaticTransferSwitchDeviceConfig.GenericModbusMapConfig)));
            }

            foreach (SystemPowerMeterConfig meterConfig in config.SystemPowerMeterConfigs)
            {
                der.SystemPowerMeters.Add(Create<SystemPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, meterConfig));
            }

            foreach (AuxiliaryPowerMeterConfig meterConfig in config.AuxiliaryPowerMeterConfigs)
            {
                der.AuxiliaryPowerMeters.Add(Create<AuxiliaryPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, meterConfig));
            }

            foreach (ExternalPowerMeterConfig meterConfig in config.ExternalPowerMeterConfigs)
            {
                der.ExternalPowerMeters.Add(Create<ExternalPowerMeter>(meterConfig.PowerMeterDeviceConfig.ClassName, meterConfig));
            }

            // Assign the devices to the device specific collections
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

            Der = der;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task StartAllPcsAsync(CancellationToken cancellationToken = default)
        {
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 10,
                CancellationToken = cancellationToken
            };

            try
            {
                await Parallel.ForEachAsync(PowerConversionSystems, options, async (pcs, token) =>
                {
                    if (pcs != null)
                    {
                        await pcs.StartAsync();
                    }
                });
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
            try
            {
                if (_powerConversionSystems.TryGetValue(deviceId, out var pcs) == false)
                {
                    _logger.LogError("Device Manager cannot find PCS with device id: {DeviceId}", deviceId);
                    return;
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
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 10,
                CancellationToken = cancellationToken
            };

            try
            {
                await Parallel.ForEachAsync(PowerConversionSystems, options, async (pcs, token) =>
                {
                    if (pcs != null)
                    {
                        await pcs.StopAsync();
                    }
                });
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
            try
            {
                if (_powerConversionSystems.TryGetValue(deviceId, out var pcs) == false)
                {
                    _logger.LogError("Device Manager cannot find PCS with device id: {DeviceId}", deviceId);
                    return;
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
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 10,
                CancellationToken = cancellationToken
            };

            try
            {
                await Parallel.ForEachAsync(PowerConversionSystems, options, async (pcs, token) =>
                {
                    if (pcs != null)
                    {
                        await pcs.StandbyAsync();
                    }
                });
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
            try
            {
                if (_powerConversionSystems.TryGetValue(deviceId, out var pcs) == false)
                {
                    _logger.LogError("Device Manager cannot find PCS with device id: {DeviceId}", deviceId);
                    return;
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
            try
            {
                if (_batteryBanks.TryGetValue(deviceId, out var bb) == false)
                {
                    _logger.LogError("Device Manager cannot find Battery Bank with device id: {DeviceId}", deviceId);
                    return;
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
            try
            {
                if (_batteryBanks.TryGetValue(deviceId, out var bb) == false)
                {
                    _logger.LogError("Device Manager cannot find Battery Bank with device id: {DeviceId}", deviceId);
                    return;
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
            DerUnit? unit = Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits).FirstOrDefault(u => u.Name == unitName);

            if (unit == null)
            {
                _logger.LogError("Device Manager cannot find Unit with name: {UnitName}", unitName);
                return;
            }

            unit.IsInMaintenanceMode = true;
        }


        /// <summary>
        /// Loads the Distributed Energy Resource (DER) root configuration object with all its content.
        /// </summary>
        /// <returns>Distributed Energy Resource (DER) root configuration object with all its content</returns>
        private async Task<DerConfig> LoadDerConfig()
        {
            try
            {
                return await _repository.GetDer(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error in loading DER configuration in Device Manager. Error: {Error}", ex.Message);
                throw;
            }
        }

        private DerBatteryStorageUnit CreateBatteryUnit(DerBatteryStorageUnitConfig config, DerCircuit circuit)
        {
            DerBatteryStorageUnit unit = new(_logger, config, circuit);
            unit.PowerConversionSystem = Create<PowerConversionSystemBase>(
                config.PowerConversionSystemConfig.PowerConversionSystemDeviceConfig.ClassName,
                config.PowerConversionSystemConfig, unit);

            foreach (BatteryBankConfig batteryConfig in config.BatteryBankConfigs)
            {
                unit.BatteryBanks.Add(Create<BatteryBankBase>(batteryConfig.BatteryBankDeviceConfig.ClassName, batteryConfig, unit));
            }

            return unit;
        }


        private DerSolarUnit CreateSolarUnit(DerSolarUnitConfig config, DerCircuit circuit)
        {
            DerSolarUnit unit = new(_logger, config, circuit);
            unit.PowerConversionSystem = Create<PowerConversionSystemBase>(
                config.PowerConversionSystemConfig.PowerConversionSystemDeviceConfig.ClassName,
                config.PowerConversionSystemConfig, unit);

            unit.SolarPanels.Add(Create<SolarPanelBase>(config.SolarPanelConfig.SolarPanelDeviceConfig.ClassName, config.SolarPanelConfig, unit));

            return unit;
        }


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
    }
}
