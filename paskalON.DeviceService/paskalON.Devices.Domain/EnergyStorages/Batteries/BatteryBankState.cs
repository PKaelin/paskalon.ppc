namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Battery bank states.
    /// </summary>
    /// <remarks>
    /// Underlying state have to be mapped to these states.
    /// </remarks>
    public enum BatteryBankState
    {
        /// <summary>
        /// Unknown means that the battery bank's current state is not determined or is in an undefined state.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Disconnected means that the battery has been physically or electronically isolated from the system’s load, inverter, or charging source. 
        /// </summary>
        Disconnected = 1,
        /// <summary>
        /// Connecting means that the battery bank is in the process of establishing a connection.
        /// </summary>
        Connecting = 2,
        /// <summary>
        /// Initializing means a battery bank must be initialized (or pre-charged) before it is fully connected to a heavy load.
        /// </summary>
        Initializing = 3,
        /// <summary>
        /// Connected means the battery bank is fully connected and operational.
        /// </summary>
        Connected = 4,
        /// <summary>
        /// Standby means the battery bank is connected and fully energized but not actively charging or discharging. 
        /// </summary>
        Standby = 5,
        /// <summary>
        /// Disconnecting means that the battery bank is in the process of disconnecting.
        /// </summary>
        Disconnecting = 6,
        /// <summary>
        /// SOC Protection refers to the automated safety mechanisms in place to prevent the battery
        /// from operating outside of its safe charge or discharge limits.
        /// </summary>
        SocProtection = 7,
        /// <summary>
        /// Fault means that the battery bank has encountered an error or abnormal condition that requires attention.
        /// </summary>
        Fault = 99
    }
}
