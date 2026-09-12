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
        private readonly List<ISimulatedDevice> _devices = new();


        /// <summary>
        /// Data lock object
        /// </summary>
        private readonly object _dataLock = new();


        /// <summary>
        /// Gets a snapshot of registered simulation devices.
        /// </summary>
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
    }
}