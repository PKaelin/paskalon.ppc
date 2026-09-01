// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs
{
    /// <summary>
    /// Type of the operating mode.
    /// </summary>
    /// <remarks>
    /// Same operating mode can be used by multiple systems like MaintenanceOperatingMode, MpptOperatingMode (Maximum Power Point Tracking), etc.
    /// As they are flags they can be used like Bess|Solar to define that they can be used for both BESS and Solar systems.
    /// </remarks>
    public enum OperatingModeType
    {
        /// <summary>
        /// Battery energy storage type: 0000000001
        /// </summary>
        Bess = 0x01,
        /// <summary>
        /// Solar energy type: 0000000010
        /// </summary>
        Solar = 0x02,
        /// <summary>
        /// Wind energy type: 0000000100
        /// </summary>
        Wind = 0x04,
        /// <summary>
        /// Nuclear energy type: 0000001000
        /// </summary>
        Nuclear = 0x08,
        /// <summary>
        /// Hydro energy type: 0000010000
        /// </summary>
        Hydro = 0x10,
        /// <summary>
        /// Gas energy type: 0000100000
        /// </summary>
        Gas = 0x20,
    }
}
