// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Buffers.Binary;
using System.Numerics;

namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// Decoded C37.118 real-time data frame.
    /// </summary>
    public sealed class C37DataFrame : C37CommonFrame
    {
        /// <summary>
        /// Gets the status word.
        /// </summary>
        public ushort Status { get; }


        /// <summary>
        /// Gets the remaining payload values in wire order.
        /// </summary>
        public IReadOnlyList<Complex> Phasors { get; }


        /// <summary>
        /// Gets the frequency value.
        /// </summary>
        public double Frequency { get; }


        /// <summary>
        /// Gets the rate of change of frequency.
        /// </summary>
        public double RateOfChangeOfFrequency { get; }


        /// <summary>
        /// Gets the analog values.
        /// </summary>
        public IReadOnlyList<double> Analogs { get; }


        /// <summary>
        /// Gets the digital status words.
        /// </summary>
        public IReadOnlyList<ushort> Digitals { get; }


        /// <summary>
        /// Constructor of <see cref="C37DataFrame"/>
        /// </summary>
        public C37DataFrame(ReadOnlyMemory<byte> frameBytes, int phasorCount, int analogCount, int digitalCount)
            : this(frameBytes, phasorCount, analogCount, digitalCount, true)
        {
        }


        /// <summary>
        /// Constructor of <see cref="C37DataFrame"/>
        /// </summary>
        public C37DataFrame(ReadOnlyMemory<byte> frameBytes, int phasorCount, int analogCount, int digitalCount, bool floatingPoint)
            : base(frameBytes)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(FrameType, C37FrameType.Data);
            ArgumentOutOfRangeException.ThrowIfNegative(phasorCount);
            ArgumentOutOfRangeException.ThrowIfNegative(analogCount);
            ArgumentOutOfRangeException.ThrowIfNegative(digitalCount);
            int cursor = 0;
            ReadOnlySpan<byte> payload = Payload.Span;
            Status = ReadUInt16(payload, ref cursor);
            List<Complex> phasors = [];

            for (int index = 0; index < phasorCount; index++)
            {
                phasors.Add(new Complex(ReadValue(payload, ref cursor, floatingPoint), ReadValue(payload, ref cursor, floatingPoint)));
            }

            Phasors = phasors;
            Frequency = ReadValue(payload, ref cursor, floatingPoint);
            RateOfChangeOfFrequency = ReadValue(payload, ref cursor, floatingPoint);
            List<double> analogs = [];

            for (int index = 0; index < analogCount; index++)
            {
                analogs.Add(ReadValue(payload, ref cursor, floatingPoint));
            }

            Analogs = analogs;
            List<ushort> digitals = [];

            for (int index = 0; index < digitalCount; index++)
            {
                digitals.Add(ReadUInt16(payload, ref cursor));
            }

            Digitals = digitals;

            ArgumentOutOfRangeException.ThrowIfNotEqual(cursor, payload.Length);
        }


        private static ushort ReadUInt16(ReadOnlySpan<byte> payload, ref int cursor)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(payload.Length - cursor, 2);

            ushort value = BinaryPrimitives.ReadUInt16BigEndian(payload.Slice(cursor, 2));
            cursor += 2;
            return value;
        }


        private static double ReadValue(ReadOnlySpan<byte> payload, ref int cursor, bool floatingPoint)
        {
            if (floatingPoint)
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(payload.Length - cursor, 4);

                float value = BinaryPrimitives.ReadSingleBigEndian(payload.Slice(cursor, 4));
                cursor += 4;
                return value;
            }

            return ReadUInt16(payload, ref cursor);
        }
    }
}
