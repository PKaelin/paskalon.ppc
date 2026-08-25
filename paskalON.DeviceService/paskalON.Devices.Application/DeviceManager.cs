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
        /// <inheritdoc/>
        /// </summary>
        public Der Der { get; private set; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<PowerConversionSystemBase> PowerConversionSystems { get; private set; } = new List<PowerConversionSystemBase>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<BatteryBankBase> BatteryBanks { get; private set; } = new List<BatteryBankBase>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<SolarPanelBase> Solars { get; private set; } = new List<SolarPanelBase>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<ExternalPowerMeter> ExternalPowerMeters { get; private set; } = new List<ExternalPowerMeter>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<AuxiliaryPowerMeter> AuxiliaryPowerMeters { get; private set; } = new List<AuxiliaryPowerMeter>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<SystemPowerMeter> SystemPowerMeters { get; private set; } = new List<SystemPowerMeter>();


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICollection<CircuitPowerMeter> CircuitPowerMeters { get; private set; } = new List<CircuitPowerMeter>();



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



        public async Task LoadDer()
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


            IEnumerable<DerUnit> units = der.DerGroups
                .SelectMany(g => g.DerCircuits)
                .SelectMany(c => c.DerUnits);

            PowerConversionSystems = units
                .SelectMany(unit => new PowerConversionSystemBase?[]
                {
                    unit switch
                    {
                        DerBatteryStorageUnit battery => battery.PowerConversionSystem,
                        DerSolarUnit solar => solar.PowerConversionSystem,
                        _ => null
                    }
                }).Where(p => p is not null).Cast<PowerConversionSystemBase>().ToList();

            BatteryBanks = units.OfType<DerBatteryStorageUnit>().SelectMany(u => u.BatteryBanks).ToList();
            Solars = units.OfType<DerSolarUnit>().SelectMany(u => u.SolarPanels).ToList();

            SystemPowerMeters = der.SystemPowerMeters;
            AuxiliaryPowerMeters = der.AuxiliaryPowerMeters;
            ExternalPowerMeters = der.ExternalPowerMeters;

            Der = der;
        }

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
