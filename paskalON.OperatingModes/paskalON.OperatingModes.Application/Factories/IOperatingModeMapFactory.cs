// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Application.Factories
{
    /// <summary>
    /// Factory interface for creating operating mode maps based on configurations and DER information.
    /// </summary>
    public interface IOperatingModeMapFactory
    {
        /// <summary>
        /// Creates an operating mode map based on the provided configuration and DER information.
        /// </summary>
        /// <param name="config">The operating mode base configuration.</param>
        /// <param name="der">The DER information.</param>
        /// <returns>The created operating mode map.</returns>
        OperatingModeBaseMap? Create(OperatingModeBaseConfig config, DerDto der);
    }
}
