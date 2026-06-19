using Microsoft.Extensions.Logging;
using paskalON.Maths.Calculuses;
using paskalON.Maths.Calculuses.Coordinates;
using paskalON.Maths.IntegrationTest.Calculuses.Logarithmics;

namespace paskalON.OperatingModes.Domain.Ramps
{
    /// <summary>
    /// Ramp controller for controlling ramps according to its configurations.
    /// </summary>
    public class RampController
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<RampController> _logger;


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
        /// Gets the start value of the ramp.
        /// </summary>
        public double StartValue { get; private set; }


        /// <summary>
        /// Gets the target value of the ramp.
        /// </summary>
        public double TargetValue { get; private set; }


        /// <summary>
        /// Gets the current value of the ramp.
        /// </summary>
        public double CurrentValue { get; private set; }


        /// <summary>
        /// Get the start date of the ramp when it was started.
        /// </summary>
        public DateTimeOffset StartDate { get; private set; } = DateTimeOffset.MinValue;


        /// <summary>
        /// Constructor of <see cref="RampController"/>
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="rampBaseConfig">Ramp base configuration containing concrete ramp configuration.</param>
        public RampController(ILogger<RampController> logger, TimeProvider timeProvider, RampBaseConfig rampBaseConfig)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(rampBaseConfig);

