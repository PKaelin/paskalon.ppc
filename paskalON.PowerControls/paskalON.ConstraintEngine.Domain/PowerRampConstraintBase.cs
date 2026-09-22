// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain
{
    public class PowerRampConstraintBase : ConstraintBase
    {
        /// <summary>
        /// Power ramp constraint configuration.
        /// </summary>
        private readonly PowerRampConstraintConfig _config;


        /// <summary>
        /// Last active watt power in watt.
        /// </summary>
        protected double _lastActiveWattPower = 0;


        /// <summary>
        /// Last reactive voltage ampere reactive in var.
        /// </summary>
        protected double _lastReactiveVarsPower = 0;


        /// <summary>
        /// Time stamp of the last ApplyLimits call.
        /// </summary>
        protected DateTimeOffset _lastApply = DateTimeOffset.MinValue;


        /// <summary>
        /// Time provider for system time abstraction.
        /// </summary>
        protected readonly TimeProvider _timeProvider;


        /// <summary>
        /// Constructor of <see cref="PowerConstraintBase"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">Power ramp constraint configuration.</param>
        /// <param name="map">Power constraint base map.</param>
        public PowerRampConstraintBase(ILogger logger, PowerRampConstraintConfig config, ConstraintBaseMap map, TimeProvider timeProvider)
            : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _config = config;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ApplyConstraints(ref ActivePower activePower, ref ReactivePower reactivePower, bool shallLogViolations = true)
        {
            // Active power
            double allowedActiveRamp = _config.MaximumActivePowerWattRampRatePerSecond * TimeSpan.FromTicks(_timeProvider.GetUtcNow().Ticks - _lastApply.Ticks).TotalSeconds;

            if (_lastApply == DateTimeOffset.MinValue && Math.Abs(activePower.Watts) > _config.MaximumActivePowerWattRampRatePerSecond)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} initial active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, _config.MaximumActivePowerWattRampRatePerSecond);
                }

                activePower.Watts = activePower.Watts < 0 ?
                    _config.MaximumActivePowerWattRampRatePerSecond * -1 : _config.MaximumActivePowerWattRampRatePerSecond;
            }
            else if ((Math.Abs(activePower.Watts) - Math.Abs(_lastActiveWattPower)) > allowedActiveRamp)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedActiveRamp);
                }

                activePower.Watts = activePower.Watts < 0 ? allowedActiveRamp * -1 : allowedActiveRamp;
            }

            _lastActiveWattPower = activePower.Watts;

            // Reactive power
            double allowedReactiveRamp = _config.MaximumReactivePowerVarsRampRatePerSecond * TimeSpan.FromTicks(_timeProvider.GetUtcNow().Ticks - _lastApply.Ticks).TotalSeconds;

            if (_lastApply == DateTimeOffset.MinValue && Math.Abs(reactivePower.VoltAmperesReactive) > _config.MaximumReactivePowerVarsRampRatePerSecond)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} initial reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, _config.MaximumReactivePowerVarsRampRatePerSecond);
                }
                reactivePower.VoltAmperesReactive = reactivePower.VoltAmperesReactive < 0 ?
                    _config.MaximumReactivePowerVarsRampRatePerSecond * -1 : _config.MaximumReactivePowerVarsRampRatePerSecond * 1;
            }
            else if ((Math.Abs(reactivePower.VoltAmperesReactive) - Math.Abs(_lastReactiveVarsPower)) > allowedReactiveRamp)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedReactiveRamp);
                }
                reactivePower.VoltAmperesReactive = reactivePower.VoltAmperesReactive < 0 ? allowedReactiveRamp * -1 : allowedReactiveRamp;
            }

            _lastReactiveVarsPower = reactivePower.VoltAmperesReactive;
            _lastApply = _timeProvider.GetUtcNow();
        }
    }
}
