
namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Enumeration of the State of the Automatic Transfer Switch (ATS).
    /// </summary>
    public enum AtsStatus
    {
        Alarmed = 4,
        Starting = 8,
        Operational = 9,
        LockedOut = 11,
        Transferred = 12,
        OnGoodSource = 27
    }
}
