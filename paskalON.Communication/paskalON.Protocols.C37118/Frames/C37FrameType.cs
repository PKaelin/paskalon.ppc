// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// Enum for C37.118 frame type encoded in the sync word.</summary>
    public enum C37FrameType : byte
    {
        /// <summary>
        /// Real-time measurement data.
        /// </summary>
        Data = 0,
        /// <summary>
        /// Human-readable header data.
        /// </summary>
        Header = 1,
        /// <summary>
        /// Configuration 1.
        /// </summary>
        Configuration1 = 2,
        /// <summary>
        /// Configuration 2.
        /// </summary>
        Configuration2 = 3,
        /// <summary>
        /// Command or control frame.
        /// </summary>
        Command = 4,
        /// <summary>
        /// Configuration 3.
        /// </summary>
        Configuration3 = 5
    }
}
