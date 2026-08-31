// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Client
{
    /// <summary>
    /// Device client interface definition.
    /// </summary>
    public interface IDeviceClient
    {
        /// <summary>
        /// Distributed Energy Resource Root.
        /// </summary>
        DerDto Der { get; }


        /// <summary>
        /// List of possible power conversion systems.
        /// </summary>
        /// <remarks>
        /// A system shall always be dedicated to a specific type. E.g. BESS, Solar, Nuclear, etc.
        /// </remarks>
        ICollection<PcsDto> PowerConversionSystems { get; }


        /// <summary>
        /// List of possible battery banks used in this system.
        /// </summary>
        ICollection<BbDto> BatteryBanks { get; }


        /// <summary>
        /// List of possible solar panels used in this system.
        /// </summary>
        ICollection<PvDto> SolarPanels { get; }


        /// <summary>
        /// List of possible external power meters used in this system.
        /// </summary>
        ICollection<PmExternalDto> ExternalPowerMeters { get; }


        /// <summary>
        /// List of possible auxiliary power meters used in this system.
        /// </summary>
        ICollection<PmAuxiliaryDto> AuxiliaryPowerMeters { get; }


        /// <summary>
        /// List of possible system power meters used in this system.
        /// </summary>
        ICollection<PmSystemDto> SystemPowerMeters { get; }


        /// <summary>
        /// List of possible circuit power meters used in this system.
        /// </summary>
        ICollection<PmCircuitDto> CircuitPowerMeters { get; }

    }
}
