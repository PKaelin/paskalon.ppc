// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.PowerConversionSystems.Simples
{
    /// <summary>
    /// Power conversion system simple description.
    /// </summary>
    /// <remarks>
    /// For device specific information like Codes, Warnings, Errors, State, Register, etc.
    /// </remarks>
    public static class PcsSimpleV1Description
    {
        /// <summary>
        /// Enumeration of states.
        /// </summary>
        public enum State
        {
            Unknown = 0,
            Initialization = 1,
            Off = 2,
            On = 3,
            Stop = 4,
            Fault = 5,
            Standby = 6,
            NightMode = 7,
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
            HighInputVoltage = 1 << 0,
            LowInputVoltage = 1 << 1,
            HighFrequency = 1 << 2,
            LowFrequency = 1 << 3,
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
            HighInputVoltage = 1 << 0,
            LowInputVoltage = 1 << 1,
            HighFrequency = 1 << 2,
            LowFrequency = 1 << 3,
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
            SelectorState = 41000,     // Start/Stop
            PReference = 41001,
            QReference = 41002,
            // Power
            P = 42000,
            Q = 42001,
            PAvailable = 42002,
            QAvailable = 42003,
            // Current, Voltage, Frequency
            Frequency = 43000,
            DCCurrent = 43001,
            DCVoltage = 43002,
            ACCurrent = 43003,
            ACVoltage = 43004,
            // State, Warnings, Faults
            CurrentState = 44000,      // Could be a mask but for simplicity just have one
            CurrentWarning = 44001,     // Could be a mask but for simplicity just have one
            CurrentFault = 44002,
            CurrentVendorEvent = 44003,
            ACBreaker = 44004,
            DcContactor = 44005,        // For simplicity just have one
        }
    }
}
