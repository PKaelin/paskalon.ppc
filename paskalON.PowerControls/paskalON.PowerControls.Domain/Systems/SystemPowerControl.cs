// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Strategies;
using paskalON.PowerControls.Domain.Configs.Systems;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Domain.Strategies;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain.Systems
{
    public class SystemPowerControl : PowerControlBase
    {
        private IDistributionStrategy? _priorityDistribution;
        private IDistributionStrategy? _equalDistribution;
        private IDistributionStrategy? _weightedDistribution;
        private IDistributionStrategy? _proportionalDistribution;
        private IDistributionStrategy? _waterFillingDistribution;
        private readonly SystemPowerControlConfig _config;
        private readonly SystemPowerControlMap _map;
        private readonly IEnumerable<ISystemConstraint> _constraints;
        private readonly IEnumerable<IDerUnitPowerControl> _units;

        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();

        public SystemState State { get => _map.State.Invoke(); }


        /// <summary>
        /// Setpoint active power is the setpoint received from the operating mode.
        /// </summary>
        public ActivePower SetpointActivePower
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Setpoint reactive power is the setpoint received from the operating mode.
        /// </summary>
        public ReactivePower SetpointReactivePower
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Setpoint active power actual is the setpoint after applying constraints and derating.
        /// </summary>
        public ActivePower SetpointActivePowerActual
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Setpoint reactive power actual is the setpoint after applying constraints and derating.
        /// </summary>
        public ReactivePower SetpointReactivePowerActual
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }



        public SystemPowerControl(ILogger logger, SystemPowerControlConfig config, SystemPowerControlMap map, IMetricsPublisher publisher,
            IEnumerable<ISystemConstraint> constraints, IEnumerable<IDerUnitPowerControl> units, DistributionStrategyProfile distribution)
            : base(logger, config, map, publisher)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(constraints);
            ArgumentNullException.ThrowIfNull(units);
            ArgumentNullException.ThrowIfNull(distribution);

            _config = config;
            _map = map;
            _constraints = constraints;
            _units = units;
            _equalDistribution = distribution.EqualDistribution;
            _priorityDistribution = distribution.PriorityDistribution;
            _weightedDistribution = distribution.WeightedDistribution;
            _proportionalDistribution = distribution.ProportionalDistribution;
            _waterFillingDistribution = distribution.WaterFillingDistribution;
            RegisterMetrics();
        }



        public override void UpdatePower(ActivePower activePower, ReactivePower reactivePower)
        {
            if (IsEnabled == true)
            {
                // Asign current setpoints to local variables.
                double? systemActivePowerDerated = null;
                double? systemReactivePowerDerated = null;
                int unitCount = _units.Count();
                // Get derate stop & maintenance configuration
                SystemPowerConstraint? derate = _constraints.OfType<SystemPowerConstraint>().FirstOrDefault();

                // Check whether we have to derate the setpoints by the actual unit stopped or in maintenance.
                if (unitCount > 0 && derate != null && (derate.DeratePerUnitStopped || derate.DeratePerUnitInMaintenance))
                {
                    int toDerate = 0;
                    toDerate += _units.Count(u => derate.DeratePerUnitStopped && u.State == DerState.Stopped);
                    toDerate += _units.Count(u => derate.DeratePerUnitInMaintenance && u.State == DerState.Maintenance);
                    systemActivePowerDerated = activePower.Watts / unitCount * (unitCount - toDerate);
                    systemReactivePowerDerated = reactivePower.VoltAmperesReactive / unitCount * (unitCount - toDerate);
                }

                _logger.LogInformation("Update system power control. Active Power {ActivePower}, Reactive Power {ReactivePower}", activePower.Watts, reactivePower.KiloVoltAmperesReactive);
                SetpointActivePower = new ActivePower(activePower.Watts);
                SetpointReactivePower = new ReactivePower(reactivePower.VoltAmperesReactive);
                SetpointActivePowerActual = systemActivePowerDerated.HasValue ? new ActivePower((double)systemActivePowerDerated)
                    : new ActivePower(activePower.Watts);
                SetpointReactivePowerActual = systemReactivePowerDerated.HasValue ? new ReactivePower((double)systemReactivePowerDerated)
                    : new ReactivePower(reactivePower.VoltAmperesReactive);

                // Check constraints and apply them to the setpoints
                foreach (ISystemConstraint constraint in _constraints)
                {
                    constraint.ApplyConstraints(ref _targetActivePower, ref _targetReactivePower);
                }

                // Distribute to all units that can have different distribution strategies.
                DistributePriority();
                DistributeEqual();
                DistributeWeighted();
                DistributeProportional();
                DistributeWaterFilling();
            }
        }


        protected override void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>()
            {
                { "Name", _config.Name }
            };

            MetricsPublisher.Initialize(nameof(SystemPowerControl), tags);
        }


        private void DistributePriority()
        {
            _priorityDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Priority));
        }

        private void DistributeEqual()
        {
            _equalDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Equal));
        }

        private void DistributeWeighted()
        {
            _weightedDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Weight));
        }

        private void DistributeProportional()
        {
            _proportionalDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Proportional));
        }

        private void DistributeWaterFilling()
        {
            _waterFillingDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.WaterFilling));
        }
    }
}
