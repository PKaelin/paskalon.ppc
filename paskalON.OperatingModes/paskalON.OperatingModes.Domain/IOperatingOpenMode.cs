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
        Task CalculateAsync(CancellationToken cancellationToken);
    }

}
