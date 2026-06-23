using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Interface definition for all operating mode base.
    /// </summary>
    public interface IOperatingMode
    {
        /// <summary>
        /// Name of the operating mode.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Gets or sets whether operating mode is enabled in the stack or not.
        /// </summary>
        public bool IsEnabled { get; set; }


        /// <summary>
        /// Time stamp when operating mode was enabled otherwise null.
        /// </summary>
        public DateTimeOffset? EnabledTime { get; }


        /// <summary>
        /// Gets the complex power setpoints for the operating mode.
        /// </summary>
        ComplexPower Setpoint { get; set; }


        /// <summary>
        /// Gets the complex power targets for the operating mode.
        /// </summary>
        ComplexPower Target { get; }


        /// <summary>
        /// Gets the current operating mode state.
        /// </summary>
        OperatingModeState State { get; }


        /// <summary>
        /// Gets the operating mode ramp controller.
        /// </summary>
        IRampController RampController { get; }


        /// <summary>
        /// Gets the operating mode curve controller.
        /// </summary>
        ICurveController? CurveController { get; }


        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        SystemConfig SystemConfig { get; }


        /// <summary>
        /// Enables the operating mode.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables the operating mode.
        /// </summary>
        void Disable();
    }

}
