// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118.Configs;
using System.Buffers.Binary;

namespace paskalON.Protocols.C37118.Frames
{
    /// <summary>
    /// C37 configuration frame event argument.
    /// </summary>
    public class C37ConfigFrameEventArgs : EventArgs
    {
        /// <summary>
        /// C37 frame header.
        /// </summary>
        public C37HeaderFrame Header { get; }


        /// <summary>
        /// Raw payload
        /// </summary>
        public ReadOnlyMemory<byte> RawPayload { get; }


        /// <summary>
        /// C37 configuration blueprint.
        /// </summary>
        public C37ConfigBlueprint Blueprint { get; }


        /// <summary>
        /// Constructor of <see cref="C37ConfigFrameEventArgs"/>.
        /// </summary>
        /// <param name="fullFrameBytes">Full frame payload.</param>
        public C37ConfigFrameEventArgs(ReadOnlyMemory<byte> fullFrameBytes)
        {
            Header = new C37HeaderFrame(fullFrameBytes);
            // Slice payload without copying array data
            RawPayload = fullFrameBytes.Slice(14, Header.FrameSize - 16);
            Blueprint = ParseToBlueprint(RawPayload.Span);
        }


        private C37ConfigBlueprint ParseToBlueprint(ReadOnlySpan<byte> payload)
        {
            C37ConfigBlueprint blueprint = new C37ConfigBlueprint { StreamIdCode = Header.StreamIdCode };

            // Parse global data stream fields
            uint timeBase = BinaryPrimitives.ReadUInt32BigEndian(payload[0..4]);
            // In IEEE C37.118 standard a device count of 0 is structurally invalid for a configuration frame
            ushort numberOfDevices = BinaryPrimitives.ReadUInt16BigEndian(payload[4..6]);

            // Track a sliding pointer coordinate across the raw configuration stream payload
            int cursor = 6;
            int dataFrameOffset = 0;

            // Iterate through each nested PMU configuration block sequentially
            for (int i = 0; i < numberOfDevices; i++)
            {
                PmuLayoutMetadata pmu = new PmuLayoutMetadata();

                // Unpack station name: 16 byte fixed width ASCII string with trailing spaces padded
                ReadOnlySpan<byte> nameBytes = payload.Slice(cursor, 16);
                pmu.StationName = System.Text.Encoding.ASCII.GetString(nameBytes).TrimEnd();
                cursor += 16;

                // Unpack metadata tracking and count loops
                pmu.StreamId = BinaryPrimitives.ReadUInt16BigEndian(payload.Slice(cursor, 2));
                cursor += 2;

                ushort formatWord = BinaryPrimitives.ReadUInt16BigEndian(payload.Slice(cursor, 2));
                cursor += 2;

                // Dissect the format bitmask to establish float vs short sizing footprints
                pmu.PhasorDataType = (formatWord & 0x0001) != 0 ? C37DataType.Float : C37DataType.Short;
                pmu.AnalogDataType = (formatWord & 0x0002) != 0 ? C37DataType.Float : C37DataType.Short;
                pmu.FrequencyDataType = (formatWord & 0x0004) != 0 ? C37DataType.Float : C37DataType.Short;
                pmu.DigitalDataType = (formatWord & 0x0008) != 0 ? C37DataType.Float : C37DataType.Short;

                pmu.NumberOfPhasors = BinaryPrimitives.ReadUInt16BigEndian(payload.Slice(cursor, 2));
                cursor += 2;
                pmu.NumberOfAnalogs = BinaryPrimitives.ReadUInt16BigEndian(payload.Slice(cursor, 2));
                cursor += 2;
                pmu.NumberOfDigitals = BinaryPrimitives.ReadUInt16BigEndian(payload.Slice(cursor, 2));
                cursor += 2;

                // Track total channels to correctly skip string array data tables afterwards
                int totalChannelCount = pmu.NumberOfPhasors + pmu.NumberOfAnalogs + (pmu.NumberOfDigitals * 16);

                // Read phasor
                for (int phasor = 0; phasor < pmu.NumberOfPhasors; phasor++)
                {
                    // A single phasor in C37 is a complex number with magnitude and phase angle of a sinusoidal voltage or current
                    string rawName = System.Text.Encoding.ASCII.GetString(payload.Slice(cursor, 16)).TrimEnd();
                    blueprint.ChannelMap[rawName] = new C37ChannelEntry(pmu.StreamId, C37SignalType.Phasor, phasor);
                    cursor += 16;
                }

                // Read analogs
                for (int analog = 0; analog < pmu.NumberOfAnalogs; analog++)
                {
                    string rawName = System.Text.Encoding.ASCII.GetString(payload.Slice(cursor, 16)).TrimEnd();
                    blueprint.ChannelMap[rawName] = new C37ChannelEntry(pmu.StreamId, C37SignalType.Analog, analog);
                    cursor += 16;
                }

                // Read digitals
                for (int digital = 0; digital < pmu.NumberOfDigitals * 16; digital++)
                {
                    string rawName = System.Text.Encoding.ASCII.GetString(payload.Slice(cursor, 16)).TrimEnd();
                    int wordIndex = digital / 16;
                    int bitPosition = digital % 16;
                    blueprint.ChannelMap[rawName] = new C37ChannelEntry(pmu.StreamId, C37SignalType.Digital, wordIndex, bitPosition);
                    cursor += 16;
                }

                // Read frequency and ROCOF
                blueprint.ChannelMap["FREQUENCY"] = new C37ChannelEntry(pmu.StreamId, C37SignalType.Frequency, 0);
                cursor += 16;
                // blueprint.ChannelMap["ROCOF"] = new C37ChannelEntry(pmu.StreamId, C37SignalType.RateOfChangeOfFrequency, 0);
                cursor += 16;

                // Skip over the structural Conversion Factors table (4 bytes per Phasor, 4 bytes per Analog, 4 bytes per Digital)
                int factorBlockSize = (pmu.NumberOfPhasors * 4) + (pmu.NumberOfAnalogs * 4) + (pmu.NumberOfDigitals * 4);
                cursor += factorBlockSize;

                // Frequency Conversion Factors
                cursor += 2;
                // ROCOF Conversion Factors
                cursor += 2;

                // Assign the resolved position of this PMU within the data frame packet
                pmu.PmuDataStartOffset = dataFrameOffset;
                dataFrameOffset += pmu.TotalPmuLengthBytes;

                blueprint.Pmus.Add(pmu);
            }

            // Skip the 2-byte Data Reporting Rate at the very end of the payload to complete full alignment
            ushort dataRate = BinaryPrimitives.ReadUInt16BigEndian(payload.Slice(cursor, 2));

            return blueprint;
        }
    }
}
