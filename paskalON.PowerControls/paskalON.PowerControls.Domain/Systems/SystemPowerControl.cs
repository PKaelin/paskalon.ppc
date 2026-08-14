// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
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
        private readonly IEnumerable<DerUnitPowerControl> _units;
        private ActivePower _actualSystemActivePowerTarget;
        private ReactivePower _actualSystemReactivePowerTarget;

        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();

        public SystemState State { get => _map.State.Invoke(); }

        public ActivePower SystemActivePowerTarget
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        public ReactivePower SystemReactivePowerTarget
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        public ActivePower ActualSystemActivePowerTarget
        {
            get { lock (dataLock) { return _actualSystemActivePowerTarget; } }
            private set { lock (dataLock) { _actualSystemActivePowerTarget = value; } }
        }


        public ReactivePower ActualSystemReactivePowerTarget
        {
            get { lock (dataLock) { return _actualSystemReactivePowerTarget; } }
            private set { lock (dataLock) { _actualSystemReactivePowerTarget = value; } }
        }


        public SystemPowerControl(ILogger logger, SystemPowerControlConfig config, SystemPowerControlMap map, IMetricsPublisher publisher,
            IEnumerable<ISystemConstraint> constraints, IEnumerable<DerUnitPowerControl> units, DistributionStrategyProfile distribution)
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
        }



        public override void UpdatePower(ActivePower activePower, ReactivePower reactivePower)
        {
            if (IsEnabled == true)
            {
                double systemActivePowerTarget = SystemActivePowerTarget.Watts;
                double systemReactivePowerTarget = SystemReactivePowerTarget.VoltAmperesReactive;
                int unitCount = _units.Count();
                // Get derate stop & maintenance configuration
                SystemPowerConstraintConfig? derate = _constraints.OfType<SystemPowerConstraintConfig>().FirstOrDefault();

                if (unitCount > 0 && derate != null && (derate.DeratePerUnitStopped || derate.DeratePerUnitInMaintenance))
                {
                    int toDerate = _units.Count(u => derate.DeratePerUnitStopped && u.State == DerState.Stopped || derate.DeratePerUnitInMaintenance && u.State == DerState.Maintenance);

                    systemActivePowerTarget = systemActivePowerTarget / unitCount * toDerate;
                    systemReactivePowerTarget = systemReactivePowerTarget / unitCount * toDerate;
                }

                if (activePower.Watts != systemActivePowerTarget || reactivePower.VoltAmperesReactivePrecision != systemReactivePowerTarget)
                {
                    _logger.LogInformation("Update system power control. Active Power Kilo {ActivePower}, Reactive Power Kilo {ReactivePower}", activePower.KiloWatts, reactivePower.KiloVoltAmperesReactive);
                    _actualSystemActivePowerTarget = new ActivePower(SystemActivePowerTarget.Watts);
                    _actualSystemReactivePowerTarget = new ReactivePower(SystemReactivePowerTarget.VoltAmperesReactive);

                    foreach (ISystemConstraint constraint in _constraints)
                    {
                        constraint.ApplyConstraints(ref _actualSystemActivePowerTarget, ref _actualSystemReactivePowerTarget);
                    }

                    // Distribute to all units that can have different distribution strategies.
                    DistributePriority();
                    DistributeEqual();
                    DistributeWeighted();
                    DistributeProportional();
                    DistributeWaterFilling();
                }
            }
        }


        private void DistributePriority()
        {
            _priorityDistribution?.Distribute(ActualSystemActivePowerTarget, ActualSystemReactivePowerTarget,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Priority));
        }

        private void DistributeEqual()
        {
            _equalDistribution?.Distribute(ActualSystemActivePowerTarget, ActualSystemReactivePowerTarget,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Equal));
        }

        private void DistributeWeighted()
        {
            _weightedDistribution?.Distribute(ActualSystemActivePowerTarget, ActualSystemReactivePowerTarget,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Weight));
        }

        private void DistributeProportional()
        {
            _proportionalDistribution?.Distribute(ActualSystemActivePowerTarget, ActualSystemReactivePowerTarget,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.Proportional));
        }

        private void DistributeWaterFilling()
        {
            _waterFillingDistribution?.Distribute(ActualSystemActivePowerTarget, ActualSystemReactivePowerTarget,
                _units.Where(u => u.DistributionStrategyType == DistributionStrategyType.WaterFilling));
        }

    }
}
