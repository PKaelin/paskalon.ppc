// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// Represents the common IEEE C37.118 frame header.
    /// </summary>
    public sealed class C37HeaderFrame : C37CommonFrame
    {
        /// <summary>
        /// Constructor of <see cref="C37HeaderFrame"/>
        /// </summary>
        public C37HeaderFrame(ReadOnlyMemory<byte> frameBytes) : base(frameBytes.Span[0..14])
        {
        }


        /// <summary>
        /// Constructor of <see cref="C37HeaderFrame"/>
        /// </summary>
        public C37HeaderFrame(C37FrameType frameType, ushort streamIdCode, uint secondOfCentury, uint fractionOfSecond, ReadOnlyMemory<byte> payload, byte version = 1)
            : base(frameType, version, streamIdCode, secondOfCentury, fractionOfSecond, payload)
        {
        }
    }
}