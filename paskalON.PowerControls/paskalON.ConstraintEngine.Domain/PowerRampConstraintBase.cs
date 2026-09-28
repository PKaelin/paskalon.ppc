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
        private double _lastActiveWattPower = 0;


        /// <summary>
        /// Last reactive voltage ampere reactive in var.
        /// </summary>
        private double _lastReactiveVarsPower = 0;


        /// <summary>
        /// Time stamp of the last ApplyLimits call.
        /// </summary>
        private long _lastApplyTimestamp;


        /// <summary>
        /// Indicates whether a power target has been applied.
        /// </summary>
        private bool _hasApplied;


        /// <summary>
        /// Synchronizes ramp state and target updates.
        /// </summary>
        private readonly object _dataLock = new();


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
            lock (_dataLock)
            {
                long currentTimestamp = _timeProvider.GetTimestamp();
                double elapsedSeconds = _hasApplied ? _timeProvider.GetElapsedTime(_lastApplyTimestamp, currentTimestamp).TotalSeconds : 1;
                double allowedActiveRamp = _config.MaximumActivePowerWattRampRatePerSecond * elapsedSeconds;
                double activeDelta = activePower.Watts - _lastActiveWattPower;

                if (Math.Abs(activeDelta) > allowedActiveRamp)
                {
                    if (shallLogViolations == true)
                    {
                        _logger.LogWarning("{Name} active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedActiveRamp);
                    }

                    activePower.Watts = _lastActiveWattPower + Math.Clamp(activeDelta, -allowedActiveRamp, allowedActiveRamp);
                }

                _lastActiveWattPower = activePower.Watts;

                double allowedReactiveRamp = _config.MaximumReactivePowerVarsRampRatePerSecond * elapsedSeconds;
                double reactiveDelta = reactivePower.VoltAmperesReactive - _lastReactiveVarsPower;

                if (Math.Abs(reactiveDelta) > allowedReactiveRamp)
                {
                    if (shallLogViolations == true)
                    {
                        _logger.LogWarning("{Name} reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedReactiveRamp);
                    }

                    reactivePower.VoltAmperesReactive = _lastReactiveVarsPower + Math.Clamp(reactiveDelta, -allowedReactiveRamp, allowedReactiveRamp);
                }

                _lastReactiveVarsPower = reactivePower.VoltAmperesReactive;
                _lastApplyTimestamp = currentTimestamp;
                _hasApplied = true;
            }
        }
    }
}
