// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using StackExchange.Redis;
using System.Reflection;

namespace paskalON.Messaging.UnitTest.TestDoubles
{
    /// <summary>
    /// In-memory driver for a real <see cref="ChannelMessageQueue"/> without a Redis connection.
    /// </summary>
    /// <remarks>
    /// <see cref="ChannelMessageQueue"/> is sealed and only constructible by StackExchange.Redis itself, therefore the
    /// internal constructor and the internal write method are accessed via reflection. If a StackExchange.Redis update
    /// changes these internals, the constructor of this class fails with a descriptive exception.
    /// </remarks>
    public sealed class TestChannelMessageQueue : IDisposable
    {
        /// <summary>
        /// Binding flags used to look up the internal members of <see cref="ChannelMessageQueue"/>.
        /// </summary>
        private const BindingFlags InternalInstance = BindingFlags.Instance | BindingFlags.NonPublic;


        /// <summary>
        /// The internal write method that enqueues a message into the queue.
        /// </summary>
        private readonly MethodInfo _writeMethod;


        /// <summary>
        /// The internal method that completes the queue and stops the message loop.
        /// </summary>
        private readonly MethodInfo _markCompletedMethod;


        /// <summary>
        /// Constructor of <see cref="TestChannelMessageQueue"/>.
        /// </summary>
        /// <param name="channel">The channel the queue is subscribed to.</param>
        public TestChannelMessageQueue(RedisChannel channel)
        {
            Type queueType = typeof(ChannelMessageQueue);
            Type redisChannelByRef = typeof(RedisChannel).MakeByRefType();
            Type redisValueByRef = typeof(RedisValue).MakeByRefType();
            Type redisSubscriberType = queueType.Assembly.GetType("StackExchange.Redis.RedisSubscriber", throwOnError: true)!;

            ConstructorInfo constructor = queueType.GetConstructor(InternalInstance, null, new Type[] { redisChannelByRef, redisSubscriberType }, null)
                ?? throw new InvalidOperationException("Internal constructor of ChannelMessageQueue not found.");

            _writeMethod = queueType.GetMethod("Write", InternalInstance, null, new Type[] { redisChannelByRef, redisValueByRef }, null)
                ?? throw new InvalidOperationException("Internal method ChannelMessageQueue.Write not found.");

            _markCompletedMethod = queueType.GetMethod("MarkCompleted", InternalInstance, null, new Type[] { typeof(Exception) }, null)
                ?? throw new InvalidOperationException("Internal method ChannelMessageQueue.MarkCompleted not found.");

            Channel = channel;
            Queue = (ChannelMessageQueue)constructor.Invoke(new object?[] { channel, null });
        }


        /// <summary>
        /// The channel the queue is subscribed to.
        /// </summary>
        public RedisChannel Channel { get; }


        /// <summary>
        /// The real queue instance that is handed out by the mocked <see cref="ISubscriber"/>.
        /// </summary>
        public ChannelMessageQueue Queue { get; }


        /// <summary>
        /// Simulates a message received from the Redis server on the subscribed channel.
        /// </summary>
        /// <param name="message">The received message payload.</param>
        public void Receive(RedisValue message)
        {
            _writeMethod.Invoke(Queue, new object[] { Channel, message });
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            _markCompletedMethod.Invoke(Queue, new object?[] { null });
        }
    }
}
