// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Systems;

namespace paskalON.OperatingModes.Application.Factories
{
    /// <summary>
    /// Factory interface for creating the input maps of operating modes from the device data.
    /// </summary>
    public interface IOperatingModeMapFactory
    {
        /// <summary>
        /// Creates the input map of an operating mode.
        /// </summary>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="der">The DER device data.</param>
        /// <param name="overrides">Lookup of overridden DER units that are excluded from the plant-level available power.</param>
        /// <returns>The created map or null if the operating mode is not supported.</returns>
        OperatingModeBaseMap? Create(OperatingModeBaseConfig config, DerDto der, IUnitOverrideLookup overrides);


        /// <summary>
        /// Creates the input map of the system operating mode control.
        /// </summary>
        /// <param name="der">The DER device data.</param>
        /// <param name="holdReason">Function that gets the reason why the setpoints must be held or null if the control may calculate.</param>
        /// <returns>The system operating mode control map.</returns>
        SystemOperatingModeControlMap CreateSystemMap(DerDto der, Func<string?> holdReason);
    }
}
