// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.OpenModes.VoltageReactives
{
    /// <summary>
    /// Mode: Reactive Power Fixed Mode
    /// Purpose: Set a fixed setpoint without feedback signal
    /// Inputs: Reactive Power setpoint, Available Reactive Power
    /// Output Controlled: Reactive Power (Q)
    /// What Output Influences: Reactive Power
    /// </summary>
    public class ReactivePowerFixedMode : OperatingOpenModeBase
    {
        /// <summary>
        /// Reactive power fixed mode configuration.
        /// </summary>
        protected readonly ReactivePowerFixedModeConfig _config;


        /// <summary>
        /// Active power fixed mode map.
        /// </summary>
        protected readonly ReactivePowerFixedModeMap _map;


        /// <summary>
        /// Constructor of <see cref="ReactivePowerFixedMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The metrics publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ReactivePowerFixedMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, ReactivePowerFixedModeConfig config,
            ReactivePowerFixedModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            StateReactive = OperatingModeState.Disabled;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            if (StateReactive != OperatingModeState.Disabled)
            {
                ReactivePower? available = _map.AvailableReactivePower?.Invoke();
                double? targetSetpoint = CheckNewReactiveTargetSetpoint(available);

                if (targetSetpoint != null)
                {
                    // Apply configured limits if configured
                    targetSetpoint = ApplyReactiveLimits(targetSetpoint.Value);

                    _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Reactive Target-Setpoint set to {ReactiveTargetSetpoint}", Name, targetSetpoint.Value);
                    RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, targetSetpoint.Value);
                }

                _targetReactivePower.KiloVoltAmperesReactive = RampControllerReactive.Calculate();
                CheckFinalReactiveTarget();
            }

            return Task.CompletedTask;
        }
    }
}