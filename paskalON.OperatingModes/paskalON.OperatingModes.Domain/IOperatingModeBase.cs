using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Interface definition for all operating mode base.
    /// </summary>
    public interface IOperatingModeBase
    {
        /// <summary>
        /// Name of the operating mode.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Gets or sets whether operating mode is enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; }


        /// <summary>
        /// Gets the current operating mode state.
        /// </summary>
        OperatingModeState State { get; set; }


        /// <summary>
        /// Gets the operating mode ramp model.
        /// </summary>
        RampController RampModel { get; }


        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        SystemConfig SystemConfig { get; }
    }

}
