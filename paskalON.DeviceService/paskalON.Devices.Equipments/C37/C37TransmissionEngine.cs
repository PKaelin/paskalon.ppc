// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Configs;
using paskalON.Protocols.C37118.Frames;
using System.Buffers.Binary;

namespace paskalON.Devices.Equipments.C37
{
    public class C37TransmissionEngine
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// The C37 client.
        /// </summary>
        private readonly IC37Client _client;


        /// <summary>
        /// The C37 dataface for loose coupling.
        /// </summary>
        private readonly IC37Dataface _dataface;



        /// <summary>
        /// Cache C37 to register mappings.
        /// </summary>
        private readonly List<C37RegisterMapEntry> _mappings = new List<C37RegisterMapEntry>();


        /// <summary>
        /// Cache entries but use a volatile reference instead of locks around mappings.
        /// </summary>
        private volatile C37RegisterMapEntry[] _runtimeMappings = Array.Empty<C37RegisterMapEntry>();


        /// <summary>
        /// Constructor of <see cref="C37TransmissionEngine"/>.
        /// </summary>
        /// <param name="client">The C37 client interface.</param>
        /// <param name="dataface">The C37 data face.</param>
        public C37TransmissionEngine(ILogger logger, IC37Client client, IC37Dataface dataface)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);

            _logger = logger;
            _dataface = dataface;
            _client = client;
            _client.ConfigFrameReceived += OnConfigFrameReceived;
            _client.DataFrameReceived += OnDataFrameReceived;
        }


        /// <summary>
        /// Triggered when a configuration frame has been received.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The C37 configuration frame event argument.</param>
        private void OnConfigFrameReceived(object? sender, C37ConfigFrameEventArgs e)
        {
            C37ConfigBlueprint activeBlueprint = e.Blueprint;
            _mappings.Clear();

            // Map registers to protocol structural coordinates by matching names            
            foreach (IC37RegisterEntry register in _dataface.Registers)
            {
                if (activeBlueprint.ChannelMap.TryGetValue(register.Name, out C37ChannelEntry? channelEntry))
                {
                    // Find matching layout specifications for structural byte offset resolution
                    PmuLayoutMetadata layout = activeBlueprint.Pmus.First(p => p.StationId == channelEntry.TargetPmuId);
                    // Create the map entry and set the PMU data offset
                    _mappings.Add(new C37RegisterMapEntry(register, channelEntry, layout));
                }
                else
                {
                    _logger.LogInformation("C37 config frame contains unbound register named {RegisterName}", channelEntry);
                }
            }

            _runtimeMappings = _mappings.ToArray();
        }


        /// <summary>
        /// Triggered when a data frame has been received.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The C37 data frame event argument.</param>
        private void OnDataFrameReceived(object? sender, C37DataFrameEventArgs e)
        {
            // Capture a local reference snapshot of the current array instance
            C37RegisterMapEntry[] mappings = _runtimeMappings;

            // No configuration frame has been received yet
            if (mappings.Length == 0)
            {
                return;
            }

            ReadOnlySpan<byte> payloadSpan = e.RawPayload.Span;

            // Iteration using direct pointer manipulation structures
            for (int i = 0; i < mappings.Length; i++)
            {
                C37RegisterMapEntry entry = mappings[i];

                // Slice the data frame segment belonging to this specific target PMU device
                ReadOnlySpan<byte> pmuSegment = payloadSpan.Slice(entry.Layout.PmuDataStartOffset, entry.Layout.TotalPmuLengthBytes);

                object parsedValue = ExtractValue(pmuSegment, entry);
                entry.Register.Update(parsedValue);
            }
        }


        /// <summary>
        /// Extract value from segment
        /// </summary>
        /// <param name="pmuSegment">The binary data.</param>
        /// <param name="entry">The C37 register map entry.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private object ExtractValue(ReadOnlySpan<byte> pmuSegment, C37RegisterMapEntry entry)
        {
            // The IEEE C37.118 protocol is strictly and always big endian
            switch (entry.Register.SignalType)
            {
                case C37SignalType.Frequency:
                    int frequencyPosition = entry.Layout.FrequencyOffsetBytes;
                    return entry.Layout.FrequencyDataType == C37DataType.Float
                        ? BinaryPrimitives.ReadSingleBigEndian(pmuSegment.Slice(frequencyPosition, 4))
                        : BinaryPrimitives.ReadInt16BigEndian(pmuSegment.Slice(frequencyPosition, 2));
                case C37SignalType.Phasor:
                    // Direct access calculation: index * pair size
                    int phasorPosition = entry.Layout.PhasorOffsetBytes + (entry.ChannelEntry.ElementIndex * (entry.Layout.PhasorDataType == C37DataType.Float ? 8 : 4));
                    return entry.Layout.PhasorDataType == C37DataType.Float
                        ? BinaryPrimitives.ReadSingleBigEndian(pmuSegment.Slice(phasorPosition, 4))
                        : BinaryPrimitives.ReadUInt16BigEndian(pmuSegment.Slice(phasorPosition, 2));
                case C37SignalType.Analog:
                    int analogSize = entry.Layout.AnalogDataType == C37DataType.Float ? 4 : 2;
                    int analogPosition = entry.Layout.AnalogOffsetBytes + (entry.ChannelEntry.ElementIndex * analogSize);
                    return entry.Layout.AnalogDataType == C37DataType.Float
                        ? BinaryPrimitives.ReadSingleBigEndian(pmuSegment.Slice(analogPosition, 4))
                        : BinaryPrimitives.ReadInt16BigEndian(pmuSegment.Slice(analogPosition, 2));
                case C37SignalType.Digital:
                    // Find target word index, extract bits using an arithmetic shift mask
                    int digitalPosition = entry.Layout.DigitalOffsetBytes + ((entry.ChannelEntry.ElementIndex) * 2);
                    ushort completeWord = BinaryPrimitives.ReadUInt16BigEndian(pmuSegment.Slice(digitalPosition, 2));
                    return (completeWord & (1 << entry.ChannelEntry.BitPosition)) != 0;
                default:
                    throw new NotImplementedException($"Extraction method not defined for {entry.Register.SignalType}");
            }
        }
    }
}
