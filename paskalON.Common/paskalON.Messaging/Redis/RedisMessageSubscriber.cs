// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using StackExchange.Redis;

namespace paskalON.Messaging.Redis
{
    public class RedisMessageSubscriber : IMessageSubscriber, IDisposable
    {
        private readonly IConnectionMultiplexer _redis;

        private readonly ISubscriber _subscriber;

        public RedisMessageSubscriber(IConnectionMultiplexer redis)
        {
            ArgumentNullException.ThrowIfNull(redis);

            _redis = redis;
            _subscriber = redis.GetSubscriber();
        }


        public void Subscribe(string topic, Action<string> callback)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(topic);
            ArgumentNullException.ThrowIfNull(callback);

            _subscriber.Subscribe(RedisChannel.Literal(topic), (_, message) => callback(message.ToString()));
        }


        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
