// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Simulated device that allows injecting warnings, faults and vendor events.
    /// </summary>
    /// <remarks>
    /// The injected values are raw 16 bit masks that are written as they are into the device registers.
    /// </remarks>
    public interface IEventInjectable
    {
        /// <summary>
        /// Flag whether the device has a warning register.
        /// </summary>
        bool HasWarningRegister { get; }


        /// <summary>
        /// Flag whether the device has a fault register.
        /// </summary>
        bool HasFaultRegister { get; }


        /// <summary>
        /// Flag whether the device has a vendor event register.
        /// </summary>
        bool HasVendorEventRegister { get; }


        /// <summary>
        /// Current warning bitmask or null when the device has no warning register.
        /// </summary>
        ushort? Warning { get; }


        /// <summary>
        /// Current fault bitmask or null when the device has no fault register.
        /// </summary>
        ushort? Fault { get; }


        /// <summary>
        /// Current vendor event bitmask or null when the device has no vendor event register.
        /// </summary>
        ushort? VendorEvent { get; }


        /// <summary>
        /// Sets the warning, fault and vendor event bitmasks.
        /// </summary>
        /// <param name="warning">Warning bitmask or null to leave it unchanged.</param>
        /// <param name="fault">Fault bitmask or null to leave it unchanged.</param>
        /// <param name="vendorEvent">Vendor event bitmask or null to leave it unchanged.</param>
        /// <exception cref="NotSupportedException">The device has no register for a provided value.</exception>
        void SetEvents(ushort? warning, ushort? fault, ushort? vendorEvent);
    }
}
