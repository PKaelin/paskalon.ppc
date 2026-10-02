// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;

namespace paskalON.OperatingModes.Application.Stacks
{
    /// <summary>
    /// Stores selected operating modes in unique priority order.
    /// </summary>
    /// <remarks>
    /// OperatingModeStack is used by the StackManager to manage the selection and ordering of operating modes.
    /// It ensures that each operating mode has a unique priority and provides thread-safe access to the stack.
    /// </remarks>
    public class OperatingModeStack
    {
        /// <summary>
        /// Lock object for thread-safe access to the operating mode stack.
        /// </summary>
        private readonly object _dataLock = new();


        /// <summary>
        /// Stores operating modes by their unique priority in ascending order.
        /// </summary>
        private readonly SortedDictionary<int, OperatingModeBase> _operatingModesByPriority = new SortedDictionary<int, OperatingModeBase>();


        /// <summary>
        /// Stores the priority of each operating mode by its name for quick lookup.
        /// </summary>
        private readonly Dictionary<string, int> _prioritiesByOperatingModeName = new Dictionary<string, int>(StringComparer.Ordinal);


        /// <summary>
        /// Gets selected operating modes ordered by ascending priority.
        /// </summary>
        public IReadOnlyCollection<OperatingModeBase> OperatingModes
        {
            get
            {
                lock (_dataLock)
                {
                    return _operatingModesByPriority.Values;
                }
            }
        }


        /// <summary>
        /// Adds an operating mode at the specified priority.
        /// </summary>
        /// <param name="operatingMode">The operating mode to add.</param>
        /// <param name="priority">The unique priority to assign.</param>
        /// <exception cref="ArgumentNullException">The operating mode is null.</exception>
        /// <exception cref="InvalidOperationException">The operating mode name is already in the stack.</exception>
        /// <exception cref="ArgumentOutOfRangeException">An existing priority cannot be shifted further.</exception>
        public void Add(OperatingModeBase operatingMode, int priority)
        {
            ArgumentNullException.ThrowIfNull(operatingMode);

            lock (_dataLock)
            {
                if (_prioritiesByOperatingModeName.ContainsKey(operatingMode.Name))
                {
                    throw new InvalidOperationException($"Operating mode '{operatingMode.Name}' is already in the stack.");
                }

                InsertAtPriority(operatingMode, priority);
            }
        }


        /// <summary>
        /// Moves an operating mode to a new priority.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to move.</param>
        /// <param name="priority">The unique priority to assign.</param>
        /// <exception cref="ArgumentException">The operating mode name is null or empty.</exception>
        /// <exception cref="KeyNotFoundException">The operating mode is not in the stack.</exception>
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

                OperatingModeBase operatingMode = _operatingModesByPriority[currentPriority];
                InsertAtPriority(operatingMode, priority, currentPriority);
            }
        }


        /// <summary>
        /// Removes an operating mode from the stack.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to remove.</param>
        /// <exception cref="ArgumentException">The operating mode name is null or empty.</exception>
        /// <exception cref="KeyNotFoundException">The operating mode is not in the stack.</exception>
        public void Remove(string operatingModeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);

            lock (_dataLock)
            {
                if (_prioritiesByOperatingModeName.Remove(operatingModeName, out int priority) is false)
                {
                    throw new KeyNotFoundException($"Operating mode '{operatingModeName}' is not in the stack.");
                }

                _operatingModesByPriority.Remove(priority);
            }
        }


        /// <summary>
        /// Removes all operating modes from the stack.
        /// </summary>
        internal void Clear()
        {
            lock (_dataLock)
            {
                _operatingModesByPriority.Clear();
                _prioritiesByOperatingModeName.Clear();
            }
        }


        /// <summary>
        /// Inserts an operating mode at the specified priority, shifting existing modes as necessary.
        /// </summary>
        /// <param name="operatingMode">The operating mode to insert.</param>
        /// <param name="priority">The priority at which to insert the operating mode.</param>
        /// <param name="priorityToReplace">The priority to replace, if any.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an existing operating mode priority cannot be shifted further.</exception>
        private void InsertAtPriority(OperatingModeBase operatingMode, int priority, int? priorityToReplace = null)
        {
            List<(int Priority, OperatingModeBase OperatingMode)> reprioritizedModes = new List<(int Priority, OperatingModeBase OperatingMode)>();

            foreach (KeyValuePair<int, OperatingModeBase> entry in _operatingModesByPriority)
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

            _operatingModesByPriority.Clear();
            _prioritiesByOperatingModeName.Clear();

            foreach ((int updatedPriority, OperatingModeBase updatedOperatingMode) in reprioritizedModes)
            {
                _operatingModesByPriority.Add(updatedPriority, updatedOperatingMode);
                _prioritiesByOperatingModeName.Add(updatedOperatingMode.Name, updatedPriority);
            }
        }
    }
}