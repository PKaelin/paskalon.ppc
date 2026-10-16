// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.SafetyFunctions
{
    /// <summary>
    /// Mode: (Graceful) Shutdown Mode
    /// Purpose: Gracefully shutdown the plant in specific situation.
    /// Inputs: Current applied active and reactive power targets, configured shutdown targets
    /// Output Controlled: Active Power (P) and Reactive Power (Q)
    /// What Output Influences: Active Power and Reactive Power
    /// </summary>
    /// <remarks>
    /// Plant-wide override mode (no target DER unit). When it is activated, the system operating mode control seeds its power
    /// controls with the current applied active and reactive power targets. The shutdown mode ramps both targets to the configured
    /// shutdown targets (zero by default) with its ramp configuration and the system operating mode control applies them
    /// immediately (plant-wide override). When it is deactivated, the applied targets transition back to the objectives of the
    /// operating mode stack with its ramp.
    /// </remarks>
    public class ShutdownMode : OperatingOpenModeBase, IOverrideMode
    {
        /// <summary>
        /// Shutdown mode configuration.
        /// </summary>
        private readonly ShutdownModeConfig _config;


        /// <summary>
        /// Gets whether the shutdown is completed, i.e. both power axes reached their shutdown targets.
        /// </summary>
        public bool IsShutdownCompleted
        {
            get => ActiveControl!.State == OperatingModeState.Enabled && ReactiveControl!.State == OperatingModeState.Enabled;
        }


        /// <summary>
        /// Constructor of <see cref="ShutdownMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The metrics publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ShutdownMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, ShutdownModeConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
            RegisterMetrics();
        }


        /// <inheritdoc/>
        /// <remarks>
        /// Ramps the active and reactive power targets to the configured shutdown targets (zero by default). The available power
        /// is not used, the shutdown targets are passed as available power so they are not reduced.
        /// The signals of the operating mode stack are not changed, the system operating mode control applies the targets.
        /// </remarks>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            ArgumentNullException.ThrowIfNull(signals);

            ActivePower activeTarget = new ActivePower(_config.ShutdownTargetActivePowerWatts);
            ReactivePower reactiveTarget = new ReactivePower(_config.ShutdownTargetReactivePowerVars);
            ActiveControl!.Calculate(activeTarget, activeTarget);
            ReactiveControl!.Calculate(reactiveTarget, reactiveTarget);

            return signals;
        }


        /// <inheritdoc/>
        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            _metricsPublisher.Register<ShutdownMode, int>(this, nameof(IsShutdownCompleted), MetricType.Gauge, x => x.IsShutdownCompleted ? 1 : 0, _config.MetricsFactorClass2);
        }
    }
}
