// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.PowerControls.Client.Subscribers;

namespace paskalON.PowerControls.Client.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerTargetSubscriberServiceTest
    {
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        [TestMethod]
        public void PowerTargetSubscriberServiceNullSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetSubscriberService(null!));
        }


        [TestMethod]
        public async Task PowerTargetSubscriberServiceRunsSubscriberUntilStoppedTest()
        {
            TaskCompletionSource<CancellationToken> started = new TaskCompletionSource<CancellationToken>(TaskCreationOptions.RunContinuationsAsynchronously);
            Mock<IPowerTargetSubscriber> subscriber = new Mock<IPowerTargetSubscriber>();
            subscriber.Setup(s => s.RunAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(async token =>
                {
                    started.TrySetResult(token);
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                });
            using PowerTargetSubscriberService service = new PowerTargetSubscriberService(subscriber.Object);

            await service.StartAsync(CancellationToken.None);
            CancellationToken stoppingToken = await started.Task.WaitAsync(TestTimeout);
            await service.StopAsync(CancellationToken.None);

            Assert.IsTrue(stoppingToken.IsCancellationRequested);
            subscriber.Verify(s => s.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
