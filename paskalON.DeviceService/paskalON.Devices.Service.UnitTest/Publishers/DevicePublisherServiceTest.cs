// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Devices.Application.Publishers;
using paskalON.Devices.Service.Publishers;

namespace paskalON.Devices.Service.UnitTest.Publishers
{
    [TestClass]
    public class DevicePublisherServiceTest
    {
        [TestMethod]
        public void DevicePublisherServiceConstructorNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DevicePublisherService(null!));
        }


        [TestMethod]
        public void DevicePublisherServiceInitializeNullDevicePublisherTest()
        {
            DevicePublisherService service = new DevicePublisherService(NullLogger<DevicePublisherService>.Instance);

            Assert.ThrowsExactly<ArgumentNullException>(() => service.Initialize(null!, 10));
        }


        [TestMethod]
        public void DevicePublisherServiceInitializeInvalidIntervalTest()
        {
            Mock<IDevicePublisher> devicePublisherMock = new Mock<IDevicePublisher>();
            DevicePublisherService service = new DevicePublisherService(NullLogger<DevicePublisherService>.Instance);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => service.Initialize(devicePublisherMock.Object, 0));
        }


        [TestMethod]
        public void DevicePublisherServiceInitializeTest()
        {
            Mock<IDevicePublisher> devicePublisherMock = new Mock<IDevicePublisher>();

            DevicePublisherService service = new DevicePublisherService(NullLogger<DevicePublisherService>.Instance);
            service.Initialize(devicePublisherMock.Object, 10);

            Assert.IsNotNull(service);
        }


        [TestMethod]
        public async Task DevicePublisherServicePublishesEntriesTest()
        {
            Mock<IDevicePublisher> devicePublisherMock = new Mock<IDevicePublisher>();
            DevicePublisherService service = new DevicePublisherService(NullLogger<DevicePublisherService>.Instance);
            service.Initialize(devicePublisherMock.Object, 1);

            Task execution = service.StartAsync(CancellationToken.None);
            await WaitForPublication(devicePublisherMock);
            await service.StopAsync(CancellationToken.None);

            await execution;
            devicePublisherMock.Verify(devicePublisher => devicePublisher.Publish(
                It.Is<int>(interval => interval >= 1)), Times.AtLeastOnce);
        }


        [TestMethod]
        public async Task DevicePublisherServiceStartStopTest()
        {
            Mock<IDevicePublisher> devicePublisherMock = new Mock<IDevicePublisher>();
            DevicePublisherService service = new DevicePublisherService(NullLogger<DevicePublisherService>.Instance);
            service.Initialize(devicePublisherMock.Object, 1000);

            Task execution = service.StartAsync(CancellationToken.None);
            await service.StopAsync(CancellationToken.None);

            await execution;
            devicePublisherMock.Verify(devicePublisher => devicePublisher.Publish(It.IsAny<int>()), Times.Never);
        }


        [TestMethod]
        public async Task DevicePublisherServicePublisherThrowsExceptionTest()
        {
            FakeLogger<DevicePublisherService> logger = new FakeLogger<DevicePublisherService>();
            Mock<IDevicePublisher> devicePublisherMock = new Mock<IDevicePublisher>();
            devicePublisherMock.Setup(devicePublisher => devicePublisher.Publish(It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));
            DevicePublisherService service = new DevicePublisherService(logger);
            service.Initialize(devicePublisherMock.Object, 1);

            Task execution = service.StartAsync(CancellationToken.None);
            await WaitForPublication(devicePublisherMock);
            await service.StopAsync(CancellationToken.None);

            await execution;
            devicePublisherMock.Verify(devicePublisher => devicePublisher.Publish(It.IsAny<int>()), Times.AtLeastOnce);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.IsGreaterThanOrEqualTo(1, logs.Count());
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("Unexpected error", StringComparison.OrdinalIgnoreCase)));
        }


        private async Task WaitForPublication(Mock<IDevicePublisher> devicePublisherMock)
        {
            DateTimeOffset timeout = DateTimeOffset.UtcNow.AddSeconds(2);

            while (DateTimeOffset.UtcNow < timeout)
            {
                if (devicePublisherMock.Invocations.Count > 0)
                {
                    return;
                }

                await Task.Delay(1);
            }

            Assert.Fail("The device publisher was not called within the timeout.");
        }
    }
}
