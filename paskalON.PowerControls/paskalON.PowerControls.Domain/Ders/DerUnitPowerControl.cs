// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
    public class DerUnitPowerControl : PowerControlBase, IDerUnitPowerControl
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
            ArgumentNullException.ThrowIfNull(config);
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
                MaximumActivePower = pc.MaximumActivePowerWatt.HasValue ? new ActivePower(pc.MaximumActivePowerWatt.Value) : new ActivePower(0);
                MinimumActivePower = pc.MinimumActivePowerWatt.HasValue ? new ActivePower(pc.MinimumActivePowerWatt.Value) : new ActivePower(0);
                MaximumReactivePower = pc.MaximumReactivePowerVars.HasValue ? new ReactivePower(pc.MaximumReactivePowerVars.Value) : new ReactivePower(0);
                MinimumReactivePower = pc.MinimumReactivePowerVars.HasValue ? new ReactivePower(pc.MinimumReactivePowerVars.Value) : new ReactivePower(0);
            }

            RegisterMetrics();
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


        protected override void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>()
            {
                { "Name", _config.Name },
                { "Unit", _config.DerUnitName }
            };

            MetricsPublisher.Initialize(nameof(DerUnitPowerControl), tags);
        }
    }
}
