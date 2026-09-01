// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using StackExchange.Redis;

namespace paskalON.Messaging.Redis
{
    public class RedisMessagePublisher : IMessagePublisher
    {
        private readonly IConnectionMultiplexer _redis;


        public RedisMessagePublisher(IConnectionMultiplexer redis)
        {
            ArgumentNullException.ThrowIfNull(redis);

            _redis = redis;
        }


        public async Task PublishAsync(string topic, string json)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
            ArgumentNullException.ThrowIfNull(json);

            ISubscriber subscriber = _redis.GetSubscriber();

            await subscriber.PublishAsync(RedisChannel.Literal(topic), json);
        }
    }
}
