// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Devices.Service.Publishers;
using paskalON.Telemetry;

namespace paskalON.Devices.Service.UnitTest.Publishers
{
    [TestClass]
    public sealed class MetricsPublisherServiceTest
    {
        [TestMethod]
        public void MetricsPublisherServiceConstructorNullLoggerTest()
        {
            Mock<IMetricsPublisher> publisherMock = new Mock<IMetricsPublisher>();

            Assert.ThrowsExactly<ArgumentNullException>(() =>
                new MetricsPublisherService(null!, [publisherMock.Object], 10));
        }


        [TestMethod]
        public void MetricsPublisherServiceConstructorNullPublishersTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                new MetricsPublisherService(NullLogger<MetricsPublisherService>.Instance, null!, 10));
        }


        [TestMethod]
        public void MetricsPublisherServiceConstructorInvalidIntervalTest()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
                new MetricsPublisherService(NullLogger<MetricsPublisherService>.Instance, [], 0));
        }


        [TestMethod]
        public void MetricsPublisherServiceConstructorTest()
        {
            MetricsPublisherService service = new MetricsPublisherService(
                NullLogger<MetricsPublisherService>.Instance, [], 10);

            Assert.IsNotNull(service);
        }


        [TestMethod]
        public async Task MetricsPublisherServiceExecuteEmptyPublishersTest()
        {
            MetricsPublisherService service = new MetricsPublisherService(
                NullLogger<MetricsPublisherService>.Instance, [], 1);
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task execution = service.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(25);
            await service.StopAsync(CancellationToken.None);
            cancellationTokenSource.Cancel();

            await execution;
        }


        [TestMethod]
        public async Task MetricsPublisherServiceExecutePublishersWithEntriesTest()
        {
            Mock<IMetricsPublisher> firstPublisherMock = new Mock<IMetricsPublisher>();
            Mock<IMetricsPublisher> secondPublisherMock = new Mock<IMetricsPublisher>();
            MetricsPublisherService service = new MetricsPublisherService(
                NullLogger<MetricsPublisherService>.Instance,
                [firstPublisherMock.Object, secondPublisherMock.Object], 1);
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task execution = service.StartAsync(cancellationTokenSource.Token);
            await WaitForPublication(firstPublisherMock, secondPublisherMock);
            await service.StopAsync(CancellationToken.None);
            cancellationTokenSource.Cancel();

            await execution;
            firstPublisherMock.Verify(publisher => publisher.Publish(It.Is<int>(interval => interval >= 1)), Times.AtLeastOnce);
            secondPublisherMock.Verify(publisher => publisher.Publish(It.Is<int>(interval => interval >= 1)), Times.AtLeastOnce);
        }


        [TestMethod]
        public async Task MetricsPublisherServiceExecutePublisherThrowsExceptionTest()
        {
            FakeLogger<MetricsPublisherService> logger = new FakeLogger<MetricsPublisherService>();
            Mock<IMetricsPublisher> publisherMock = new Mock<IMetricsPublisher>();
            publisherMock.Setup(publisher => publisher.Publish(It.IsAny<int>()))
                .Throws(new InvalidOperationException("Test exception"));
            MetricsPublisherService service = new MetricsPublisherService(logger, [publisherMock.Object], 1);
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task execution = service.StartAsync(cancellationTokenSource.Token);
            await Task.Delay(25);
            await service.StopAsync(CancellationToken.None);
            cancellationTokenSource.Cancel();

            await execution;
            publisherMock.Verify(publisher => publisher.Publish(It.IsAny<int>()), Times.AtLeastOnce);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.IsGreaterThanOrEqualTo(1, logs.Count());
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("Unexpected error", StringComparison.OrdinalIgnoreCase)));
        }


        private async Task WaitForPublication(Mock<IMetricsPublisher> firstPublisherMock, Mock<IMetricsPublisher> secondPublisherMock)
        {
            DateTimeOffset timeout = DateTimeOffset.UtcNow.AddSeconds(2);

            while (DateTimeOffset.UtcNow < timeout)
            {
                if (firstPublisherMock.Invocations.Count > 0 && secondPublisherMock.Invocations.Count > 0)
                {
                    return;
                }

                await Task.Delay(1);
            }

            Assert.Fail("The metrics publishers were not called within the timeout.");
        }
    }
}
