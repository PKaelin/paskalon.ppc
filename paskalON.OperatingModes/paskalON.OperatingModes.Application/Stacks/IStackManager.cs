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
    /// Interface for managing the operating mode stack.
    /// </summary>
    public interface IStackManager
    {
        /// <summary>
        /// Gets the operating mode stack.
        /// </summary>
        OperatingModeStack Stack { get; }


        /// <summary>
        /// Operating modes available for selection by a user.
        /// </summary>
        IReadOnlyCollection<IStackableMode> AvailableOperatingModes { get; }


        /// <summary>
        /// Selected operating modes ordered by ascending priority.
        /// </summary>
        IReadOnlyCollection<IStackableMode> ActiveOperatingModes { get; }


        /// <summary>
        /// Selected operating modes with their priorities ordered by ascending priority.
        /// </summary>
        IReadOnlyList<OperatingModeStackEntry> ActiveOperatingModeEntries { get; }


        /// <summary>
        /// Initializes the stack manager with the configured operating modes. Only stackable operating modes are available for selection.
        /// </summary>
        /// <param name="operatingModes">A dictionary of configured operating modes keyed by their names.</param>
        void InitializeStack(Dictionary<string, OperatingModeBase> operatingModes);


        /// <summary>
        /// Adds an available operating mode to the selected stack. It is activated by the next calculation cycle.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to add.</param>
        /// <param name="priority">The unique priority to assign.</param>
        void AddOperatingMode(string operatingModeName, int priority);


        /// <summary>
        /// Moves a selected operating mode to a new priority.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to move.</param>
        /// <param name="priority">The unique priority to assign.</param>
        void MoveOperatingMode(string operatingModeName, int priority);


        /// <summary>
        /// Removes an operating mode from the selected stack. It is deactivated by the next calculation cycle which transitions the applied target.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to remove.</param>
        void RemoveOperatingMode(string operatingModeName);


        /// <summary>
        /// Replaces a selected operating mode with an available operating mode at the same priority in one atomic change.
        /// </summary>
        /// <param name="predecessorName">The name of the operating mode to replace.</param>
        /// <param name="operatingModeName">The name of the replacing operating mode.</param>
        void ReplaceOperatingMode(string predecessorName, string operatingModeName);
    }
}
