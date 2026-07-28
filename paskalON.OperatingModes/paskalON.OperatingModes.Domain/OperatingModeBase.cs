// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Base class for all operating modes.
    /// </summary>
    /// <remarks>
    /// Operating Mode defines the specific behavior and control strategy the system uses to interact with the power grid.
    /// </remarks>
    public abstract class OperatingModeBase : IOperatingMode
    {
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
            get;
            protected set
            {
                if (value != field)
                {
                    _logger.LogInformation("{Name} OperatingModeState changed to: {State}", Name, value);
                    field = value;
                }
            }
        } = OperatingModeState.Disabled;


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
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode base configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public OperatingModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeBaseConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController = null)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(systemConfig);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(rampController);

            _logger = logger;
            _timeProvider = timeProvider;
            _config = config;
            _map = map;
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

            if (State != OperatingModeState.Enabled)
            {
                double setpointActive = GetActivePowerSetpoint();
                double setpointReactive = GetReactivePowerSetpoint();

                _logger.LogInformation("{Name} operating mode enabled. Active target: {ActiveTarget}. Reactive target: {ReactiveTarget}", Name, setpointActive, setpointReactive);
                State = OperatingModeState.Enabling;

                if (setpointActive != 0)
                {
                    RampControllerActive.Start(TargetActivePower.KiloWatts, setpointActive);
                    State = OperatingModeState.RampingToEnabled;
                }

                if (setpointReactive != 0)
                {
                    RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, setpointReactive);
                    State = OperatingModeState.RampingToEnabled;
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Disable()
        {
            IsEnabled = false;

            if (State != OperatingModeState.Disabled)
            {
                _logger.LogInformation("{Name} operating mode disabled. Active target: {ActiveTarget}. Reactive target: {ReactiveTarget}", Name, 0, 0);
                SetpointActivePower = new ActivePower(0);
                SetpointReactivePower = new ReactivePower(0);
                State = OperatingModeState.RampingToDisabled;
                RampControllerActive.Start(TargetActivePower.KiloWatts, 0);
                RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, 0);
            }
        }


        /// <summary>
        /// Get the active power setpoint using setpoint and available power.
        /// </summary>
        /// <returns>Available power if setpoint is higher otherwise setpoint.</returns>        
        protected double GetActivePowerSetpoint()
        {
            double setpoint = 0;
            ActivePower? available = AvailableActivePower;
            // Only change initial 0 setpoint when there is available power
            if (available != null)
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (Math.Abs(available.Value.KiloWatts) <= Math.Abs(SetpointActivePower.KiloWatts))
                {
                    setpoint = available.Value.KiloWatts;
                }
                // Available is more then setpoint use setpoint
                else
                {
                    setpoint = SetpointActivePower.KiloWatts;
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

            return setpoint;
        }


        /// <summary>
        /// Get the reactive power setpoint using setpoint and available power.
        /// </summary>
        /// <returns>Available power if setpoint is higher otherwise setpoint.</returns>
        protected double GetReactivePowerSetpoint()
        {
            double setpoint = 0;
            ReactivePower? available = AvailableReactivePower;
            // Only change initial 0 setpoint when there is available power
            if (available != null)
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (Math.Abs(available.Value.KiloVoltAmperesReactive) <= Math.Abs(SetpointReactivePower.KiloVoltAmperesReactive))
                {
                    setpoint = available.Value.KiloVoltAmperesReactive;
                }
                // Available is more then setpoint use setpoint
                else
                {
                    setpoint = SetpointReactivePower.KiloVoltAmperesReactive;
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

            return setpoint;
        }

    }
}
