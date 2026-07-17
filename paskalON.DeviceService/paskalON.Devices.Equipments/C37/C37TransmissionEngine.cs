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
        /// C37 station Id.
        /// </summary>
        /// <remarks>
        /// Can be either the name of the phasor measurement unit (PMU)
        /// Or can be the name of the phasor data concentrator (PDC).
        /// </remarks>
        private string _stationName;


        /// <summary>
        /// C37 stream id within the C37 data stream.
        /// </summary>
        /// <remarks>
        /// This identifies the PMU.
        /// </remarks>
        private ushort _streamId;


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
        /// Current registered mappings that were generated via the configuration frame.
        /// </summary>
        public List<C37RegisterMapEntry> Mappings { get => _mappings.ToList(); }


        /// <summary>
        /// Constructor of <see cref="C37TransmissionEngine"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="client">The C37 client interface.</param>
        /// <param name="dataface">The C37 data face.</param>
        public C37TransmissionEngine(ILogger logger, IC37Client client, IC37Dataface dataface, string stationName, ushort streamId)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(dataface);
            ArgumentNullException.ThrowIfNull(stationName);

            _logger = logger;
            _dataface = dataface;
            _client = client;
            _stationName = stationName;
            _streamId = streamId;
            _client.ConfigFrameReceived += OnConfigFrameReceived;
            _client.DataFrameReceived += OnDataFrameReceived;
            _logger.LogInformation("C37 transmission engine created for: {Name} {Address} {Port}", dataface.Name, client.ServerAddress, client.ServerPort);
        }


        /// <summary>
        /// Triggered when a configuration frame has been received.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The C37 configuration frame event argument.</param>
        private void OnConfigFrameReceived(object? sender, C37ConfigFrameEventArgs e)
        {
            try
            {
                C37ConfigBlueprint activeBlueprint = e.Blueprint;
                IEnumerable<PmuLayoutMetadata> pmuLayouts = activeBlueprint.Pmus.Where(p => p.StationName == _stationName);

                // Only configure if the station name and stream id matches the configured name and id
                if ((pmuLayouts.Count() > 0) && (e.Header.StreamIdCode == _streamId))
                {
                    _mappings.Clear();

                    // Map registers to protocol structural coordinates by matching names            
                    foreach (IC37RegisterEntry register in _dataface.Registers)
                    {
                        if (activeBlueprint.ChannelMap.TryGetValue(register.Name, out C37ChannelEntry? channelEntry))
                        {
                            // Find matching layout specifications for structural byte offset resolution
                            PmuLayoutMetadata? layout = pmuLayouts.FirstOrDefault(p => p.StreamId == channelEntry.TargetStreamId);

                            if (layout != null)
                            {
                                // Create the map entry and set the PMU data offset
                                _mappings.Add(new C37RegisterMapEntry(register, channelEntry, layout));
                            }
                        }
                        else
                        {
                            _logger.LogInformation("C37 config frame contains unbound register named {RegisterName}", channelEntry);
                        }
                    }

                    _runtimeMappings = _mappings.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occurred in C37 transmission on config frame. {C37Error}:", ex.Message);
            }
        }


        /// <summary>
        /// Triggered when a data frame has been received.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The C37 data frame event argument.</param>
        private void OnDataFrameReceived(object? sender, C37DataFrameEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occurred in C37 transmission on data frame. {C37Error}:", ex.Message);
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
                    int phasorSize = entry.Layout.PhasorDataType == C37DataType.Float ? 8 : 4;
                    int phasorPosition = entry.Layout.PhasorOffsetBytes + (entry.ChannelEntry.ElementIndex * phasorSize);
                    ReadOnlySpan<byte> phasorBytes = pmuSegment.Slice(phasorPosition, phasorSize);

                    if (entry.Layout.PhasorDataType == C37DataType.Float)
                    {
                        // Reads 8 bytes natively into a 64-bit layout
                        // First 4 bytes (0..4) = Magnitude, Second 4 bytes (4..8) = Angle
                        return BinaryPrimitives.ReadUInt64BigEndian(phasorBytes);
                    }
                    else
                    {
                        // First 2 bytes (0..2) = Magnitude, Second 2 bytes (2..4) = Angle
                        ushort magnitude = BinaryPrimitives.ReadUInt16BigEndian(phasorBytes[0..2]);
                        short angle = BinaryPrimitives.ReadInt16BigEndian(phasorBytes[2..4]);
                        // Upcast Magnitude to 32 bits (always positive, shifts safely into upper 4 bytes)
                        uint magnitudeBits = (uint)magnitude;
                        // Upcast Angle to 32 bits, but mask out any sign extension to protect the upper bits
                        uint angleBits = (uint)angle & 0xFFFFFFFF;
                        // First 4 bytes (0..4) = Magnitude, Second 4 bytes (4..8) = Angle
                        // Later on get the magnitude:  double magBits = (double)(normalizedBits >> 32);
                        // Later on get the angle: double angBits = (double)(int)(normalizedBits & 0xFFFFFFFF); cast to int first to preserve negative values
                        return ((ulong)magnitudeBits << 32) | angleBits;
                    }

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