            _logger = logger;
            _timeProvider = timeProvider;
            _rampBaseConfig = rampBaseConfig;
        }


        /// <summary>
        /// Starts the ramping.
        /// </summary>
        /// <param name="startValue">Start value of the ramp.</param>
        /// <param name="targetValue">Target value of the ramp.</param>
        public void Start(double startValue, double targetValue)
        {
            StartDate = _timeProvider.GetUtcNow();
            StartValue = startValue;
            TargetValue = targetValue;
            CurrentValue = startValue;
            Initialize();
        }


        /// <summary>
        /// Stops the ramping regardless of its current position.
        /// </summary>
        public void Stop()
        {
            StartDate = DateTimeOffset.MinValue;
        }


        /// <summary>
        /// Calculate current ramp.
        /// </summary>
        /// <returns>Current value of the ramp which is also assigned to the CurrentValue property.</returns>
        /// <exception cref="NotImplementedException">Returns exception if the ramp configuration is not implemented.</exception>
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
                    // Long to double conversion. Double can hold 5.7 x 10^308.
                    // Delay the ramp by the configured RampTimeSeconds
                    CurrentValue = _rampFunction.CalculateOutput(_timeProvider.GetUtcNow().Ticks - TimeSpan.FromSeconds(_rampBaseConfig.RampTimeSeconds).Ticks);
                }
                else
                {
                    throw new NotImplementedException($"Ramp controller for type: {_rampBaseConfig.GetType()} has not been implemented");
                }
            }

            return CurrentValue;
        }


        /// <summary>
        /// Calculate current ramp.
        /// </summary>
        /// <param name="precision">Precision of the current value.</param>
        /// <returns>Current value of the ramp which is also assigned to the CurrentValue property.</returns>
        /// <exception cref="NotImplementedException">Returns exception if the ramp configuration is not implemented.</exception>
        public double CalculatePrecision(int precision = 3)
        {
            return Math.Round(Calculate(), precision);
        }


        /// <summary>
        /// Initializes the ramp controller
        /// </summary>
        /// <exception cref="NotImplementedException">Returns exception if the ramp configuration is not implemented.</exception>
        private void Initialize()
        {
            lock (_rampBaseConfig)
            {
                if (_rampBaseConfig is RampRateConfig)
                {
                    List<LinearPoint> linearPoints = new List<LinearPoint>();
                    linearPoints.Add(new LinearPoint(StartDate.UtcTicks, StartValue));
                    double rampStepValue = StartValue;
                    int steps = 1;

                    if (StartValue <= TargetValue)
                    {
                        while (rampStepValue < TargetValue)
                        {
                            rampStepValue += ((RampRateConfig)_rampBaseConfig).RampUpRatePerSecond;
                            linearPoints.Add(new LinearPoint(StartDate.AddSeconds(steps++).UtcTicks, rampStepValue));
                        }

                        linearPoints.Last().Y = TargetValue;
                    }
                    else
                    {
                        while (rampStepValue > TargetValue)
                        {
                            rampStepValue -= ((RampRateConfig)_rampBaseConfig).RampDownRatePerSecond;
                            linearPoints.Add(new LinearPoint(StartDate.AddSeconds(steps++).UtcTicks, rampStepValue));
                        }

                        linearPoints.Last().Y = TargetValue;
                    }

                    _rampFunction = new LinearPointFunction(linearPoints);
                }
                else if (_rampBaseConfig is RampRatePercentageConfig)
                {
                    /// Current: 0, Target: 60, RampUpRatePercentPerSecond: 50 = Every second a new target 0s=0, 1s=30, 2s=45, 3s=67.5

                    List<LinearPoint> linearPoints = new List<LinearPoint>();
                    linearPoints.Add(new LinearPoint(StartDate.UtcTicks, StartValue));
                    int precision = ((RampRatePercentageConfig)_rampBaseConfig).RampRatePrecision;
                    int steps = 1;

                    if (StartValue <= TargetValue)
                    {
                        double rampRate = ((RampRatePercentageConfig)_rampBaseConfig).RampUpRatePercentPerSecond;
                        double rampStepValue = TargetValue / 100 * rampRate;
                        linearPoints.Add(new LinearPoint(StartDate.AddSeconds(steps++).UtcTicks, rampStepValue));

                        while (Math.Round(rampStepValue, precision) < TargetValue)
                        {
                            rampStepValue = rampStepValue + (rampStepValue / 100 * rampRate);
                            linearPoints.Add(new LinearPoint(StartDate.AddSeconds(steps++).UtcTicks, rampStepValue));
                        }

                        linearPoints.Last().Y = TargetValue;
                    }
                    else
                    {
                        double rampRate = ((RampRatePercentageConfig)_rampBaseConfig).RampDownRatePercentPerSecond;
                        double rampStepValue = StartValue / 100 * rampRate;
                        linearPoints.Add(new LinearPoint(StartDate.AddSeconds(steps++).UtcTicks, rampStepValue));

                        while (Math.Round(rampStepValue, precision) > TargetValue)
                        {
                            rampStepValue = rampStepValue - (rampStepValue / 100 * rampRate);
                            linearPoints.Add(new LinearPoint(StartDate.AddSeconds(steps++).UtcTicks, rampStepValue));
                        }

                        linearPoints.Last().Y = TargetValue;
                    }

                    _rampFunction = new LinearPointFunction(linearPoints);
                }
                else if (_rampBaseConfig is RampTimeConfig)
                {
                    List<LinearPoint> linearPoints = new List<LinearPoint>();
                    linearPoints.Add(new LinearPoint(StartDate.UtcTicks, StartValue));

                    if (StartValue <= TargetValue)
                    {
                        linearPoints.Add(new LinearPoint(StartDate.AddSeconds(((RampTimeConfig)_rampBaseConfig).RampUpTimeSeconds).UtcTicks, TargetValue));
                    }
                    else
                    {
                        linearPoints.Add(new LinearPoint(StartDate.AddSeconds(((RampTimeConfig)_rampBaseConfig).RampDownTimeSeconds).UtcTicks, TargetValue));
                    }

                    _rampFunction = new LinearPointFunction(linearPoints);
                }
                else if (_rampBaseConfig is RampTimeConstantConfig)
                {
                    if (StartValue <= TargetValue)
                    {
                        long period = TimeSpan.FromSeconds(((RampTimeConstantConfig)_rampBaseConfig).RampUpTimeConstantSeconds).Ticks;
                        _rampFunction = new LogarithmicEasingFunction(StartValue, TargetValue, period, StartDate.UtcTicks, ((RampTimeConstantConfig)_rampBaseConfig).TuningValue);
                    }
                    else
                    {
                        long period = TimeSpan.FromSeconds(((RampTimeConstantConfig)_rampBaseConfig).RampDownTimeConstantSeconds).Ticks;
                        _rampFunction = new LogarithmicEasingFunction(StartValue, TargetValue, period, StartDate.UtcTicks, ((RampTimeConstantConfig)_rampBaseConfig).TuningValue);
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
