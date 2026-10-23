// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Stacks;

namespace paskalON.OperatingModes.Application.Stacks
{
    /// <summary>
    /// Manages the stack of operating modes, allowing for adding, moving, and removing operating modes from the stack.
    /// </summary>
    /// <remarks>
    /// StackManager is part of the <see cref="OperatingModeManager"/> to provide a structured way to manage the selection and
    /// prioritization of operating modes. It maintains a collection of available operating modes and a stack of active
    /// operating modes, enabling dynamic adjustments to the operating modes selected.
    /// The stack validates and records every change. The system operating mode control activates added, deactivates removed
    /// operating modes and transitions the applied targets in its next calculation cycle.
    /// </remarks>
    public class StackManager : IStackManager
    {
        /// <summary>
        /// Operating modes available for selection.
        /// </summary>
        private Dictionary<string, IStackableMode> _availableOperatingModes = new Dictionary<string, IStackableMode>(StringComparer.Ordinal);


        /// <inheritdoc/>
        public OperatingModeStack Stack { get; } = new OperatingModeStack();


        /// <inheritdoc/>
        public IReadOnlyCollection<IStackableMode> AvailableOperatingModes { get => _availableOperatingModes.Values; }


        /// <inheritdoc/>
        public IReadOnlyCollection<IStackableMode> ActiveOperatingModes { get => Stack.OperatingModes; }


        /// <inheritdoc/>
        public IReadOnlyList<OperatingModeStackEntry> ActiveOperatingModeEntries { get => Stack.Entries; }


        /// <inheritdoc/>
        public void InitializeStack(Dictionary<string, OperatingModeBase> operatingModes)
        {
            ArgumentNullException.ThrowIfNull(operatingModes);

            _availableOperatingModes = operatingModes.Values
                .OfType<IStackableMode>()
                .ToDictionary(operatingMode => operatingMode.Name, StringComparer.Ordinal);
            Stack.Clear();
        }


        /// <inheritdoc/>
        public void AddOperatingMode(string operatingModeName, int priority)
        {
            IStackableMode operatingMode = GetAvailableOperatingMode(operatingModeName);
            Stack.Add(operatingMode, priority);
        }


        /// <inheritdoc/>
        public void MoveOperatingMode(string operatingModeName, int priority)
        {
            Stack.Move(operatingModeName, priority);
        }


        /// <inheritdoc/>
        public void RemoveOperatingMode(string operatingModeName)
        {
            IStackableMode operatingMode = GetAvailableOperatingMode(operatingModeName);
            Stack.Remove(operatingMode);
        }


        /// <inheritdoc/>
        public void ReplaceOperatingMode(string predecessorName, string operatingModeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(predecessorName);

            IStackableMode operatingMode = GetAvailableOperatingMode(operatingModeName);
            Stack.Replace(predecessorName, operatingMode);
        }


        /// <summary>
        /// Gets an available operating mode by its name.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode.</param>
        /// <returns>The available operating mode.</returns>
        /// <exception cref="KeyNotFoundException">The operating mode is not configured or not selectable.</exception>
        private IStackableMode GetAvailableOperatingMode(string operatingModeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);

            if (_availableOperatingModes.TryGetValue(operatingModeName, out IStackableMode? operatingMode) is false)
            {
                throw new KeyNotFoundException($"Operating mode '{operatingModeName}' is not configured or not selectable.");
            }

            return operatingMode;
        }
    }
}
