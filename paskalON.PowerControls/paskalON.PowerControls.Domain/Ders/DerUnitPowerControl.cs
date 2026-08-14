// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Strategies;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain.Ders
{
    public class DerUnitPowerControl : PowerControlBase
    {
        private readonly DerUnitPowerControlConfig _config;
        private readonly DerUnitPowerControlMap _map;


        public IEnumerable<IDerUnitConstraint> Constraints { get; init; }


        public DerState State { get => _map.State.Invoke(); }


        /// <summary>
        /// If priority distribution strategy is used then this priority is used.
        /// </summary>
        public int Priority { get; init; }


        /// <summary>
        /// If weighted distribution strategy is used then this weight is used.
        /// </summary>
        public double Weight { get; set; }


        public DistributionStrategyType DistributionStrategyType { get => _config.DistributionStrategyType; }


        /// <summary>
        /// Maximum Active Power is the possible technical or nameplate limits of the unit.
        /// </summary>
        public ActivePower MaximumActivePower { get; init; }


        /// <summary>
        /// Minimum Active Power is the possible technical or nameplate limits of the unit.
        /// </summary>
        public ActivePower MinimumActivePower { get; init; }


        /// <summary>
        /// Maximum Reactive Power is the possible technical or nameplate limits of the unit.
        /// </summary>
        public ReactivePower MaximumReactivePower { get; init; }


        /// <summary>
        /// Minimum Reactive Power is the possible technical or nameplate limits of the unit.
        /// </summary>
        public ReactivePower MinimumReactivePower { get; init; }


        public DerUnitPowerControl(ILogger logger, DerUnitPowerControlConfig config, DerUnitPowerControlMap map, IMetricsPublisher publisher, IEnumerable<IDerUnitConstraint> constraints)
            : base(logger, config, map, publisher)
        {
            ArgumentNullException.ThrowIfNull(constraints);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            Constraints = constraints;
            Priority = _config.Priority ?? int.MaxValue;
            Weight = _config.Weight ?? 1;

            // Get power constraints here and assign them to properties so that we dont have to do them in every control loop
            DerUnitPowerConstraintConfig? pc = Constraints.OfType<DerUnitPowerConstraintConfig>().FirstOrDefault();

            if (pc != null)
            {
                MaximumActivePower = pc.MaximumActivePowerKiloWatt.HasValue ? ActivePower.FromKilo((double)pc.MaximumActivePowerKiloWatt) : new ActivePower(0);
                MinimumActivePower = pc.MinimumActivePowerKiloWatt.HasValue ? ActivePower.FromKilo((double)pc.MinimumActivePowerKiloWatt) : new ActivePower(0);
                MaximumReactivePower = pc.MaximumReactivePowerKiloVars.HasValue ? ReactivePower.FromKilo((double)pc.MaximumReactivePowerKiloVars) : new ReactivePower(0);
                MinimumReactivePower = pc.MinimumReactivePowerKiloVars.HasValue ? ReactivePower.FromKilo((double)pc.MinimumReactivePowerKiloVars) : new ReactivePower(0);
            }
        }


        public override void UpdatePower(ActivePower activePower, ReactivePower reactivePower)
        {
            if (IsEnabled == true)
            {
                foreach (IDerUnitConstraint constraint in Constraints)
                {
                    constraint.ApplyConstraints(ref activePower, ref reactivePower);
                }
            }
        }
    }
}
