// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Ders;
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


        /// <summary>
        /// If priority distribution strategy is used then this priority is used.
        /// </summary>
        public int Priority { get; init; }


        /// <summary>
        /// If weighted distribution strategy is used then this weight is used.
        /// </summary>
        public double Weight { get; set; }



        public DerUnitPowerControl(ILogger logger, DerUnitPowerControlConfig config, DerUnitPowerControlMap map, IMetricsPublisher publisher, IEnumerable<IDerUnitConstraint> constraints)
            : base(logger, config, map, publisher)
        {
            ArgumentNullException.ThrowIfNull(constraints);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            Constraints = constraints;

            MaximumActivePower = ActivePower.FromKilo(_config.DerUnitPowerConstraintConfig.MaximumActivePowerKiloWatt ?? 0);
            MinimumActivePower = ActivePower.FromKilo(_config.DerUnitPowerConstraintConfig.MinimumActivePowerKiloWatt ?? 0);
            MaximumReactivePower = ReactivePower.FromKilo(_config.DerUnitPowerConstraintConfig.MaximumReactivePowerKiloVars ?? 0);
            MinimumReactivePower = ReactivePower.FromKilo(_config.DerUnitPowerConstraintConfig.MinimumReactivePowerKiloVars ?? 0);
            Priority = _config.Priority ?? int.MaxValue;
            Weight = _config.Weight ?? 1;
        }


        public override void UpdatePower(ActivePower activePower, ReactivePower reactivePower)
        {
            foreach (IDerUnitConstraint constraint in Constraints)
            {
                constraint.ApplyConstraints(ref activePower, ref reactivePower);
            }
        }
    }
}
