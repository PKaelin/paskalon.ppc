// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Application.Simulations
{
    /// <summary>
    /// Creates a simulator model for a production device.
    /// </summary>
    public interface ISimulationModelFactory
    {
        /// <summary>
        /// Creates a simulation model when the device type is supported.
        /// </summary>
        /// <param name="device">Production device proxy.</param>
        /// <param name="store">Backing Modbus store.</param>
        /// <returns>A simulation model, or null when unsupported.</returns>
        ISimulatedDevice? Create(PowerConversionSystemBase device, IModbusDataStore store);
    }
}