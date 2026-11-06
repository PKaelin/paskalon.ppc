// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Alarms.Application.Subscribers;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Alarms.Application.UnitTest.Subscribers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmMessageQueue"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmMessageQueueTest
    {
        /// <summary>
        /// Time when the first data was received.
        /// </summary>
        private static readonly DateTimeOffset ReceivedOn = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero);


        /// <summary>
        /// The queued data is read in receive order.
        /// </summary>
        [TestMethod]
        public async Task ReadAllAsyncReadsInReceiveOrderTest()
        {
            PpcAlarmMessageQueue queue = new PpcAlarmMessageQueue(NullLogger<PpcAlarmMessageQueue>.Instance);
            queue.Enqueue(CreateMessage(7, 0));
            queue.Enqueue(CreateMessage(8, 1));
            queue.Enqueue(CreateMessage(7, 2));

            List<PpcAlarmMessage> messages = await ReadAsync(queue, 3);

            CollectionAssert.AreEqual(new List<int?> { 7, 8, 7 }, messages.Select(message => message.Key.DeviceId).ToList());
            CollectionAssert.AreEqual(new List<DateTimeOffset> { ReceivedOn, ReceivedOn.AddSeconds(1), ReceivedOn.AddSeconds(2) },
                messages.Select(message => message.ReceivedOn).ToList());
        }


        /// <summary>
        /// A full queue drops the oldest data and keeps the latest data.
        /// </summary>
        [TestMethod]
        public async Task EnqueueFullQueueDropsOldestTest()
        {
            PpcAlarmMessageQueue queue = new PpcAlarmMessageQueue(NullLogger<PpcAlarmMessageQueue>.Instance, capacity: 2);
            queue.Enqueue(CreateMessage(7, 0));
            queue.Enqueue(CreateMessage(7, 1));

            queue.Enqueue(CreateMessage(7, 2));

            List<PpcAlarmMessage> messages = await ReadAsync(queue, 2);
            CollectionAssert.AreEqual(new List<DateTimeOffset> { ReceivedOn.AddSeconds(1), ReceivedOn.AddSeconds(2) },
                messages.Select(message => message.ReceivedOn).ToList());
        }


        /// <summary>
        /// A capacity of less than one is rejected.
        /// </summary>
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void ConstructorInvalidCapacityThrowsTest(int capacity)
        {
            Action create = () => new PpcAlarmMessageQueue(NullLogger<PpcAlarmMessageQueue>.Instance, capacity);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(create);
        }


        /// <summary>
        /// Reads the number of queued messages.
        /// </summary>
        private static async Task<List<PpcAlarmMessage>> ReadAsync(PpcAlarmMessageQueue queue, int count)
        {
            using CancellationTokenSource timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            List<PpcAlarmMessage> messages = new List<PpcAlarmMessage>();

            await foreach (PpcAlarmMessage message in queue.ReadAllAsync(timeout.Token))
            {
                messages.Add(message);

                if (messages.Count == count)
                {
                    break;
                }
            }

            return messages;
        }


        /// <summary>
        /// Creates the received PCS detail of a PCS.
        /// </summary>
        private static PpcAlarmMessage CreateMessage(int deviceId, int receivedAfterSeconds)
        {
            PcsDetailDto detail = new PcsDetailDto
            {
                DeviceId = deviceId,
                IsACBreakerClosed = true,
                FaultStates = new Dictionary<string, bool> { { "OverTemperature", true }, { "GridLoss", false } }
            };

            return new PpcAlarmMessage
            {
                Key = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = deviceId },
                Data = detail,
                ReceivedOn = ReceivedOn.AddSeconds(receivedAfterSeconds)
            };
        }
    }
}
