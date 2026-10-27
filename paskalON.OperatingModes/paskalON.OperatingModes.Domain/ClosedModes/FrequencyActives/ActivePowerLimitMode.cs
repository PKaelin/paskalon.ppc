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
    /// Mode: Maximum Active Power Limit Mode
    /// Purpose: Restricts the active power output to a cap
    /// Inputs: Measured Active Power at POI, Maximum Active Power setpoint (configuration)
    /// Output Controlled: Active Power (P)
    /// What Output Influences: Active Power
    /// </summary>
    /// <remarks>
    /// The cap is the configured maximum active power limit, otherwise the nameplate. The limit is applied to the final active
    /// power of the stack: <c>LimitedActivePower = min(FinalActivePower, limit)</c>.
    /// While the limit is binding, a ramp-limited integral feedback corrects the limit with the measured active power at the POI,
    /// so losses or auxiliary loads do not keep the POI below or push it above the cap. The correction is reset while the limit
    /// is not binding.
    /// </remarks>
    public class MaximumActivePowerLimitMode : OperatingClosedModeBase, ILimitingActiveMode
    {
        /// <summary>
        /// Maximum active power limit mode configuration.
        /// </summary>
        protected readonly MaximumActivePowerLimitModeConfig _config;


        /// <summary>
        /// Maximum active power limit mode map.
        /// </summary>
        protected readonly MaximumActivePowerLimitModeMap _map;


        /// <summary>
        /// Gets the configured cap or the nameplate if the cap is not configured.
        /// </summary>
        public ActivePower MaximumActivePower { get => new ActivePower(_config.MaximumActivePowerLimitWatts ?? _systemConfig.NameplateMaximumActivePowerWatt); }


        /// <summary>
        /// Gets whether the limit was binding in the last calculation.
        /// </summary>
        public bool IsLimiting { get; private set; }


        /// <summary>
        /// Constructor of <see cref="MaximumActivePowerLimitMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public MaximumActivePowerLimitMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, MaximumActivePowerLimitModeConfig config,
            MaximumActivePowerLimitModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            FeedbackSettings settings = new FeedbackSettings(0, config.DeadbandError) { Correction = FeedbackCorrection.RampLimitedIntegral };
            CreateActiveFeedback("ActivePower", () => _map.ActivePowerAtPoi.Invoke()?.Watts, settings, rampController.ShallowCopy());
            RegisterMetrics();
        }


        /// <inheritdoc/>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            ArgumentNullException.ThrowIfNull(signals);

            // The cap is the target of the power axis, a changed cap is transitioned by the system operating mode control
            ActivePower maximum = MaximumActivePower;
            ActivePower limit = ActiveControl!.Calculate(maximum, maximum);
            ActivePower final = signals.FinalActivePower;
            IsLimiting = final.Watts > limit.Watts;
            ActivePower correctedLimit = limit;

            // Correct the limit with the measurement only while it is binding, otherwise the correction would wind up
            if (IsLimiting)
            {
                correctedLimit = ActiveFeedback!.Correct(limit, limit.Watts);
            }
            else if (ActiveFeedback!.ErrorAdjustment.Watts != 0)
            {
                ActiveFeedback.Reset();
            }

            signals.LimitedActivePower = new ActivePower(Math.Min(final.Watts, correctedLimit.Watts));

            return signals;
        }


        /// <inheritdoc/>
        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            _metricsPublisher.Register<MaximumActivePowerLimitMode, double>(this, nameof(MaximumActivePower), MetricType.Gauge, x => x.MaximumActivePower.Watts, _config.MetricsFactorClass2);
        }
    }
}
