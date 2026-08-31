// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging
{
    /// <summary>
    /// Defines an interface for publishing to message brokers.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publish message to a message broker.
        /// </summary>
        /// <param name="topic">The topic to publish.</param>
        /// <param name="json">The json message to publish.</param>
        Task PublishAsync(string topic, string json);
    }
}
