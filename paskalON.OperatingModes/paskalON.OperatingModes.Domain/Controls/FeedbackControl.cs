// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Closed loop feedback correction of one power axis.
    /// </summary>
    /// <remarks>
    /// The feedback control is independent of the controlled quantity (active power, reactive power, voltage, current, power factor, ...).
    /// The measured value is read from the operating mode map via the measured source bound in the constructor of the operating mode.
    /// Reference, measured value and error are in the unit of the controlled quantity, the error adjustment is in power units.
    /// </remarks>
    /// <typeparam name="TPower">The power type of the axis.</typeparam>
    public sealed class FeedbackControl<TPower> where TPower : struct
    {
        /// <summary>
        /// Corrections closer than this value in the base unit to the correction target are reached.
        /// </summary>
        private const double CorrectionTolerance = 3;


        /// <summary>
        /// Lock object for thread safety between the calculation and reset.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Name of the operating mode owning this feedback control.
        /// </summary>
        private readonly string _operatingModeName;


        /// <summary>
        /// Source of the measured value of the controlled quantity bound to the operating mode map.
        /// </summary>
        private readonly Func<double?> _measuredSource;


        /// <summary>
        /// Converter between the power type and its base unit value.
        /// </summary>
        private readonly PowerConverter<TPower> _converter;


        /// <summary>
        /// Limits used to limit the corrected value.
        /// </summary>
        private readonly PowerControlLimits _limits;


        /// <summary>
        /// Measured value of the last calculation used to detect a stuck measurement.
        /// </summary>
        private double? _lastMeasured;


        /// <summary>
        /// Error adjustment in base unit.
        /// </summary>
        private double _errorAdjustment;


        /// <summary>
        /// Corrected value in base unit.
        /// </summary>
        private double _corrected;


        /// <summary>
        /// Ramp controller moving the error adjustment for <see cref="FeedbackCorrection.RampLimitedIntegral"/>, otherwise null.
        /// </summary>
        private readonly IRampController? _rampController;


        /// <summary>
        /// Error adjustment the ramp moves to for <see cref="FeedbackCorrection.RampLimitedIntegral"/> in base unit.
        /// </summary>
        private double _correctionTarget;


        /// <summary>
        /// Gets the power axis of this feedback control.
        /// </summary>
        public PowerAxis Axis { get => _converter.Axis; }


        /// <summary>
        /// Gets the name of the controlled quantity (e.g. ActivePower, Voltage, PowerFactor, Current, StateOfCharge).
        /// </summary>
        public string Quantity { get; }


        /// <summary>
        /// Gets the feedback settings.
        /// </summary>
        public FeedbackSettings Settings { get; }


        /// <summary>
        /// Gets the reference of the last calculation in the unit of the controlled quantity.
        /// </summary>
        public double Reference { get; private set; }


        /// <summary>
        /// Gets the measured value of the last calculation in the unit of the controlled quantity.
        /// </summary>
        public double? Measured { get; private set; }


        /// <summary>
        /// Gets the error (reference - measured) of the last calculation in the unit of the controlled quantity.
        /// </summary>
        public double Error { get; private set; }


        /// <summary>
        /// Gets the error adjustment in power units.
        /// </summary>
        public TPower ErrorAdjustment { get => _converter.FromValue(_errorAdjustment); }


        /// <summary>
        /// Gets the corrected value (target + error adjustment, limited) that the operating mode writes into the signals.
        /// </summary>
        public TPower Corrected { get => _converter.FromValue(_corrected); }



        /// <summary>
        /// Constructor of <see cref="FeedbackControl{TPower}"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="operatingModeName">Name of the operating mode owning this feedback control.</param>
        /// <param name="quantity">Name of the controlled quantity.</param>
        /// <param name="measuredSource">Source of the measured value bound to the operating mode map.</param>
        /// <param name="settings">The feedback settings.</param>
        /// <param name="limits">The limits used to limit the corrected value.</param>
        /// <param name="converter">The power converter of this power axis.</param>
        /// <param name="rampController">Own ramp controller of the operating mode, required for <see cref="FeedbackCorrection.RampLimitedIntegral"/>.</param>
        public FeedbackControl(ILogger logger, string operatingModeName, string quantity, Func<double?> measuredSource, FeedbackSettings settings,
            PowerControlLimits limits, PowerConverter<TPower> converter, IRampController? rampController = null)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);
            ArgumentException.ThrowIfNullOrWhiteSpace(quantity);
            ArgumentNullException.ThrowIfNull(measuredSource);
            ArgumentNullException.ThrowIfNull(settings);
            ArgumentNullException.ThrowIfNull(limits);
            ArgumentNullException.ThrowIfNull(converter);

            if (settings.Correction == FeedbackCorrection.RampLimitedIntegral)
            {
                ArgumentNullException.ThrowIfNull(rampController);
            }

            _logger = logger;
            _operatingModeName = operatingModeName;
            _measuredSource = measuredSource;
            _limits = limits;
            _converter = converter;
            _rampController = rampController;
            Quantity = quantity;
            Settings = settings;
        }


        /// <summary>
        /// Corrects the target with the feedback error.
        /// </summary>
        /// <param name="target">The target of the power axis (uncorrected output from PowerControl).</param>
        /// <param name="reference">The reference in the unit of the controlled quantity (what the loop aims for).</param>
        /// <returns>The corrected and limited value.</returns>
        /// <remarks>
        /// Same quantity (ActivePowerMode / ReactivePowerMode): the loop regulates P (or Q) at the POI towards the mode's own target.
        /// It calls Correct(target, target.Watts). The error is target minus measured P at POI with a gain of 0.25 (default) the mode compensates part of the losses.
        /// Not the same quantity (e.g. Voltage): reference would be the voltage setpoint, e.g. 20 000 V, the measured value the voltage at the POI from its Map.
        /// The gain would be in VAr per V, e.g. 500 VAr/V. target would be the mode's base Q contribution (possibly 0), and the correction moves Q to pull the voltage towards the reference.
        /// </remarks>
        internal TPower Correct(TPower target, double reference)
        {
            double? measured = _measuredSource.Invoke();

            lock (_dataLock)
            {
                Reference = reference;
                Measured = measured;

                // Without a measurement there is nothing to correct
                if (measured.HasValue is false)
                {
                    Error = 0;
                    _errorAdjustment = 0;
                    _correctionTarget = 0;
                    _rampController?.Stop();
                    _lastMeasured = null;
                }
                else
                {
                    Error = reference - measured.Value;

                    bool isMeasurementChanged = _lastMeasured.HasValue is false || _lastMeasured.Value != measured.Value;

                    if (Settings.Correction == FeedbackCorrection.RampLimitedIntegral)
                    {
                        CorrectRampLimitedIntegral(_converter.ToValue(target), isMeasurementChanged);
                    }
                    // Don't try to fix minor noise
                    else if (Math.Abs(Error) < Settings.DeadbandError)
                    {
                        _errorAdjustment = 0;
                    }
                    // Do not change the adjustment if the measurement is stuck somehow
                    else if (isMeasurementChanged)
                    {
                        _errorAdjustment = Error * Settings.ProportionalGain;
                    }

                    _lastMeasured = measured;
                }

                _corrected = _limits.Clamp(_converter.ToValue(target) + _errorAdjustment);

                return _converter.FromValue(_corrected);
            }
        }


        /// <summary>
        /// Resets the feedback correction.
        /// </summary>
        internal void Reset()
        {
            lock (_dataLock)
            {
                _errorAdjustment = 0;
                _correctionTarget = 0;
                _rampController?.Stop();
                _lastMeasured = null;
                Error = 0;

                _logger.LogDebug("{Name} {Axis} feedback {Quantity} reset.", _operatingModeName, Axis, Quantity);
            }
        }


        /// <summary>
        /// Calculates the error adjustment of the ramp-limited integral correction.
        /// </summary>
        /// <remarks>
        /// Once the previous correction is reached and the measurement changed, an error outside the deadband is accumulated into
        /// the correction target (previous correction + error). The error adjustment moves to the correction target with the ramp.
        /// Waiting for the previous correction and a new measurement avoids accumulating the same error twice.
        /// </remarks>
        /// <param name="target">The target of the power axis in base unit.</param>
        /// <param name="isMeasurementChanged">Whether the measurement changed since the last calculation.</param>
        private void CorrectRampLimitedIntegral(double target, bool isMeasurementChanged)
        {
            bool isCorrectionReached = Math.Abs(_errorAdjustment - _correctionTarget) < CorrectionTolerance;

            if (isCorrectionReached && isMeasurementChanged && Math.Abs(Error) >= Settings.DeadbandError)
            {
                // The correction target is limited so that the corrected value stays within the limits
                _correctionTarget = _limits.Clamp(target + _errorAdjustment + Error) - target;
                _rampController!.Start(_errorAdjustment, _correctionTarget);
                isCorrectionReached = Math.Abs(_errorAdjustment - _correctionTarget) < CorrectionTolerance;
            }

            if (isCorrectionReached)
            {
                return;
            }

            _errorAdjustment = _rampController!.Calculate();

            if (Math.Abs(_errorAdjustment - _correctionTarget) < CorrectionTolerance)
            {
                _errorAdjustment = _correctionTarget;
                _rampController.Stop();
            }
        }


        /// <summary>
        /// Registers the metrics of this feedback control at the publisher of the owning operating mode.
        /// </summary>
        /// <param name="publisher">The metrics publisher of the owning operating mode.</param>
        /// <param name="config">The operating mode configuration containing the metrics factor classes.</param>
        internal void RegisterMetrics(IMetricsPublisher publisher, OperatingModeBaseConfig config)
        {
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(config);

            string prefix = _converter.MetricPrefix;
            // MetricsFactorClass1
            publisher.Register<FeedbackControl<TPower>, double>(this, $"{prefix}ErrorAdjustment", MetricType.Gauge, x => x._errorAdjustment, config.MetricsFactorClass1);
            publisher.Register<FeedbackControl<TPower>, double>(this, $"{prefix}Corrected", MetricType.Gauge, x => x._corrected, config.MetricsFactorClass1);
            publisher.Register<FeedbackControl<TPower>, double>(this, $"{prefix}FeedbackMeasured", MetricType.Gauge, x => x.Measured, config.MetricsFactorClass1);
            publisher.Register<FeedbackControl<TPower>, double>(this, $"{prefix}FeedbackError", MetricType.Gauge, x => x.Error, config.MetricsFactorClass1);
            publisher.Register<FeedbackControl<TPower>, double>(this, $"{prefix}FeedbackReference", MetricType.Gauge, x => x.Reference, config.MetricsFactorClass1);
        }
    }
}
