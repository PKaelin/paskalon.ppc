// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
        /// Last active watt power in kilo watt.
        /// </summary>
        protected double _lastActiveKiloWattPower = 0;


        /// <summary>
        /// Last reactive voltage ampere reactive in kilo var.
        /// </summary>
        protected double _lastReactiveKiloVarsPower = 0;


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
            double allowedActiveRamp = _config.MaximumActivePowerKiloWattRampRatePerSecond * TimeSpan.FromTicks(_timeProvider.GetUtcNow().Ticks - _lastApply.Ticks).TotalSeconds;

            if (_lastApply == DateTimeOffset.MinValue && Math.Abs(activePower.KiloWatts) > _config.MaximumActivePowerKiloWattRampRatePerSecond)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} initial active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, _config.MaximumActivePowerKiloWattRampRatePerSecond);
                }
                activePower.KiloWatts = activePower.Watts < 0 ?
                    _config.MaximumActivePowerKiloWattRampRatePerSecond * -1 : _config.MaximumActivePowerKiloWattRampRatePerSecond;
            }
            else if ((Math.Abs(activePower.KiloWatts) - Math.Abs(_lastActiveKiloWattPower)) > allowedActiveRamp)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedActiveRamp);
                }
                activePower.KiloWatts = activePower.Watts < 0 ? allowedActiveRamp * -1 : allowedActiveRamp;
            }

            _lastActiveKiloWattPower = activePower.KiloWatts;

            // Reactive power
            double allowedReactiveRamp = _config.MaximumReactivePowerKiloVarsRampRatePerSecond * TimeSpan.FromTicks(_timeProvider.GetUtcNow().Ticks - _lastApply.Ticks).TotalSeconds;

            if (_lastApply == DateTimeOffset.MinValue && Math.Abs(reactivePower.KiloVoltAmperesReactive) > _config.MaximumReactivePowerKiloVarsRampRatePerSecond)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} initial reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, _config.MaximumReactivePowerKiloVarsRampRatePerSecond);
                }
                reactivePower.KiloVoltAmperesReactive = reactivePower.VoltAmperesReactive < 0 ?
                    _config.MaximumReactivePowerKiloVarsRampRatePerSecond * -1 : _config.MaximumReactivePowerKiloVarsRampRatePerSecond * 1;
            }
            else if ((Math.Abs(reactivePower.KiloVoltAmperesReactive) - Math.Abs(_lastReactiveKiloVarsPower)) > allowedReactiveRamp)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedReactiveRamp);
                }
                reactivePower.KiloVoltAmperesReactive = reactivePower.VoltAmperesReactive < 0 ? allowedReactiveRamp * -1 : allowedReactiveRamp;
            }

            _lastReactiveKiloVarsPower = reactivePower.KiloVoltAmperesReactive;
            _lastApply = _timeProvider.GetUtcNow();
        }
    }
}
