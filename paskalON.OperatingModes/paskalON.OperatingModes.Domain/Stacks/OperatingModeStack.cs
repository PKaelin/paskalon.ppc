// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Transitions;

namespace paskalON.OperatingModes.Domain.Stacks
{
    /// <summary>
    /// Stores selected operating modes in unique priority order and records the committed changes.
    /// </summary>
    /// <remarks>
    /// This operating mode stack is the actual active stack of operating modes that is applied by the system operating mode control.
    /// It is managed by the <see cref="IStackManager"/>
    /// Every change is validated with <see cref="OperatingModeStackValidator"/> before it is committed, so the stack is always valid.
    /// Committed changes are applied by the system operating mode control in its next calculation cycle: it activates added,
    /// deactivates removed operating modes and transitions the applied targets. See <see cref="TakeCycle"/>.
    /// </remarks>
    public class OperatingModeStack
    {
        /// <summary>
        /// Lock object for thread-safe access to the operating mode stack.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Stores operating modes by their unique priority in ascending order.
        /// </summary>
        private readonly SortedDictionary<int, IStackableMode> _operatingModesByPriority = new SortedDictionary<int, IStackableMode>();


        /// <summary>
        /// Stores the priority of each operating mode by its name for quick lookup.
        /// </summary>
        private readonly Dictionary<string, int> _prioritiesByOperatingModeName = new Dictionary<string, int>(StringComparer.Ordinal);


        /// <summary>
        /// Committed changes that are not yet applied by a calculation cycle.
        /// </summary>
        private readonly List<OperatingModeChange<IStackableMode>> _pendingChanges = new List<OperatingModeChange<IStackableMode>>();


        /// <summary>
        /// Gets a snapshot of the selected operating modes ordered by ascending priority.
        /// </summary>
        public IReadOnlyList<IStackableMode> OperatingModes
        {
            get
            {
                lock (_dataLock)
                {
                    return _operatingModesByPriority.Values.ToArray();
                }
            }
        }


        /// <summary>
        /// Gets a snapshot of the committed changes that are not yet applied by a calculation cycle.
        /// </summary>
        public IReadOnlyList<OperatingModeChange<IStackableMode>> PendingChanges
        {
            get
            {
                lock (_dataLock)
                {
                    return _pendingChanges.ToArray();
                }
            }
        }


        /// <summary>
        /// Adds an operating mode at the specified priority.
        /// </summary>
        /// <param name="operatingMode">The operating mode to add.</param>
        /// <param name="priority">The unique priority to assign.</param>
        /// <exception cref="ArgumentNullException">The operating mode is null.</exception>
        /// <exception cref="InvalidOperationException">The operating mode is already in the stack or the resulting stack is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">An existing priority cannot be shifted further.</exception>
        public void Add(IStackableMode operatingMode, int priority)
        {
            ArgumentNullException.ThrowIfNull(operatingMode);

            lock (_dataLock)
            {
                if (_prioritiesByOperatingModeName.ContainsKey(operatingMode.Name))
                {
                    throw new InvalidOperationException($"Operating mode '{operatingMode.Name}' is already in the stack.");
                }

                // Insert the new operating mode at the specified priority, shifting existing modes as necessary
                List<(int Priority, IStackableMode OperatingMode)> candidate = InsertAtPriority(operatingMode, priority);
                // Validate the resulting stack to ensure it is valid
                OperatingModeStackValidator.Validate(candidate.Select(entry => entry.OperatingMode).ToList());
                // Only now commit the changes to the stack and record the pending change
                Commit(candidate);
                _pendingChanges.Add(new OperatingModeChange<IStackableMode>(OperatingModeChangeKind.Added, operatingMode));
            }
        }


