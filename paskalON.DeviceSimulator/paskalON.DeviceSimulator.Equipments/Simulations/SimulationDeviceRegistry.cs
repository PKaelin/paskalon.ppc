// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Registry of simulated devices driven by the simulation worker.
    /// </summary>
    public class SimulationDeviceRegistry
    {
        /// <summary>
        /// Simulated devices.
        /// </summary>
        private readonly List<ISimulatedDevice> _devices = new List<ISimulatedDevice>();


        /// <summary>
        /// Data lock object
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Gets a snapshot of registered simulation devices.
        /// </summary>
        /// <remarks>
        /// The devices are returned in registration order which is also the order they are ticked.
        /// </remarks>
        public IReadOnlyList<ISimulatedDevice> Devices
        {
            get
            {
                lock (_dataLock)
                {
                    return _devices.ToArray();
                }
            }
        }


        /// <summary>
        /// Registers a simulation device.
        /// </summary>
        /// <param name="device">Device model to register.</param>
        public void Add(ISimulatedDevice device)
        {
            ArgumentNullException.ThrowIfNull(device);

            lock (_dataLock)
            {
                _devices.Add(device);
            }
        }


        /// <summary>
        /// Finds a simulated device of a type by its device ID.
        /// </summary>
        /// <typeparam name="T">Type of the simulated device.</typeparam>
        /// <param name="deviceId">Device ID that is unique within the device type.</param>
        /// <returns>The simulated device or null when not found.</returns>
        public T? Find<T>(int deviceId) where T : class, ISimulatedDevice
        {
            lock (_dataLock)
            {
                return _devices.OfType<T>().FirstOrDefault(device => device.DeviceId == deviceId);
            }
        }
    }
}
