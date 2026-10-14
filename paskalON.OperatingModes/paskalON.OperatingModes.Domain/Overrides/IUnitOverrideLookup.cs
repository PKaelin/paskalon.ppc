// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Overrides
{
    /// <summary>
    /// Read-only lookup whether a DER unit is currently overridden and therefore removed from normal dispatch.
    /// </summary>
    public interface IUnitOverrideLookup
    {
        /// <summary>
        /// Gets whether the DER unit is currently overridden.
        /// </summary>
        /// <param name="derUnitName">The name of the DER unit.</param>
        /// <returns>True if an active override mode targets the DER unit.</returns>
        bool IsOverridden(string derUnitName);
    }
}
