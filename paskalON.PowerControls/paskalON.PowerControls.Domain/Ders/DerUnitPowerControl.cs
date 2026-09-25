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
    /// <summary>
    /// DER unit power control.
    /// </summary>
    public class DerUnitPowerControl : PowerControlBase, IDerUnitPowerControl
    {
        /// <summary>
        /// DER unit power control configuration.
        /// </summary>
        private readonly DerUnitPowerControlConfig _config;


        /// <summary>
        /// DER unit power control map.
        /// </summary>
        private readonly DerUnitPowerControlMap _map;


        /// <summary>
        /// DER unit power control constraints.
        /// </summary>
        public IEnumerable<IDerUnitConstraint> Constraints { get; init; }


        /// <summary>
        /// DER unit power conversion systems identifier.
        /// </summary>
        public int PcsDeviceId { get => _map.PcsDeviceId; }


        /// <summary>
        /// DER unit power control state.
        /// </summary>
        public DerState State { get => _map.State.Invoke(); }


        /// <summary>
        /// If priority distribution strategy is used then this priority is used.
        /// </summary>
        public int Priority { get; init; }


        /// <summary>
        /// If weighted distribution strategy is used then this weight is used.
        /// </summary>
        public double Weight { get; set; }


        /// <summary>
        /// Distribution strategy type used for distribution.
        /// </summary>
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


        /// <summary>
        /// Constructor of <see cref="DerUnitPowerControl"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="config">The DER unit power control configuration.</param>
        /// <param name="map">The DER unit power control map.</param>
        /// <param name="publisher">The metrics publisher.</param>
        /// <param name="constraints">The DER unit constraints.</param>
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


        /// <inheritdoc/>
        public override void UpdatePower(ActivePower activePower, ReactivePower reactivePower)
        {
            if (IsEnabled == true)
            {
                foreach (IDerUnitConstraint constraint in Constraints)
                {
                    constraint.ApplyConstraints(ref activePower, ref reactivePower);
                }

                SetTargetPower(activePower, reactivePower);
            }
        }


        /// <inheritdoc/>
        protected override void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>()
            {
                { "Name", _config.Name },
                { "Unit", _config.DerUnitName }
            };

            // Initialize metrics
            MetricsPublisher.Initialize(nameof(DerUnitPowerControl), tags);
            // MetricsFactorClass1
            MetricsPublisher.Register<DerUnitPowerControl, int>(this, nameof(State), MetricType.Gauge, x => (int)x.State, _config.MetricsFactorClass1);
            MetricsPublisher.Register<DerUnitPowerControl, double>(this, nameof(TargetActivePower), MetricType.Gauge, x => x.TargetActivePower.Watts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<DerUnitPowerControl, double>(this, nameof(TargetReactivePower), MetricType.Gauge, x => x.TargetReactivePower.VoltAmperesReactive, _config.MetricsFactorClass1);
            // MetricsFactorClass4
            MetricsPublisher.Register<DerUnitPowerControl, int>(this, nameof(IsEnabled), MetricType.Gauge, x => x.IsEnabled ? 1 : 0, _config.MetricsFactorClass4);
        }
    }
}
