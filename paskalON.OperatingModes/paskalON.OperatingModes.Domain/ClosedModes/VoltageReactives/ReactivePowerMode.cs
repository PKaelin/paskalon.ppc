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
    /// Mode: Reactive Power Mode
    /// Purpose: Volt-Ampere Reactive setpoint
    /// Inputs: Reactive Power setpoint, Available Reactive Power, Measured Reactive Power at POI
    /// Output Controlled: Reactive Power (Q)
    /// What Output Influences: Reactive Power, Grid voltage
    /// </summary>
    public class ReactivePowerMode : OperatingClosedModeBase, IBaseReactiveMode
    {
        /// <summary>
        /// Reactive power mode configuration.
        /// </summary>
        protected readonly ReactivePowerModeConfig _config;


        /// <summary>
        /// Reactive power mode map.
        /// </summary>
        protected readonly ReactivePowerModeMap _map;


        /// <summary>
        /// Constructor of <see cref="ReactivePowerMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ReactivePowerMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, ReactivePowerModeConfig config,
            ReactivePowerModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            CreateReactiveFeedback("ReactivePower", () => _map.ReactivePowerAtPoi.Invoke()?.VoltAmperesReactive, new FeedbackSettings(config.ProportionalGain, config.DeadbandError));
            RegisterMetrics();
        }


        /// <inheritdoc/>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            ArgumentNullException.ThrowIfNull(signals);

            ReactivePower target = ReactiveControl!.Calculate(signals.SetpointReactivePower, _map.AvailableReactivePower.Invoke());
            signals.BaseReactivePower = ReactiveFeedback!.Correct(target, target.VoltAmperesReactive);

            return signals;
        }
    }
}
