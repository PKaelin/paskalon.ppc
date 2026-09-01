// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Meters.PowerMeters;

namespace paskalON.Devices.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER.
    /// </summary>
    public record DerDto
    {
        /// <summary>
        /// List of DER groups that can be split up onto different device service services 
        /// and therefore run on different machines.
        /// </summary>
        public List<DerGroupDto> DerGroups { get; set; } = new List<DerGroupDto>();


        // TODO: add GMDs


        /// <summary>
        /// List of system meters. Usually there is one with redundancy within.
        /// </summary>
        public List<PmSystemDto> SystemPowerMeters { get; set; } = new List<PmSystemDto>();


        /// <summary>
        /// List of auxiliary power meters.
        /// </summary>
        public List<PmAuxiliaryDto> AuxiliaryPowerMeters { get; set; } = new List<PmAuxiliaryDto>();


        /// <summary>
        /// List of external power meters.
        /// </summary>
        public List<PmExternalDto> ExternalPowerMeters { get; set; } = new List<PmExternalDto>();
    }
}
