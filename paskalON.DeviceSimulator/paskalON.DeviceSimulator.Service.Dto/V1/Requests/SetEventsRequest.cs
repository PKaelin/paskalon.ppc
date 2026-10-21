// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Dto.Simulations;

namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Sets the warning, fault and vendor event bitmasks of a simulated PCS or battery bank.
    /// </summary>
    /// <remarks>
    /// The bitmasks are written as they are into the device registers. A null value leaves the register unchanged.
    /// A PCS fault bitmask other than 0 puts the PCS into the fault state.
    /// </remarks>
    public class SetEventsRequest
    {
        /// <summary>
        /// Type of the simulated device.
        /// </summary>
        public SimulationDeviceType DeviceType { get; set; }


        /// <summary>
        /// Device ID of the simulated device (unique within the device type).
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// Warning bitmask or null to leave it unchanged.
        /// </summary>
        public ushort? Warning { get; set; }


        /// <summary>
        /// Fault bitmask or null to leave it unchanged.
        /// </summary>
        public ushort? Fault { get; set; }


        /// <summary>
        /// Vendor event bitmask or null to leave it unchanged.
        /// </summary>
        public ushort? VendorEvent { get; set; }
    }
}
