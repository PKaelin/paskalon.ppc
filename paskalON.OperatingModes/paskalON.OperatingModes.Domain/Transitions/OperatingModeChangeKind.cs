// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Transitions
{
    /// <summary>
    /// Kind of a committed change of the operating mode stack or the override slot.
    /// </summary>
    public enum OperatingModeChangeKind
    {
        /// <summary>
        /// The operating mode was added.
        /// </summary>
        Added,

        /// <summary>
        /// The operating mode was removed.
        /// </summary>
        Removed,

        /// <summary>
        /// The operating mode replaced its predecessor.
        /// </summary>
        Replaced,
    }
}
