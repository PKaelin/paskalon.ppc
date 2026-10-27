// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Mode: Reactive Power Limit Mode
    /// Purpose: Restricts the reactive power output to a band between a minimum and a maximum
    /// Inputs: Measured Reactive Power at POI, Minimum and Maximum Reactive Power limits (configuration)
    /// Output Controlled: Reactive Power (Q)
    /// What Output Influences: Reactive Power
    /// </summary>
    /// <remarks>
    /// The limits are the configured minimum and maximum reactive power limits, otherwise the nameplate. The limits are applied to the
    /// final reactive power of the stack: <c>LimitedReactivePower = clamp(FinalReactivePower, minimum, maximum)</c>.
    /// While a limit is binding, a ramp-limited integral feedback corrects the binding limit with the measured reactive power at the POI,
    /// so losses or auxiliary loads do not keep the POI inside or push it outside the limits. The correction may move the limit up to the
    /// nameplate. It is reset while no limit is binding and when the binding limit switches between minimum and maximum.
    /// </remarks>
    public class ReactivePowerLimitMode : OperatingClosedModeBase, ILimitingReactiveMode
    {
        /// <summary>
        /// Reactive power limit mode configuration.
        /// </summary>
        protected readonly ReactivePowerLimitModeConfig _config;


        /// <summary>
        /// Reactive power limit mode map.
        /// </summary>
        protected readonly ReactivePowerLimitModeMap _map;


        /// <summary>
        /// Gets the configured maximum limit or the nameplate if the maximum limit is not configured.
        /// </summary>
        public ReactivePower MaximumReactivePower { get => new ReactivePower(ReactiveControl!.Limits.Maximum); }


        /// <summary>
        /// Gets the configured minimum limit or the nameplate if the minimum limit is not configured.
        /// </summary>
        public ReactivePower MinimumReactivePower { get => new ReactivePower(ReactiveControl!.Limits.Minimum); }


        /// <summary>
        /// Gets whether the maximum limit was binding in the last calculation.
        /// </summary>
        public bool IsLimitingMaximum { get; private set; }


        /// <summary>
        /// Gets whether the minimum limit was binding in the last calculation.
        /// </summary>
        public bool IsLimitingMinimum { get; private set; }


        /// <summary>
        /// Gets whether a limit was binding in the last calculation.
        /// </summary>
        public bool IsLimiting { get => IsLimitingMaximum || IsLimitingMinimum; }


        /// <summary>
        /// Constructor of <see cref="ReactivePowerLimitMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ReactivePowerLimitMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, ReactivePowerLimitModeConfig config,
            ReactivePowerLimitModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            FeedbackSettings settings = new FeedbackSettings(0, config.DeadbandError) { Correction = FeedbackCorrection.RampLimitedIntegral };

            // The corrected limit may compensate losses beyond the configured limits, only the nameplate restricts it
            PowerControlLimits nameplate = new PowerControlLimits(systemConfig.NameplateMinimumReactivePowerVars, systemConfig.NameplateMaximumReactivePowerVars,
                config.DeadbandSetpoint, config.DeadbandAvailable);
            CreateReactiveFeedback("ReactivePower", () => _map.ReactivePowerAtPoi.Invoke()?.VoltAmperesReactive, settings, rampController.ShallowCopy(), nameplate);
            RegisterMetrics();
        }


        /// <inheritdoc/>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            ArgumentNullException.ThrowIfNull(signals);

            // The maximum is the target of the power axis, a changed maximum is transitioned by the system operating mode control
            ReactivePower maximum = MaximumReactivePower;
            ReactivePower maximumLimit = ReactiveControl!.Calculate(maximum, maximum);
            ReactivePower minimumLimit = MinimumReactivePower;
            ReactivePower final = signals.FinalReactivePower;
            bool wasLimitingMaximum = IsLimitingMaximum;
            bool wasLimitingMinimum = IsLimitingMinimum;
            IsLimitingMaximum = final.VoltAmperesReactive > maximumLimit.VoltAmperesReactive;
            IsLimitingMinimum = final.VoltAmperesReactive < minimumLimit.VoltAmperesReactive;
            bool isBindingLimitSwitched = (IsLimitingMaximum && wasLimitingMinimum) || (IsLimitingMinimum && wasLimitingMaximum);

            // The correction belongs to the binding limit, otherwise it would wind up or be applied to the wrong limit
            if ((IsLimiting is false || isBindingLimitSwitched) && ReactiveFeedback!.ErrorAdjustment.VoltAmperesReactive != 0)
            {
                ReactiveFeedback.Reset();
            }

            double limited = final.VoltAmperesReactive;

            if (IsLimitingMaximum)
            {
                limited = Math.Min(final.VoltAmperesReactive, ReactiveFeedback!.Correct(maximumLimit, maximumLimit.VoltAmperesReactive).VoltAmperesReactive);
            }
            else if (IsLimitingMinimum)
            {
                limited = Math.Max(final.VoltAmperesReactive, ReactiveFeedback!.Correct(minimumLimit, minimumLimit.VoltAmperesReactive).VoltAmperesReactive);
            }

            signals.LimitedReactivePower = new ReactivePower(limited);

            return signals;
        }


        /// <inheritdoc/>
        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            _metricsPublisher.Register<ReactivePowerLimitMode, double>(this, nameof(MaximumReactivePower), MetricType.Gauge, x => x.MaximumReactivePower.VoltAmperesReactive,
                _config.MetricsFactorClass2);
            _metricsPublisher.Register<ReactivePowerLimitMode, double>(this, nameof(MinimumReactivePower), MetricType.Gauge, x => x.MinimumReactivePower.VoltAmperesReactive,
                _config.MetricsFactorClass2);
        }
    }
}
