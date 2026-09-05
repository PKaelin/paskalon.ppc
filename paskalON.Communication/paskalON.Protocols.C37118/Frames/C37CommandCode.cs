// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// Standard C37.118 command codes.
    /// </summary>
    public enum C37CommandCode : ushort
    {
        /// <summary>
        /// Turn data transmission off.
        /// </summary>
        TurnOff = 0x0000,
        /// <summary>
        /// Turn data transmission on.
        /// </summary>
        TurnOn = 0x0001,
        /// <summary>
        /// Request configuration frame 1.
        /// </summary>
        SendConfiguration1 = 0x0002,
        /// <summary>
        /// Request configuration frame 2.
        /// </summary>
        SendConfiguration2 = 0x0003,
        /// <summary>
        /// Request configuration frame 3.
        /// </summary>
        SendConfiguration3 = 0x0004,
        /// <summary>
        /// Extended command.
        /// </summary>
        Extended = 0x0008
    }
}
