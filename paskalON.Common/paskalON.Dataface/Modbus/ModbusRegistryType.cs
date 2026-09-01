// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Enum for Modbus registry types.
    /// </summary>
    public enum ModbusRegistryType
    {
        /// <summary>
        /// Access: Read and Write (1 bit)
        /// Description: Binary outputs typically used for turning devices on and off (e.g., relays, motors, or indicator lights)
        /// </summary>
        Coil,
        /// <summary>
        /// Access: Read Only (1 bit)
        /// Description: Binary inputs or sate flags that provide digital feedback from field devices (e.g., safety switches, emergency stops, or alarm states).
        /// </summary>
        DiscreteInput,
        /// <summary>
        /// Access: Read Only (16 bits)
        /// Description: Numeric values that represent real-time measurements or data from analog sensors (e.g., temperature, pressure, voltage, or current).
        /// </summary>
        InputRegister,
        /// <summary>
        /// Access: Read and Write (16 bits)
        /// Description: General-purpose numeric data used for configuration, calibration, and storing system parameters (e.g., target setpoints, device IP addresses, or baud rates).
        /// </summary>
        HoldingRegister,
    }
}
