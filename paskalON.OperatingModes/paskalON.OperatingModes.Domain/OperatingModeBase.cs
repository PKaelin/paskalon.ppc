// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Base class for all operating modes that defines the specific behavior and control strategy
    /// the system uses to interact with the power grid.
    /// </summary>
    /// <remarks>    
    /// Definitions:
    /// Setpoint -> Value set by an external process requesting to hit that setpoint within a definition.
    /// TargetSetpoint -> Value that is either the setpoint or available power.
    /// Target -> Value that is the current target considering ramp, curve, feedback calculations.
    /// </remarks>
    public abstract class OperatingModeBase : IOperatingMode
    {
        /// <summary>
        /// Interface for registering and publishing metrics for a given type T.
        /// </summary>
        /// <remarks>
        /// <see cref="IMetricsPublisher"/> should only publish metrics if this operating state is not disabled.
        /// </remarks>
        public IMetricsPublisher MetricsPublisher { get; init; }


        /// <summary>
        /// Feedback map for the operating mode.
        /// </summary>
        private readonly OperatingModeBaseMap _map;


        /// <summary>
        /// Last active power available change after the available power is outside deadband.
        /// </summary>
        protected ActivePower? _lastAvailableActive;


        /// <summary>
        /// Last reactive power available change after the available power is outside deadband.
        /// </summary>
        protected ReactivePower? _lastAvailableReactive;


        /// <summary>
        /// Last active power setpoint change after the setpoint is outside deadband.
        /// </summary>
        protected ActivePower? _lastSetpointActive;


        /// <summary>
        /// Last reactive power setpoint change after the setpoint is outside deadband.
        /// </summary>
        protected ReactivePower? _lastSetpointReactive;


        /// <summary>
        /// Active power target for the operating mode.
        /// </summary>
        /// <remarks>
        /// For performance this is a class variable.
        /// </remarks>
        protected ActivePower _targetActivePower = new ActivePower(0);


        /// <summary>
        /// Reactive power target for the operating mode.
        /// </summary>
        /// <remarks>
        /// For performance this is a class variable.
        /// </remarks>
        protected ReactivePower _targetReactivePower = new ReactivePower(0);


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// Time provider for system time abstraction.
        /// </summary>
        protected readonly TimeProvider _timeProvider;


        /// <summary>
        /// Operating mode base configuration.
        /// </summary>
        private readonly OperatingModeBaseConfig _config;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Name { get => _config.Name; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsEnabled
        {
            get;
            private set;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTimeOffset LastEnabled { get; protected set; } = DateTimeOffset.MinValue;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        public OperatingModeState State
        {
            get
            {
                if (Matches(StateActive, StateReactive, OperatingModeState.Enabled))
                    return OperatingModeState.Enabled;

                if (Matches(StateActive, StateReactive, OperatingModeState.Enabling))
                    return OperatingModeState.Enabling;

                if (Matches(StateActive, StateReactive, OperatingModeState.RampingToEnabled))
                    return OperatingModeState.RampingToEnabled;

                if (Matches(StateActive, StateReactive, OperatingModeState.RampingToDisabled))
                    return OperatingModeState.RampingToDisabled;

                if (Matches(StateActive, StateReactive, OperatingModeState.Disabling))
                    return OperatingModeState.Disabling;

                return OperatingModeState.Disabled;
            }
        }


        /// <summary>
        /// The active power operating mode state.
        /// </summary>
        /// <remarks>
        /// An operating mode can have active and reactive control and they can operate differently (ramps).
        /// The base class implements both but the modes have to be activated in the subclasses with the following:
        /// StateActive = OperatingModeState.Disabled;
        /// </remarks>
        public OperatingModeState? StateActive
        {
            get;
            protected set
            {
                if (value != field)
                {
                    _logger.LogInformation("{Name} OperatingModeStateActive changed to: {State}", Name, value);
                    field = value;
                }
            }
        }


        /// <summary>
        /// The reactive power operating mode state.
        /// </summary>
        /// <remarks>
        /// An operating mode can have active and reactive control and they can operate differently (ramps).
        /// The base class implements both but the modes have to be activated in the subclasses with the following:
        /// StateReactive = OperatingModeState.Disabled;
        /// </remarks>
        public OperatingModeState? StateReactive
        {
            get;
            protected set
            {
                if (value != field)
                {
                    _logger.LogInformation("{Name} OperatingModeStateReactive changed to: {State}", Name, value);
                    field = value;
                }
            }
        }



        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower SetpointActivePower
        {
            get { lock (dataLock) { return field; } }
            set
            {
                lock (dataLock)
                {
                    if (value.KiloWatts < SystemConfig.NameplateMinimumActivePowerKiloWatt || value.KiloWatts > SystemConfig.NameplateMaximumActivePowerKiloWatt)
                    {
                        _logger.LogError("{Name} SetpointActivePower is outside the defined nameplates. Min: {NameplateMinimumActivePowerKiloWatt} Max: {NameplateMaximumActivePowerKiloWatt}", Name,
                            SystemConfig.NameplateMinimumActivePowerKiloWatt, SystemConfig.NameplateMaximumActivePowerKiloWatt);
                        throw new InvalidOperationException($"{Name} SetpointActivePower is outside the defined nameplates.");
                    }

                    if (value.Watts != field.Watts)
                    {
                        field = value;
                        _logger.LogInformation("{Name} SetpointActivePower changed to: {SetpointActivePower}", Name, value.KiloWatts);
                    }
                }
            }
        } = new ActivePower(0);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower? AvailableActivePower { get => _map.AvailableActivePower.Invoke(); }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower TargetActivePower { get => _targetActivePower; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower SetpointReactivePower
        {
            get { lock (dataLock) { return field; } }
            set
            {
                lock (dataLock)
                {
                    if (value.KiloVoltAmperesReactive < SystemConfig.NameplateMinimumReactivePowerKiloVars || value.KiloVoltAmperesReactive > SystemConfig.NameplateMaximumReactivePowerKiloVars)
                    {
                        _logger.LogError("{Name} SetpointReactivePower is outside the defined nameplates. Min: {NameplateMinimumReactivePowerKiloVars} Max: {NameplateMaximumReactivePowerKiloVars}", Name,
                            SystemConfig.NameplateMinimumReactivePowerKiloVars, SystemConfig.NameplateMaximumReactivePowerKiloVars);
                        throw new InvalidOperationException($"{Name} SetpointReactivePower is outside the defined nameplates.");
                    }

                    if (value.VoltAmperesReactive != field.VoltAmperesReactive)
                    {
                        field = value;
                        _logger.LogInformation("{Name} SetpointReactivePower changed to: {SetpointReactivePower}", Name, value.KiloVoltAmperesReactive);
                    }
                }
            }
        } = new ReactivePower(0);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower? AvailableReactivePower { get => _map.AvailableReactivePower.Invoke(); }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower TargetReactivePower { get => _targetReactivePower; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IRampController RampControllerActive { get; protected set; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IRampController RampControllerReactive { get; protected set; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICurveController? CurveController { get; protected set; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SystemConfig SystemConfig { get; init; }



        /// <summary>
        /// Constructor of <see cref="OperatingModeBase"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode base configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public OperatingModeBase(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, OperatingModeBaseConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController = null)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(systemConfig);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(rampController);

            _logger = logger;
            _timeProvider = timeProvider;
            _config = config;
            _map = map;
            MetricsPublisher = publisher;
            SystemConfig = systemConfig;
            RampControllerActive = rampController;
            RampControllerReactive = rampController.ShallowCopy();
            CurveController = curveController;
            _logger.LogInformation("{Name} operating mode created.", Name);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Enable()
        {
            IsEnabled = true;
            _logger.LogInformation("{Name} operating mode enabled.", Name);

            if (StateActive.HasValue && StateActive != OperatingModeState.Enabled)
            {
                StateActive = OperatingModeState.Enabling;
                double setpointActive = GetActivePowerTargetSetpoint();
                // Only start if there is available and if there is a setpoint that is not 0
                if (setpointActive != 0)
                {
                    _logger.LogInformation("{Name} operating mode enabled. Active Target-Setpoint: {ActiveTargetSetpoint}.", Name, setpointActive);
                    RampControllerActive.Start(TargetActivePower.KiloWatts, setpointActive);
                    StateActive = OperatingModeState.RampingToEnabled;
                }
            }

            if (StateReactive.HasValue && StateReactive != OperatingModeState.Enabled)
            {
                StateReactive = OperatingModeState.Enabling;
                double setpointReactive = GetReactivePowerTargetSetpoint();
                // Only start if there is available and if there is a setpoint that is not 0
                if (setpointReactive != 0)
                {
                    _logger.LogInformation("{Name} operating mode enabled. Reactive Target-Setpoint: {ReactiveTargetSetpoint}.", Name, setpointReactive);
                    RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, setpointReactive);
                    StateReactive = OperatingModeState.RampingToEnabled;
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Disable()
        {
            IsEnabled = false;
            _logger.LogInformation("{Name} operating mode disabled.", Name);

            if (StateActive.HasValue && StateActive != OperatingModeState.Disabled)
            {
                _logger.LogInformation("{Name} operating mode disabled. Active Target-Setpoint: {ActiveTargetSetpoint}.", Name, 0);
                SetpointActivePower = new ActivePower(0);
                StateActive = OperatingModeState.RampingToDisabled;
                RampControllerActive.Start(TargetActivePower.KiloWatts, 0);
            }

            if (StateReactive.HasValue && StateReactive != OperatingModeState.Disabled)
            {
                _logger.LogInformation("{Name} operating mode disabled. Reactive Target-Setpoint: {ReactiveTargetSetpoint}.", Name, 0);
                SetpointReactivePower = new ReactivePower(0);
                StateReactive = OperatingModeState.RampingToDisabled;
                RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, 0);
            }
        }


        /// <summary>
        /// Get the active power target setpoint using setpoint and available power.
        /// </summary>
        /// <returns>Available power if setpoint is higher otherwise setpoint.</returns>        
        protected double GetActivePowerTargetSetpoint()
        {
            double targetSetpoint = 0;
            ActivePower? available = AvailableActivePower;
            // Only change initial 0 setpoint when there is available power
            if (available != null)
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (Math.Abs(available.Value.KiloWatts) <= Math.Abs(SetpointActivePower.KiloWatts))
                {
                    targetSetpoint = available.Value.KiloWatts;
                }
                // Available is more then setpoint use setpoint that might or might not be 0
                else
                {
                    targetSetpoint = SetpointActivePower.KiloWatts;
                }

                // Set last to the initial values
                _lastAvailableActive = available;
                _lastSetpointActive = SetpointActivePower;
            }
            else
            {
                // Clear last values
                _lastAvailableActive = null;
                _lastSetpointActive = null;
            }

            return targetSetpoint;
        }


        /// <summary>
        /// Get the reactive power target setpoint using setpoint and available power.
        /// </summary>
        /// <returns>Available power if setpoint is higher otherwise setpoint.</returns>
        protected double GetReactivePowerTargetSetpoint()
        {
            double targetSetpoint = 0;
            ReactivePower? available = AvailableReactivePower;
            // Only change initial 0 setpoint when there is available power
            if (available != null)
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (Math.Abs(available.Value.KiloVoltAmperesReactive) <= Math.Abs(SetpointReactivePower.KiloVoltAmperesReactive))
                {
                    targetSetpoint = available.Value.KiloVoltAmperesReactive;
                }
                // Available is more then setpoint use setpoint that might or might not be 0
                else
                {
                    targetSetpoint = SetpointReactivePower.KiloVoltAmperesReactive;
                }

                // Set last to the initial values
                _lastAvailableReactive = available;
                _lastSetpointReactive = SetpointReactivePower;
            }
            else
            {
                // Clear last values
                _lastAvailableReactive = null;
                _lastSetpointReactive = null;
            }

            return targetSetpoint;
        }


        /// <summary>
        /// Checks whether there needs to be a new target calculation due to a new setpoint or new available change.
        /// </summary>
        /// <param name="available">The current available power.</param>
        /// <returns>Returns the new target if there has been a change otherwise null.</returns>
        protected double? CheckNewActiveTargetSetpoint(ActivePower? available)
        {
            double? targetSetpoint = null;
            // If available is outside deadband of lastAvailable or if setpoint is outside deadband of lastSetpoint
            if (StateActive.HasValue && StateActive != OperatingModeState.RampingToDisabled && (available != null && _lastAvailableActive != null && _lastSetpointActive != null) &&
                  ((Math.Abs(available.Value.KiloWatts) > (Math.Abs(_lastAvailableActive.Value.KiloWatts) + _config.DeadbandAvailableKilo)) ||
                  (Math.Abs(SetpointActivePower.KiloWatts) > (Math.Abs(_lastSetpointActive.Value.KiloWatts) + _config.DeadbandSetpointKilo))))
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (Math.Abs(available.Value.KiloWatts) <= Math.Abs(SetpointActivePower.KiloWatts))
                {
                    targetSetpoint = available.Value.KiloWatts;
                    _logger.LogDebug("{Name} new available for active power", Name);
                }
                // Available is more then setpoint use setpoint
                else
                {
                    targetSetpoint = SetpointActivePower.KiloWatts;
                    _logger.LogDebug("{Name} new setpoint for active power", Name);
                }

                if (targetSetpoint != null)
                {
                    // Restart/Start when setpoint wasn't set before or setpoint was 0 before and now it is not 0 anymore
                    // Don't restart when if available gets bigger but still bigger than setpoint and setpoint hasn't changed
                    if (targetSetpoint.Value != 0 && (_lastSetpointActive.HasValue == false || _lastSetpointActive.Value.Watts == 0) &&
                       (targetSetpoint != _lastSetpointActive?.KiloWatts))
                    {
                        if (StateActive == OperatingModeState.Enabling)
                        {
                            // In case things have changed after the Enable() command
                            StateActive = OperatingModeState.RampingToEnabled;
                        }
                    }
                    else
                    {
                        // Target hasn't changed so don't return a value
                        targetSetpoint = null;
                    }

                    // Only now update the last available and the last setpoint
                    _lastAvailableActive = available;
                    _lastSetpointActive = SetpointActivePower;
                }
            }

            return targetSetpoint;
        }


        /// <summary>
        /// Checks whether the target is equal or within the deadband of the setpoint.
        /// Sets the final target and the operating state to enabled/disabled if its within.
        /// </summary>
        protected void CheckFinalActiveTarget()
        {
            if (StateActive.HasValue && StateActive != OperatingModeState.Enabled && StateActive != OperatingModeState.Disabled)
            {
                // Set final target and change state to enabled if we are within a deadband
                if (Math.Abs(TargetActivePower.KiloWatts) > (Math.Abs(SetpointActivePower.KiloWatts) - _config.DeadbandSetpointKilo))
                {
                    if (StateActive == OperatingModeState.RampingToEnabled)
                    {
                        // Once within deadband we set the actual the precise target regardless available and set state to enabled 
                        _targetActivePower.KiloWatts = SetpointActivePower.KiloWatts;
                        StateActive = OperatingModeState.Enabled;
                    }
                    else if (StateActive == OperatingModeState.RampingToDisabled)
                    {
                        // Once within deadband we set the actual the precise target regardless available and set state to disabled
                        _targetActivePower.KiloWatts = SetpointActivePower.KiloWatts;
                        StateActive = OperatingModeState.Disabled;
                        RampControllerActive.Stop();
                    }

                    _logger.LogDebug("{Name} final target reached setpoint for active power: {ActiveTargetSetpoint}", Name, _targetActivePower.KiloWatts);
                }
            }
        }


        /// <summary>
        /// Checks whether there needs to be a new target calculation due to a new setpoint or new available change.
        /// </summary>
        /// <param name="available">The current available power.</param>
        /// <returns>Returns the new target if there has been a change otherwise null.</returns>
        protected double? CheckNewReactiveTargetSetpoint(ReactivePower? available)
        {
            double? targetSetpoint = null;
            // If available is outside deadband of lastAvailable or if setpoint is outside deadband of lastSetpoint
            if (StateReactive.HasValue && StateReactive != OperatingModeState.RampingToDisabled && (available != null && _lastAvailableReactive != null && _lastSetpointReactive != null) &&
                  ((Math.Abs(available.Value.KiloVoltAmperesReactive) > (Math.Abs(_lastAvailableReactive.Value.KiloVoltAmperesReactive) + _config.DeadbandAvailableKilo)) ||
                  (Math.Abs(SetpointReactivePower.KiloVoltAmperesReactive) > (Math.Abs(_lastSetpointReactive.Value.KiloVoltAmperesReactive) + _config.DeadbandSetpointKilo))))
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (Math.Abs(available.Value.KiloVoltAmperesReactive) <= Math.Abs(SetpointReactivePower.KiloVoltAmperesReactive))
                {
                    targetSetpoint = available.Value.KiloVoltAmperesReactive;
                    _logger.LogDebug("{Name} new available for reactive power.", Name);
                }
                // Available is more then setpoint use setpoint
                else
                {
                    targetSetpoint = SetpointReactivePower.KiloVoltAmperesReactive;
                    _logger.LogDebug("{Name} new setpoint for reactive power.", Name);
                }

                if (targetSetpoint != null)
                {
                    // Restart/Start when setpoint wasn't set before or setpoint was 0 before and now it is not 0 anymore
                    // Don't restart when if available gets bigger but still bigger than setpoint and setpoint hasn't changed
                    if (targetSetpoint.Value != 0 && (_lastSetpointReactive.HasValue == false || _lastSetpointReactive.Value.VoltAmperesReactive == 0) &&
                       (targetSetpoint != _lastSetpointReactive?.KiloVoltAmperesReactive))
                    {
                        if (StateReactive == OperatingModeState.Enabling)
                        {
                            // In case things have changed after the Enable() command
                            StateReactive = OperatingModeState.RampingToEnabled;
                        }
                    }
                    else
                    {
                        // Target hasn't changed so don't return a value as there is no need to start/restart ramp
                        targetSetpoint = null;
                    }

                    // Only now update the last available and the last setpoint
                    _lastAvailableReactive = available;
                    _lastSetpointReactive = SetpointReactivePower;
                }
            }

            return targetSetpoint;
        }



        /// <summary>
        /// Checks whether the target is equal or within the deadband of the setpoint.
        /// Sets the final target and the operating state to enabled/disabled if its within.
        /// </summary>
        protected void CheckFinalReactiveTarget()
        {
            if (StateReactive.HasValue && StateReactive != OperatingModeState.Enabled && StateReactive != OperatingModeState.Disabled)
            {
                // Set final target and change state to enabled if we are within a deadband
                if (Math.Abs(TargetReactivePower.KiloVoltAmperesReactive) > (Math.Abs(SetpointReactivePower.KiloVoltAmperesReactive) - _config.DeadbandSetpointKilo))
                {
                    if (StateReactive == OperatingModeState.RampingToEnabled)
                    {
                        // Once within deadband we set the actual the precise target regardless available and set state to enabled 
                        _targetReactivePower.KiloVoltAmperesReactive = SetpointReactivePower.KiloVoltAmperesReactive;
                        StateReactive = OperatingModeState.Enabled;
                    }
                    else if (StateReactive == OperatingModeState.RampingToDisabled)
                    {
                        // Once within deadband we set the actual the precise target regardless available and set state to disabled
                        _targetReactivePower.KiloVoltAmperesReactive = SetpointReactivePower.KiloVoltAmperesReactive;
                        StateReactive = OperatingModeState.Disabled;
                        RampControllerReactive.Stop();
                    }

                    _logger.LogDebug("{Name} final target reached setpoint for reactive power: {ReactiveTargetSetpoint}", Name, _targetReactivePower.KiloVoltAmperesReactive);
                }
            }
        }


        /// <summary>
        /// Apply configured active power limits to targets if configured.
        /// </summary>
        /// <param name="targetSetpoint">The intended target.</param>
        /// <returns>Target or limited target.</returns>
        protected double ApplyActiveLimits(double targetSetpoint)
        {
            // Careful this is intentional. Local operating mode configuration can overwrite the nameplate
            if ((_config.MaximumActivePowerLimitKiloWatt.HasValue == true) && (targetSetpoint > _config.MaximumActivePowerLimitKiloWatt.Value))
            {
                _logger.LogWarning("{Name} operating mode limited due MaximumActivePowerLimitKiloWatt configuration. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = _config.MaximumActivePowerLimitKiloWatt.Value;
            }
            else if ((_config.MaximumActivePowerLimitKiloWatt.HasValue == false) && (targetSetpoint > SystemConfig.NameplateMaximumActivePowerKiloWatt))
            {
                _logger.LogWarning("{Name} operating mode limited due NameplateMaximumActivePowerKiloWatt configuration. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = SystemConfig.NameplateMaximumActivePowerKiloWatt;
            }
            else if ((_config.MinimumActivePowerLimitKiloWatt.HasValue == true) && (targetSetpoint < _config.MinimumActivePowerLimitKiloWatt.Value))
            {
                _logger.LogWarning("{Name} operating mode limited due MinimumActivePowerLimitKiloWatt configuration. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = _config.MinimumActivePowerLimitKiloWatt.Value;
            }
            else if ((_config.MinimumActivePowerLimitKiloWatt.HasValue == false) && (targetSetpoint < SystemConfig.NameplateMinimumActivePowerKiloWatt))
            {
                _logger.LogWarning("{Name} operating mode limited due NameplateMinimumActivePowerKiloWatt configuration. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = SystemConfig.NameplateMinimumActivePowerKiloWatt;
            }

            return targetSetpoint;
        }


        /// <summary>
        /// Apply configured reactive power limits to targets if configured.
        /// </summary>
        /// <param name="targetSetpoint">The intended target.</param>
        /// <returns>Target or limited target.</returns>
        protected double ApplyReactiveLimits(double targetSetpoint)
        {
            // Careful this is intentional. Local operating mode configuration can overwrite the nameplate
            if ((_config.MaximumReactivePowerLimitKiloVars.HasValue == true) && (targetSetpoint > _config.MaximumReactivePowerLimitKiloVars.Value))
            {
                _logger.LogInformation("{Name} operating mode limited due MaximumReactivePowerLimitKiloVars configuration. Reactive Target-Setpoint set to {ReactiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = _config.MaximumReactivePowerLimitKiloVars.Value;
            }
            else if ((_config.MaximumReactivePowerLimitKiloVars.HasValue == false) && (targetSetpoint > SystemConfig.NameplateMaximumReactivePowerKiloVars))
            {
                _logger.LogWarning("{Name} operating mode limited due NameplateMaximumReactivePowerKiloVars configuration. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = SystemConfig.NameplateMaximumReactivePowerKiloVars;
            }
            else if ((_config.MinimumReactivePowerLimitKiloVars.HasValue == true) && (targetSetpoint < _config.MinimumReactivePowerLimitKiloVars.Value))
            {
                _logger.LogInformation("{Name} operating mode limited due MinimumReactivePowerLimitKiloVars configuration. Reactive Target-Setpoint set to {ReactiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = _config.MinimumReactivePowerLimitKiloVars.Value;

            }
            else if ((_config.MinimumReactivePowerLimitKiloVars.HasValue == false) && (targetSetpoint < SystemConfig.NameplateMinimumReactivePowerKiloVars))
            {
                _logger.LogWarning("{Name} operating mode limited due NameplateMinimumReactivePowerKiloVars configuration. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, targetSetpoint);
                targetSetpoint = SystemConfig.NameplateMinimumReactivePowerKiloVars;
            }

            return targetSetpoint;
        }


        /// <summary>
        /// Register metrics at the publisher.
        /// </summary>
        protected virtual void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>
            {
                { "Name", Name }
            };

            // Initialize metrics
            MetricsPublisher.Initialize("OperatingMode", tags);
            // MetricsFactorClass1
            MetricsPublisher.Register<OperatingModeBase, double>(this, nameof(TargetActivePower), MetricType.Gauge, x => x.TargetActivePower.KiloWatts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<OperatingModeBase, double>(this, nameof(TargetReactivePower), MetricType.Gauge, x => x.TargetReactivePower.KiloVoltAmperesReactive, _config.MetricsFactorClass1);
            MetricsPublisher.Register<OperatingModeBase, double>(this, nameof(AvailableActivePower), MetricType.Gauge, x => x.AvailableActivePower?.KiloWatts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<OperatingModeBase, double>(this, nameof(AvailableReactivePower), MetricType.Gauge, x => x.AvailableReactivePower?.KiloVoltAmperesReactive, _config.MetricsFactorClass1);
            // MetricsFactorClass2
            MetricsPublisher.Register<OperatingModeBase, OperatingModeState>(this, nameof(State), MetricType.Gauge, x => x.State, _config.MetricsFactorClass2);
            MetricsPublisher.Register<OperatingModeBase, OperatingModeState>(this, nameof(StateActive), MetricType.Gauge, x => x.StateActive, _config.MetricsFactorClass2);
            MetricsPublisher.Register<OperatingModeBase, OperatingModeState>(this, nameof(StateReactive), MetricType.Gauge, x => x.StateReactive, _config.MetricsFactorClass2);
            MetricsPublisher.Register<OperatingModeBase, bool>(this, nameof(IsEnabled), MetricType.Gauge, x => x.IsEnabled, _config.MetricsFactorClass2);
            MetricsPublisher.Register<OperatingModeBase, double>(this, nameof(SetpointActivePower), MetricType.Gauge, x => x.SetpointActivePower.KiloWatts, _config.MetricsFactorClass2);
            MetricsPublisher.Register<OperatingModeBase, double>(this, nameof(SetpointReactivePower), MetricType.Gauge, x => x.SetpointReactivePower.KiloVoltAmperesReactive, _config.MetricsFactorClass2);
        }


        /// <summary>
        /// Matches the active and reactive operating state to a common state.
        /// </summary>
        /// <param name="active">The active operating state.</param>
        /// <param name="reactive">The reactive operating state.</param>
        /// <param name="state">The operating state to match.</param>
        /// <returns></returns>
        private bool Matches(OperatingModeState? active, OperatingModeState? reactive, OperatingModeState state)
        {
            return (active == state && reactive == state) ||
                   (active == state && (reactive == null || reactive == OperatingModeState.Disabled)) ||
                   (reactive == state && (active == null || active == OperatingModeState.Disabled));
        }

    }
}
