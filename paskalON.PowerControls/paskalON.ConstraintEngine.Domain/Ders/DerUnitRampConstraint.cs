// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain.Ders
{
    public class DerUnitRampConstraint : ConstraintBase, IDerUnitConstraint
    {
        /// <summary>
        /// Time provider for system time abstraction.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// System ramp constraint configuration.
        /// </summary>
        private readonly DerUnitRampConstraintConfig _config;


        /// <summary>
        /// System ramp constraint map.
        /// </summary>
        private readonly DerUnitRampConstraintMap _map;


        /// <summary>
        /// Last active watt power in kilo watt.
        /// </summary>
        private double _lastActiveKiloWattPower = 0;


        /// <summary>
        /// Last reactive voltage ampere reactive in kilo var.
        /// </summary>
        private double _lastReactiveKiloVarsPower = 0;


        /// <summary>
        /// Time stamp of the last ApplyLimits call.
        /// </summary>
        private DateTimeOffset _lastApply = DateTimeOffset.MinValue;


        /// <summary>
        /// Constructor of <see cref="DerUnitRampConstraint"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">System ramp constraint configuration.</param>
        /// <param name="map">System ramp constraint map.</param>
        /// <param name="timeProvider">Time provider for system time abstraction.</param>
        public DerUnitRampConstraint(ILogger logger, DerUnitRampConstraintConfig config, DerUnitRampConstraintMap map, TimeProvider timeProvider) : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _config = config;
            _map = map;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ApplyConstraints(ref ActivePower activePower, ref ReactivePower reactivePower)
        {
            // Active power
            double allowedActiveRamp = _config.MaximumActivePowerKiloWattRampRatePerSecond * TimeSpan.FromTicks(_timeProvider.GetUtcNow().Ticks - _lastApply.Ticks).TotalSeconds;

            if (_lastApply == DateTimeOffset.MinValue && Math.Abs(activePower.KiloWatts) > _config.MaximumActivePowerKiloWattRampRatePerSecond)
            {
                _logger.LogWarning("{Name} units initial active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, _config.MaximumActivePowerKiloWattRampRatePerSecond);
                activePower.KiloWatts = activePower.Watts < 0 ?
                    _config.MaximumActivePowerKiloWattRampRatePerSecond * -1 : _config.MaximumActivePowerKiloWattRampRatePerSecond;
            }
            else if ((Math.Abs(activePower.KiloWatts) - Math.Abs(_lastActiveKiloWattPower)) > allowedActiveRamp)
            {
                _logger.LogWarning("{Name} units active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedActiveRamp);
                activePower.KiloWatts = activePower.Watts < 0 ? allowedActiveRamp * -1 : allowedActiveRamp;
            }

            _lastActiveKiloWattPower = activePower.KiloWatts;

            // Reactive power
            double allowedReactiveRamp = _config.MaximumReactivePowerKiloVarsRampRatePerSecond * TimeSpan.FromTicks(_timeProvider.GetUtcNow().Ticks - _lastApply.Ticks).TotalSeconds;

            if (_lastApply == DateTimeOffset.MinValue && Math.Abs(reactivePower.KiloVoltAmperesReactive) > _config.MaximumReactivePowerKiloVarsRampRatePerSecond)
            {
                _logger.LogWarning("{Name} units initial reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, _config.MaximumReactivePowerKiloVarsRampRatePerSecond);
                reactivePower.KiloVoltAmperesReactive = reactivePower.VoltAmperesReactive < 0 ?
                    _config.MaximumReactivePowerKiloVarsRampRatePerSecond * -1 : _config.MaximumReactivePowerKiloVarsRampRatePerSecond * 1;
            }
            else if ((Math.Abs(reactivePower.KiloVoltAmperesReactive) - Math.Abs(_lastReactiveKiloVarsPower)) > allowedReactiveRamp)
            {
                _logger.LogWarning("{Name} units reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedReactiveRamp);
                reactivePower.KiloVoltAmperesReactive = reactivePower.VoltAmperesReactive < 0 ? allowedReactiveRamp * -1 : allowedReactiveRamp;
            }

            _lastReactiveKiloVarsPower = reactivePower.KiloVoltAmperesReactive;
            _lastApply = _timeProvider.GetUtcNow();
        }
    }
}
