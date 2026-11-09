// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples
{
    /// <summary>
    /// Battery bank simple description.
    /// </summary>
    /// <remarks>
    /// For device specific information like Codes, Warnings, Errors, State, Register, etc.
    /// </remarks>
    public static class BbSimpleV1Description
    {
        /// <summary>
        /// The number of racks in this battery bank.
        /// </summary>
        public const ushort RackCount = 10;


        /// <summary>
        /// Enumeration of states.
        /// </summary>
        public enum State
        {
            Disconnected = 0,
            Connected = 1,
            Idle = 2,
            Charging = 3,
            Discharging = 4,
        }


        /// <summary>
        /// Bit flags of the warning codes.
        /// </summary>
        /// <remarks>
        /// Several warnings can be active at the same time. Bits that are not defined are reported as Unknown.
        /// </remarks>
        [Flags]
        public enum WarningCode
        {
            None = 0,
            CurrentOverLimit = 1 << 0,
            SystemOverVoltage = 1 << 1,
            SystemUnderVoltage = 1 << 2,
            BigVoltageDifferenceSingleCell = 1 << 3,
            BigTemperatureDifferenceBank = 1 << 4,
            CellExtremeTemperature = 1 << 5,
            CellExtremeVoltage = 1 << 6,
        }


        /// <summary>
        /// Bit flags of the fault (error) codes.
        /// </summary>
        /// <remarks>
        /// Several faults can be active at the same time. Bits that are not defined are reported as Unknown.
        /// </remarks>
        [Flags]
        public enum FaultCode
        {
            None = 0,
            CurrentOverLimit = 1 << 0,
            SystemOverVoltage = 1 << 1,
            SystemUnderVoltage = 1 << 2,
            BigVoltageDifferenceSingleCell = 1 << 3,
            BigTemperatureDifferenceBank = 1 << 4,
            CellExtremeTemperature = 1 << 5,
            CellExtremeVoltage = 1 << 6,
        }


        /// <summary>
        /// Bit flags of the vendor events.
        /// </summary>
        /// <remarks>
        /// Several vendor events can be active at the same time. Bits that are not defined are reported as Unknown.
        /// </remarks>
        [Flags]
        public enum VendorEvents
        {
            None = 0,
            MaintenanceDue = 1 << 0,
            EndOfLifeDue = 1 << 1,
        }


        /// <summary>
        /// Enumeration of registers.
        /// </summary>
        public enum Register
        {
            //Heartbeat
            Heartbeat = 40000,
            //Control
            SelectorState = 41000,     // Connect/Disconnect
            // Current, Voltage, Charge, Health
            TotalStateOfCharge = 43000,
            TotalStateOfHealth = 43001,
            TotalDCVoltage = 43002,
            TotalDCCurrent = 43003,
            // State, Warnings, Faults
            CurrentState = 44000,
            CurrentWarning = 44001,
            CurrentFault = 44002,
            CurrentVendorEvent = 44003,
        }
    }
}
