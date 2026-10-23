// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Stacks
{
    /// <summary>
    /// Operating mode stack type is analog to the operating mode abstractions in the domain.
    /// </summary>
    [Flags]
    public enum OperatingModeStackTypeDto
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,


        /// <summary>
        /// Has additive active mode interface implemented.
        /// </summary>
        AdditiveActive = 1 << 0,


        /// <summary>
        /// Has additive reactive mode interface implemented.
        /// </summary>
        AdditiveReactive = 1 << 1,


        /// <summary>
        /// Has base active mode interface implemented.
        /// </summary>
        BaseActive = 1 << 2,


        /// <summary>
        /// Has base reactive mode interface implemented.
        /// </summary>
        BaseReactive = 1 << 3,


        /// <summary>
        /// Has exclusive active mode interface implemented.
        /// </summary>
        ExclusiveActive = 1 << 4,


        /// <summary>
        /// Has exclusive reactive mode interface implemented.
        /// </summary>
        ExclusiveReactive = 1 << 5,


        /// <summary>
        /// Has limiting active mode interface implemented.
        /// </summary>
        LimitingActive = 1 << 6,


        /// <summary>
        /// Has limiting reactive mode interface implemented.
        /// </summary>
        LimitingReactive = 1 << 7,
    }
}
