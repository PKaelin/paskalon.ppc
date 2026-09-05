// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Buffers.Binary;

namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// Common IEEE C37.118 frame representation.
    /// </summary>
    public abstract class C37CommonFrame
    {
        /// <summary>
        /// IEEE C37.118 sync byte.
        /// </summary>
        public const byte SyncByte = 0xAA;


        /// <summary>
        /// Length of the common header, excluding the CRC.
        /// </summary>
        public const int HeaderLength = 14;


        /// <summary>
        /// Length of the CRC field.
        /// </summary>
        public const int ChecksumLength = 2;


        /// <summary>
        /// Gets the encoded sync word.
        /// </summary>
        public ushort SyncWord { get; }


        /// <summary>
        /// Gets the frame type.
        /// </summary>
        public C37FrameType FrameType { get; }


        /// <summary>
        /// Gets the protocol version encoded in the sync word.
        /// </summary>
        public byte Version { get; }


        /// <summary>
        /// Gets the total frame length, including the CRC.
        /// </summary>
        public ushort FrameSize { get; }


        /// <summary>
        /// Gets the PMU or PDC stream identifier.
        /// </summary>
        public ushort StreamIdCode { get; }


        /// <summary>
        /// Gets the seconds-of-century timestamp field.
        /// </summary>
        /// <remarks>
        /// Gets the number of seconds since the beginning of 1970 UTC.
        /// </remarks>
        public uint SecondOfCentury { get; }


        /// <summary>
        /// Gets the fractional-second and time-quality field.
        /// </summary>
        public uint FractionOfSecond { get; }


        /// <summary>
        /// Gets the frame payload without the common header or CRC.
        /// </summary>
        public ReadOnlyMemory<byte> Payload { get; }


        /// <summary>
        /// Gets the CRC transmitted with the frame.
        /// </summary>
        public ushort Checksum { get; }


        /// <summary>
        /// Gets the complete frame bytes.
        /// </summary>
        public ReadOnlyMemory<byte> RawBytes { get; }


        /// <summary>
        /// Gets the timestamp represented by the seconds field.
        /// </summary>
        public DateTimeOffset Timestamp => DateTimeOffset.UnixEpoch.AddSeconds(SecondOfCentury);


        /// <summary>
        /// Constructor of <see cref="C37CommonFrame"/>.
        /// </summary>
        protected C37CommonFrame(ReadOnlyMemory<byte> frameBytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(frameBytes.Length, HeaderLength + ChecksumLength);

            ReadOnlySpan<byte> bytes = frameBytes.Span;
            SyncWord = BinaryPrimitives.ReadUInt16BigEndian(bytes[0..2]);
            ArgumentOutOfRangeException.ThrowIfNotEqual(bytes[0], SyncByte);
            FrameSize = BinaryPrimitives.ReadUInt16BigEndian(bytes[2..4]);
            ArgumentOutOfRangeException.ThrowIfNotEqual(FrameSize, frameBytes.Length);
            StreamIdCode = BinaryPrimitives.ReadUInt16BigEndian(bytes[4..6]);
            SecondOfCentury = BinaryPrimitives.ReadUInt32BigEndian(bytes[6..10]);
            FractionOfSecond = BinaryPrimitives.ReadUInt32BigEndian(bytes[10..14]);
            FrameType = (C37FrameType)((SyncWord >> 4) & 0x0F);
            Version = (byte)(SyncWord & 0x0F);
            Checksum = BinaryPrimitives.ReadUInt16BigEndian(bytes[^ChecksumLength..]);
            ushort calculatedChecksum = C37FrameCodec.ComputeChecksum(bytes[..^ChecksumLength]);
            ArgumentOutOfRangeException.ThrowIfNotEqual(Checksum, calculatedChecksum);
            RawBytes = frameBytes.ToArray();
            Payload = RawBytes.Slice(HeaderLength, FrameSize - HeaderLength - ChecksumLength);
        }


        /// <summary>
        /// Constructor of <see cref="C37CommonFrame"/>.
        /// </summary>
        protected C37CommonFrame(ReadOnlySpan<byte> headerBytes)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(headerBytes.Length, HeaderLength);

            SyncWord = BinaryPrimitives.ReadUInt16BigEndian(headerBytes[0..2]);
            ArgumentOutOfRangeException.ThrowIfNotEqual(headerBytes[0], SyncByte);
            FrameSize = BinaryPrimitives.ReadUInt16BigEndian(headerBytes[2..4]);
            StreamIdCode = BinaryPrimitives.ReadUInt16BigEndian(headerBytes[4..6]);
            SecondOfCentury = BinaryPrimitives.ReadUInt32BigEndian(headerBytes[6..10]);
            FractionOfSecond = BinaryPrimitives.ReadUInt32BigEndian(headerBytes[10..14]);
            FrameType = (C37FrameType)((SyncWord >> 4) & 0x0F);
            Version = (byte)(SyncWord & 0x0F);
            Payload = ReadOnlyMemory<byte>.Empty;
            RawBytes = headerBytes.ToArray();
        }


        /// <summary>
        /// Constructor of <see cref="C37CommonFrame"/>.
        /// </summary>
        protected C37CommonFrame(C37FrameType frameType, byte version, ushort streamIdCode, uint secondOfCentury, uint fractionOfSecond, ReadOnlyMemory<byte> payload)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan((int)frameType, 15);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(version, 15);

            int frameSize = HeaderLength + payload.Length + ChecksumLength;
            ArgumentOutOfRangeException.ThrowIfGreaterThan(frameSize, ushort.MaxValue);

            FrameType = frameType;
            Version = version;
            SyncWord = (ushort)(SyncByte << 8 | ((byte)frameType << 4) | version);
            FrameSize = (ushort)frameSize;
            StreamIdCode = streamIdCode;
            SecondOfCentury = secondOfCentury;
            FractionOfSecond = fractionOfSecond;
            Payload = payload.ToArray();
            RawBytes = BuildBytes();
            Checksum = BinaryPrimitives.ReadUInt16BigEndian(RawBytes.Span[^ChecksumLength..]);
        }


        /// <summary>
        /// Build raw byte interpretation.
        /// </summary>
        /// <returns>Byte array.</returns>
        private byte[] BuildBytes()
        {
            byte[] bytes = new byte[FrameSize];
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(0, 2), SyncWord);
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(2, 2), FrameSize);
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(4, 2), StreamIdCode);
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(6, 4), SecondOfCentury);
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(10, 4), FractionOfSecond);
            Payload.Span.CopyTo(bytes.AsSpan(HeaderLength));
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(^ChecksumLength), C37FrameCodec.ComputeChecksum(bytes.AsSpan(0, bytes.Length - ChecksumLength)));

            return bytes;
        }
    }
}