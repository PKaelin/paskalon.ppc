// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Time.Testing;
using paskalON.Alarms.Application.Outboxes;

namespace paskalON.Alarms.Application.UnitTest.Outboxes
{
    /// <summary>
    /// Tests <see cref="AlarmActionOutboxSignal"/> with a fake time.
    /// </summary>
    [TestClass]
    public class AlarmActionOutboxSignalTest
    {
        /// <summary>
        /// Wait timeout, the send interval of the outbox publisher service.
        /// </summary>
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(1000);


        /// <summary>
        /// Maximum real time to wait for a completion in the tests.
        /// </summary>
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        /// <summary>
        /// Fake time of the wait timeout.
        /// </summary>
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 11, 6, 9, 0, 0, TimeSpan.Zero));


        /// <summary>
        /// A notification before the wait completes the wait immediately.
        /// </summary>
        [TestMethod]
        public async Task WaitAsyncNotifiedBeforeReturnsTrueTest()
        {
            AlarmActionOutboxSignal signal = new AlarmActionOutboxSignal(_timeProvider);
            signal.Notify();

            bool isNotified = await signal.WaitAsync(Timeout).WaitAsync(TestTimeout);

            Assert.IsTrue(isNotified);
        }


        /// <summary>
        /// A notification during the wait completes the wait before the timeout.
        /// </summary>
        [TestMethod]
        public async Task WaitAsyncNotifiedDuringWaitReturnsTrueTest()
        {
            AlarmActionOutboxSignal signal = new AlarmActionOutboxSignal(_timeProvider);
            Task<bool> wait = signal.WaitAsync(Timeout);
            bool isCompletedBeforeNotify = wait.IsCompleted;

            signal.Notify();

            Assert.IsFalse(isCompletedBeforeNotify);
            Assert.IsTrue(await wait.WaitAsync(TestTimeout));
        }


        /// <summary>
        /// Without a notification the wait returns false after the timeout.
        /// </summary>
        [TestMethod]
        public async Task WaitAsyncTimeoutReturnsFalseTest()
        {
            AlarmActionOutboxSignal signal = new AlarmActionOutboxSignal(_timeProvider);
            Task<bool> wait = signal.WaitAsync(Timeout);
            _timeProvider.Advance(Timeout - TimeSpan.FromMilliseconds(1));
            bool isCompletedBeforeTimeout = wait.IsCompleted;

            _timeProvider.Advance(TimeSpan.FromMilliseconds(1));

            Assert.IsFalse(isCompletedBeforeTimeout);
            Assert.IsFalse(await wait.WaitAsync(TestTimeout));
        }


        /// <summary>
        /// Several notifications before the wait are combined into one, because one send sends all pending actions.
        /// </summary>
        [TestMethod]
        public async Task NotifySeveralTimesCombinesIntoOneTest()
        {
            AlarmActionOutboxSignal signal = new AlarmActionOutboxSignal(_timeProvider);
            signal.Notify();
            signal.Notify();
            signal.Notify();

            bool isFirstNotified = await signal.WaitAsync(Timeout).WaitAsync(TestTimeout);
            Task<bool> secondWait = signal.WaitAsync(Timeout);
            _timeProvider.Advance(Timeout);

            Assert.IsTrue(isFirstNotified);
            Assert.IsFalse(await secondWait.WaitAsync(TestTimeout));
        }


        /// <summary>
        /// A cancelled wait (e.g. shutdown) throws instead of returning a timeout.
        /// </summary>
        [TestMethod]
        public async Task WaitAsyncCancelledThrowsTest()
        {
            AlarmActionOutboxSignal signal = new AlarmActionOutboxSignal(_timeProvider);
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            Task<bool> wait = signal.WaitAsync(Timeout, cancellation.Token);

            await cancellation.CancelAsync();

            await Assert.ThrowsAsync<OperationCanceledException>(() => wait);
        }
    }
}
