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
    /// Controls one power axis (active or reactive) of an operating mode.
    /// </summary>
    /// <remarks>
    /// Definitions:
    /// Setpoint -> Value the operating mode aims for (supervisory setpoint, operator setpoint or available power depending on the operating mode).
    /// TargetSetpoint -> Setpoint reduced to the available power and limited by the configured limits. It is the end point of the ramp.
    /// Target -> Ramped value that is the output of this power axis.
    /// The ramp covers setpoint and available power changes during operation. Transitions caused by adding, replacing or
    /// removing stack operating modes are handled by the system operating mode control.
    /// </remarks>
    /// <typeparam name="TPower">The power type of the axis.</typeparam>
    public sealed class PowerControl<TPower> where TPower : struct
    {
        /// <summary>
        /// Lock object for thread safety between the calculation and the activation commands.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Name of the operating mode owning this power control.
        /// </summary>
        private readonly string _operatingModeName;


        /// <summary>
        /// Converter between the power type and its base unit value.
        /// </summary>
        private readonly PowerConverter<TPower> _converter;


        /// <summary>
        /// Setpoint in base unit.
        /// </summary>
        private double _setpoint;


        /// <summary>
        /// Available power in base unit.
        /// </summary>
        private double? _available;


        /// <summary>
        /// Target setpoint in base unit.
        /// </summary>
        private double _targetSetpoint;


        /// <summary>
        /// Target in base unit.
        /// </summary>
        private double _target;


        /// <summary>
        /// Setpoint of the last ramp start or restart in base unit.
        /// </summary>
        private double? _lastSetpoint;


        /// <summary>
        /// Available power of the last ramp start or restart in base unit.
        /// </summary>
        private double? _lastAvailable;


        /// <summary>
        /// Defines how the target is established after the activation.
        /// </summary>
        private PowerControlStart _start = PowerControlStart.AtTargetSetpoint;


        /// <summary>
        /// Gets the power axis of this power control.
        /// </summary>
        public PowerAxis Axis { get => _converter.Axis; }


        /// <summary>
        /// Gets the state of this power axis.
        /// </summary>
        public OperatingModeState State
        {
            get;
            private set
            {
                if (value != field)
                {
                    _logger.LogInformation("{Name} {Axis} power state changed to: {State}", _operatingModeName, Axis, value);
                    field = value;
                }
            }
        } = OperatingModeState.Disabled;


        /// <summary>
        /// Gets the setpoint the operating mode aimed for in the last calculation.
        /// </summary>
        public TPower Setpoint { get => _converter.FromValue(_setpoint); }


        /// <summary>
        /// Gets the available power of the last calculation.
        /// </summary>
        public TPower? Available { get => _available.HasValue ? _converter.FromValue(_available.Value) : null; }


        /// <summary>
        /// Gets the target setpoint which is the end point of the ramp.
        /// </summary>
        public TPower TargetSetpoint { get => _converter.FromValue(_targetSetpoint); }


        /// <summary>
        /// Gets the ramped target which is the output of this power axis.
        /// </summary>
        public TPower Target { get => _converter.FromValue(_target); }


        /// <summary>
        /// Gets the ramp controller of this power axis.
        /// </summary>
        public IRampController RampController { get; }


        /// <summary>
        /// Gets the resolved limits and deadbands of this power axis.
        /// </summary>
        public PowerControlLimits Limits { get; }



        /// <summary>
        /// Constructor of <see cref="PowerControl{TPower}"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="operatingModeName">Name of the operating mode owning this power control.</param>
        /// <param name="rampController">The ramp controller of this power axis.</param>
        /// <param name="limits">The resolved limits and deadbands.</param>
        /// <param name="converter">The power converter of this power axis.</param>
        public PowerControl(ILogger logger, string operatingModeName, IRampController rampController, PowerControlLimits limits, PowerConverter<TPower> converter)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);
            ArgumentNullException.ThrowIfNull(rampController);
            ArgumentNullException.ThrowIfNull(limits);
            ArgumentNullException.ThrowIfNull(converter);

            _logger = logger;
            _operatingModeName = operatingModeName;
            _converter = converter;
            RampController = rampController;
            Limits = limits;
        }


        /// <summary>
        /// Activates the power axis. The target is established in the next calculation once the setpoint and available power are known.
        /// </summary>
        /// <param name="start">Defines how the target is established.</param>
        internal void Activate(PowerControlStart start)
        {
            lock (_dataLock)
            {
                if (State == OperatingModeState.Enabling || State == OperatingModeState.RampingToEnabled || State == OperatingModeState.Enabled)
                {
                    return;
                }

                _start = start;
                _lastSetpoint = null;
                _lastAvailable = null;
                State = OperatingModeState.Enabling;
            }
        }


        /// <summary>
        /// Deactivates the power axis immediately and resets the target to zero.
        /// </summary>
        /// <remarks>
        /// A removed stack operating mode does not ramp itself, the system operating mode control transitions the applied target.
        /// </remarks>
        internal void Deactivate()
        {
            lock (_dataLock)
            {
                if (State == OperatingModeState.Disabled)
                {
                    return;
                }

                RampController.Stop();
                _setpoint = 0;
                _targetSetpoint = 0;
                _target = 0;
                _lastSetpoint = null;
                _lastAvailable = null;
                State = OperatingModeState.Disabled;
            }
        }


        /// <summary>
        /// Calculates the target of this power axis.
        /// </summary>
        /// <param name="setpoint">The setpoint the operating mode aims for.</param>
        /// <param name="available">The available power or null if the resources are not ready.</param>
        /// <returns>The ramped target.</returns>
        internal TPower Calculate(TPower setpoint, TPower? available)
        {
            double setpointValue = _converter.ToValue(setpoint);
            double? availableValue = available.HasValue ? _converter.ToValue(available.Value) : null;

            lock (_dataLock)
            {
                _available = availableValue;

                if (State == OperatingModeState.Enabling)
                {
                    _setpoint = setpointValue;
                    EstablishTarget(setpointValue, availableValue);
                }
                else if (State == OperatingModeState.RampingToEnabled || State == OperatingModeState.Enabled)
                {
                    _setpoint = setpointValue;
                    RestartRampOnChange(setpointValue, availableValue);
                }

                if (State == OperatingModeState.RampingToEnabled)
                {
                    _target = RampController.Calculate();
                    CheckFinalTarget();
                }

                return _converter.FromValue(_target);
            }
        }


        /// <summary>
        /// Registers the metrics of this power axis at the publisher of the owning operating mode.
        /// </summary>
        /// <param name="publisher">The metrics publisher of the owning operating mode.</param>
        /// <param name="config">The operating mode configuration containing the metrics factor classes.</param>
        internal void RegisterMetrics(IMetricsPublisher publisher, OperatingModeBaseConfig config)
        {
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(config);

            string prefix = _converter.MetricPrefix;
            // MetricsFactorClass1
            publisher.Register<PowerControl<TPower>, double>(this, $"{prefix}Target", MetricType.Gauge, x => x._target, config.MetricsFactorClass1);
            publisher.Register<PowerControl<TPower>, double>(this, $"{prefix}Available", MetricType.Gauge, x => x._available, config.MetricsFactorClass1);
            // MetricsFactorClass2
            publisher.Register<PowerControl<TPower>, int>(this, $"{prefix}State", MetricType.Gauge, x => (int)x.State, config.MetricsFactorClass2);
            publisher.Register<PowerControl<TPower>, double>(this, $"{prefix}Setpoint", MetricType.Gauge, x => x._setpoint, config.MetricsFactorClass2);
            publisher.Register<PowerControl<TPower>, double>(this, $"{prefix}TargetSetpoint", MetricType.Gauge, x => x._targetSetpoint, config.MetricsFactorClass2);
        }


        /// <summary>
        /// Establishes the target after the activation once the available power is known.
        /// </summary>
        /// <param name="setpoint">The setpoint.</param>
        /// <param name="available">The available power.</param>
        private void EstablishTarget(double setpoint, double? available)
        {
            if (available.HasValue is false)
            {
                _logger.LogDebug("{Name} {Axis} power waits for available power before establishing the target.", _operatingModeName, Axis);

                return;
            }

            _lastSetpoint = setpoint;
            _lastAvailable = available;
            double targetSetpoint = Limits.Clamp(SelectTargetSetpoint(setpoint, available.Value));

            if (_start == PowerControlStart.RampFromCurrentTarget)
            {
                StartRampTo(targetSetpoint);

                return;
            }

            _targetSetpoint = targetSetpoint;
            _target = targetSetpoint;
            State = OperatingModeState.Enabled;

            _logger.LogInformation("{Name} {Axis} power established target setpoint {TargetSetpoint}.", _operatingModeName, Axis, targetSetpoint);
        }



        /// <summary>
        /// Restarts the ramp if the setpoint or available power changed outside their deadbands and the target setpoint changed.
        /// </summary>
        /// <param name="setpoint">The setpoint.</param>
        /// <param name="available">The available power.</param>
        private void RestartRampOnChange(double setpoint, double? available)
        {
            // Keep the current ramp if the available power is temporarily unknown
            if (available.HasValue is false)
            {
                return;
            }

            bool isSetpointChanged = (_lastSetpoint.HasValue is false) || (Math.Abs(setpoint - _lastSetpoint.Value) > Limits.DeadbandSetpoint);
            bool isAvailableChanged = (_lastAvailable.HasValue is false) || (Math.Abs(available.Value - _lastAvailable.Value) > Limits.DeadbandAvailable);

            if (isSetpointChanged is false && isAvailableChanged is false)
            {
                return;
            }

            _lastSetpoint = setpoint;
            _lastAvailable = available;
            double targetSetpoint = Limits.Clamp(SelectTargetSetpoint(setpoint, available.Value));

            // E.g. available power gets bigger but the setpoint is still the smaller value
            if (targetSetpoint == _targetSetpoint)
            {
                return;
            }

            StartRampTo(targetSetpoint);
        }


        /// <summary>
        /// Starts the ramp from the current target to the target setpoint.
        /// </summary>
        /// <param name="targetSetpoint">The limited target setpoint.</param>
        private void StartRampTo(double targetSetpoint)
        {
            _targetSetpoint = targetSetpoint;
            RampController.Start(_target, targetSetpoint);
            State = OperatingModeState.RampingToEnabled;

            _logger.LogInformation("{Name} {Axis} power ramps from {Target} to target setpoint {TargetSetpoint}.", _operatingModeName, Axis, _target, targetSetpoint);
        }


        /// <summary>
        /// Sets the final target and the enabled state once the ramp is within the setpoint deadband.
        /// </summary>
        private void CheckFinalTarget()
        {
            if (State == OperatingModeState.RampingToEnabled && Math.Abs(_target - _targetSetpoint) <= Limits.DeadbandSetpoint)
            {
                _target = _targetSetpoint;
                State = OperatingModeState.Enabled;
                RampController.Stop();

                _logger.LogDebug("{Name} {Axis} power reached target setpoint: {TargetSetpoint}", _operatingModeName, Axis, _targetSetpoint);
            }
        }


        /// <summary>
        /// Selects the target setpoint from the setpoint and the available power.
        /// </summary>
        /// <param name="setpoint">The setpoint.</param>
        /// <param name="available">The available power.</param>
        /// <returns>The setpoint reduced to the magnitude of the available power so that no unachievable setpoint is set.</returns>
        private double SelectTargetSetpoint(double setpoint, double available)
        {
            return Math.Sign(setpoint) * Math.Min(Math.Abs(setpoint), Math.Abs(available));
        }
    }
}
