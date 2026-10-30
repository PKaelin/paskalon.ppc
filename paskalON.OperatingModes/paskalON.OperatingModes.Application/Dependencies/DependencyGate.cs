// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Messaging.Health;

namespace paskalON.OperatingModes.Application.Dependencies
{
    /// <summary>
    /// Decides whether the operating mode control must hold its setpoints based on the availability of its dependencies.
    /// </summary>
    /// <remarks>
    /// Thread safe. The operating mode control holds while a dependency is lost and until all dependencies are available
    /// continuously for the resume settle time, so that a flapping dependency does not cause repeated resumes.
    /// </remarks>
    public sealed class DependencyGate
    {
        /// <summary>
        /// Monitor tracking the availability of the dependencies.
        /// </summary>
        private readonly IDependencyHealthMonitor _monitor;


        /// <summary>
        /// Time all dependencies must be available before the operating mode control resumes.
        /// </summary>
        private readonly TimeSpan _resumeSettleTime;


        /// <summary>
        /// Time provider to measure the settle time.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Lock object to protect the settle start.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Timestamp of the time provider since when all dependencies are available or null if a dependency is lost.
        /// </summary>
        private long? _availableSinceTimestamp;


        /// <summary>
        /// Constructor of <see cref="DependencyGate"/>.
        /// </summary>
        /// <param name="monitor">Monitor tracking the availability of the dependencies.</param>
        /// <param name="resumeSettleTime">Time all dependencies must be available before the operating mode control resumes.</param>
        /// <param name="timeProvider">Time provider to measure the settle time.</param>
        public DependencyGate(IDependencyHealthMonitor monitor, TimeSpan resumeSettleTime, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(monitor);
            ArgumentOutOfRangeException.ThrowIfLessThan(resumeSettleTime, TimeSpan.Zero);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _monitor = monitor;
            _resumeSettleTime = resumeSettleTime;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// Gets the reason why the operating mode control must hold its setpoints.
        /// </summary>
        /// <returns>The hold reason or null if the operating mode control may calculate.</returns>
        public string? GetHoldReason()
        {
            IReadOnlyList<string> lostDependencies = _monitor.GetLostDependencies();

            lock (_dataLock)
            {
                if (lostDependencies.Count > 0)
                {
                    _availableSinceTimestamp = null;

                    return $"Lost dependencies: {string.Join(", ", lostDependencies)}.";
                }

                long now = _timeProvider.GetTimestamp();
                _availableSinceTimestamp ??= now;

                if (_timeProvider.GetElapsedTime(_availableSinceTimestamp.Value, now) < _resumeSettleTime)
                {
                    return $"Dependencies are available, settling for {_resumeSettleTime.TotalMilliseconds} ms.";
                }

                return null;
            }
        }
    }
}
