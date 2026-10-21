// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries
{
    /// <summary>
    /// Vendor independent state of a simulated battery bank.
    /// </summary>
    public enum BatteryBankSimulationState
    {
        /// <summary>
        /// Battery bank is disconnected from the DC bus.
        /// </summary>
        Disconnected = 0,

        /// <summary>
        /// Battery bank pre-charges the DC bus before it connects.
        /// </summary>
        PreCharging = 1,

        /// <summary>
        /// Battery bank is connected to the DC bus.
        /// </summary>
        Connected = 2,
    }
}
