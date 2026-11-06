// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.RabbitMq
{
    /// <summary>
    /// Publishes messages reliably to a RabbitMQ exchange.
    /// </summary>
    public interface IRabbitMqPublisher
    {
        /// <summary>
        /// Publishes a message and waits until the broker confirmed it.
        /// </summary>
        /// <param name="message">Message to publish.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that completes when the broker confirmed the message.</returns>
        /// <exception cref="Exception">Thrown when the broker is not available, rejected or could not route the message.</exception>
        Task PublishAsync(RabbitMqMessage message, CancellationToken cancellationToken = default);
    }
}
