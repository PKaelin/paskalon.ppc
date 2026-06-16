namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// <see cref="OperatingModeState"/> enum to track the state of the operating mode.
    /// </summary>
    public enum OperatingModeState
    {
        /// <summary>
        /// Operating mode is disabled.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Operating mode is enabling.
        /// </summary>
        Enabling = 1,

        /// <summary>
        /// Operating mode is ramping to target and therefore not fully enabled.
        /// </summary>
        RampingToEnabled = 2,

        /// <summary>
        /// Operating mode is at the expected set point(s) and is therefore fully enabled.
        /// </summary>
        Enabled = 3,

        /// <summary>
        /// Operating mode is ramping torwards a disabling state but is not fully disabled.
        /// </summary>
        RampingToDisabled = 4,

        /// <summary>
        /// Operating mode is disabling.
        /// </summary>
        Disabling = 5,
    }
}