        /// <summary>
        /// Replaces an operating mode with another operating mode at the same priority in one atomic change.
        /// </summary>
        /// <param name="predecessorName">The name of the operating mode to replace.</param>
        /// <param name="operatingMode">The replacing operating mode.</param>
        /// <exception cref="ArgumentException">The predecessor name is null or empty.</exception>
        /// <exception cref="ArgumentNullException">The operating mode is null.</exception>
        /// <exception cref="KeyNotFoundException">The predecessor is not in the stack.</exception>
        /// <exception cref="InvalidOperationException">The operating mode is already in the stack or the resulting stack is invalid.</exception>
        public void Replace(string predecessorName, IStackableMode operatingMode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(predecessorName);
            ArgumentNullException.ThrowIfNull(operatingMode);

            lock (_dataLock)
            {
                if (_prioritiesByOperatingModeName.TryGetValue(predecessorName, out int priority) is false)
                {
                    throw new KeyNotFoundException($"Operating mode '{predecessorName}' is not in the stack.");
                }

                if (_prioritiesByOperatingModeName.ContainsKey(operatingMode.Name))
                {
                    throw new InvalidOperationException($"Operating mode '{operatingMode.Name}' is already in the stack.");
                }

                IStackableMode predecessor = _operatingModesByPriority[priority];
                List<(int Priority, IStackableMode OperatingMode)> candidate = _operatingModesByPriority
                    .Select(entry => (entry.Key, entry.Key == priority ? operatingMode : entry.Value))
                    .ToList();
                OperatingModeStackValidator.Validate(candidate.Select(entry => entry.OperatingMode).ToList());
                Commit(candidate);
                _pendingChanges.Add(new OperatingModeChange<IStackableMode>(OperatingModeChangeKind.Replaced, operatingMode, predecessor));
            }
        }


        /// <summary>
        /// Moves an operating mode to a new priority.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to move.</param>
        /// <param name="priority">The unique priority to assign.</param>
        /// <exception cref="ArgumentException">The operating mode name is null or empty.</exception>
        /// <exception cref="KeyNotFoundException">The operating mode is not in the stack.</exception>
        /// <exception cref="InvalidOperationException">The resulting stack is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">An existing priority cannot be shifted further.</exception>
        public void Move(string operatingModeName, int priority)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);

