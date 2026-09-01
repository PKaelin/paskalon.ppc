// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Configs.GenericModbusDevices
{
    /// <summary>
    /// Enumeration of the State of the Automatic Transfer Switch (ATS).
    /// </summary>
    public enum AtsState
    {
        Alarmed = 4,
        Starting = 8,
        Operational = 9,
        LockedOut = 11,
        Transferred = 12,
        OnGoodSource = 27
    }
}
