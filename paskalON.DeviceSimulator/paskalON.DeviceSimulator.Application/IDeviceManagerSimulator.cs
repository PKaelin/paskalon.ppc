// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Application;
using paskalON.DeviceSimulator.Dto.Simulations;
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
        /// Registry of the simulated devices.
        /// </summary>
        SimulationDeviceRegistry SimulationDevices { get; }


        /// <summary>
        /// Name of the DER units that are expanded and need updating.
        /// </summary>
        List<string> ExpandedUnits { get; set; }


        /// <summary>
        /// Name of the DER devices that are expanded and need updating.
        /// </summary>
        List<string> ExpandedDevices { get; set; }


        /// <summary>
        /// Gets whether the Modbus server of an endpoint answers requests.
        /// </summary>
        /// <param name="address">Endpoint address.</param>
        /// <param name="port">Endpoint port.</param>
        /// <returns>False when a communication loss is simulated, otherwise true.</returns>
        bool IsResponding(string address, int port);


        /// <summary>
        /// Sets the warning, fault and vendor event bitmasks of a simulated device.
        /// </summary>
        /// <param name="deviceType">Type of the simulated device.</param>
        /// <param name="deviceId">Device ID of the simulated device.</param>
        /// <param name="warning">Warning bitmask or null to leave it unchanged.</param>
        /// <param name="fault">Fault bitmask or null to leave it unchanged.</param>
        /// <param name="vendorEvent">Vendor event bitmask or null to leave it unchanged.</param>
        /// <exception cref="KeyNotFoundException">The simulated device does not exist.</exception>
        /// <exception cref="NotSupportedException">The device has no register for a provided value.</exception>
        void SetEvents(SimulationDeviceType deviceType, int deviceId, ushort? warning, ushort? fault, ushort? vendorEvent);


        /// <summary>
        /// Starts or stops answering Modbus requests of a simulated device to simulate a communication loss.
        /// </summary>
        /// <param name="deviceType">Type of the simulated device.</param>
        /// <param name="deviceId">Device ID of the simulated device.</param>
        /// <param name="isResponding">True to answer requests, false to stop answering (communication loss).</param>
        /// <returns>Task</returns>
        /// <exception cref="KeyNotFoundException">The device or its Modbus server does not exist.</exception>
        Task SetCommunicationAsync(SimulationDeviceType deviceType, int deviceId, bool isResponding);


        /// <summary>
        /// Slows down the active and reactive power response of a simulated PCS.
        /// </summary>
        /// <param name="deviceId">Device ID of the simulated PCS.</param>
        /// <param name="slowActivePowerByPercent">Maximum active power change per second in percent of the nameplate. 0 means instant.</param>
        /// <param name="slowReactivePowerByPercent">Maximum reactive power change per second in percent of the nameplate. 0 means instant.</param>
        /// <exception cref="KeyNotFoundException">The simulated PCS does not exist.</exception>
        void SlowDownPower(int deviceId, double slowActivePowerByPercent, double slowReactivePowerByPercent);
    }
}
