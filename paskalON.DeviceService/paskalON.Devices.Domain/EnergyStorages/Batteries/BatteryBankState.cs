namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    /// <summary>
    /// Battery bank states.
    /// </summary>
    public enum BatteryBankState
    {
        Unknown = 0,
        Disconnected = 1,
        Initializing = 2,
        Connected = 3,
        Standby = 4,
        SocProtection = 5,
        Fault = 99
    }
}
