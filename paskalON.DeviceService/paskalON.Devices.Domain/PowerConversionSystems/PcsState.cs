namespace paskalON.Devices.Domain.PowerConversionSystems
{
    /// <summary>
    /// These are the PCS states logic is based on.
    /// Specialized PCS has to map its states to the states below.
    /// </summary>
    public enum PcsState
    {
        /// <summary>
        /// Unknown when initialized.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// PCS has stopped or is off.
        /// </summary>
        Stopped = 1,
        /// <summary>
        /// PCS is starting initializing.
        /// </summary>
        Starting = 2,
        /// <summary>
        /// PCS is started and ready to receive targets.
        /// </summary>
        Started = 3,
        /// <summary>
        /// PCS has been commanded to stop and is stopping.
        /// </summary>
        Stopping = 4,
        /// <summary>
        /// PCS is sleeping, kind of standby but needs more time to get into started state.
        /// </summary>
        Sleeping = 5,
        /// <summary>
        /// PCS entering standby.
        /// </summary>
        EnteringStandby = 6,
        /// <summary>
        /// PCS is in standby.
        /// </summary>
        Standby = 7,
        /// <summary>
        /// PCS is exiting standby.
        /// </summary>
        ExitingStandby = 8,
        /// <summary>
        /// PCS is in night mode.
        /// </summary>
        NightMode = 9,
    }
}
