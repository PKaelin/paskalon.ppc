// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Application;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Application
{
    /// <summary>
    /// Device manager simulator interface definition.
    /// </summary>
    public interface IDeviceManagerSimulator : IDeviceManager
    {
        /// <summary>
        /// Registry of simulator Modbus stores.
        /// </summary>
        ISimulationStoreRegistry StoreRegisters { get; }


        /// <summary>
        /// Registry of simulator C37 streams.
        /// </summary>
        ISimulationStreamRegistry StreamRegisters { get; }


        /// <summary>
        /// Name of the DER units that are expanded and need updating.
        /// </summary>
        List<string> ExpandedUnits { get; set; }


        /// <summary>
        /// Name of the DER devices that are expanded and need updating.
        /// </summary>
        List<string> ExpandedDevices { get; set; }
    }
}
