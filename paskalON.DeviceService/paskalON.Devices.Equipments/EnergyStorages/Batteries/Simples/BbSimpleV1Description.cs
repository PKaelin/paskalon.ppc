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
        /// Enumeration of warning codes.
        /// </summary>
        public enum WarningCode
        {
            None = 0,
            CurrentOverLimit = 1,
            SystemOverVoltage = 2,
            SystemUnderVoltage = 3,
            BigVoltageDifferenceSingleCell = 4,
            BigTemperatureDifferenceBank = 5,
            CellExtremeTemperature = 6,
            CellExtremeVoltage = 7,
        }


        /// <summary>
        /// Enumeration of fault (error) codes.
        /// </summary>
        public enum FaultCode
        {
            None = 0,
            CurrentOverLimit = 1,
            SystemOverVoltage = 2,
            SystemUnderVoltage = 3,
            BigVoltageDifferenceSingleCell = 4,
            BigTemperatureDifferenceBank = 5,
            CellExtremeTemperature = 6,
            CellExtremeVoltage = 7,
        }


        /// <summary>
        /// Enumeration of vendor events.
        /// </summary>
        public enum VendorEvents
        {
            None = 0,
            MaintenanceDue = 1,
            EndOfLifeDue = 2,
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
