// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.PowerConversionSystems.Simples
{
    /// <summary>
    /// Power conversion system simple description.
    /// </summary>
    /// <remarks>
    /// For device specific information like Codes, Warnings, Errors, Status, Register, etc.
    /// </remarks>
    public class PcsSimpleV1Description
    {
        /// <summary>
        /// Enumeration of status.
        /// </summary>
        public enum Status
        {
            Initialization = 1,
            Off = 2,
            On = 3,
            Stop = 4,
            Fault = 5,
            Standby = 6,
            NightMode = 7,
        }

        /// <summary>
        /// Enumeration of warning codes.
        /// </summary>
        public enum WarningCode
        {
            Unknown = -1,
            None = 0,
            HighInputVoltage = 1,
            LowInputVoltage = 2,
            HighFrequency = 3,
            LowFrequency = 4,
        }

        /// <summary>
        /// Enumeration of fault (error) codes.
        /// </summary>
        public enum FaultCode
        {
            Unknown = -1,
            None = 0,
            HighInputVoltage = 1,
            LowInputVoltage = 2,
            HighFrequency = 3,
            LowFrequency = 4,
        }

        /// <summary>
        /// Enumeration of registers.
        /// </summary>
        public enum Register
        {
            //Heartbeat
            Heartbeat = 40000,
            //Control
            PReference = 41000,
            QReference = 41001,
            // Power
            P = 42000,
            Q = 42000,
            S = 42000,
            // Current, Voltage, Frequency
            Frequency = 43000,
            DCCurrent = 43001,
            DCVoltage = 43002,
            ACCurrent = 43003,
            ACVoltage = 43004,
            // Status, Warnings, Faults
            CurrentStatus = 44000,      // Could be a mask but for simplicity just have one
            CurrentWarning = 44001,     // Could be a mask but for simplicity just have one
            CurrentFault = 44002,
            ACBreaker = 44003,
            DcContactor = 44004,        // For simplicity just have one
        }
    }
}
