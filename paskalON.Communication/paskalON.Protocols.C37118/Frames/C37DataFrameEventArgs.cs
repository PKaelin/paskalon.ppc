// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// C37 data frame event argument.
    /// </summary>
    public class C37DataFrameEventArgs : EventArgs
    {
        /// <summary>
        /// Header frame.
        /// </summary>
        public C37HeaderFrame Header { get; }


        /// <summary>
        /// Raw payload.
        /// </summary>
        public ReadOnlyMemory<byte> RawPayload { get; }


        /// <summary>
        /// Constructor of <see cref="C37DataFrameEventArgs"/>.
        /// </summary>
        /// <param name="fullFrameBytes">Full frame payload.</param>
        public C37DataFrameEventArgs(ReadOnlyMemory<byte> fullFrameBytes)
        {
            Header = new C37HeaderFrame(fullFrameBytes);
            // Actual payload minus the checksum at the end
            RawPayload = fullFrameBytes.Slice(14, Header.FrameSize - 16);
        }
    }
}
