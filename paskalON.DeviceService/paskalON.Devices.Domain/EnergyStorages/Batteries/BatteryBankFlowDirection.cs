namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    /// <summary>
    /// Charging and discharging refer to the direction of current flow and the conversion of energy.
    /// </summary>
    public enum BatteryBankFlowDirection
    {
        /// <summary>
        /// Battery bank is neither charging nor discharging, it is in a state of idle
        /// </summary>
        Idle = 0,
        /// <summary>
        /// Charging forces current into the battery to store energy.
        /// </summary>
        Charging = 1,
        /// <summary>
        /// Discharging draws current out to release energy.
        /// </summary>
        Discharging = 2
    }
}
