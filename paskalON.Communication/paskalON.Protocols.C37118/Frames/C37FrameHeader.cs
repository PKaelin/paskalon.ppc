// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Buffers.Binary;

namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// In IEEE C37.118 synchrophasor protocol every message frame (Data, Configuration, Header, Command)
    /// begins with a mandatory 16-byte frame header.
    /// </summary>
    public readonly struct C37FrameHeader
    {
        /// <summary>
        /// Sync Word (2 bytes):
        /// Byte 1: Always $0xAA$.
        /// Byte 2: Indicates the frame type and protocol version.
        /// Bits 4-6 specify the frame type: 0 (Data), 1 (Header), 2 / 3 / 5 (Configuration), and 4 (Command).
        /// </summary>      
        public ushort SyncWord { get; }


        /// <summary>
        /// Frame Size (2 bytes): Specifies the total number of bytes in the entire message frame
        /// including the header and the final checksum.
        /// </summary>
        public ushort FrameSize { get; }


        /// <summary>
        /// A unique 16-bit identifier for the PMU (Phasor Measurement Unit) or PDC (Phasor Data Concentrator) sending the message.
        /// </summary>
        public ushort StreamIdCode { get; }


        /// <summary>
        /// Second Of Century (4 bytes): Seconds Of Century. The standard Unix-style time format tracking
        /// the number of seconds since January 1, 1970.
        /// </summary>
        public uint SecondOfCentury { get; }


        /// <summary>
        /// Fraction Of Second (4 bytes): Fraction of a Second. This field contains the precise fractional second
        /// and also includes internal flags for Time Quality (e.g., Leap Second, Time Sync Error).
        /// </summary>
        public uint FractionOfSecond { get; }


        /// <summary>
        /// Date time offset representation of the Second Of Century.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public DateTimeOffset Timestamp { get; }


        /// <summary>
        /// Constructor of <see cref="C37FrameHeader"/>.
        /// </summary>
        /// <param name="headerBytes">The header bytes.</param>
        public C37FrameHeader(ReadOnlySpan<byte> headerBytes)
        {
            // Parse Big-Endian safely from network stream
            SyncWord = BinaryPrimitives.ReadUInt16BigEndian(headerBytes[0..2]);
            FrameSize = BinaryPrimitives.ReadUInt16BigEndian(headerBytes[2..4]);
            StreamIdCode = BinaryPrimitives.ReadUInt16BigEndian(headerBytes[4..6]);
            SecondOfCentury = BinaryPrimitives.ReadUInt32BigEndian(headerBytes[6..10]);
            FractionOfSecond = BinaryPrimitives.ReadUInt32BigEndian(headerBytes[10..14]);
            /// Deliberately leave out FractionOfSecond for the time stamp for performance reason.
            Timestamp = DateTimeOffset.UnixEpoch.AddSeconds(SecondOfCentury);
        }
    }
}