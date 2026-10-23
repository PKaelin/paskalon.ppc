// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Domains.Configs
{
    /// <summary>
    /// Type of the power control.
    /// </summary>
    /// <remarks>
    /// Same power control can be used by multiple systems like PowerConstraintConfig, DerUnitPowerConstraintConfig, etc.
    /// As they are flags they can be used like Bess|Solar to define that they can be used for both BESS and Solar systems.
    /// </remarks>
    [Flags]
    public enum PowerControlType
    {
        /// <summary>
        /// No power control type defined: 0000000000
        /// </summary>
        None = 0,
        /// <summary>
        /// Battery energy storage type: 0000000001
        /// </summary>
        Bess = 1 << 0,
        /// <summary>
        /// Solar energy type: 0000000010
        /// </summary>
        Solar = 1 << 1,
        /// <summary>
        /// Wind energy type: 0000000100
        /// </summary>
        Wind = 1 << 2,
        /// <summary>
        /// Nuclear energy type: 0000001000
        /// </summary>
        Nuclear = 1 << 3,
        /// <summary>
        /// Hydro energy type: 0000010000
        /// </summary>
        Hydro = 1 << 4,
        /// <summary>
        /// Gas energy type: 0000100000
        /// </summary>
        Gas = 1 << 5,
    }
}
