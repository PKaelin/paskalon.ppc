// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.RabbitMq
{
    /// <summary>
    /// Subscriber of the messages of a RabbitMQ topic exchange, every subscriber instance receives every message.
    /// </summary>
    public interface IRabbitMqSubscriber
    {
        /// <summary>
        /// Connects, creates the queue of this subscriber, binds it with the routing keys and starts consuming the messages.
        /// </summary>
        /// <param name="routingKeys">Routing keys of the messages to receive (e.g. the action names).</param>
        /// <param name="handler">Handles a received message, the message is acknowledged after the handler completed.</param>
        /// <param name="recovered">
        /// Called after a lost connection was recovered and the queue was created again. Messages published while the
        /// connection was lost are not received, e.g. the recipient loads the current state again.
        /// </param>
        /// <param name="cancellationToken">Cancellation token of the subscription.</param>
        /// <returns>Task that completes when the subscription is established.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the subscriber is already subscribed.</exception>
        Task SubscribeAsync(IReadOnlyCollection<string> routingKeys, Func<RabbitMqMessage, CancellationToken, Task> handler, Action recovered,
            CancellationToken cancellationToken = default);
    }
}
