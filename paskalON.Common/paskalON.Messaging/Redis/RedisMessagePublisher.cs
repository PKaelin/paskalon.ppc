// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using StackExchange.Redis;

namespace paskalON.Messaging.Redis
{
    /// <summary>
    /// Redis message publisher that publishes messages to a Redis channel.
    /// </summary>
    public class RedisMessagePublisher : IMessagePublisher
    {
        /// <summary>
        /// Connection multiplexer for Redis.
        /// </summary>
        private readonly IConnectionMultiplexer _redis;


        /// <summary>
        /// Constructor of <see cref="RedisMessagePublisher"/>.
        /// </summary>
        /// <param name="redis">The Redis connection multiplexer.</param>
        public RedisMessagePublisher(IConnectionMultiplexer redis)
        {
            ArgumentNullException.ThrowIfNull(redis);

            _redis = redis;
        }


        /// <inheritdoc/>
        public async Task PublishAsync(string topic, string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
            ArgumentNullException.ThrowIfNull(json);

            // Skip empty messages, since they cannot carry a valid json payload.
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            ISubscriber subscriber = _redis.GetSubscriber();

            await subscriber.PublishAsync(RedisChannel.Literal(topic), json);
        }
    }
}
