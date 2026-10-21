// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Dto.Simulations
{
    /// <summary>
    /// Type of a simulated Modbus device.
    /// </summary>
    /// <remarks>
    /// Device IDs are only unique within one device type, so requests identify a device by its type and device ID.
    /// </remarks>
    public enum SimulationDeviceType
    {
        /// <summary>
        /// Power conversion system.
        /// </summary>
        PowerConversionSystem = 0,

        /// <summary>
        /// Battery bank.
        /// </summary>
        BatteryBank = 1,
    }
}
