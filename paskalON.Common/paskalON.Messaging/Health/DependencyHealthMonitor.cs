// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;

namespace paskalON.Messaging.Health
{
    /// <summary>
    /// Tracks the availability of the dependencies of a service based on signals and polled conditions.
    /// </summary>
    /// <remarks>
    /// Thread safe. A dependency is lost until it is seen the first time, so that a service starts in a safe state.
    /// The state is evaluated when it is requested and a state change is logged once.
    /// </remarks>
    public sealed class DependencyHealthMonitor : IDependencyHealthMonitor
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DependencyHealthMonitor> _logger;


        /// <summary>
        /// Time provider to measure the time since a dependency was last seen.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Lock object to protect the registered dependencies.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Registered dependencies by name.
        /// </summary>
        private readonly Dictionary<string, Dependency> _dependencies = new Dictionary<string, Dependency>(StringComparer.Ordinal);


        /// <summary>
        /// Constructor of <see cref="DependencyHealthMonitor"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="timeProvider">Time provider to measure the time since a dependency was last seen.</param>
        public DependencyHealthMonitor(ILogger<DependencyHealthMonitor> logger, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        public void RegisterSignal(string name, TimeSpan lossTimeout)
        {
            Register(name, null, lossTimeout);
        }


        /// <inheritdoc/>
        public void RegisterCondition(string name, Func<bool> isAvailable, TimeSpan lossTimeout)
        {
            ArgumentNullException.ThrowIfNull(isAvailable);

            Register(name, isAvailable, lossTimeout);
        }


        /// <inheritdoc/>
        public void Signal(string name)
        {
            lock (_dataLock)
            {
                Dependency dependency = GetDependency(name);
                dependency.LastSeenTimestamp = _timeProvider.GetTimestamp();
            }
        }


        /// <inheritdoc/>
        public DependencyState GetState(string name)
        {
            lock (_dataLock)
            {
                return Evaluate(GetDependency(name));
            }
        }


        /// <inheritdoc/>
        public IReadOnlyList<string> GetLostDependencies()
        {
            lock (_dataLock)
            {
                List<string> lostDependencies = new List<string>();

                foreach (Dependency dependency in _dependencies.Values)
                {
                    if (Evaluate(dependency) == DependencyState.Lost)
                    {
                        lostDependencies.Add(dependency.Name);
                    }
                }

                return lostDependencies;
            }
        }


        /// <summary>
        /// Registers a dependency.
        /// </summary>
        /// <param name="name">Unique name of the dependency.</param>
        /// <param name="isAvailable">Condition that returns true while the dependency is available or null for a signaled dependency.</param>
        /// <param name="lossTimeout">Time without being seen after which the dependency is lost.</param>
        private void Register(string name, Func<bool>? isAvailable, TimeSpan lossTimeout)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(lossTimeout, TimeSpan.Zero);

            lock (_dataLock)
            {
                if (_dependencies.ContainsKey(name))
                {
                    throw new ArgumentException($"Dependency {name} is already registered.", nameof(name));
                }

                _dependencies.Add(name, new Dependency(name, lossTimeout, isAvailable));
            }
        }


        /// <summary>
        /// Gets the registered dependency. Must be called within the data lock.
        /// </summary>
        /// <param name="name">Name of the registered dependency.</param>
        /// <returns>The registered dependency.</returns>
        private Dependency GetDependency(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (_dependencies.TryGetValue(name, out Dependency? dependency) is false)
            {
                throw new ArgumentException($"Dependency {name} is not registered.", nameof(name));
            }

            return dependency;
        }


        /// <summary>
        /// Evaluates the availability state of the dependency and logs a state change. Must be called within the data lock.
        /// </summary>
        /// <param name="dependency">The dependency to evaluate.</param>
        /// <returns>The availability state.</returns>
        private DependencyState Evaluate(Dependency dependency)
        {
            long now = _timeProvider.GetTimestamp();

            if (dependency.IsAvailable is not null && dependency.IsAvailable.Invoke())
            {
                dependency.LastSeenTimestamp = now;
            }

            bool isSeen = dependency.LastSeenTimestamp.HasValue && _timeProvider.GetElapsedTime(dependency.LastSeenTimestamp.Value, now) < dependency.LossTimeout;
            DependencyState state = isSeen ? DependencyState.Available : DependencyState.Lost;

            if (state != dependency.ReportedState)
            {
                dependency.ReportedState = state;

                if (state == DependencyState.Lost)
                {
                    _logger.LogWarning("Dependency {Dependency} is lost, not seen within {LossTimeout}.", dependency.Name, dependency.LossTimeout);
                }
                else
                {
                    _logger.LogInformation("Dependency {Dependency} is available.", dependency.Name);
                }
            }

            return state;
        }


        /// <summary>
        /// Registered dependency and its last seen time.
        /// </summary>
        private sealed class Dependency
        {
            /// <summary>
            /// Unique name of the dependency.
            /// </summary>
            public string Name { get; }


            /// <summary>
            /// Time without being seen after which the dependency is lost.
            /// </summary>
            public TimeSpan LossTimeout { get; }


            /// <summary>
            /// Condition that returns true while the dependency is available or null for a signaled dependency.
            /// </summary>
            public Func<bool>? IsAvailable { get; }


            /// <summary>
            /// Timestamp of the time provider when the dependency was last seen or null if never seen.
            /// </summary>
            public long? LastSeenTimestamp { get; set; }


            /// <summary>
            /// Last logged availability state.
            /// </summary>
            public DependencyState ReportedState { get; set; } = DependencyState.Lost;


            /// <summary>
            /// Constructor of <see cref="Dependency"/>.
            /// </summary>
            /// <param name="name">Unique name of the dependency.</param>
            /// <param name="lossTimeout">Time without being seen after which the dependency is lost.</param>
            /// <param name="isAvailable">Condition that returns true while the dependency is available or null for a signaled dependency.</param>
            public Dependency(string name, TimeSpan lossTimeout, Func<bool>? isAvailable)
            {
                Name = name;
                LossTimeout = lossTimeout;
                IsAvailable = isAvailable;
            }
        }
    }
}
