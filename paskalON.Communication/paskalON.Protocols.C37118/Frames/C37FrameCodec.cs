// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Protocols.C37118.Simulations;
using System.Buffers.Binary;
using System.Text;

namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// Creates wire frames used by the C37.118 transport implementation.
    /// </summary>
    public static class C37FrameCodec
    {
        /// <summary>
        /// Header length.
        /// </summary>
        private const int HeaderLength = 14;


        /// <summary>
        /// Creates a configuration frame for the supplied simulations.
        /// </summary>
        public static byte[] CreateConfigurationFrame(IReadOnlyList<IPmuDataSimulation> simulations, ushort dataRate)
        {
            ArgumentNullException.ThrowIfNull(simulations);

            using MemoryStream payload = new MemoryStream();
            WriteUInt32(payload, 1000000);
            WriteUInt16(payload, checked((ushort)simulations.Count));

            foreach (IPmuDataSimulation simulation in simulations)
            {
                WriteName(payload, $"PMU");
                WriteUInt16(payload, simulation.StreamId);
                WriteUInt16(payload, 0x0007);
                WriteUInt16(payload, checked((ushort)simulation.Phasors.Count));
                WriteUInt16(payload, checked((ushort)simulation.Analogs.Count));
                WriteUInt16(payload, 0);

                foreach (PhasorMeasurement phasor in simulation.Phasors)
                {
                    WriteName(payload, phasor.Name);
                }

                foreach (AnalogMeasurement analog in simulation.Analogs)
                {
                    WriteName(payload, analog.Name);
                }

                WriteName(payload, "FREQUENCY");
                WriteName(payload, "ROCOF");

                for (int index = 0; index < simulation.Phasors.Count + simulation.Analogs.Count; index++)
                {
                    WriteUInt32(payload, 0);
                }

                WriteUInt16(payload, 0);
                WriteUInt16(payload, 0);
            }

            WriteUInt16(payload, dataRate);

            return CreateFrame(0xAA21, simulations.Count == 0 ? (ushort)0 : simulations[0].StreamId, payload.ToArray());
        }


        /// <summary>
        /// Creates a data frame containing the current simulation values.
        /// </summary>
        public static byte[] CreateDataFrame(IPmuDataSimulation simulation)
        {
            ArgumentNullException.ThrowIfNull(simulation);

            using MemoryStream payload = new MemoryStream();
            WriteUInt16(payload, 0);

            foreach (PhasorMeasurement phasor in simulation.Phasors)
            {
                WriteSingle(payload, phasor.Magnitude);
                WriteSingle(payload, phasor.Angle);
            }

            WriteSingle(payload, simulation.Frequency);
            WriteSingle(payload, simulation.FrequencyRateOfChange);

            foreach (AnalogMeasurement analog in simulation.Analogs)
            {
                WriteSingle(payload, analog.Measurement);
            }

            return CreateFrame(0xAA01, simulation.StreamId, payload.ToArray());
        }


        /// <summary>
        /// Creates a command frame.
        /// </summary>
        public static byte[] CreateCommandFrame(ushort streamId, ushort command)
        {
            using MemoryStream payload = new MemoryStream();
            WriteUInt16(payload, command);

            return CreateFrame(0xAA41, streamId, payload.ToArray());
        }


        /// <summary>
        /// Computes the CRC-CCITT checksum used by C37.118 frames.
        /// </summary>
        public static ushort ComputeChecksum(ReadOnlySpan<byte> data)
        {
            ushort crc = 0xFFFF;

            foreach (byte value in data)
            {
                crc ^= (ushort)(value << 8);
                for (int bit = 0; bit < 8; bit++)
                {
                    crc = (ushort)((crc & 0x8000) is not 0 ? (crc << 1) ^ 0x1021 : crc << 1);
                }
            }

            return crc;
        }


        private static byte[] CreateFrame(ushort syncWord, ushort streamId, byte[] payload)
        {
            byte[] frame = new byte[HeaderLength + payload.Length + 2];
            BinaryPrimitives.WriteUInt16BigEndian(frame.AsSpan(0, 2), syncWord);
            BinaryPrimitives.WriteUInt16BigEndian(frame.AsSpan(2, 2), checked((ushort)frame.Length));
            BinaryPrimitives.WriteUInt16BigEndian(frame.AsSpan(4, 2), streamId);
            BinaryPrimitives.WriteUInt32BigEndian(frame.AsSpan(6, 4), checked((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            BinaryPrimitives.WriteUInt32BigEndian(frame.AsSpan(10, 4), 0);
            payload.CopyTo(frame, HeaderLength);
            BinaryPrimitives.WriteUInt16BigEndian(frame.AsSpan(frame.Length - 2), ComputeChecksum(frame.AsSpan(0, frame.Length - 2)));

            return frame;
        }


        private static void WriteName(Stream stream, string value)
        {
            byte[] bytes = new byte[16];
            Array.Fill(bytes, (byte)' ');
            Encoding.ASCII.GetBytes(value[..Math.Min(value.Length, 16)]).CopyTo(bytes, 0);
            stream.Write(bytes);
        }


        private static void WriteUInt16(Stream stream, ushort value)
        {
            Span<byte> buffer = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
            stream.Write(buffer);
        }


        private static void WriteUInt32(Stream stream, uint value)
        {
            Span<byte> buffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
            stream.Write(buffer);
        }


        private static void WriteSingle(Stream stream, float value)
        {
            Span<byte> buffer = stackalloc byte[4];
            BinaryPrimitives.WriteSingleBigEndian(buffer, value);
            stream.Write(buffer);
        }
    }
}
