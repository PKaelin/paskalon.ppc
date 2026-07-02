namespace paskalON.Devices.Domain.PowerConversionSystems
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// These are the PCS states logic is based on.
    /// </summary>
    /// <remarks>
    /// Underlying state have to be mapped to these states.
    /// </remarks>
    public enum PcsState
    {
        /// <summary>
        /// Unknown means that the PCS current state is not determined or is in an undefined state.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Stopped means PCS has stopped or is off.
        /// </summary>
        Stopped = 1,
        /// <summary>
        /// Starting means PCS is starting initializing.
        /// </summary>
        Starting = 2,
        /// <summary>
        /// Started means PCS is started and ready to receive targets.
        /// </summary>
        Started = 3,
        /// <summary>
        /// Stopping means PCS has been commanded to stop and is stopping.
        /// </summary>
        Stopping = 4,
        /// <summary>
        /// Sleeping means PCS is sleeping, kind of standby but needs more time to get into started state.
        /// </summary>
        Sleeping = 5,
        /// <summary>
        /// EnteringStandby means PCS entering standby.
        /// </summary>
        EnteringStandby = 6,
        /// <summary>
        /// Standby means PCS is in standby.
        /// </summary>
        Standby = 7,
        /// <summary>
        /// ExitingStandby means PCS is exiting standby.
        /// </summary>
        ExitingStandby = 8,
        /// <summary>
        /// NightMode means PCS is in night mode.
        /// </summary>
        NightMode = 9,
    }
}
