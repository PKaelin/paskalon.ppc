// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    /// <summary>
    /// Battery bank states published by the device service.
    /// </summary>
    /// <remarks>
    /// Mirrors BatteryBankState (paskalON.Devices.Domain) with the same names and numbers, mapped in the application layer.
    /// </remarks>
    public enum BatteryBankStateDto
    {
        /// <summary>
        /// The current state of the battery bank is not determined or is undefined.
        /// </summary>
        Unknown = 0,


        /// <summary>
        /// The battery bank is physically or electronically isolated from the load, inverter or charging source.
        /// </summary>
        Disconnected = 1,


        /// <summary>
        /// The battery bank is establishing a connection.
        /// </summary>
        Connecting = 2,


        /// <summary>
        /// The battery bank is initializing (pre-charging) before it is fully connected.
        /// </summary>
        Initializing = 3,


        /// <summary>
        /// The battery bank is fully connected and operational.
        /// </summary>
        Connected = 4,


        /// <summary>
        /// The battery bank is connected and energized but not charging or discharging.
        /// </summary>
        Standby = 5,


        /// <summary>
        /// The battery bank is disconnecting.
        /// </summary>
        Disconnecting = 6,


        /// <summary>
        /// The battery bank is protected from operating outside of its safe charge or discharge limits.
        /// </summary>
        SocProtection = 7,


        /// <summary>
        /// The battery bank encountered an error or abnormal condition that requires attention.
        /// </summary>
        Fault = 99
    }
}
