// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.ConstraintEngine.Domain.Ders;
using paskalON.ConstraintEngine.Domain.Systems;
using paskalON.Devices.Client;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.PowerControls.Domain.Systems;
using paskalON.PowerControls.Infrastructure.Storage;
using paskalON.Telemetry;
using paskalON.Telemetry.Factories;
using System.Collections.ObjectModel;

namespace paskalON.PowerControls.Application
{
    /// <summary>
    /// Power control manager responsible for managing system and DER unit power controls, configurations, and constraints.
    /// </summary>
    public class PowerControlManager : IPowerControlManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<PowerControlManager> _logger;


        /// <summary>
        /// Device client to receive device DTOs from pub/sub.
        /// </summary>
        private readonly IDeviceClient _deviceClient;


        /// <summary>
        /// Metrics publisher factory to create metrics publishers for telemetry.
        /// </summary>
        private readonly IMetricsPublisherFactory _metricsPublisherFactory;

        /// <summary>
        /// Lock object for synchronizing access to shared data.
        /// </summary>
        private readonly object _dataLock = new();


        /// <summary>
        /// System power control instance.
        /// </summary>
        private SystemPowerControl? _systemPowerControl;


        /// <summary>
        /// List of system power control configurations.
        /// </summary>
        private IReadOnlyList<SystemPowerControlConfig> _systemPowerControlConfigurations = [];


        /// <summary>
        /// List of DER unit power control configurations.
        /// </summary>
        private IReadOnlyList<DerUnitPowerControlConfig> _derUnitPowerControlConfigurations = [];


        /// <summary>
        /// List of DER unit energy storage power control configurations.
        /// </summary>
        private IReadOnlyList<DerUnitEnergyStoragePowerControlConfig> _derUnitEnergyStoragePowerControlConfigurations = [];


        /// <summary>
        /// List of constraint configurations.
        /// </summary>
        private IReadOnlyList<ConstraintBaseConfig> _constraintConfigurations = [];


        /// <inheritdoc/>
        public ICollection<IMetricsPublisher> MetricsPublishers { get; } = new List<IMetricsPublisher>();


        /// <inheritdoc/>
        public IReadOnlyList<SystemPowerControlConfig> SystemPowerControlConfigurations
        {
            get { lock (_dataLock) { return _systemPowerControlConfigurations; } }
        }


        /// <inheritdoc/>
        public IReadOnlyList<DerUnitPowerControlConfig> DerUnitPowerControlConfigurations
        {
            get { lock (_dataLock) { return _derUnitPowerControlConfigurations; } }
        }


        /// <inheritdoc/>
        public IReadOnlyList<DerUnitEnergyStoragePowerControlConfig> DerUnitEnergyStoragePowerControlConfigurations
        {
            get { lock (_dataLock) { return _derUnitEnergyStoragePowerControlConfigurations; } }
        }


        /// <inheritdoc/>
        public IReadOnlyList<ConstraintBaseConfig> ConstraintConfigurations
        {
            get { lock (_dataLock) { return _constraintConfigurations; } }
        }


        /// <inheritdoc/>
        public SystemPowerControl? SystemPowerControl
        {
            get { lock (_dataLock) { return _systemPowerControl; } }
        }


        /// <summary>
        /// Constructo of <see cref="PowerControlManager"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="deviceClient">The device client instance.</param>
        /// <param name="metricsPublisherFactory">The metrics publisher factory instance.</param>
        public PowerControlManager(ILogger<PowerControlManager> logger, IDeviceClient deviceClient, IMetricsPublisherFactory metricsPublisherFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);
            ArgumentNullException.ThrowIfNull(metricsPublisherFactory);

