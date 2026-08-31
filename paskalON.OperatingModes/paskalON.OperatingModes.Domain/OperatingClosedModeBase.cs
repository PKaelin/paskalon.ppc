// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Base class for all closed operating modes that defines the specific behavior and control strategy
    /// the system uses to interact with the power grid.
    /// </summary>
    public abstract class OperatingClosedModeBase : OperatingModeBase, IOperatingClosedMode
    {
        /// <summary>
        /// Operating closed mode base configuration.
        /// </summary>
        private readonly OperatingClosedModeBaseConfig _config;


        /// <summary>
        /// Error adjustment for active power, used to correct any discrepancies between the target and actual active power.
        /// </summary>
        protected ActivePower _errorAdjustmentActive = new ActivePower(0);


        /// <summary>
        /// Error adjustment for reactive power, used to correct any discrepancies between the target and actual reactive power.
        /// </summary>
        protected ReactivePower _errorAdjustmentReactive = new ReactivePower(0);


        /// <summary>
        /// Constructor of <see cref="OperatingClosedModeBase"/>
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public OperatingClosedModeBase(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, OperatingClosedModeBaseConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            _config = config;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower ErrorAdjustmentActive { get => _errorAdjustmentActive; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower ErrorAdjustmentReactive { get => _errorAdjustmentReactive; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract Task CalculateAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            MetricsPublisher.Register<OperatingClosedModeBase, double>(this, nameof(ErrorAdjustmentActive), MetricType.Gauge, x => x.ErrorAdjustmentActive.KiloWatts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<OperatingClosedModeBase, double>(this, nameof(ErrorAdjustmentReactive), MetricType.Gauge, x => x.ErrorAdjustmentReactive.KiloVoltAmperesReactive, _config.MetricsFactorClass1);
        }
    }
}
