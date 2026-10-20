// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Maths.Calculuses;
using paskalON.Maths.Calculuses.Coordinates;
using paskalON.Maths.Calculuses.Logarithmics;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Application.Ramps
{
    /// <summary>
    /// Ramp controller for controlling ramps according to its configurations.
    /// </summary>
    public class RampController : IRampController
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Time provider for system time abstraction.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Ramp base configuration containing concrete ramp configuration.
        /// </summary>
        private readonly RampBaseConfig _rampBaseConfig;


        /// <summary>
        /// Ramp function for ramp controller.
        /// </summary>
        private ICalculateOutputFunction? _rampFunction;


        /// <summary>
        /// Start value of the ramp.
        /// </summary>
        public double StartValue { get; protected set; }


        /// <summary>
        /// Target value of the ramp.
        /// </summary>
        public double TargetValue { get; protected set; }


        /// <summary>
        /// Current value of the ramp.
        /// </summary>
        public double CurrentValue { get; protected set; }


        /// <summary>
        /// Get the start date of the ramp when it was started.
        /// </summary>
        public DateTimeOffset StartDate { get; protected set; } = DateTimeOffset.MinValue;


        /// <summary>
        /// Constructor of <see cref="RampController"/>
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="timeProvider">Time provider for system time abstraction.</param>
        /// <param name="rampBaseConfig">Ramp base configuration containing concrete ramp configuration.</param>
        public RampController(ILogger logger, TimeProvider timeProvider, RampBaseConfig rampBaseConfig)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(rampBaseConfig);

            _logger = logger;
            _timeProvider = timeProvider;
            _rampBaseConfig = rampBaseConfig;
        }


        /// <inheritdoc/>
        public void Start(double startValue, double targetValue)
        {
            StartDate = _timeProvider.GetUtcNow();
            StartValue = startValue;
            TargetValue = targetValue;
            CurrentValue = startValue;
            Initialize();
        }


        /// <inheritdoc/>
        public void Stop()
        {
            StartDate = DateTimeOffset.MinValue;
        }


        /// <inheritdoc/>
        public double Calculate()
        {
            lock (_rampBaseConfig)
            {
                if (StartDate == DateTimeOffset.MinValue)
                {
                    return CurrentValue;
                }

                if (((_rampBaseConfig is RampRateConfig) || (_rampBaseConfig is RampRatePercentageConfig) ||
                    (_rampBaseConfig is RampTimeConfig) || (_rampBaseConfig is RampTimeConstantConfig)) && (_rampFunction != null))
                {
                    // The ramp functions use the elapsed ticks since the start date. Absolute ticks would lose precision in the
                    // long to double conversion (about 128 ticks), so that the ramp would miss its values by some watts.
                    // Delay the ramp by the configured RampTimeSeconds
                    long elapsedTicks = (_timeProvider.GetUtcNow() - StartDate).Ticks;
                    CurrentValue = _rampFunction.CalculateOutput(elapsedTicks - TimeSpan.FromSeconds(_rampBaseConfig.RampTimeSeconds).Ticks);
                }
                else
                {
                    throw new NotImplementedException($"Ramp controller for type: {_rampBaseConfig.GetType()} has not been implemented");
                }
            }

            return CurrentValue;
        }


        /// <inheritdoc/>
        public double CalculatePrecision(int precision = 3)
        {
            return Math.Round(Calculate(), precision);
        }


        /// <inheritdoc/>
        public IRampController ShallowCopy()
        {
            return (RampController)MemberwiseClone();
        }


        /// <summary>
        /// Initializes the ramp controller
        /// </summary>
        /// <exception cref="NotImplementedException">Returns exception if the ramp configuration is not implemented.</exception>
        private void Initialize()
        {
            lock (_rampBaseConfig)
            {
                if (_rampBaseConfig is RampRateConfig rampRateConfig)
                {
                    List<LinearPoint> linearPoints = new List<LinearPoint>();
                    linearPoints.Add(new LinearPoint(0, StartValue));

                    if (StartValue != TargetValue)
                    {
                        double rampRatePerSecond = StartValue < TargetValue ? rampRateConfig.RampUpRatePerSecond : rampRateConfig.RampDownRatePerSecond;
                        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rampRatePerSecond);

                        // The target is reached at the exact time of the ramp rate, also if the difference is not a multiple of the ramp rate.
                        // The ticks are not rounded so that the slope is exactly the ramp rate.
                        double rampTicks = Math.Abs(TargetValue - StartValue) / rampRatePerSecond * TimeSpan.TicksPerSecond;
                        linearPoints.Add(new LinearPoint(rampTicks, TargetValue));
                    }

                    _rampFunction = new LinearPointFunction(linearPoints);
                }
                else if (_rampBaseConfig is RampRatePercentageConfig)
                {
                    /// Current: 0, Target: 60, RampUpRatePercentPerSecond: 50 = Every second a new target 0s=0, 1s=30, 2s=45, 3s=67.5

                    List<LinearPoint> linearPoints = new List<LinearPoint>();
                    linearPoints.Add(new LinearPoint(0, StartValue));
                    int precision = ((RampRatePercentageConfig)_rampBaseConfig).RampRatePrecision;
                    int steps = 1;

                    if (StartValue <= TargetValue)
                    {
                        double rampRate = ((RampRatePercentageConfig)_rampBaseConfig).RampUpRatePercentPerSecond;
                        double rampStepValue = TargetValue / 100 * rampRate;
                        linearPoints.Add(new LinearPoint(TimeSpan.FromSeconds(steps++).Ticks, rampStepValue));

                        while (Math.Round(rampStepValue, precision) < TargetValue)
                        {
                            rampStepValue = rampStepValue + (rampStepValue / 100 * rampRate);
                            linearPoints.Add(new LinearPoint(TimeSpan.FromSeconds(steps++).Ticks, rampStepValue));
                        }

                        linearPoints.Last().Y = TargetValue;
                    }
                    else
                    {
                        double rampRate = ((RampRatePercentageConfig)_rampBaseConfig).RampDownRatePercentPerSecond;
                        double rampStepValue = StartValue / 100 * rampRate;
                        linearPoints.Add(new LinearPoint(TimeSpan.FromSeconds(steps++).Ticks, rampStepValue));

                        while (Math.Round(rampStepValue, precision) > TargetValue)
                        {
                            rampStepValue = rampStepValue - (rampStepValue / 100 * rampRate);
                            linearPoints.Add(new LinearPoint(TimeSpan.FromSeconds(steps++).Ticks, rampStepValue));
                        }

                        linearPoints.Last().Y = TargetValue;
                    }

                    _rampFunction = new LinearPointFunction(linearPoints);
                }
                else if (_rampBaseConfig is RampTimeConfig)
                {
                    List<LinearPoint> linearPoints = new List<LinearPoint>();
                    linearPoints.Add(new LinearPoint(0, StartValue));

                    if (StartValue <= TargetValue)
                    {
                        linearPoints.Add(new LinearPoint(TimeSpan.FromSeconds(((RampTimeConfig)_rampBaseConfig).RampUpTimeSeconds).Ticks, TargetValue));
                    }
                    else
                    {
                        linearPoints.Add(new LinearPoint(TimeSpan.FromSeconds(((RampTimeConfig)_rampBaseConfig).RampDownTimeSeconds).Ticks, TargetValue));
                    }

                    _rampFunction = new LinearPointFunction(linearPoints);
                }
                else if (_rampBaseConfig is RampTimeConstantConfig)
                {
                    if (StartValue <= TargetValue)
                    {
                        long period = TimeSpan.FromSeconds(((RampTimeConstantConfig)_rampBaseConfig).RampUpTimeConstantSeconds).Ticks;
                        _rampFunction = new LogarithmicEasingFunction(StartValue, TargetValue, period, 0, ((RampTimeConstantConfig)_rampBaseConfig).TuningValue);
                    }
                    else
                    {
                        long period = TimeSpan.FromSeconds(((RampTimeConstantConfig)_rampBaseConfig).RampDownTimeConstantSeconds).Ticks;
                        _rampFunction = new LogarithmicEasingFunction(StartValue, TargetValue, period, 0, ((RampTimeConstantConfig)_rampBaseConfig).TuningValue);
                    }
                }
                else
                {
                    throw new NotImplementedException($"Ramp controller for type: {_rampBaseConfig.GetType()} has not been implemented");
                }
            }

            if (_rampFunction != null)
            {
                _logger.LogDebug("{Class} initialized: {context}", nameof(RampController), _rampFunction.ToString());
            }
        }


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{_rampBaseConfig.ToString()} StartDate: {StartDate}, StartValue: {StartValue} TargetValue: {TargetValue} CurrentValue: {CurrentValue}";
        }
    }
}