            _logger = logger;
            _deviceClient = deviceClient;
            _metricsPublisherFactory = metricsPublisherFactory;
        }


        /// <inheritdoc/>
        public async Task Initialize(PowerControlContext context)
        {
            // Load power control configurations and constraints from the database
            List<SystemPowerControlConfig> systemConfigs = await context.SystemPowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
            List<DerUnitPowerControlConfig> unitConfigs = await context.DerUnitPowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
            List<DerUnitEnergyStoragePowerControlConfig> storageConfigs = await context.DerUnitEnergyStoragePowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
            List<ConstraintBaseConfig> constraintConfigs = systemConfigs.SelectMany(c => c.Constraints)
                .Concat(unitConfigs.SelectMany(c => c.Constraints))
                .Concat(storageConfigs.SelectMany(c => c.Constraints))
                .DistinctBy(c => c.Name)
                .ToList();

            SystemPowerControlConfig systemConfig = systemConfigs.Single();
            List<IDerUnitPowerControl> units = new List<IDerUnitPowerControl>();

            foreach (DerUnitDto unit in GetDerUnits())
            {
                if (unit is DerBatteryStorageUnitDto battery)
                {
                    DerUnitEnergyStoragePowerControlConfig storageConfig = storageConfigs.Single(c => c.DerUnitName == battery.Name);
                    units.Add(CreateStoragePowerControl(battery, storageConfig));
                }
                else
                {
                    DerUnitPowerControlConfig unitConfig = unitConfigs.Single(c => c.DerUnitName == unit.Name);
                    units.Add(CreateUnitPowerControl(unit, unitConfig));
                }
            }

            IMetricsPublisher systemPublisher = _metricsPublisherFactory.Create();
            SystemPowerControl systemPowerControl = new SystemPowerControl(_logger, systemConfig, CreateSystemMap(), systemPublisher,
                CreateSystemConstraints(systemConfig.Constraints), units, CreateDistributionProfile());

            lock (_dataLock)
            {
                _systemPowerControlConfigurations = new ReadOnlyCollection<SystemPowerControlConfig>(systemConfigs);
                _derUnitPowerControlConfigurations = new ReadOnlyCollection<DerUnitPowerControlConfig>(unitConfigs);
                _derUnitEnergyStoragePowerControlConfigurations = new ReadOnlyCollection<DerUnitEnergyStoragePowerControlConfig>(storageConfigs);
                _constraintConfigurations = new ReadOnlyCollection<ConstraintBaseConfig>(constraintConfigs);
                _systemPowerControl = systemPowerControl;
                MetricsPublishers.Add(systemPublisher);
            }
        }


        /// <inheritdoc/>
        public async Task SetSystemPowerTarget(ActivePower activePower, ReactivePower reactivePower)
        {
            _systemPowerControl?.UpdatePower(activePower, reactivePower);
        }


        /// <summary>
        /// Get all DER units from the device client.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="DerUnitDto"/> representing all DER units.</returns>
        private IEnumerable<DerUnitDto> GetDerUnits()
        {
            return _deviceClient.Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits);
        }


        /// <summary>
        /// Create a DER unit power control instance for the specified DER unit and configuration.
        /// </summary>
        /// <param name="unit">The DER unit for which to create the power control.</param>
        /// <param name="config">The configuration for the DER unit power control.</param>
        /// <returns>A <see cref="DerUnitPowerControl"/> instance.</returns>
        private DerUnitPowerControl CreateUnitPowerControl(DerUnitDto unit, DerUnitPowerControlConfig config)
        {
            DerUnitPowerControlMap map = new DerUnitPowerControlMap
            {
                State = () => GetDerState(unit, GetPcs(unit))
            };

            IMetricsPublisher publisher = _metricsPublisherFactory.Create();
            DerUnitPowerControl control = new DerUnitPowerControl(_logger, config, map, publisher, CreateUnitConstraints(config.Constraints));
            MetricsPublishers.Add(publisher);

            return control;
        }


        /// <summary>
        /// Create a DER unit energy storage power control instance for the specified battery storage unit and configuration.
        /// </summary>
        /// <param name="unit">The battery storage unit for which to create the power control.</param>
        /// <param name="config">The configuration for the DER unit energy storage power control.</param>
        /// <returns>A <see cref="DerUnitPowerEnergyStorageControl"/> instance.</returns>
        private DerUnitPowerEnergyStorageControl CreateStoragePowerControl(DerBatteryStorageUnitDto unit, DerUnitEnergyStoragePowerControlConfig config)
        {
            DerUnitPowerEnergyStorageControlMap map = new DerUnitPowerEnergyStorageControlMap
            {
                State = () => GetDerState(unit, unit.PowerConversionSystem),
                StateOfCharge = () => unit.BatteryBanks.Select(b => b.Core?.StateOfCharge ?? 0).DefaultIfEmpty().Average(),
                StateOfChargeMaximum = () => unit.BatteryBanks.Select(b => b.Definition.PreferredMaximumStateOfCharge).DefaultIfEmpty().Average(),
                StateOfChargeMinimum = () => unit.BatteryBanks.Select(b => b.Definition.PreferredMinimumStateOfCharge).DefaultIfEmpty().Average(),
                StateOfHealth = () => unit.BatteryBanks.Select(b => b.Detail?.StateOfHealth ?? 0).DefaultIfEmpty().Average()
            };

            IMetricsPublisher publisher = _metricsPublisherFactory.Create();
            DerUnitPowerEnergyStorageControl control = new DerUnitPowerEnergyStorageControl(_logger, config, map, publisher, CreateUnitConstraints(config.Constraints));
            MetricsPublishers.Add(publisher);

            return control;
        }


        /// <summary>
        /// Get the power conversion system (PCS) associated with the specified DER unit.
        /// </summary>
        /// <param name="unit">The DER unit for which to get the PCS.</param>
        /// <returns>The PCS associated with the DER unit, or null if none exists.</returns>
        private PcsDto? GetPcs(DerUnitDto unit)
        {
            return unit switch
            {
                DerBatteryStorageUnitDto battery => battery.PowerConversionSystem,
                DerSolarUnitDto solar => solar.PowerConversionSystem,
                _ => null
            };
        }


        /// <summary>
        /// Get the DER state based on the unit and its associated PCS.
        /// </summary>
        /// <param name="unit">The DER unit for which to get the state.</param>
        /// <param name="pcs">The PCS associated with the DER unit.</param>
        /// <returns>The state of the DER unit.</returns>
        private DerState GetDerState(DerUnitDto unit, PcsDto? pcs)
        {
            if (unit.IsInMaintenanceMode == true)
            {
                return DerState.Maintenance;
            }

            return pcs?.Core?.State == PcsState.Started ? DerState.Started : DerState.Stopped;
        }


        /// <summary>
        /// Create a system power control map that defines the state of the system based on the states of its DER units.
        /// </summary>
        /// <returns>The system power control map.</returns>
        private SystemPowerControlMap CreateSystemMap()
        {
            return new SystemPowerControlMap
            {
                State = () => GetDerUnits().Any(u => GetPcs(u)?.Core?.State == PcsState.Started) ? SystemState.Started : SystemState.Stopped
            };
        }


        /// <summary>
        /// Create a list of system constraints based on the provided configurations.
        /// </summary>
        /// <param name="configurations">The configurations used to create the system constraints.</param>
        /// <returns>A list of system constraints.</returns>
        private List<ISystemConstraint> CreateSystemConstraints(IEnumerable<ConstraintBaseConfig> configurations)
        {
            List<ISystemConstraint> constraints = new List<ISystemConstraint>();

            foreach (ConstraintBaseConfig configuration in configurations)
            {
                switch (configuration)
                {
                    case SystemPowerConstraintConfig power:
                        constraints.Add(new SystemPowerConstraint(_logger, power, new SystemPowerConstraintMap()));
                        break;

                    case SystemRampConstraintConfig ramp:
                        constraints.Add(new SystemRampConstraint(_logger, ramp, new SystemRampConstraintMap(), TimeProvider.System));
                        break;
                }
            }

            return constraints;
        }


        /// <summary>
        /// Create a list of DER unit constraints based on the provided configurations.
        /// </summary>
        /// <param name="configurations">The configurations used to create the DER unit constraints.</param>
        /// <returns>A list of DER unit constraints.</returns>
        private List<IDerUnitConstraint> CreateUnitConstraints(IEnumerable<ConstraintBaseConfig> configurations)
        {
            List<IDerUnitConstraint> constraints = new();

            foreach (ConstraintBaseConfig configuration in configurations)
            {
                switch (configuration)
                {
                    case DerUnitPowerConstraintConfig power:
                        constraints.Add(new DerUnitPowerConstraint(_logger, power, new DerUnitPowerConstraintMap()));
                        break;

                    case DerUnitRampConstraintConfig ramp:
                        constraints.Add(new DerUnitRampConstraint(_logger, ramp, new DerUnitRampConstraintMap(), TimeProvider.System));
                        break;
                }
            }

            return constraints;
        }


        /// <summary>
        /// Create a distribution strategy profile that defines the available distribution strategies for power control.
        /// </summary>
        /// <returns>A distribution strategy profile.</returns>
        private DistributionStrategyProfile CreateDistributionProfile()
        {
            return new DistributionStrategyProfile
            {
                PriorityDistribution = new PriorityDistributionStrategy(_logger),
                EqualDistribution = new EqualDistributionStrategy(_logger),
                WeightedDistribution = new WeightDistributionStrategy(_logger),
                ProportionalDistribution = new ProportionalDistributionStrategy(_logger),
                WaterFillingDistribution = new WaterFillingDistributionStrategy(_logger)
            };
        }
    }
}
