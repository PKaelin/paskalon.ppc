// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118.Configs
{
    /// <summary>
    /// Protocol Configuration Blueprint refers to the Configuration Frame (CFG) in the IEEE C37.118 protocol.
    /// </summary>
    /// <remarks>
    /// The data frames in C37.118 are completely raw optimized binary streams to minimize network latency
    /// they contain no metadata. The Configuration Frame acts as the mandatory structural blueprint or map
    /// that a parser must read first to understand how many bytes to read and how to decode the incoming raw stream.
    /// </remarks>
    /// 
    public class C37ConfigBlueprint
    {
        /// <summary>
        /// Stream Id Code
        /// </summary>
        public ushort StreamIdCode { get; set; }


        /// <summary>
        /// List of PMU layout meta data.
        /// </summary>
        public List<PmuLayoutMetadata> Pmus { get; } = new();


        /// <summary>
        /// Channel map with name of the channel and its channel coordinates (metadata).
        /// </summary>
        /// <remarks>
        /// Create these when parsing the configuration frame bytes.
        /// </remarks>
        public Dictionary<string, C37ChannelEntry> ChannelMap { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
