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
    public abstract class OperatingModeBase : IOperatingMode
    {
        private readonly OperatingModeBaseMap _map;
        protected ActivePower? _lastAvailableActive;
        protected ActivePower? _lastSetpointActive;
        protected ReactivePower? _lastAvailableReactive;
        protected ReactivePower? _lastSetpointReactive;


        /// <summary>
        /// Active power target for the operating mode.
        /// </summary>
        protected ActivePower _targetActivePower = new ActivePower(0);


        /// <summary>
        /// Reactive power target for the operating mode.
        /// </summary>
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
            get { lock (dataLock) { return field; } }
            set
            {
                lock (dataLock)
                {
                    field = value;

                    if (value == true)
                    {
                        LastEnabled = DateTimeOffset.UtcNow;
                    }
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTimeOffset LastEnabled { get; protected set; } = DateTimeOffset.MinValue;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public OperatingModeState State { get; protected set; } = OperatingModeState.Disabled;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower SetpointActivePower
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        } = new ActivePower(0);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<ActivePower?> AvailableActivePower { get => _map.AvailableActivePower; }


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
            set { lock (dataLock) { field = value; } }
        } = new ReactivePower(0);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<ReactivePower?> AvailableReactivePower { get => _map.AvailableReactivePower; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower TargetReactivePower { get => _targetReactivePower; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IRampController RampController { get; protected set; }


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
            RampController = rampController;
            CurveController = curveController;
            _logger.LogInformation("Operating Mode created. Name: {Name}", Name);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Enable()
        {
            if (State != OperatingModeState.Enabled)
            {
                double setpointActive = GetActiveSetpoint();
                double setpointReactive = GetReactiveSetpoint();

                _logger.LogInformation("Operating mode enabled: {Name}. Active target: {ActiveTarget}. Reactive target: {ReactiveTarget}", Name, setpointActive, setpointReactive);
                State = OperatingModeState.Enabling;

                if (setpointActive != 0)
                {
                    RampController.Start(TargetActivePower.KiloWatts, setpointActive);
                }

                if (setpointReactive != 0)
                {
                    RampController.Start(TargetReactivePower.KiloVoltAmperesReactive, setpointReactive);
                }

                State = OperatingModeState.RampingToEnabled;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Disable()
        {
            if (State != OperatingModeState.Disabled)
            {
                _logger.LogInformation("Operating mode disabled: {Name}. Target set to {Target}", Name, 0);
                State = OperatingModeState.RampingToDisabled;
                RampController.Start(TargetActivePower.KiloWatts, 0);
            }
        }


        protected double GetActiveSetpoint()
        {
            double setpoint = 0;
            ActivePower? available = AvailableActivePower.Invoke();
            // Only change 0 setpoint when there is available power
            if (available != null)
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (available.Value.KiloWatts <= SetpointActivePower.KiloWatts)
                {
                    setpoint = available.Value.KiloWatts;
                }
                // Available is more then setpoint use setpoint
                else
                {
                    setpoint = SetpointActivePower.KiloWatts;
                }

                _lastAvailableActive = available;
                _lastSetpointActive = new ActivePower(setpoint);
            }
            else
            {
                _lastAvailableActive = null;
                _lastSetpointActive = null;
            }

            return setpoint;
        }

        protected double GetReactiveSetpoint()
        {
            double setpoint = 0;
            ReactivePower? available = AvailableReactivePower.Invoke();
            // Only change 0 setpoint when there is available power
            if (available != null)
            {
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (available.Value.KiloVoltAmperesReactive <= SetpointReactivePower.KiloVoltAmperesReactive)
                {
                    setpoint = available.Value.KiloVoltAmperesReactive;
                }
                // Available is more then setpoint use setpoint
                else
                {
                    setpoint = SetpointReactivePower.KiloVoltAmperesReactive;
                }

                _lastAvailableReactive = available;
                _lastSetpointReactive = new ReactivePower(setpoint);
            }
            else
            {
                _lastAvailableReactive = null;
                _lastSetpointReactive = null;
            }

            return setpoint;
        }

    }
}
