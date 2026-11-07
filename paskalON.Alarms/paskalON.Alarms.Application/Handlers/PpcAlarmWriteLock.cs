// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Application.Handlers
{
    /// <summary>
    /// Lock of the alarm writers, so that only one writer at a time changes and saves the cached active alarms.
    /// </summary>
    /// <remarks>
    /// - Singleton shared by the alarm handler service (evaluations) and the web API (e.g. normalize orphaned alarms
    ///   after a configuration reload, acknowledge).
    /// - The cached active alarms are shared instances that are changed before they are saved, two writers at the same
    ///   time would change and save the same instances.
    /// - Asynchronous, so that the lock can be held while saving to the database.
    /// </remarks>
    public class PpcAlarmWriteLock : IPpcAlarmWriteLock
    {
        /// <summary>
        /// Semaphore of the alarm writers.
        /// </summary>
        private readonly SemaphoreSlim _dataLock = new SemaphoreSlim(1, 1);


        /// <inheritdoc/>
        public async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default)
        {
            await _dataLock.WaitAsync(cancellationToken);

            return new Releaser(_dataLock);
        }


        /// <summary>
        /// Releases the lock once when disposed.
        /// </summary>
        private sealed class Releaser : IDisposable
        {
            /// <summary>
            /// Semaphore to release, null after it has been released.
            /// </summary>
            private SemaphoreSlim? _semaphore;


            /// <summary>
            /// Constructor of <see cref="Releaser"/>.
            /// </summary>
            /// <param name="semaphore">Acquired semaphore.</param>
            public Releaser(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }


            /// <inheritdoc/>
            public void Dispose()
            {
                Interlocked.Exchange(ref _semaphore, null)?.Release();
            }
        }
    }
}
