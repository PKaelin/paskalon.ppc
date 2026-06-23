using Microsoft.Extensions.Logging;
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
        /// System configuration for all configured operating modes.
        /// </summary>
        protected readonly SystemConfig _systemConfig;


        /// <summary>
        /// Operating mode base configuration.
        /// </summary>
        private readonly OperatingModeConfig _operatingModeConfig;


        /// <summary>
        /// Gets the name of the operating mode.
        /// </summary>
        public string Name { get => _operatingModeConfig.Name; }


        /// <summary>
        /// Gets or sets whether operating mode is enabled in the stack or not.
        /// </summary>
        public bool IsEnabled
        {
            get;
            set
            {
                field = value;
                if (value == true) EnabledTime = DateTimeOffset.UtcNow;
                else EnabledTime = null;
            }
        }


        /// <summary>
        /// Time stamp when operating mode was enabled otherwise null.
        /// </summary>
        public DateTimeOffset? EnabledTime { get; protected set; }


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
        public SystemConfig SystemConfig { get => _systemConfig; }




        public OperatingModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeConfig operatingModeConfigBase,
            IRampController rampController, ICurveController? curveController)
        {
            _logger = logger;
            _timeProvider = timeProvider;
            _systemConfig = systemConfig;
            _operatingModeConfig = operatingModeConfigBase;
            RampController = rampController;
            CurveController = curveController;
        }


        /// <summary>
        /// Enables the operating mode.
        /// </summary>
        public virtual void Enable()
        {
            _logger.LogInformation("Operating mode enabled: {Name}", Name);

            if (State != OperatingModeState.Enabled)
            {
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
            _logger.LogInformation("Operating mode disabled: {Name}", Name);

            if (State != OperatingModeState.Disabled)
            {
                State = OperatingModeState.RampingToDisabled;
                State = OperatingModeState.Disabling;
                RampController.Stop();
                State = OperatingModeState.Disabled;
            }
        }
    }
}
