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
        public string DefinitionTopic { get; set; }


        /// <summary>
        /// The core topic to subscribe to.
        /// </summary>
        public string CoreTopic { get; set; }


        /// <summary>
        /// The detail topic to subscribe to.
        /// </summary>
        public string DetailTopic { get; set; }


        /// <summary>
        /// Constructor of <see cref="SubscriberTopicEntry"/>.
        /// </summary>
        /// <param name="coreTopic">Core topic.</param>
        /// <param name="detailsTopic">Detail topic.</param>
        /// <param name="definitionTopic">Definition topic.</param>
        public SubscriberTopicEntry(string coreTopic, string detailsTopic, string definitionTopic = "")
        {
            CoreTopic = coreTopic;
            DetailTopic = detailsTopic;
            DefinitionTopic = definitionTopic;
        }
    }
}
