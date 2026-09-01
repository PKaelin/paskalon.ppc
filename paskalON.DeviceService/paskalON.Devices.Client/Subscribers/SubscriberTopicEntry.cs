// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Client.Subscribers
{
    /// <summary>
    /// Subscriber topic entry.
    /// </summary>
    public class SubscriberTopicEntry
    {
        /// <summary>
        /// The definition topic to subscribe to.
        /// </summary>
        public required string DefinitionTopic { get; set; }


        /// <summary>
        /// The core topic to subscribe to.
        /// </summary>
        public required string CoreTopic { get; set; }


        /// <summary>
        /// The detail topic to subscribe to.
        /// </summary>
        public required string DetailTopic { get; set; }
    }
}
