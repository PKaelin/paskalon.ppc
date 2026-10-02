// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;

namespace paskalON.OperatingModes.Application.Stacks
{
    public interface IStackManager
    {
        /// <summary>
        /// Initializes the stack manager with a collection of available operating modes.
        /// </summary>
        /// <param name="availableOperatingModes">A dictionary of available operating modes keyed by their names.</param>
        void InitializeStack(Dictionary<string, OperatingModeBase> availableOperatingModes);


        /// <summary>
        /// Selected operating modes ordered by ascending priority.
        /// </summary>
        IReadOnlyCollection<OperatingModeBase> AvailableOperatingModes { get; }


        /// <summary>
        /// Selected operating modes ordered by ascending priority.
        /// </summary>
        IReadOnlyCollection<OperatingModeBase> SelectedOperatingModes { get; }


        /// <summary>
        /// Adds an initialized operating mode to the selected stack.
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
        /// Removes an operating mode from the selected stack.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode to remove.</param>
        void RemoveOperatingMode(string operatingModeName);
    }
}
