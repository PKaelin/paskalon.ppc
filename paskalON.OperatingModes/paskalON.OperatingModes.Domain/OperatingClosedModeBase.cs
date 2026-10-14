// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes;
using paskalON.OperatingModes.Domain.Controls;
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
    /// <remarks>
    /// Closed modes react to measurements read from their map. Modes that correct their target with a feedback signal
    /// create a feedback control in their constructor with <see cref="CreateActiveFeedback"/> or <see cref="CreateReactiveFeedback"/>.
    /// Curve based modes (droops) don't need a feedback control.
    /// </remarks>
    public abstract class OperatingClosedModeBase : OperatingModeBase, IOperatingClosedMode
    {
        /// <summary>
        /// Operating closed mode base configuration.
        /// </summary>
        private readonly OperatingClosedModeBaseConfig _config;


        /// <inheritdoc/>
        public FeedbackControl<ActivePower>? ActiveFeedback { get; private set; }


        /// <inheritdoc/>
        public FeedbackControl<ReactivePower>? ReactiveFeedback { get; private set; }



        /// <summary>
        /// Constructor of <see cref="OperatingClosedModeBase"/>
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The metrics publisher interface.</param>
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


        /// <inheritdoc/>
        internal override void Activate()
        {
            ActiveFeedback?.Reset();
            ReactiveFeedback?.Reset();
            base.Activate();
        }


        /// <summary>
        /// Creates the active power feedback control. Call it in the constructor before registering the metrics.
        /// </summary>
        /// <param name="quantity">Name of the controlled quantity (e.g. ActivePower, Voltage, PowerFactor).</param>
        /// <param name="measuredSource">Source of the measured value bound to the operating mode map.</param>
        /// <param name="settings">The feedback settings.</param>
        /// <returns>The created active power feedback control.</returns>
        /// <exception cref="InvalidOperationException">The operating mode has no active power classification.</exception>
        protected FeedbackControl<ActivePower> CreateActiveFeedback(string quantity, Func<double?> measuredSource, FeedbackSettings settings)
        {
            PowerControl<ActivePower> activeControl = ActiveControl ?? throw new InvalidOperationException($"{Name} has no active power classification.");
            ActiveFeedback = new FeedbackControl<ActivePower>(_logger, Name, quantity, measuredSource, settings, activeControl.Limits, PowerConverters.Active);

            return ActiveFeedback;
        }


        /// <summary>
        /// Creates the reactive power feedback control. Call it in the constructor before registering the metrics.
        /// </summary>
        /// <param name="quantity">Name of the controlled quantity (e.g. ReactivePower, Voltage, PowerFactor).</param>
        /// <param name="measuredSource">Source of the measured value bound to the operating mode map.</param>
        /// <param name="settings">The feedback settings.</param>
        /// <returns>The created reactive power feedback control.</returns>
        /// <exception cref="InvalidOperationException">The operating mode has no reactive power classification.</exception>
        protected FeedbackControl<ReactivePower> CreateReactiveFeedback(string quantity, Func<double?> measuredSource, FeedbackSettings settings)
        {
            PowerControl<ReactivePower> reactiveControl = ReactiveControl ?? throw new InvalidOperationException($"{Name} has no reactive power classification.");
            ReactiveFeedback = new FeedbackControl<ReactivePower>(_logger, Name, quantity, measuredSource, settings, reactiveControl.Limits, PowerConverters.Reactive);

            return ReactiveFeedback;
        }


        /// <inheritdoc/>
        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            ActiveFeedback?.RegisterMetrics(_metricsPublisher, _config);
            ReactiveFeedback?.RegisterMetrics(_metricsPublisher, _config);
        }
    }
}
