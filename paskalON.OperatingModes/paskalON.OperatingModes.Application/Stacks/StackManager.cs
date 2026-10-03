// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;

namespace paskalON.OperatingModes.Application.Stacks
{
    /// <summary>
    /// Manages the stack of operating modes, allowing for adding, moving, and removing operating modes from the stack.
    /// </summary>
    /// <remarks>
    /// StackManager is part of the <see cref="OperatingModeManager"/> to provide a structured way to manage the selection and
    /// prioritization of operating modes. It maintains a collection of available operating modes and a stack of selected
    /// operating modes, enabling dynamic adjustments to the operating modes selected.
    /// </remarks>
    public class StackManager : IStackManager
    {
        /// <summary>
        /// Operating modes available for selection.
        /// </summary>
        private Dictionary<string, OperatingModeBase> _availableOperatingModes = new Dictionary<string, OperatingModeBase>(StringComparer.Ordinal);


        /// <summary>
        /// Stack of selected operating modes.
        /// </summary>
        private readonly OperatingModeStack _operatingModeStack = new OperatingModeStack();


        /// <inheritdoc/>
        public IReadOnlyCollection<OperatingModeBase> AvailableOperatingModes => _availableOperatingModes.Values;


        /// <inheritdoc/>
        public IReadOnlyCollection<OperatingModeBase> SelectedOperatingModes => _operatingModeStack.OperatingModes;


        /// <inheritdoc/>
        public void InitializeStack(Dictionary<string, OperatingModeBase> availableOperatingModes)
        {
            ArgumentNullException.ThrowIfNull(availableOperatingModes);

            _availableOperatingModes = availableOperatingModes;
            _operatingModeStack.Clear();
        }


        /// <inheritdoc/>
        public void AddOperatingMode(string operatingModeName, int priority)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);

            if (_availableOperatingModes.TryGetValue(operatingModeName, out OperatingModeBase? operatingMode) is false)
            {
                throw new KeyNotFoundException($"Operating mode '{operatingModeName}' is not configured.");
            }

            _operatingModeStack.Add(operatingMode, priority);
        }


        /// <inheritdoc/>
        public void MoveOperatingMode(string operatingModeName, int priority)
        {
            _operatingModeStack.Move(operatingModeName, priority);
        }


        /// <inheritdoc/>
        public void RemoveOperatingMode(string operatingModeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);

            if (_availableOperatingModes.TryGetValue(operatingModeName, out OperatingModeBase? operatingMode) is false)
            {
                throw new KeyNotFoundException($"Operating mode '{operatingModeName}' is not configured.");
            }

            _operatingModeStack.Remove(operatingMode);
        }
    }
}
