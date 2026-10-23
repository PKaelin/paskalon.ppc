// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.OperatingModes.Client.Subscribers;

namespace paskalON.OperatingModes.Client.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerSetpointSubscriberServiceTest
    {
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        [TestMethod]
        public void PowerSetpointSubscriberServiceNullSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointSubscriberService(null!));
        }


        [TestMethod]
        public async Task PowerSetpointSubscriberServiceRunsSubscriberUntilStoppedTest()
        {
            TaskCompletionSource<CancellationToken> started = new TaskCompletionSource<CancellationToken>(TaskCreationOptions.RunContinuationsAsynchronously);
            Mock<IPowerSetpointSubscriber> subscriber = new Mock<IPowerSetpointSubscriber>();
            subscriber.Setup(s => s.RunAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(async token =>
                {
                    started.TrySetResult(token);
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                });
            using PowerSetpointSubscriberService service = new PowerSetpointSubscriberService(subscriber.Object);

            await service.StartAsync(CancellationToken.None);
            CancellationToken stoppingToken = await started.Task.WaitAsync(TestTimeout);
            await service.StopAsync(CancellationToken.None);

            Assert.IsTrue(stoppingToken.IsCancellationRequested);
            subscriber.Verify(s => s.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
