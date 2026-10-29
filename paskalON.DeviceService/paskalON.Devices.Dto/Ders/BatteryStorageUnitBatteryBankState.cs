// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.Ders
{
    /// <summary>
    /// Combined state of the battery banks of a battery storage unit.
    /// </summary>
    /// <remarks>
    /// Has the values of BatteryBankState (paskalON.Devices.Domain) with the same numbers, used for a single battery bank,
    /// plus the combinations of multiple battery banks.
    /// </remarks>
    public enum BatteryStorageUnitBatteryBankState
    {
        /// <summary>
        /// The state of the battery bank is not determined.
        /// </summary>
        Unknown = 0,


        /// <summary>
        /// The battery bank is disconnected, or all battery banks are disconnected.
        /// </summary>
        Disconnected = 1,


        /// <summary>
        /// The battery bank is connecting.
        /// </summary>
        Connecting = 2,


        /// <summary>
        /// The battery bank is initializing.
        /// </summary>
        Initializing = 3,


        /// <summary>
        /// The battery bank is connected, or all battery banks are connected.
        /// </summary>
        Connected = 4,


        /// <summary>
        /// The battery bank is in standby.
        /// </summary>
        Standby = 5,


        /// <summary>
        /// The battery bank is disconnecting.
        /// </summary>
        Disconnecting = 6,


        /// <summary>
        /// The battery bank is in state of charge protection.
        /// </summary>
        SocProtection = 7,


        /// <summary>
        /// The battery bank is in fault.
        /// </summary>
        Fault = 99,


        /// <summary>
        /// At least one, but not all battery banks are connected.
        /// </summary>
        PartiallyConnected = 100,


        /// <summary>
        /// The battery banks are in any other combination of states, or the unit has no battery bank.
        /// </summary>
        UnknownCombination = 101
    }
}
