namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
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
