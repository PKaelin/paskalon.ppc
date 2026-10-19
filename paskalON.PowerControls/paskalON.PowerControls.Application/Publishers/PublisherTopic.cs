// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Domain.Configs;

namespace paskalON.PowerControls.Application.Publishers
{
    /// <summary>
    /// Message publisher topic.
    /// </summary>
    public class PublisherTopic
    {
        /// <summary>
        /// The topic for the power targets of the power conversion systems (PCS).
        /// </summary>
        public string? PowerTargetTopic { get; set; }


        /// <summary>
        /// Creates a publisher topic with a configuration input.
        /// </summary>
        /// <param name="config">SystemConfig with publisher topic configuration.</param>
        /// <returns>The publisher topic.</returns>
        public static PublisherTopic Create(SystemConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            return new PublisherTopic
            {
                PowerTargetTopic = config.PublisherTopicPowerTarget
            };
        }
    }
}
