// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Transitions;

namespace paskalON.OperatingModes.Domain.Overrides
{
    /// <summary>
    /// Override slot that stores the active override modes. Override modes are calculated before the operating mode stack.
    /// </summary>
    /// <remarks>
    /// Cardinality: 0..1 override per DER unit and 0..1 plant-wide override (target DER unit is null).
    /// Committed changes are applied by the system operating mode control in its next calculation cycle: it activates added and
    /// deactivates removed override modes. See <see cref="TakeCycle"/>.
    /// </remarks>
    public class OperatingModeOverrides : IUnitOverrideLookup
    {
        /// <summary>
        /// Lock object for thread-safe access to the override slot.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Active override modes.
        /// </summary>
        private readonly List<IOverrideMode> _operatingModes = new List<IOverrideMode>();


        /// <summary>
        /// Committed changes that are not yet applied by a calculation cycle.
        /// </summary>
        private readonly List<OperatingModeChange<IOverrideMode>> _pendingChanges = new List<OperatingModeChange<IOverrideMode>>();


        /// <summary>
        /// Gets a snapshot of the active override modes.
        /// </summary>
        public IReadOnlyList<IOverrideMode> OperatingModes
        {
            get
            {
                lock (_dataLock)
                {
                    return _operatingModes.ToArray();
                }
            }
        }


        /// <summary>
        /// Adds an override mode to the override slot.
        /// </summary>
        /// <param name="operatingMode">The override mode to add.</param>
        /// <exception cref="ArgumentNullException">The override mode is null.</exception>
        /// <exception cref="InvalidOperationException">The override mode is already active or its DER unit is already overridden.</exception>
        public void Add(IOverrideMode operatingMode)
        {
            ArgumentNullException.ThrowIfNull(operatingMode);

            lock (_dataLock)
            {
                if (_operatingModes.Any(mode => string.Equals(mode.Name, operatingMode.Name, StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException($"Override mode '{operatingMode.Name}' is already active.");
                }

                IOverrideMode? existing = _operatingModes.FirstOrDefault(mode => string.Equals(mode.TargetDerUnit, operatingMode.TargetDerUnit, StringComparison.Ordinal));

                if (existing is not null)
                {
                    string target = operatingMode.TargetDerUnit ?? "plant";
                    throw new InvalidOperationException($"Override target '{target}' is already overridden by '{existing.Name}'.");
                }

                _operatingModes.Add(operatingMode);
                _pendingChanges.Add(new OperatingModeChange<IOverrideMode>(OperatingModeChangeKind.Added, operatingMode));
            }
        }


        /// <summary>
        /// Removes an override mode from the override slot.
        /// </summary>
        /// <param name="operatingMode">The override mode to remove.</param>
        /// <exception cref="ArgumentNullException">The override mode is null.</exception>
        /// <exception cref="KeyNotFoundException">The override mode is not active.</exception>
        public void Remove(IOverrideMode operatingMode)
        {
            ArgumentNullException.ThrowIfNull(operatingMode);

            lock (_dataLock)
            {
                if (_operatingModes.Remove(operatingMode) is false)
                {
                    throw new KeyNotFoundException($"Override mode '{operatingMode.Name}' is not active.");
                }

                _pendingChanges.Add(new OperatingModeChange<IOverrideMode>(OperatingModeChangeKind.Removed, operatingMode));
            }
        }


        /// <inheritdoc/>
        public bool IsOverridden(string derUnitName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(derUnitName);

            lock (_dataLock)
            {
                return _operatingModes.Any(mode => string.Equals(mode.TargetDerUnit, derUnitName, StringComparison.Ordinal));
            }
        }


        /// <summary>
        /// Takes a consistent snapshot of the active override modes and the committed changes for one calculation cycle.
        /// </summary>
        /// <remarks>
        /// The returned changes are removed from the pending changes, they are applied by the calling calculation cycle.
        /// </remarks>
        /// <returns>The active override modes and the changes since the last cycle.</returns>
        public OperatingModeCycle<IOverrideMode> TakeCycle()
        {
            lock (_dataLock)
            {
                OperatingModeChange<IOverrideMode>[] changes = _pendingChanges.ToArray();
                _pendingChanges.Clear();

                return new OperatingModeCycle<IOverrideMode>(_operatingModes.ToArray(), changes);
            }
        }


        /// <summary>
        /// Removes all override modes from the override slot.
        /// </summary>
        public void Clear()
        {
            lock (_dataLock)
            {
                _operatingModes.Clear();
                _pendingChanges.Clear();
            }
        }
    }
}
