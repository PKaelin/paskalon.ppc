// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    /// <summary>
    /// Direction of the current flow of a battery bank published by the device service.
    /// </summary>
    /// <remarks>
    /// Mirrors BatteryBankFlowDirection (paskalON.Devices.Domain) with the same names and numbers, mapped in the application layer.
    /// </remarks>
    public enum BatteryBankFlowDirectionDto
    {
        /// <summary>
        /// The battery bank is neither charging nor discharging.
        /// </summary>
        Idle = 0,


        /// <summary>
        /// Current is forced into the battery bank to store energy.
        /// </summary>
        Charging = 1,


        /// <summary>
        /// Current is drawn out of the battery bank to release energy.
        /// </summary>
        Discharging = 2
    }
}
