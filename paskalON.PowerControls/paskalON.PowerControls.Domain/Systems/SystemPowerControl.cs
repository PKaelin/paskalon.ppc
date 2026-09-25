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
    /// <summary>
    /// System power control is responsible for controlling the power of a system of DER units.
    /// It receives setpoints from the operating mode and distributes them to the DER units based on their distribution strategy.
    /// It also applies constraints to the setpoints before distributing them to the DER units.
    /// </summary>
    public class SystemPowerControl : PowerControlBase
    {
        /// <summary>
        /// Distribution strategy for priority distribution.
        /// </summary>
        private IDistributionStrategy? _priorityDistribution;


        /// <summary>
        /// Distribution strategy for equal distribution.
        /// </summary>
        private IDistributionStrategy? _equalDistribution;


        /// <summary>
        /// Distribution strategy for weighted distribution.
        /// </summary>
        private IDistributionStrategy? _weightedDistribution;


        /// <summary>
        /// Distribution strategy for proportional distribution.
        /// </summary>
        private IDistributionStrategy? _proportionalDistribution;


        /// <summary>
        /// Distribution strategy for water filling distribution.
        /// </summary>
        private IDistributionStrategy? _waterFillingDistribution;


        /// <summary>
        /// System power control configuration.
        /// </summary>
        private readonly SystemPowerControlConfig _config;


        /// <summary>
        /// System power control map.
        /// </summary>
        private readonly SystemPowerControlMap _map;


        /// <summary>
        /// List of system constraints that are part of this system.
        /// </summary>
        private readonly IEnumerable<ISystemConstraint> _constraints;


        /// <summary>
        /// List of DER unit power controls that are part of this system.
        /// </summary>
        private readonly IEnumerable<IDerUnitPowerControl> _units;


        /// <summary>
        /// System state.
        /// </summary>
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


        /// <summary>
        /// Constructor of <see cref="SystemPowerControl"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="config">The system power control configuration.</param>
        /// <param name="map">The system power control map.</param>
        /// <param name="publisher">The metrics publisher.</param>
        /// <param name="constraints">The system constraints.</param>
        /// <param name="units">The DER unit power controls.</param>
        /// <param name="distribution">The distribution strategy profile.</param>
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


        /// <inheritdoc/>
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
                // Set the received setpoints
                SetpointActivePower = new ActivePower(activePower.Watts);
                SetpointReactivePower = new ReactivePower(reactivePower.VoltAmperesReactive);
                // Set the actual setpoints after applying derating if applicable
                SetpointActivePowerActual = systemActivePowerDerated.HasValue ? new ActivePower((double)systemActivePowerDerated)
                    : new ActivePower(activePower.Watts);
                SetpointReactivePowerActual = systemReactivePowerDerated.HasValue ? new ReactivePower((double)systemReactivePowerDerated)
                    : new ReactivePower(reactivePower.VoltAmperesReactive);
                // Create local for thread safety
                ActivePower constrainedActivePower = TargetActivePower;
                ReactivePower constrainedReactivePower = TargetReactivePower;
                // Check constraints and apply them to the targets.
                foreach (ISystemConstraint constraint in _constraints)
                {
                    constraint.ApplyConstraints(ref constrainedActivePower, ref constrainedReactivePower);
                }

                // Set the target power to the actual targets
                SetTargetPower(constrainedActivePower, constrainedReactivePower);

                // Distribute to all units that can have different distribution strategies.
                DistributePriority();
                DistributeEqual();
                DistributeWeighted();
                DistributeProportional();
                DistributeWaterFilling();
            }
        }


        /// <inheritdoc/>
        protected override void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>()
            {
                { "Name", _config.Name }
            };

            // Initialize metrics
            MetricsPublisher.Initialize(nameof(SystemPowerControl), tags);
            // MetricsFactorClass1
            MetricsPublisher.Register<SystemPowerControl, int>(this, nameof(State), MetricType.Gauge, x => (int)x.State, _config.MetricsFactorClass1);
            MetricsPublisher.Register<SystemPowerControl, double>(this, nameof(TargetActivePower), MetricType.Gauge, x => x.TargetActivePower.Watts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<SystemPowerControl, double>(this, nameof(TargetReactivePower), MetricType.Gauge, x => x.TargetReactivePower.VoltAmperesReactive, _config.MetricsFactorClass1);
            MetricsPublisher.Register<SystemPowerControl, double>(this, nameof(SetpointActivePower), MetricType.Gauge, x => x.SetpointActivePower.Watts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<SystemPowerControl, double>(this, nameof(SetpointReactivePower), MetricType.Gauge, x => x.SetpointReactivePower.VoltAmperesReactive, _config.MetricsFactorClass1);
            MetricsPublisher.Register<SystemPowerControl, double>(this, nameof(SetpointActivePowerActual), MetricType.Gauge, x => x.SetpointActivePowerActual.Watts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<SystemPowerControl, double>(this, nameof(SetpointReactivePowerActual), MetricType.Gauge, x => x.SetpointReactivePowerActual.VoltAmperesReactive, _config.MetricsFactorClass1);
            // MetricsFactorClass4
            MetricsPublisher.Register<SystemPowerControl, int>(this, nameof(IsEnabled), MetricType.Gauge, x => x.IsEnabled ? 1 : 0, _config.MetricsFactorClass4);
        }


        /// <summary>
        /// Distribute the setpoints to the units based on their distribution strategy.
        /// </summary>
        private void DistributePriority()
        {
            _priorityDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Priority));
        }


        /// <summary>
        /// Distribute the setpoints to the units based on their distribution strategy equal.
        /// </summary>
        private void DistributeEqual()
        {
            _equalDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Equal));
        }


        /// <summary>
        /// Distribute the setpoints to the units based on their distribution strategy weighted.
        /// </summary>
        private void DistributeWeighted()
        {
            _weightedDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Weight));
        }


        /// <summary>
        /// Distribute the setpoints to the units based on their distribution strategy proportional.
        /// </summary>
        private void DistributeProportional()
        {
            _proportionalDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Proportional));
        }


        /// <summary>
        /// Distribute the setpoints to the units based on their distribution strategy water filling.
        /// </summary>
        private void DistributeWaterFilling()
        {
            _waterFillingDistribution?.Distribute(SetpointActivePowerActual, SetpointReactivePowerActual,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.WaterFilling));
        }
    }
}
