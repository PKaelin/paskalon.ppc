// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Messaging.Health
{
    /// <summary>
    /// Defines a monitor that tracks the availability of the dependencies of a service.
    /// </summary>
    public interface IDependencyHealthMonitor
    {
        /// <summary>
        /// Registers a dependency whose availability is signaled, e.g. by a received heartbeat or data message.
        /// </summary>
        /// <param name="name">Unique name of the dependency.</param>
        /// <param name="lossTimeout">Time without a signal after which the dependency is lost.</param>
        void RegisterSignal(string name, TimeSpan lossTimeout);


        /// <summary>
        /// Registers a dependency whose availability is polled by a condition, e.g. the connection state of the message broker.
        /// </summary>
        /// <param name="name">Unique name of the dependency.</param>
        /// <param name="isAvailable">Condition that returns true while the dependency is available. Must be fast and thread safe.</param>
        /// <param name="lossTimeout">Time the condition must be false continuously after which the dependency is lost.</param>
        void RegisterCondition(string name, Func<bool> isAvailable, TimeSpan lossTimeout);


        /// <summary>
        /// Signals that the dependency is alive.
        /// </summary>
        /// <param name="name">Name of the registered dependency.</param>
        void Signal(string name);


        /// <summary>
        /// Gets the current availability state of the dependency.
        /// </summary>
        /// <param name="name">Name of the registered dependency.</param>
        /// <returns>The availability state.</returns>
        DependencyState GetState(string name);


        /// <summary>
        /// Gets the names of all registered dependencies that are currently lost.
        /// </summary>
        /// <returns>The names of the lost dependencies, empty if all are available.</returns>
        IReadOnlyList<string> GetLostDependencies();
    }
}
