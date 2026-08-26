// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Application.Publishers
{
    /// <summary>
    /// Publisher topic entry.
    /// </summary>
    public class PublisherTopicEntry
    {
        /// <summary>
        /// The definition topic to publish to.
        /// </summary>
        public required string DefinitionTopic { get; set; }


        /// <summary>
        /// The core topic to publish to.
        /// </summary>
        public required string CoreTopic { get; set; }


        /// <summary>
        /// The detail topic to publish to.
        /// </summary>
        public required string DetailTopic { get; set; }
    }
}
