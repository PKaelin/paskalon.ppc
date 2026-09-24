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
    public class DerUnitPowerEnergyStorageControl : PowerControlBase, IDerUnitPowerControl
    {
        private readonly DerUnitEnergyStoragePowerControlConfig _config;
        private readonly DerUnitPowerEnergyStorageControlMap _map;


        public IEnumerable<IDerUnitConstraint> Constraints { get; init; }


        public DerState State { get => _map.State.Invoke(); }


        public int Priority { get; init; }


        public double Weight { get; init; }


        public DistributionStrategyType DistributionStrategyType { get => _config.DistributionStrategyType; }


        public ActivePower MaximumActivePower { get; init; }


        public ActivePower MinimumActivePower { get; init; }


        public ReactivePower MaximumReactivePower { get; init; }


        public ReactivePower MinimumReactivePower { get; init; }

        public DerUnitPowerEnergyStorageControl(ILogger logger, DerUnitEnergyStoragePowerControlConfig config, DerUnitPowerEnergyStorageControlMap map,
            IMetricsPublisher publisher, IEnumerable<IDerUnitConstraint> constraints)
            : base(logger, config, map, publisher)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(constraints);

            _config = config;
            _map = map;
            Constraints = constraints;
            Priority = int.MaxValue;
            Weight = 1;

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


        public double StateOfCharge { get => _map.StateOfCharge.Invoke(); }


        public double StateOfChargeMaximum { get => _map.StateOfChargeMaximum.Invoke(); }


        public double StateOfChargeMinimum { get => _map.StateOfChargeMinimum.Invoke(); }


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

            MetricsPublisher.Initialize(nameof(DerUnitPowerEnergyStorageControl), tags);
        }
    }
}
