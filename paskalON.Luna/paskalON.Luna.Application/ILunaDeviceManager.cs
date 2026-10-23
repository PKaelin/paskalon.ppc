// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna device manager interface definition.
    /// </summary>
    public interface ILunaDeviceManager
    {
        /// <summary>
        /// Get the distributed energy resource (DER) device DTO.
        /// </summary>
        /// <returns>The DER device DTO.</returns>
        DerDto GetDer();
    }
}
