// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyResources.Solars
{
    /// <summary>
    /// Data Transfer Object for a Photovoltaic (Solar) energy resource.
    /// </summary>
    public record PvDto : DeviceBase<PvDefinitionDto, PvCoreDto, PvDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PvDto"/>.
        /// </summary>
        /// <param name="definition">PV (Solar) definition DTO.</param>
        public PvDto(PvDefinitionDto definition) : base(definition)
        {
        }
    }
}
