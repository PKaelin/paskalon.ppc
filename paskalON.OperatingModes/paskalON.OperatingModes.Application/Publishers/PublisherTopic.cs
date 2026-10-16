// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Application.Publishers
{
    /// <summary>
    /// Message publisher topic.
    /// </summary>
    public class PublisherTopic
    {
        /// <summary>
        /// The topic for the current setpoint.
        /// </summary>
        public string? CurrentSetpointTopic { get; set; }


        /// <summary>
        /// The topic for the current DER unit setpoint.
        /// </summary>
        public string? CurrentDerUnitSetpointTopic { get; set; }


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
                CurrentSetpointTopic = config.PublisherTopicCurrentSetpoint,
                CurrentDerUnitSetpointTopic = config.PublisherTopicCurrentDerUnitSetpoint
            };
        }
    }
}
