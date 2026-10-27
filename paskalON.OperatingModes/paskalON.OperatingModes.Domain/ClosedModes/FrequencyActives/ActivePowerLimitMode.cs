// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Mode: Active Power Limit Mode
    /// Purpose: Restricts the active power output to a band between a minimum and a maximum
    /// Inputs: Measured Active Power at POI, Minimum and Maximum Active Power limits (configuration)
    /// Output Controlled: Active Power (P)
    /// What Output Influences: Active Power
    /// </summary>
    /// <remarks>
    /// The limits are the configured minimum and maximum active power limits, otherwise the nameplate. The limits are applied to the
    /// final active power of the stack: <c>LimitedActivePower = clamp(FinalActivePower, minimum, maximum)</c>.
    /// While a limit is binding, a ramp-limited integral feedback corrects the binding limit with the measured active power at the POI,
    /// so losses or auxiliary loads do not keep the POI inside or push it outside the limits. The correction may move the limit up to the
    /// nameplate. It is reset while no limit is binding and when the binding limit switches between minimum and maximum.
    /// </remarks>
    public class ActivePowerLimitMode : OperatingClosedModeBase, ILimitingActiveMode
    {
        /// <summary>
        /// Active power limit mode configuration.
        /// </summary>
        protected readonly ActivePowerLimitModeConfig _config;


        /// <summary>
        /// Active power limit mode map.
        /// </summary>
        protected readonly ActivePowerLimitModeMap _map;


        /// <summary>
        /// Gets the configured maximum limit or the nameplate if the maximum limit is not configured.
        /// </summary>
        public ActivePower MaximumActivePower { get => new ActivePower(ActiveControl!.Limits.Maximum); }


        /// <summary>
        /// Gets the configured minimum limit or the nameplate if the minimum limit is not configured.
        /// </summary>
        public ActivePower MinimumActivePower { get => new ActivePower(ActiveControl!.Limits.Minimum); }


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
        /// Constructor of <see cref="ActivePowerLimitMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ActivePowerLimitMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, ActivePowerLimitModeConfig config,
            ActivePowerLimitModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            FeedbackSettings settings = new FeedbackSettings(0, config.DeadbandError) { Correction = FeedbackCorrection.RampLimitedIntegral };

            // The corrected limit may compensate losses beyond the configured limits, only the nameplate restricts it
            PowerControlLimits nameplate = new PowerControlLimits(systemConfig.NameplateMinimumActivePowerWatt, systemConfig.NameplateMaximumActivePowerWatt,
                config.DeadbandSetpoint, config.DeadbandAvailable);
            CreateActiveFeedback("ActivePower", () => _map.ActivePowerAtPoi.Invoke()?.Watts, settings, rampController.ShallowCopy(), nameplate);
            RegisterMetrics();
        }


        /// <inheritdoc/>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            ArgumentNullException.ThrowIfNull(signals);

            // The maximum is the target of the power axis, a changed maximum is transitioned by the system operating mode control
            ActivePower maximum = MaximumActivePower;
            ActivePower maximumLimit = ActiveControl!.Calculate(maximum, maximum);
            ActivePower minimumLimit = MinimumActivePower;
            ActivePower final = signals.FinalActivePower;
            bool wasLimitingMaximum = IsLimitingMaximum;
            bool wasLimitingMinimum = IsLimitingMinimum;
            IsLimitingMaximum = final.Watts > maximumLimit.Watts;
            IsLimitingMinimum = final.Watts < minimumLimit.Watts;
            bool isBindingLimitSwitched = (IsLimitingMaximum && wasLimitingMinimum) || (IsLimitingMinimum && wasLimitingMaximum);

            // The correction belongs to the binding limit, otherwise it would wind up or be applied to the wrong limit
            if ((IsLimiting is false || isBindingLimitSwitched) && ActiveFeedback!.ErrorAdjustment.Watts != 0)
            {
                ActiveFeedback.Reset();
            }

            double limited = final.Watts;

            if (IsLimitingMaximum)
            {
                limited = Math.Min(final.Watts, ActiveFeedback!.Correct(maximumLimit, maximumLimit.Watts).Watts);
            }
            else if (IsLimitingMinimum)
            {
                limited = Math.Max(final.Watts, ActiveFeedback!.Correct(minimumLimit, minimumLimit.Watts).Watts);
            }

            signals.LimitedActivePower = new ActivePower(limited);

            return signals;
        }


        /// <inheritdoc/>
        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            _metricsPublisher.Register<ActivePowerLimitMode, double>(this, nameof(MaximumActivePower), MetricType.Gauge, x => x.MaximumActivePower.Watts, _config.MetricsFactorClass2);
            _metricsPublisher.Register<ActivePowerLimitMode, double>(this, nameof(MinimumActivePower), MetricType.Gauge, x => x.MinimumActivePower.Watts, _config.MetricsFactorClass2);
        }
    }
}
