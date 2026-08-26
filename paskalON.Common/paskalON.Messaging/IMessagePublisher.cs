// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
        Task Publish(string topic, string json);
    }
}
