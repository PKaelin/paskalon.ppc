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
        /// Gets the name of the operating mode.
        /// </summary>
        public string Name { get => _config.Name; }


        /// <summary>
        /// Gets or sets whether operating mode is enabled in the stack or not.
        /// </summary>
        public bool IsEnabled
        {
            get;
            set
            {
                field = value;
                if (value == true)
                {
                    LastEnabled = DateTimeOffset.UtcNow;
                }
            }
        }


        /// <summary>
        /// Time stamp when operating mode was enabled the last time otherwise min value.
        /// </summary>
        public DateTimeOffset LastEnabled { get; protected set; } = DateTimeOffset.MinValue;


        /// <summary>
        /// Gets the current operating mode state.
        /// </summary>
        public OperatingModeState State { get; protected set; } = OperatingModeState.Disabled;


        /// <summary>
        /// Gets the complex power setpoints for the operating mode.
        /// </summary>
        public ComplexPower Setpoint { get; set; } = new ComplexPower();


        /// <summary>
        /// Gets the complex power targets for the operating mode.
        /// </summary>
        public ComplexPower Target { get; protected set; } = new ComplexPower();


        /// <summary>
        /// Gets the operating mode ramp controller.
        /// </summary>
        public IRampController RampController { get; protected set; }


        /// <summary>
        /// Gets the operating mode curve controller.
        /// </summary>
        public ICurveController? CurveController { get; protected set; }


        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        public SystemConfig SystemConfig { get; init; }



        /// <summary>
        /// Constructor of <see cref="OperatingModeBase"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode base configuration.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public OperatingModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeBaseConfig config,
            IRampController rampController, ICurveController? curveController)
        {
            _logger = logger;
            _timeProvider = timeProvider;
            _config = config;
            SystemConfig = systemConfig;
            RampController = rampController;
            CurveController = curveController;
            _logger.LogInformation("Operating Mode created. Name: {Name}", Name);
        }


        /// <summary>
        /// Enables the operating mode.
        /// </summary>
        public virtual void Enable()
        {
            if (State != OperatingModeState.Enabled)
            {
                _logger.LogInformation("Operating mode enabled: {Name}", Name);
                State = OperatingModeState.Enabling;
                State = OperatingModeState.RampingToEnabled;
                RampController.Start(0, 0);
                State = OperatingModeState.Enabled;
            }
        }


        /// <summary>
        /// Disables the operating mode.
        /// </summary>
        public virtual void Disable()
        {
            if (State != OperatingModeState.Disabled)
            {
                _logger.LogInformation("Operating mode disabled: {Name}", Name);
                State = OperatingModeState.RampingToDisabled;
                State = OperatingModeState.Disabling;
                RampController.Stop();
                State = OperatingModeState.Disabled;
            }
        }
    }
}
