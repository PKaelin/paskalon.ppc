// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Handlers;

namespace paskalON.Alarms.Application.UnitTest.Handlers
{
    /// <summary>
    /// Tests <see cref="PpcAlarmWriteLock"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmWriteLockTest
    {
        /// <summary>
        /// Maximum real time to wait for a completion in the tests.
        /// </summary>
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        /// <summary>
        /// A second writer waits until the first writer released the lock.
        /// </summary>
        [TestMethod]
        public async Task AcquireAsyncSecondWriterWaitsForReleaseTest()
        {
            PpcAlarmWriteLock writeLock = new PpcAlarmWriteLock();
            IDisposable firstWriter = await writeLock.AcquireAsync();
            Task<IDisposable> secondWriter = writeLock.AcquireAsync();
            bool isAcquiredWhileLocked = secondWriter.IsCompleted;

            firstWriter.Dispose();

            using IDisposable acquired = await secondWriter.WaitAsync(TestTimeout);
            Assert.IsFalse(isAcquiredWhileLocked);
        }


        /// <summary>
        /// Disposing the releaser twice releases the lock only once, a third writer still waits for the second writer.
        /// </summary>
        [TestMethod]
        public async Task AcquireAsyncDisposeTwiceReleasesOnceTest()
        {
            PpcAlarmWriteLock writeLock = new PpcAlarmWriteLock();
            IDisposable firstWriter = await writeLock.AcquireAsync();
            Task<IDisposable> secondWriter = writeLock.AcquireAsync();
            Task<IDisposable> thirdWriter = writeLock.AcquireAsync();

            firstWriter.Dispose();
            firstWriter.Dispose();

            using IDisposable acquired = await secondWriter.WaitAsync(TestTimeout);
            Assert.IsFalse(thirdWriter.IsCompleted);
        }


        /// <summary>
        /// A cancelled wait (e.g. shutdown) throws and does not acquire the lock.
        /// </summary>
        [TestMethod]
        public async Task AcquireAsyncCancelledThrowsTest()
        {
            PpcAlarmWriteLock writeLock = new PpcAlarmWriteLock();
            using IDisposable firstWriter = await writeLock.AcquireAsync();
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            Task<IDisposable> secondWriter = writeLock.AcquireAsync(cancellation.Token);

            await cancellation.CancelAsync();

            await Assert.ThrowsAsync<OperationCanceledException>(() => secondWriter);
        }
    }
}
