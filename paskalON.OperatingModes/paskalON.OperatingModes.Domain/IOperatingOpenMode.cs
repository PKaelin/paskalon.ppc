// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Interface definition for all operating mode base.
    /// </summary>
    public interface IOperatingOpenMode
    {
        /// <summary>
        /// Calculates the operating modes power target.
        /// </summary>
        /// <remarks>
        /// Though they are closed loops they still need inputs like availability/capability, etc.
        /// Those inputs are necessary so that it doesn't calculate an impossible target.
        /// </remarks>
        Task CalculateAsync<TInput>(TInput input, CancellationToken cancellationToken = default) where TInput : class;
    }

}
