// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Service.Publishers;

namespace paskalON.Alarms.Service.UnitTest.Publishers
{
    /// <summary>
    /// Tests <see cref="AlarmActionOutboxPublisherService"/> with a mocked sender in a real service provider, the real signal and a fake time.
    /// </summary>
    [TestClass]
    public class AlarmActionOutboxPublisherServiceTest
    {
        /// <summary>
        /// Configured send interval in milliseconds.
        /// </summary>
        private const int SendIntervalMilliseconds = 1000;


        /// <summary>
        /// Configured retry interval in milliseconds.
        /// </summary>
        private const int RetryIntervalMilliseconds = 5000;


        /// <summary>
        /// Maximum real time to wait for a completion in the tests.
        /// </summary>
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        /// <summary>
        /// Mocked outbox sender.
        /// </summary>
        private readonly Mock<IAlarmActionOutboxSender> _sender = new Mock<IAlarmActionOutboxSender>();


        /// <summary>
        /// Fake time of the waits.
        /// </summary>
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 11, 6, 9, 0, 0, TimeSpan.Zero));


        /// <summary>
        /// Number of created senders, one per scope.
        /// </summary>
        private int _createdSenders;


        /// <summary>
        /// Signal of the saved alarm actions.
        /// </summary>
        private AlarmActionOutboxSignal _signal = null!;


        /// <summary>
        /// Service provider with the scoped sender.
        /// </summary>
        private ServiceProvider _serviceProvider = null!;


        /// <summary>
        /// Builds the service provider with the mocked sender registered scoped.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _signal = new AlarmActionOutboxSignal(_timeProvider);
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<IAlarmActionOutboxSender>(_ =>
            {
                Interlocked.Increment(ref _createdSenders);

                return _sender.Object;
            });
            _serviceProvider = services.BuildServiceProvider();
        }


        /// <summary>
        /// Disposes the service provider.
        /// </summary>
        [TestCleanup]
        public async Task Cleanup()
        {
            await _serviceProvider.DisposeAsync();
        }


        /// <summary>
        /// The next send depends on the send result.
        /// </summary>
        [TestMethod]
        [DataRow(false, false, AlarmActionOutboxNextSend.OnSignalOrSendInterval)]
        [DataRow(false, true, AlarmActionOutboxNextSend.Immediately)]
        [DataRow(true, true, AlarmActionOutboxNextSend.AfterRetryInterval)]
        public async Task SendPendingAsyncReturnsNextSendOfResultTest(bool isFailed, bool hasMore, AlarmActionOutboxNextSend expectedNextSend)
        {
            SetupResult(new AlarmActionOutboxSendResult { SentCount = 1, IsFailed = isFailed, HasMore = hasMore });
            AlarmActionOutboxPublisherService service = CreateInitializedService();

            AlarmActionOutboxNextSend nextSend = await service.SendPendingAsync();

            Assert.AreEqual(expectedNextSend, nextSend);
        }


        /// <summary>
        /// An unexpected error (e.g. database not reachable) does not stop the service, the send is retried later.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncSenderThrowsRetriesTest()
        {
            _sender.Setup(sender => sender.SendPendingAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database is not reachable."));
            AlarmActionOutboxPublisherService service = CreateInitializedService();

            AlarmActionOutboxNextSend nextSend = await service.SendPendingAsync();

            Assert.AreEqual(AlarmActionOutboxNextSend.AfterRetryInterval, nextSend);
        }


        /// <summary>
        /// Each send gets a new scope, so that every send uses a new database context.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncCreatesScopePerSendTest()
        {
            SetupResult(new AlarmActionOutboxSendResult { SentCount = 0, IsFailed = false, HasMore = false });
            AlarmActionOutboxPublisherService service = CreateInitializedService();

            await service.SendPendingAsync();
            await service.SendPendingAsync();

            Assert.AreEqual(2, _createdSenders);
        }


        /// <summary>
        /// A service that has not been initialized with the intervals refuses to send.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncNotInitializedThrowsTest()
        {
            AlarmActionOutboxPublisherService service = CreateService();

            Func<Task> send = () => service.SendPendingAsync();

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(send);
        }


        /// <summary>
        /// Intervals of zero or less are rejected.
        /// </summary>
        [TestMethod]
        [DataRow(0, RetryIntervalMilliseconds)]
        [DataRow(SendIntervalMilliseconds, 0)]
        [DataRow(-1, RetryIntervalMilliseconds)]
        public void InitializeInvalidIntervalThrowsTest(int sendIntervalMilliseconds, int retryIntervalMilliseconds)
        {
            AlarmActionOutboxPublisherService service = CreateService();

            Action initialize = () => service.Initialize(sendIntervalMilliseconds, retryIntervalMilliseconds);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(initialize);
        }


        /// <summary>
        /// More pending actions than one batch are sent without waiting.
        /// </summary>
        [TestMethod]
        public void WaitForNextSendAsyncImmediatelyDoesNotWaitTest()
        {
            AlarmActionOutboxPublisherService service = CreateInitializedService();

            Task wait = service.WaitForNextSendAsync(AlarmActionOutboxNextSend.Immediately);

            Assert.IsTrue(wait.IsCompletedSuccessfully);
        }


        /// <summary>
        /// Signaled actions are sent immediately and do not wait for the send interval (time critical actions).
        /// </summary>
        [TestMethod]
        public async Task WaitForNextSendAsyncSignalEndsWaitBeforeSendIntervalTest()
        {
            AlarmActionOutboxPublisherService service = CreateInitializedService();
            Task wait = service.WaitForNextSendAsync(AlarmActionOutboxNextSend.OnSignalOrSendInterval);
            bool isCompletedBeforeSignal = wait.IsCompleted;

            _signal.Notify();

            await wait.WaitAsync(TestTimeout);
            Assert.IsFalse(isCompletedBeforeSignal);
            Assert.AreEqual(_timeProvider.Start, _timeProvider.GetUtcNow());
        }


        /// <summary>
        /// Without a signal the pending actions are sent after the send interval (safety net).
        /// </summary>
        [TestMethod]
        public async Task WaitForNextSendAsyncWithoutSignalEndsAfterSendIntervalTest()
        {
            AlarmActionOutboxPublisherService service = CreateInitializedService();
            Task wait = service.WaitForNextSendAsync(AlarmActionOutboxNextSend.OnSignalOrSendInterval);
            _timeProvider.Advance(TimeSpan.FromMilliseconds(SendIntervalMilliseconds - 1));
            bool isCompletedBeforeInterval = wait.IsCompleted;

            _timeProvider.Advance(TimeSpan.FromMilliseconds(1));

            await wait.WaitAsync(TestTimeout);
            Assert.IsFalse(isCompletedBeforeInterval);
        }


        /// <summary>
        /// After a failure signals are ignored until the retry interval elapsed, so that an outage does not cause a retry storm.
        /// </summary>
        [TestMethod]
        public async Task WaitForNextSendAsyncRetryIgnoresSignalTest()
        {
            AlarmActionOutboxPublisherService service = CreateInitializedService();
            Task wait = service.WaitForNextSendAsync(AlarmActionOutboxNextSend.AfterRetryInterval);
            _signal.Notify();
            _timeProvider.Advance(TimeSpan.FromMilliseconds(RetryIntervalMilliseconds - 1));
            bool isCompletedBeforeRetryInterval = wait.IsCompleted;

            _timeProvider.Advance(TimeSpan.FromMilliseconds(1));

            await wait.WaitAsync(TestTimeout);
            Assert.IsFalse(isCompletedBeforeRetryInterval);
        }


        /// <summary>
        /// The running service sends the pending actions at start and sends again immediately when new actions are signaled.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncSignalSendsImmediatelyTest()
        {
            int sendCount = 0;
            TaskCompletionSource firstSend = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource secondSend = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _sender.Setup(sender => sender.SendPendingAsync(It.IsAny<CancellationToken>()))
                .Callback(() => (Interlocked.Increment(ref sendCount) == 1 ? firstSend : secondSend).TrySetResult())
                .ReturnsAsync(new AlarmActionOutboxSendResult { SentCount = 1, IsFailed = false, HasMore = false });
            AlarmActionOutboxPublisherService service = CreateInitializedService();
            await service.StartAsync(CancellationToken.None);
            await firstSend.Task.WaitAsync(TestTimeout);

            _signal.Notify();

            await secondSend.Task.WaitAsync(TestTimeout);
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(_timeProvider.Start, _timeProvider.GetUtcNow());
        }


        /// <summary>
        /// Sets up the sender to return the result.
        /// </summary>
        private void SetupResult(AlarmActionOutboxSendResult result)
        {
            _sender.Setup(sender => sender.SendPendingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(result);
        }


        /// <summary>
        /// Creates the service initialized with the configured intervals.
        /// </summary>
        private AlarmActionOutboxPublisherService CreateInitializedService()
        {
            AlarmActionOutboxPublisherService service = CreateService();
            service.Initialize(SendIntervalMilliseconds, RetryIntervalMilliseconds);

            return service;
        }


        /// <summary>
        /// Creates the service with the service provider, the signal and the fake time.
        /// </summary>
        private AlarmActionOutboxPublisherService CreateService()
        {
            return new AlarmActionOutboxPublisherService(NullLogger<AlarmActionOutboxPublisherService>.Instance,
                _serviceProvider.GetRequiredService<IServiceScopeFactory>(), _signal, _timeProvider);
        }
    }
}
