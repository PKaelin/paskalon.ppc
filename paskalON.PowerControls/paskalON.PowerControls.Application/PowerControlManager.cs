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


        private readonly IMetricsPublisherFactory _metricsPublisherFactory;


        private readonly object _dataLock = new();


        private SystemPowerControl? _systemPowerControl;


        private IReadOnlyList<SystemPowerControlConfig> _systemPowerControlConfigurations = [];


        private IReadOnlyList<DerUnitPowerControlConfig> _derUnitPowerControlConfigurations = [];


        private IReadOnlyList<DerUnitEnergyStoragePowerControlConfig> _derUnitEnergyStoragePowerControlConfigurations = [];


        private IReadOnlyList<ConstraintBaseConfig> _constraintConfigurations = [];

        public ICollection<IMetricsPublisher> MetricsPublishers { get; } = new List<IMetricsPublisher>();


        public IReadOnlyList<SystemPowerControlConfig> SystemPowerControlConfigurations
        {
            get { lock (_dataLock) { return _systemPowerControlConfigurations; } }
        }


        public IReadOnlyList<DerUnitPowerControlConfig> DerUnitPowerControlConfigurations
        {
            get { lock (_dataLock) { return _derUnitPowerControlConfigurations; } }
        }


        public IReadOnlyList<DerUnitEnergyStoragePowerControlConfig> DerUnitEnergyStoragePowerControlConfigurations
        {
            get { lock (_dataLock) { return _derUnitEnergyStoragePowerControlConfigurations; } }
        }


        public IReadOnlyList<ConstraintBaseConfig> ConstraintConfigurations
        {
            get { lock (_dataLock) { return _constraintConfigurations; } }
        }


        public SystemPowerControl? SystemPowerControl
        {
            get { lock (_dataLock) { return _systemPowerControl; } }
        }


        public PowerControlManager(ILogger<PowerControlManager> logger, IDeviceClient deviceClient, IMetricsPublisherFactory metricsPublisherFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);
            ArgumentNullException.ThrowIfNull(metricsPublisherFactory);

            _logger = logger;
            _deviceClient = deviceClient;
            _metricsPublisherFactory = metricsPublisherFactory;
        }

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


        private IEnumerable<DerUnitDto> GetDerUnits()
        {
            return _deviceClient.Der.DerGroups.SelectMany(g => g.DerCircuits).SelectMany(c => c.DerUnits);
        }


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


        public async Task SetSystemPowerTarget(ActivePower activePower, ReactivePower reactivePower)
        {
            _systemPowerControl?.UpdatePower(activePower, reactivePower);
        }


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


        private PcsDto? GetPcs(DerUnitDto unit)
        {
            return unit switch
            {
                DerBatteryStorageUnitDto battery => battery.PowerConversionSystem,
                DerSolarUnitDto solar => solar.PowerConversionSystem,
                _ => null
            };
        }


        private DerState GetDerState(DerUnitDto unit, PcsDto? pcs)
        {
            if (unit.IsInMaintenanceMode == true)
            {
                return DerState.Maintenance;
            }

            return pcs?.Core?.State == PcsState.Started ? DerState.Started : DerState.Stopped;
        }


        private SystemPowerControlMap CreateSystemMap()
        {
            return new SystemPowerControlMap
            {
                State = () => GetDerUnits().Any(u => GetPcs(u)?.Core?.State == PcsState.Started) ? SystemState.Started : SystemState.Stopped
            };
        }


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
