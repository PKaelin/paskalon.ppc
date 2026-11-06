// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.RabbitMq
{
    /// <summary>
    /// JSON message published to a RabbitMQ topic exchange.
    /// </summary>
    public record RabbitMqMessage
    {
        /// <summary>
        /// Unique identifier of the message, recipients use it to ignore duplicated deliveries.
        /// </summary>
        public required Guid MessageId { get; init; }


        /// <summary>
        /// Routing key of the message, recipients bind their queues with it (e.g. the action name ReducePower).
        /// </summary>
        public required string RoutingKey { get; init; }


        /// <summary>
        /// Message type, e.g. the name of the command, set as type property of the message.
        /// </summary>
        public required string Type { get; init; }


        /// <summary>
        /// JSON body of the message.
        /// </summary>
        public required string Body { get; init; }


        /// <summary>
        /// Time when the message was created, set as timestamp property of the message.
        /// </summary>
        public required DateTimeOffset CreatedOn { get; init; }
    }
}
