// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Dto.Stacks;

namespace paskalON.OperatingModes.Application.Stacks
{
    /// <summary>
    /// Factory that creates the stack DTOs of the stackable operating modes.
    /// </summary>
    public interface IOperatingModeStackDtoFactory
    {
        /// <summary>
        /// Creates the stack DTO of an operating mode.
        /// </summary>
        /// <param name="operatingMode">The stackable operating mode.</param>
        /// <param name="priority">The priority of the operating mode in the stack or null if it is not in the stack.</param>
        /// <returns>The specialized stack DTO of the operating mode or <see cref="OperatingModeStackDto"/> as default.</returns>
        OperatingModeStackDto Create(IStackableMode operatingMode, int? priority = null);


        /// <summary>
        /// Creates the stack DTOs of the available operating modes without priority in their order.
        /// </summary>
        /// <param name="operatingModes">The stackable operating modes.</param>
        /// <returns>The stack DTOs of the operating modes.</returns>
        IReadOnlyList<OperatingModeStackDto> Create(IEnumerable<IStackableMode> operatingModes);


        /// <summary>
        /// Creates the stack DTOs of the active operating modes with their priorities in their order.
        /// </summary>
        /// <param name="entries">The operating modes of the stack with their priorities.</param>
        /// <returns>The stack DTOs of the operating modes.</returns>
        IReadOnlyList<OperatingModeStackDto> Create(IEnumerable<OperatingModeStackEntry> entries);
    }
}
