// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
        Task CalculateAsync(CancellationToken cancellationToken = default);
    }

}
