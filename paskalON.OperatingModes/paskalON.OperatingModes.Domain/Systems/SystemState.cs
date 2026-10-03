namespace paskalON.OperatingModes.Domain.Systems
{
    /// <summary>
    /// Represents the various states of a system operating mode control.
    /// </summary>
    public enum SystemState
    {
        /// <summary>
        /// State has not yet been determined.
        /// </summary>
        Unknown,
        /// <summary>
        /// Initialized correctly and capable of executing an operating mode, but not currently controlling.
        /// </summary>
        Ready,
        /// <summary>
        /// Operating mode control is actively executing and controlling the system.
        /// </summary>
        Running,
        /// <summary>
        /// Transitioning from running to stopped, but not yet fully stopped.
        /// This state indicates that the system is in the process of stopping its operations.
        /// </summary>
        Stopping,
        /// <summary>
        /// The system has fully stopped and is not currently executing any operating mode.
        /// </summary>
        Stopped,
        /// <summary>
        /// The system has encountered a fault or error condition that prevents it from operating normally.
        /// </summary>
        Faulted
    }
}
