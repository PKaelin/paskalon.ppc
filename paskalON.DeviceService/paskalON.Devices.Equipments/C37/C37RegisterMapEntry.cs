// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118.Configs;

namespace paskalON.Devices.Equipments.C37
{
    /// <summary>
    /// C37 channel to dataface registers map entry.
    /// </summary>
    public class C37RegisterMapEntry
    {
        /// <summary>
        /// The C37 dataface register entry.
        /// </summary>
        public IC37RegisterEntry Register { get; }


        /// <summary>
        /// The C37 channel entry.
        /// </summary>
        public C37ChannelEntry ChannelEntry { get; }


        /// <summary>
        /// Phasor measurement unit (PMU) layout.
        /// </summary>
        public PmuLayoutMetadata Layout { get; }


        /// <summary>
        /// Constructor of <see cref="C37RegisterMapEntry"/>
        /// </summary>
        /// <param name="register">The C37 dataface register entry.</param>
        /// <param name="channelEntry">The C37 channel entry.</param>
        /// <param name="layout">Phasor measurement unit (PMU) layout.</param>
        public C37RegisterMapEntry(IC37RegisterEntry register, C37ChannelEntry channelEntry, PmuLayoutMetadata layout)
        {
            Register = register;
            ChannelEntry = channelEntry;
            Layout = layout;
        }
    }
}