            lock (_dataLock)
            {
                if (_prioritiesByOperatingModeName.TryGetValue(operatingModeName, out int currentPriority) is false)
                {
                    throw new KeyNotFoundException($"Operating mode '{operatingModeName}' is not in the stack.");
                }

                if (currentPriority == priority)
                {
                    return;
                }

                // Get the operating mode to move and its current priority
                IStackableMode operatingMode = _operatingModesByPriority[currentPriority];
                // Insert the new operating mode at the specified priority, shifting existing modes as necessary
                List<(int Priority, IStackableMode OperatingMode)> candidate = InsertAtPriority(operatingMode, priority, currentPriority);
                // Validate the resulting stack to ensure it is valid
                OperatingModeStackValidator.Validate(candidate.Select(entry => entry.OperatingMode).ToList());
                // Only now commit the changes to the stack and record the pending change
                Commit(candidate);
                // No pending changes are recorded for a move, as it does not change the set of operating modes, only their order.
            }
        }


        /// <summary>
        /// Removes an operating mode from the stack.
        /// </summary>
        /// <param name="operatingMode">The operating mode to remove.</param>
        /// <exception cref="ArgumentNullException">The operating mode is null.</exception>
        /// <exception cref="KeyNotFoundException">The operating mode is not in the stack.</exception>
        /// <exception cref="InvalidOperationException">The resulting stack is invalid, e.g. a base mode is removed while its dependents remain.</exception>
        public void Remove(IStackableMode operatingMode)
        {
            ArgumentNullException.ThrowIfNull(operatingMode);

            lock (_dataLock)
            {
                if (_prioritiesByOperatingModeName.TryGetValue(operatingMode.Name, out int priority) is false)
                {
                    throw new KeyNotFoundException($"Operating mode '{operatingMode.Name}' is not in the stack.");
                }

                // Remove the operating mode from the stack and validate the resulting stack
                List<(int Priority, IStackableMode OperatingMode)> candidate = _operatingModesByPriority
                    .Where(entry => entry.Key != priority)
                    .Select(entry => (entry.Key, entry.Value))
                    .ToList();
                // Validate the resulting stack to ensure it is valid
                OperatingModeStackValidator.Validate(candidate.Select(entry => entry.OperatingMode).ToList());
                // Only now commit the changes to the stack and record the pending change
                Commit(candidate);
                _pendingChanges.Add(new OperatingModeChange<IStackableMode>(OperatingModeChangeKind.Removed, operatingMode));
            }
        }


        /// <summary>
        /// Takes a consistent snapshot of the selected operating modes and the committed changes for one calculation cycle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The stack is changed by the API thread (<see cref="Add"/>, <see cref="Remove"/>, <see cref="Replace"/>, <see cref="Move"/>)
        /// while the system operating mode control calculates periodically on its own thread. The stack only commits a change
        /// (validated, in the priority order) and records it as pending change. It does not activate or deactivate operating modes.
        /// The system operating mode control calls this method once at the beginning of every calculation cycle and applies the
        /// returned changes itself: it activates added, deactivates removed or replaced operating modes and starts the transitions
        /// from the current applied target. Like this the lifecycle and the transitions run on the calculation thread only and
        /// an operating mode is never activated or deactivated while a cycle is calculating it.
        /// </para>
        /// <para>
        /// The selected operating modes and the changes are read under the same lock, so they always belong together:
        /// every returned change is already reflected in the returned operating modes, and no change committed between two calls
        /// is lost or applied twice. A change committed while a cycle is running is returned by the next call.
        /// </para>
        /// <para>
        /// Taking the cycle removes the returned changes from the pending changes, so each change is applied by exactly one
        /// calculation cycle. Only the system operating mode control may call this method; use <see cref="OperatingModes"/> and
        /// <see cref="PendingChanges"/> to read the stack without consuming the changes.
        /// </para>
        /// <para>
        /// Several changes between two cycles are returned in commit order, e.g. an operating mode added and removed again before
        /// the next cycle is returned as an added and a removed change. The calculation cycle combines them per operating mode.
        /// </para>
        /// </remarks>
        /// <returns>The selected operating modes in priority order (calculation order) and the changes since the last cycle in commit order.</returns>
        public OperatingModeCycle<IStackableMode> TakeCycle()
        {
            lock (_dataLock)
            {
                // Copy the changes before clearing them, the caller owns the returned array
                OperatingModeChange<IStackableMode>[] changes = _pendingChanges.ToArray();

                // Consume the changes so that the next cycle only receives changes committed after this call
                _pendingChanges.Clear();

                // Copy the operating modes in the same lock so that they include every returned change
                IStackableMode[] operatingModes = _operatingModesByPriority.Values.ToArray();

                return new OperatingModeCycle<IStackableMode>(operatingModes, changes);
            }
        }


        /// <summary>
        /// Removes all operating modes and pending changes from the stack.
        /// </summary>
        public void Clear()
        {
            lock (_dataLock)
            {
                _operatingModesByPriority.Clear();
                _prioritiesByOperatingModeName.Clear();
                _pendingChanges.Clear();
            }
        }


        /// <summary>
        /// Builds the candidate priorities with the operating mode inserted at the specified priority, shifting existing modes as necessary.
        /// </summary>
        /// <param name="operatingMode">The operating mode to insert.</param>
        /// <param name="priority">The priority at which to insert the operating mode.</param>
        /// <param name="priorityToReplace">The priority to replace, if any.</param>
        /// <returns>The candidate priorities ordered by ascending priority.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an existing operating mode priority cannot be shifted further.</exception>
        private List<(int Priority, IStackableMode OperatingMode)> InsertAtPriority(IStackableMode operatingMode, int priority, int? priorityToReplace = null)
        {
            List<(int Priority, IStackableMode OperatingMode)> reprioritizedModes = new List<(int Priority, IStackableMode OperatingMode)>();

            foreach (KeyValuePair<int, IStackableMode> entry in _operatingModesByPriority)
            {
                if (priorityToReplace.HasValue && entry.Key == priorityToReplace.Value)
                {
                    continue;
                }

                if (entry.Key >= priority && entry.Key == int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(priority), "An existing operating mode priority cannot be shifted further.");
                }

                int updatedPriority = entry.Key >= priority ? entry.Key + 1 : entry.Key;
                reprioritizedModes.Add((updatedPriority, entry.Value));
            }

            reprioritizedModes.Add((priority, operatingMode));
            reprioritizedModes.Sort((left, right) => left.Priority.CompareTo(right.Priority));

            return reprioritizedModes;
        }


        /// <summary>
        /// Commits the candidate priorities to the stack.
        /// </summary>
        /// <param name="candidate">The validated candidate priorities.</param>
        private void Commit(List<(int Priority, IStackableMode OperatingMode)> candidate)
        {
            // Lock is on the calling method
            _operatingModesByPriority.Clear();
            _prioritiesByOperatingModeName.Clear();

            foreach ((int updatedPriority, IStackableMode updatedOperatingMode) in candidate)
            {
                _operatingModesByPriority.Add(updatedPriority, updatedOperatingMode);
                _prioritiesByOperatingModeName.Add(updatedOperatingMode.Name, updatedPriority);
            }
        }
    }
}
