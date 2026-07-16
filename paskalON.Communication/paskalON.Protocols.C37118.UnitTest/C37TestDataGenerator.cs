// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using System.Buffers.Binary;
using System.Text;

namespace paskalON.Protocols.C37118.UnitTest
{
    public static class C37TestDataGenerator
    {
        public static byte[] CreateMockConfigFrame(ushort stationId, List<string> phasorNames, List<string> analogNames)
        {
            using MemoryStream ms = new MemoryStream();

            // Placeholder for Header (14 bytes)
            byte[] headerPlaceholder = new byte[14];
            ms.Write(headerPlaceholder);
            // Global Configuration Data
            byte[] timeBaseBytes = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(timeBaseBytes, 1000000);
            ms.Write(timeBaseBytes);
            // PMU (just one PMU)
            byte[] numDevicesBytes = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(numDevicesBytes, 1);
            ms.Write(numDevicesBytes);
            // PMU Block Start
            // Station Name (16 bytes fixed width ASCII)
            byte[] nameBytes = new byte[16];
            Array.Fill(nameBytes, (byte)0x20);
            Encoding.ASCII.GetBytes("PMU-1").CopyTo(nameBytes, 0);
            ms.Write(nameBytes);
            // Station ID (2 bytes)
            byte[] stationIdBytes = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(stationIdBytes, stationId);
            ms.Write(stationIdBytes);
            // Format Word (2 bytes) -> 0x0007 (Floats for Phasor, Analog, Freq)
            byte[] formatWordBytes = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(formatWordBytes, 0x0007);
            ms.Write(formatWordBytes);
            // Channels Counts (2 bytes each)
            byte[] phasorCount = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(phasorCount, (ushort)phasorNames.Count);
            byte[] analogCount = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(analogCount, (ushort)analogNames.Count);
            // No digitals
            byte[] digitalCount = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(digitalCount, 0);
            ms.Write(phasorCount);
            ms.Write(analogCount);
            ms.Write(digitalCount);

            // Write phasors (16 bytes per channel entry)
            foreach (var phasorName in phasorNames)
            {
                byte[] channelNameBytes = new byte[16];
                Array.Fill(channelNameBytes, (byte)0x20);
                Encoding.ASCII.GetBytes(phasorName).CopyTo(channelNameBytes, 0);
                ms.Write(channelNameBytes);
            }

            // Write analog (16 bytes per channel entry)
            foreach (var analogName in analogNames)
            {
                byte[] channelNameBytes = new byte[16];
                Array.Fill(channelNameBytes, (byte)0x20);
                Encoding.ASCII.GetBytes(analogName).CopyTo(channelNameBytes, 0);
                ms.Write(channelNameBytes);
            }

            // Conversion Factors (4 bytes per channel item)
            // Phasor conversion factors
            for (int i = 0; i < phasorNames.Count; i++)
            {
                ms.Write(new byte[4]);
            }

            // Analog conversion factors
            for (int i = 0; i < analogNames.Count; i++)
            {
                ms.Write(new byte[4]);
            }

            // Frequency (Mandatory entry each per PMU block) (4 bytes)
            ms.Write(new byte[4]);

            // Data Reporting Rate (2 bytes) at the very end
            byte[] rateBytes = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(rateBytes, 30);
            ms.Write(rateBytes);

            // Checksum Placeholder (2 bytes)
            ms.Write(new byte[2]);

            byte[] finalFrame = ms.ToArray();

            // Overwrite Valid Header Values (Backpatching)
            ushort frameSize = (ushort)finalFrame.Length;
            finalFrame[0] = 0xAA;
            finalFrame[1] = 0x21; // Config Frame Type 2
            BinaryPrimitives.WriteUInt16BigEndian(finalFrame.AsSpan(2, 2), frameSize);
            BinaryPrimitives.WriteUInt16BigEndian(finalFrame.AsSpan(4, 2), stationId);

            return finalFrame;
        }


        public static byte[] CreateMockDataFrame(ushort streamId, List<(float Mag, float Ang)> phasorValues, List<float> analogValues, float frequency)
        {
            using MemoryStream ms = new MemoryStream();

            // Placeholder for Header (14 bytes)
            ms.Write(new byte[14]);
            // Data Values - Following IEEE C37.118 Structural sequence:
            // Phasor Data (Each phasor has 4 bytes Mag + 4 bytes Ang = 8 bytes total if float)
            foreach ((float Mag, float Ang) phasor in phasorValues)
            {
                byte[] magBytes = new byte[4];
                byte[] angBytes = new byte[4];
                BinaryPrimitives.WriteSingleBigEndian(magBytes, phasor.Mag);
                BinaryPrimitives.WriteSingleBigEndian(angBytes, phasor.Ang);
                ms.Write(magBytes);
                ms.Write(angBytes);
            }

            // In theory the IEEE C37.118 should always have at least one signal
            if (phasorValues.Count > 0 || analogValues.Count > 0)
            {
                // Frequency (4 bytes float)
                byte[] frequencyBytes = new byte[4];
                BinaryPrimitives.WriteSingleBigEndian(frequencyBytes, frequency);
                ms.Write(frequencyBytes);
            }

            // Analog Data (4 bytes float each)
            foreach (float analog in analogValues)
            {
                byte[] analogBytes = new byte[4];
                BinaryPrimitives.WriteSingleBigEndian(analogBytes, analog);
                ms.Write(analogBytes);
            }

            // Checksum Placeholder (2 bytes)
            ms.Write(new byte[2]);

            byte[] finalFrame = ms.ToArray();

            // Overwrite Valid Header Values
            ushort frameSize = (ushort)finalFrame.Length;
            finalFrame[0] = 0xAA;
            finalFrame[1] = 0x01; // Data Frame Type 0
            BinaryPrimitives.WriteUInt16BigEndian(finalFrame.AsSpan(2, 2), frameSize);
            BinaryPrimitives.WriteUInt16BigEndian(finalFrame.AsSpan(4, 2), streamId);

            return finalFrame;
        }
    }
}
