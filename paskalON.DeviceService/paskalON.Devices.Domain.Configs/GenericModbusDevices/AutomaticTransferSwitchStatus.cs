// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
