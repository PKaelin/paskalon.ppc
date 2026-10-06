// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using StackExchange.Redis;

namespace paskalON.Messaging.Redis
{
    /// <summary>
    /// Implements a Redis message subscriber that allows subscribing to Redis topics and receiving messages.
    /// </summary>
    public class RedisMessageSubscriber : IMessageSubscriber
    {
        /// <summary>
        /// The Redis connection multiplexer used for subscribing to topics.
        /// </summary>
        private readonly IConnectionMultiplexer _redis;


        /// <summary>
        /// The Redis subscriber used for subscribing to topics and receiving messages.
        /// </summary>
        private readonly ISubscriber _subscriber;


        /// <summary>
        /// Constructor of <see cref="RedisMessageSubscriber"/>.
        /// </summary>
        /// <param name="redis">The Redis connection multiplexer.</param>
        public RedisMessageSubscriber(IConnectionMultiplexer redis)
        {
            ArgumentNullException.ThrowIfNull(redis);

            _redis = redis;
            _subscriber = redis.GetSubscriber();
        }


        /// <inheritdoc/>
        public void Subscribe(string topic, Action<string> callback)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
            ArgumentNullException.ThrowIfNull(callback);

            // The queue based subscription guarantees sequential, in-order handling of messages per channel.
            ChannelMessageQueue queue = _subscriber.Subscribe(RedisChannel.Literal(topic));
            queue.OnMessage(channelMessage => HandleMessage(channelMessage, callback));
        }


        /// <summary>
        /// Handles a received channel message and forwards its payload to the callback.
        /// </summary>
        /// <param name="channelMessage">The received channel message.</param>
        /// <param name="callback">The action executed with the message payload.</param>
        private void HandleMessage(ChannelMessage channelMessage, Action<string> callback)
        {
            // Skip null or empty messages, since they cannot carry a valid json payload.
            if (channelMessage.Message.IsNullOrEmpty)
            {
                return;
            }

            // Callback errors should be handled by the caller, so we do not catch exceptions here.
            callback(channelMessage.Message.ToString());
        }
    }
}
