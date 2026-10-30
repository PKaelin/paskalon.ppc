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
        /// Violation name of an active power step above the ramp limit.
        /// </summary>
        private const string ActiveRampExceeded = nameof(ActiveRampExceeded);


        /// <summary>
        /// Violation name of a reactive power step above the ramp limit.
        /// </summary>
        private const string ReactiveRampExceeded = nameof(ReactiveRampExceeded);


        /// <summary>
        /// Power ramp constraint map with the targets currently applied by the devices.
        /// </summary>
        private readonly PowerRampConstraintBaseMap _map;


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
        /// <param name="map">Power ramp constraint base map.</param>
        /// <param name="timeProvider">Time provider for system time abstraction.</param>
        public PowerRampConstraintBase(ILogger logger, PowerRampConstraintConfig config, PowerRampConstraintBaseMap map, TimeProvider timeProvider)
            : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _config = config;
            _map = map;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The allowed ramp step is the ramp rate multiplied with the time since the last applied target, at most
        /// <see cref="PowerRampConstraintConfig.MaximumRampIntervalMilliseconds"/>. The first target ramps from the target currently
        /// applied by the devices, or from zero if unknown, with an interval of one second at most.
        /// </remarks>
        public override void ApplyConstraints(ref ActivePower activePower, ref ReactivePower reactivePower, bool shallLogViolations = true)
        {
            lock (_dataLock)
            {
                long currentTimestamp = _timeProvider.GetTimestamp();
                double elapsedSeconds = GetRampIntervalSeconds(currentTimestamp);
                double allowedActiveRamp = _config.MaximumActivePowerWattRampRatePerSecond * elapsedSeconds;
                double activeDelta = activePower.Watts - _lastActiveWattPower;

                bool isActiveRampExceeded = Math.Abs(activeDelta) > allowedActiveRamp;

                if (shallLogViolations == true && UpdateViolation(ActiveRampExceeded, isActiveRampExceeded))
                {
                    if (isActiveRampExceeded)
                    {
                        _logger.LogWarning("{Name} active power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedActiveRamp);
                    }
                    else
                    {
                        _logger.LogInformation("{Name} active power ramp reached {ActivePower} within the limit.", Name, activePower.Watts);
                    }
                }

                if (isActiveRampExceeded)
                {
                    activePower.Watts = _lastActiveWattPower + Math.Clamp(activeDelta, -allowedActiveRamp, allowedActiveRamp);
                }

                _lastActiveWattPower = activePower.Watts;

                double allowedReactiveRamp = _config.MaximumReactivePowerVarsRampRatePerSecond * elapsedSeconds;
                double reactiveDelta = reactivePower.VoltAmperesReactive - _lastReactiveVarsPower;

                bool isReactiveRampExceeded = Math.Abs(reactiveDelta) > allowedReactiveRamp;

                if (shallLogViolations == true && UpdateViolation(ReactiveRampExceeded, isReactiveRampExceeded))
                {
                    if (isReactiveRampExceeded)
                    {
                        _logger.LogWarning("{Name} reactive power ramp exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, allowedReactiveRamp);
                    }
                    else
                    {
                        _logger.LogInformation("{Name} reactive power ramp reached {ReactivePower} within the limit.", Name, reactivePower.VoltAmperesReactive);
                    }
                }

                if (isReactiveRampExceeded)
                {
                    reactivePower.VoltAmperesReactive = _lastReactiveVarsPower + Math.Clamp(reactiveDelta, -allowedReactiveRamp, allowedReactiveRamp);
                }

                _lastReactiveVarsPower = reactivePower.VoltAmperesReactive;
                _lastApplyTimestamp = currentTimestamp;
                _hasApplied = true;
            }
        }


        /// <summary>
        /// Gets the interval used to calculate the allowed ramp step and starts the first ramp from the current targets of the devices.
        /// Must be called within the data lock.
        /// </summary>
        /// <param name="currentTimestamp">The current timestamp of the time provider.</param>
        /// <returns>The ramp interval in seconds.</returns>
        private double GetRampIntervalSeconds(long currentTimestamp)
        {
            TimeSpan maximumInterval = TimeSpan.FromMilliseconds(_config.MaximumRampIntervalMilliseconds);

            if (_hasApplied is false)
            {
                StartFromCurrentTargets();

                return Math.Min(1, maximumInterval.TotalSeconds);
            }

            TimeSpan elapsed = _timeProvider.GetElapsedTime(_lastApplyTimestamp, currentTimestamp);

            if (elapsed > maximumInterval)
            {
                _logger.LogInformation("{Name} last target was applied {Elapsed} ago, the ramp step is limited to {MaximumInterval}.", Name, elapsed, maximumInterval);

                return maximumInterval.TotalSeconds;
            }

            return elapsed.TotalSeconds;
        }


        /// <summary>
        /// Starts the first ramp from the targets currently applied by the devices, so that a restarted power control does not ramp from zero.
        /// Must be called within the data lock.
        /// </summary>
        private void StartFromCurrentTargets()
        {
            ActivePower? currentActivePowerTarget = _map.CurrentActivePowerTarget.Invoke();
            ReactivePower? currentReactivePowerTarget = _map.CurrentReactivePowerTarget.Invoke();
            _lastActiveWattPower = currentActivePowerTarget?.Watts ?? 0;
            _lastReactiveVarsPower = currentReactivePowerTarget?.VoltAmperesReactive ?? 0;

            if (currentActivePowerTarget.HasValue is false || currentReactivePowerTarget.HasValue is false)
            {
                _logger.LogInformation("{Name} current targets of the devices are unknown, the first ramp starts from {ActivePower} W and {ReactivePower} var.",
                    Name, _lastActiveWattPower, _lastReactiveVarsPower);

                return;
            }

            _logger.LogInformation("{Name} first ramp starts from the current targets of the devices {ActivePower} W and {ReactivePower} var.",
                Name, _lastActiveWattPower, _lastReactiveVarsPower);
        }
    }
}
