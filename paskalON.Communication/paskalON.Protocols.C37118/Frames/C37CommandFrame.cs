// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Buffers.Binary;

namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// Decoded IEEE C37.118 command frame.
    /// </summary>
    public sealed class C37CommandFrame : C37CommonFrame
    {
        /// <summary>
        /// Gets the command code.
        /// </summary>
        public C37CommandCode CommandCode { get; }


        /// <summary>
        /// Gets command-specific trailing bytes.
        /// </summary>
        public ReadOnlyMemory<byte> Parameters { get; }


        /// <summary>
        /// Constructor of <see cref="C37CommandFrame"/>.
        /// </summary>
        public C37CommandFrame(ReadOnlyMemory<byte> frameBytes) : base(frameBytes)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(FrameType, C37FrameType.Command);
            ArgumentOutOfRangeException.ThrowIfLessThan(Payload.Length, 2);

            CommandCode = (C37CommandCode)BinaryPrimitives.ReadUInt16BigEndian(Payload.Span[0..2]);
            Parameters = Payload[2..];
        }


        /// <summary>
        /// Constructor of <see cref="C37CommandFrame"/>.
        /// </summary>
        public C37CommandFrame(ushort streamIdCode, C37CommandCode commandCode, ReadOnlyMemory<byte> parameters = default)
            : base(C37FrameType.Command, 1, streamIdCode, 0, 0, BuildPayload(commandCode, parameters))
        {
            CommandCode = commandCode;
            Parameters = parameters.ToArray();
        }


        /// <summary>
        /// Build payload from bytes.
        /// </summary>
        /// <param name="commandCode">The command code.</param>
        /// <param name="parameters">The parameters passed in.</param>
        /// <returns></returns>
        private static byte[] BuildPayload(C37CommandCode commandCode, ReadOnlyMemory<byte> parameters)
        {
            byte[] payload = new byte[2 + parameters.Length];
            BinaryPrimitives.WriteUInt16BigEndian(payload.AsSpan(0, 2), (ushort)commandCode);
            parameters.Span.CopyTo(payload.AsSpan(2));

            return payload;
        }
    }
}
