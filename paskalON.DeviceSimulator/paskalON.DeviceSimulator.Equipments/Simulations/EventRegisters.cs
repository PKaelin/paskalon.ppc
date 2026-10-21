// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Warning, fault and vendor event registers of a simulated device.
    /// </summary>
    /// <remarks>
    /// The registers hold raw 16 bit masks. The simulation never overwrites them, so injected values stay
    /// until they are changed again.
    /// </remarks>
    public sealed class EventRegisters
    {
        /// <summary>
        /// Data store the registers are written to.
        /// </summary>
        private readonly IModbusDataStore _store;


        /// <summary>
        /// Warning register address or null when the device has none.
        /// </summary>
        public int? WarningAddress { get; }


        /// <summary>
        /// Fault register address or null when the device has none.
        /// </summary>
        public int? FaultAddress { get; }


        /// <summary>
        /// Vendor event register address or null when the device has none.
        /// </summary>
        public int? VendorEventAddress { get; }


        /// <summary>
        /// Current warning bitmask or null when the device has no warning register.
        /// </summary>
        public ushort? Warning { get => Read(WarningAddress); }


        /// <summary>
        /// Current fault bitmask or null when the device has no fault register.
        /// </summary>
        public ushort? Fault { get => Read(FaultAddress); }


        /// <summary>
        /// Current vendor event bitmask or null when the device has no vendor event register.
        /// </summary>
        public ushort? VendorEvent { get => Read(VendorEventAddress); }


        /// <summary>
        /// Constructor of <see cref="EventRegisters"/>.
        /// </summary>
        /// <param name="store">Data store the registers are written to.</param>
        /// <param name="warningAddress">Warning register address or null when the device has none.</param>
        /// <param name="faultAddress">Fault register address or null when the device has none.</param>
        /// <param name="vendorEventAddress">Vendor event register address or null when the device has none.</param>
        public EventRegisters(IModbusDataStore store, int? warningAddress, int? faultAddress, int? vendorEventAddress)
        {
            ArgumentNullException.ThrowIfNull(store);

            _store = store;
            WarningAddress = warningAddress;
            FaultAddress = faultAddress;
            VendorEventAddress = vendorEventAddress;
        }


        /// <summary>
        /// Writes the warning, fault and vendor event bitmasks.
        /// </summary>
        /// <param name="warning">Warning bitmask or null to leave it unchanged.</param>
        /// <param name="fault">Fault bitmask or null to leave it unchanged.</param>
        /// <param name="vendorEvent">Vendor event bitmask or null to leave it unchanged.</param>
        /// <exception cref="NotSupportedException">The device has no register for a provided value.</exception>
        public void Write(ushort? warning, ushort? fault, ushort? vendorEvent)
        {
            // Validate everything first so that nothing is written when one value is not supported
            Validate(warning, WarningAddress, "warning");
            Validate(fault, FaultAddress, "fault");
            Validate(vendorEvent, VendorEventAddress, "vendor event");

            Write(WarningAddress, warning);
            Write(FaultAddress, fault);
            Write(VendorEventAddress, vendorEvent);
        }


        /// <summary>
        /// Validates that a register exists for a provided value.
        /// </summary>
        private static void Validate(ushort? value, int? address, string name)
        {
            if (value.HasValue && address.HasValue is false)
            {
                throw new NotSupportedException($"The device has no {name} register.");
            }
        }


        /// <summary>
        /// Writes a value when both the value and the register address exist.
        /// </summary>
        private void Write(int? address, ushort? value)
        {
            if (address.HasValue && value.HasValue)
            {
                RegisterSimulation.Write(_store, address.Value, value.Value);
            }
        }


        /// <summary>
        /// Reads a register when the address exists.
        /// </summary>
        private ushort? Read(int? address)
        {
            if (address.HasValue is false)
            {
                return null;
            }

            return RegisterSimulation.Read(_store, address.Value);
        }
    }
}
