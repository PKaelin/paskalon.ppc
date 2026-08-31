// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Devices.Client.Registers;
using paskalON.Devices.Client.Subscribers;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Messaging;
using paskalON.PhysicalUnits.Electricals.Powers;
using System.Text.Json;

namespace paskalON.Devices.Client.UnitTest.Subscribers
{
    [TestClass]
    public class DeviceSubscriberTest
    {
        private Mock<IMessageSubscriber> _messageSubscriberMock = null!;
        private Mock<IDeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>> _registerMock = null!;
        private string _definitionTopic = "pcs/definition";
        private string _coreTopic = "pcs/core";
        private string _detailTopic = "pcs/detail";


        [TestInitialize]
        public void Initialize()
        {
            _messageSubscriberMock = new Mock<IMessageSubscriber>();
            _registerMock = new Mock<IDeviceRegister<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>>();
        }


        [TestMethod]
        public void DeviceSubscriberConstructorTest()
        {
            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    NullLogger.Instance, _messageSubscriberMock.Object, _registerMock.Object,
                    _definitionTopic, _coreTopic, _detailTopic);

            Assert.IsNotNull(subscriber);
        }


        [TestMethod]
        public void DeviceSubscriberConstructorNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    null!, _messageSubscriberMock.Object, _registerMock.Object, _definitionTopic, _coreTopic, _detailTopic);
            });
        }


        [TestMethod]
        public void DeviceSubscriberConstructorNullSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    NullLogger.Instance, null!, _registerMock.Object, _definitionTopic, _coreTopic, _detailTopic);
            });
        }


        [TestMethod]
        public void DeviceSubscriberConstructorNullRegisterTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    NullLogger.Instance, _messageSubscriberMock.Object, null!, _definitionTopic, _coreTopic, _detailTopic);
            });
        }


        [TestMethod]
        public void DeviceSubscriberConstructorSubscribesToDefinitionTopicTest()
        {
            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    NullLogger.Instance, _messageSubscriberMock.Object, _registerMock.Object, _definitionTopic, string.Empty, string.Empty);

            _messageSubscriberMock.Verify(x => x.Subscribe(_definitionTopic, It.IsAny<Action<string>>()), Times.Once);
        }


        [TestMethod]
        public void DeviceSubscriberConstructorSubscribesToCoreTopicTest()
        {
            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    NullLogger.Instance, _messageSubscriberMock.Object, _registerMock.Object, string.Empty, _coreTopic, string.Empty);

            _messageSubscriberMock.Verify(x => x.Subscribe(_coreTopic, It.IsAny<Action<string>>()), Times.Once);
        }


        [TestMethod]
        public void DeviceSubscriberConstructorSubscribesToDetailTopicTest()
        {
            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    NullLogger.Instance, _messageSubscriberMock.Object, _registerMock.Object, string.Empty, string.Empty, _detailTopic);

            _messageSubscriberMock.Verify(x => x.Subscribe(_detailTopic, It.IsAny<Action<string>>()), Times.Once);
        }


        [TestMethod]
        public void DeviceSubscriberConstructorNullTopicsTest()
        {
            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(
                    NullLogger.Instance, _messageSubscriberMock.Object, _registerMock.Object, null!, null!, null!);

            _messageSubscriberMock.Verify(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public void DeviceSubscriberUpdateDefinitionValidJsonTest()
        {
            Action<string>? definitionCallback = null;
            FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>> logger =
                new FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>>();

            _messageSubscriberMock.Setup(x => x.Subscribe(_definitionTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => { definitionCallback = callback; });

            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(logger,
                _messageSubscriberMock.Object, _registerMock.Object, _definitionTopic, string.Empty, string.Empty);

            Assert.IsNotNull(definitionCallback);

            PcsDefinitionDto message = new PcsDefinitionDto
            {
                DeviceId = 1,
                Name = "Test",
                MaximumDCVoltage = 11,
                MinimumDCVoltage = 12,
                NameplateMaximumACCurrent = 13,
                NameplateMaximumActivePower = new ActivePower(14),
                NameplateMaximumReactivePower = new ReactivePower(15),
                NameplateMaximumApparentPower = new ApparentPower(16),
            };

            string json = JsonSerializer.Serialize(message);

            definitionCallback(json);

            // Dont test actual updated values as this is not part of the DeviceSubscriber.
            _registerMock.Verify(x => x.UpdateDefinition(It.Is<PcsDefinitionDto>(value => value.DeviceId == message.DeviceId)), Times.Once);
            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            Assert.HasCount(0, logs.Where(l => l.Level == LogLevel.Error));
        }


        [TestMethod]
        public void DeviceSubscriberUpdateDefinitionInvalidJsonTest()
        {
            FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>> logger =
                new FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>>();

            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(logger,
                _messageSubscriberMock.Object, _registerMock.Object, _definitionTopic, string.Empty, string.Empty);

            subscriber.UpdateDefinition("{ invalid json }");

            _registerMock.Verify(x => x.UpdateDefinition(It.IsAny<PcsDefinitionDto>()), Times.Never);
            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            Assert.HasCount(1, logs.Where(l => l.Level == LogLevel.Error));
            Assert.IsNotNull(logs.Where(l => l.Level == LogLevel.Error).First().Message.Contains("Deserialize", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public void DeviceSubscriberUpdateCoreValidJsonTest()
        {
            Action<string>? coreCallback = null;
            FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>> logger =
                new FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>>();

            _messageSubscriberMock.Setup(x => x.Subscribe(_coreTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => { coreCallback = callback; });

            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(logger,
                _messageSubscriberMock.Object, _registerMock.Object, string.Empty, _coreTopic, string.Empty);

            Assert.IsNotNull(coreCallback);

            PcsCoreDto message = new PcsCoreDto
            {
                DeviceId = 1,
                ActivePower = new ActivePower(11),
                ReactivePower = new ReactivePower(12),
                ACCurrent = 13,
                ACVoltage = 14,
            };

            string json = JsonSerializer.Serialize(message);

            coreCallback(json);

            // Dont test actual updated values as this is not part of the DeviceSubscriber.
            _registerMock.Verify(x => x.UpdateCore(It.Is<PcsCoreDto>(value => value.DeviceId == message.DeviceId)), Times.Once);
            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            Assert.HasCount(0, logs.Where(l => l.Level == LogLevel.Error));
        }


        [TestMethod]
        public void DeviceSubscriberUpdateCoreInvalidJsonTest()
        {
            FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>> logger =
                new FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>>();

            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(logger,
                _messageSubscriberMock.Object, _registerMock.Object, string.Empty, _coreTopic, string.Empty);

            subscriber.UpdateCore("{ invalid json }");

            _registerMock.Verify(x => x.UpdateCore(It.IsAny<PcsCoreDto>()), Times.Never);
            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            Assert.HasCount(1, logs.Where(l => l.Level == LogLevel.Error));
            Assert.IsNotNull(logs.Where(l => l.Level == LogLevel.Error).First().Message.Contains("Deserialize", StringComparison.OrdinalIgnoreCase));
        }




        //---------------------------------------------------------------------------------------------

        [TestMethod]
        public void DeviceSubscriberUpdateDetailValidJsonTest()
        {
            Action<string>? detailCallback = null;
            FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>> logger =
                new FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>>();

            _messageSubscriberMock.Setup(x => x.Subscribe(_detailTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => { detailCallback = callback; });

            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(logger,
                _messageSubscriberMock.Object, _registerMock.Object, string.Empty, string.Empty, _detailTopic);

            Assert.IsNotNull(detailCallback);

            PcsDetailDto message = new PcsDetailDto
            {
                DeviceId = 1,
                ActivePowerTarget = new ActivePower(11),
                ReactivePowerTarget = new ReactivePower(12),
                IsACBreakerClosed = false,
                IsDcContactorClosed = new bool[] { true },
            };

            string json = JsonSerializer.Serialize(message);

            detailCallback(json);

            // Dont test actual updated values as this is not part of the DeviceSubscriber.
            _registerMock.Verify(x => x.UpdateDetail(It.Is<PcsDetailDto>(value => value.DeviceId == message.DeviceId)), Times.Once);
            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            Assert.HasCount(0, logs.Where(l => l.Level == LogLevel.Error));
        }


        [TestMethod]
        public void DeviceSubscriberUpdateDetailInvalidJsonTest()
        {
            FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>> logger =
                new FakeLogger<DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>>();

            DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto> subscriber =
                new DeviceSubscriber<PcsDto, PcsDefinitionDto, PcsCoreDto, PcsDetailDto>(logger,
                _messageSubscriberMock.Object, _registerMock.Object, string.Empty, string.Empty, _detailTopic);

            subscriber.UpdateDetail("{ invalid json }");

            _registerMock.Verify(x => x.UpdateDetail(It.IsAny<PcsDetailDto>()), Times.Never);
            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot();
            Assert.HasCount(1, logs.Where(l => l.Level == LogLevel.Error));
            Assert.IsNotNull(logs.Where(l => l.Level == LogLevel.Error).First().Message.Contains("Deserialize", StringComparison.OrdinalIgnoreCase));
        }
    }
}