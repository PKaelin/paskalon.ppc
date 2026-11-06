// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Alarms.Application.Subscribers;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Messaging;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.PowerControls.Dto.Powers;
using System.Text.Json;

namespace paskalON.Alarms.Application.UnitTest.Subscribers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmSubscriber"/> with a mocked message subscriber and a mocked queue.
    /// </summary>
    [TestClass]
    public class PpcAlarmSubscriberTest
    {
        /// <summary>
        /// Topic of the PCS detail data.
        /// </summary>
        private const string PcsDetailTopic = "ppc:pcs:detail";


        /// <summary>
        /// Topic of the power targets.
        /// </summary>
        private const string PowerTargetTopic = "ppc:pc:targets";


        /// <summary>
        /// Topic of the current system setpoint.
        /// </summary>
        private const string CurrentSetpointTopic = "ppc:om:setpoint";


        /// <summary>
        /// Time when the data is received.
        /// </summary>
        private static readonly DateTimeOffset ReceivedOn = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero);


        /// <summary>
        /// Mocked message subscriber.
        /// </summary>
        private readonly Mock<IMessageSubscriber> _messageSubscriber = new Mock<IMessageSubscriber>();


        /// <summary>
        /// Mocked queue.
        /// </summary>
        private readonly Mock<IPpcAlarmMessageQueue> _queue = new Mock<IPpcAlarmMessageQueue>();


        /// <summary>
        /// Messages added to the queue.
        /// </summary>
        private readonly List<PpcAlarmMessage> _queuedMessages = new List<PpcAlarmMessage>();


        /// <summary>
        /// Callbacks of the subscribed topics.
        /// </summary>
        private readonly Dictionary<string, Action<string>> _callbacks = new Dictionary<string, Action<string>>();


        /// <summary>
        /// Sets up the message subscriber to record the callbacks and the queue to record the messages.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _messageSubscriber.Setup(subscriber => subscriber.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => _callbacks[topic] = callback);
            _queue.Setup(queue => queue.Enqueue(It.IsAny<PpcAlarmMessage>()))
                .Callback<PpcAlarmMessage>(message => _queuedMessages.Add(message));
        }


        /// <summary>
        /// Received PCS detail data is queued with the PCS as publisher and the receive time.
        /// </summary>
        [TestMethod]
        public void SubscribeDevicePcsDetailQueuesDataOfPcsTest()
        {
            PpcAlarmSubscriber subscriber = CreateSubscriber();
            subscriber.SubscribeDevice<PcsDetailDto>(PcsDetailTopic, PpcAlarmType.PcsAlarm);

            _callbacks[PcsDetailTopic](JsonSerializer.Serialize(CreatePcsDetail(7)));

            Assert.HasCount(1, _queuedMessages);
            PpcAlarmMessage message = _queuedMessages[0];
            Assert.AreEqual(PpcAlarmType.PcsAlarm, message.Key.PpcAlarmType);
            Assert.AreEqual(7, message.Key.DeviceId);
            Assert.AreEqual(ReceivedOn, message.ReceivedOn);
            PcsDetailDto detail = (PcsDetailDto)message.Data;
            Assert.IsTrue(detail.FaultStates["OverTemperature"]);
        }


        /// <summary>
        /// Received power targets of the power control are queued with the target PCS as device.
        /// </summary>
        [TestMethod]
        public void SubscribePowerTargetQueuesDataOfTargetDeviceTest()
        {
            PpcAlarmSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe<CurrentTargetsDto>(PowerTargetTopic, PpcAlarmType.PowerControlAlarm, target => target.DeviceId);
            CurrentTargetsDto target = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 1000000, ReactivePowerVar = 200000 };

            _callbacks[PowerTargetTopic](JsonSerializer.Serialize(target));

            Assert.HasCount(1, _queuedMessages);
            Assert.AreEqual(PpcAlarmType.PowerControlAlarm, _queuedMessages[0].Key.PpcAlarmType);
            Assert.AreEqual(7, _queuedMessages[0].Key.DeviceId);
            Assert.AreEqual(1000000, ((CurrentTargetsDto)_queuedMessages[0].Data).ActivePowerWatt);
        }


        /// <summary>
        /// Received system wide setpoints of the operating mode are queued without device.
        /// </summary>
        [TestMethod]
        public void SubscribeCurrentSetpointQueuesSystemDataWithoutDeviceTest()
        {
            PpcAlarmSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe<CurrentSetpointsDto>(CurrentSetpointTopic, PpcAlarmType.OperatingModeAlarm, _ => null);
            CurrentSetpointsDto setpoints = new CurrentSetpointsDto
            {
                SetpointActivePowerWatts = 2500000,
                SetpointReactivePowerVars = -300000,
                SourceInstance = "OperatingMode-1a2b3c",
                Sequence = 4711,
                Timestamp = ReceivedOn
            };

            _callbacks[CurrentSetpointTopic](JsonSerializer.Serialize(setpoints));

            Assert.HasCount(1, _queuedMessages);
            Assert.AreEqual(PpcAlarmType.OperatingModeAlarm, _queuedMessages[0].Key.PpcAlarmType);
            Assert.IsNull(_queuedMessages[0].Key.DeviceId);
        }


        /// <summary>
        /// Invalid published data is not queued and does not throw into the message subscriber.
        /// </summary>
        [TestMethod]
        [DataRow("{ invalid json")]
        [DataRow("null")]
        public void SubscribeDeviceInvalidDataIsNotQueuedTest(string json)
        {
            PpcAlarmSubscriber subscriber = CreateSubscriber();
            subscriber.SubscribeDevice<PcsDetailDto>(PcsDetailTopic, PpcAlarmType.PcsAlarm);

            _callbacks[PcsDetailTopic](json);

            Assert.IsEmpty(_queuedMessages);
        }


        /// <summary>
        /// An empty topic is rejected.
        /// </summary>
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void SubscribeDeviceEmptyTopicThrowsTest(string topic)
        {
            PpcAlarmSubscriber subscriber = CreateSubscriber();

            Action subscribe = () => subscriber.SubscribeDevice<PcsDetailDto>(topic, PpcAlarmType.PcsAlarm);

            Assert.ThrowsExactly<ArgumentException>(subscribe);
        }


        /// <summary>
        /// Creates the subscriber with the mocks and a fake time.
        /// </summary>
        private PpcAlarmSubscriber CreateSubscriber()
        {
            return new PpcAlarmSubscriber(NullLogger<PpcAlarmSubscriber>.Instance, _messageSubscriber.Object, _queue.Object,
                new FakeTimeProvider(ReceivedOn));
        }


        /// <summary>
        /// Creates a PCS detail with an over temperature fault state and a fan failure warning.
        /// </summary>
        private PcsDetailDto CreatePcsDetail(int deviceId)
        {
            return new PcsDetailDto
            {
                DeviceId = deviceId,
                IsInMaintenanceMode = false,
                IsACBreakerClosed = true,
                FaultStates = new Dictionary<string, bool> { { "OverTemperature", true }, { "GridLoss", false } },
                WarningStates = new Dictionary<string, bool> { { "FanFailure", true } },
                VendorEvents = new Dictionary<string, bool> { { "FirmwareUpdate", false } }
            };
        }
    }
}
