// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging
{
    /// <summary>
    /// Defines an interface for subscribing to message brokers.
    /// </summary>
    public interface IMessageSubscriber : IDisposable
    {
        /// <summary>
        /// Subscribes a callback action to a specific message topic channel.
        /// </summary>
        /// <param name="topic">The name of the channel or topic to listen to.</param>
        /// <param name="callback">The action executed when a message is received.</param>
        void Subscribe(string topic, Action<string> callback);
    }
}
