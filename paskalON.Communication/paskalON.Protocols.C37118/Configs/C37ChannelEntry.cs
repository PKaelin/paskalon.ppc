// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;

namespace paskalON.Protocols.C37118.Configs
{
    /// <summary>
    /// C37 channel entry that get created when parsing the configuration frame bytes.
    /// </summary>
    public class C37ChannelEntry
    {
        /// <summary>
        /// Target stream Id.
        /// </summary>
        public ushort TargetStreamId { get; }


        /// <summary>
        /// C37 signal type.
        /// </summary>
        public C37SignalType SignalType { get; }


        /// <summary>
        /// Element index.
        /// </summary>
        public int ElementIndex { get; }


        /// <summary>
        /// Bit position.
        /// </summary>
        public int BitPosition { get; }


        /// <summary>
        /// Constructor of <see cref="C37ChannelEntry"/>.
        /// </summary>
        /// <param name="streamId">Target stream (PMU) Id.</param>
        /// <param name="signalType">C37 signal type.</param>
        /// <param name="elementIndex">Element index.</param>
        /// <param name="bitPosition">Bit position.</param>
        public C37ChannelEntry(ushort streamId, C37SignalType signalType, int elementIndex, int bitPosition = 0)
        {
            TargetStreamId = streamId;
            SignalType = signalType;
            ElementIndex = elementIndex;
            BitPosition = bitPosition;
        }
    }
}
