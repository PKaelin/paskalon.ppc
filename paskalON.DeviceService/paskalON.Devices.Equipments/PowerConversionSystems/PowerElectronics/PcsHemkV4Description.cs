// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics
{
    internal class PcsHemkV4Description
    {
        // TODO: Implement PcsHemkV4Description

        /// <summary>
        /// Enumeration of states.
        /// </summary>
        public enum State
        {
            PowerUp = 0,
            Initialization = 1,
            Off = 2,
            Precharge = 3,
            Ready = 4,
            Wait = 5,
            On = 6,
            Stop = 7,
            Discharge = 8,
            Fault = 9,
            LVRT = 10,
            OVRT = 11,
            Night = 12,
            NightDcOff = 13,
            Standby = 14,
            HVPL = 15,
            PreOn = 17,
            // TODO: add all states
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
            NoStartConditions = 5,
            NoModules = 6,
            PLimit = 7,
            QLimit = 8,
            // TODO: add all warnings
        }


        /// <summary>
        /// Enumeration of fault (error) codes.
        /// </summary>
        public enum FaultCode
        {
            Unknown = -1,
            None = 0,
            Watchdog = 1,
            HwVbus = 2,
            Softcharge = 3,
            Discharge = 4,
            HighVac = 5,
            LowVac = 6,
            HighFrequency = 7,
            LowFrequency = 8,
            // TODO: add all faults
        }


        /// <summary>
        /// Enumeration of registers.
        /// </summary>
        public enum Register
        {
            //Control
            PControlMode = 40551,
            QControlMode = 40552,
            PReference = 40553,
            QReference = 40558,
            P = 41008,
            Q = 41009,
            S = 41010,

            // DC Contactor State
            DcBreakerState = 47813,

            // Warnings
            CurrentWarning = 41122,
            CurrentState = 41123,

            // Faults
            CurrentFault = 41121,
            CurrentFaultModule = 41131,
            ResetFaults = 43012,

            // Capabilities
            PCapability = 41141,
            QCapability = 41142,

            // State
            SelectorState = 42042,

            // TODO: add all registers
        }
    }
}
