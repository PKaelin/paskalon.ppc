// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    /// <summary>
    /// Data Transfer Object for a Battery Bank.
    /// </summary>
    public record BbDto : DeviceBase<BbDefinitionDto, BbCoreDto, BbDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="BbDto"/>.
        /// </summary>
        /// <param name="definition">Battery Bank definition DTO.</param>
        public BbDto(BbDefinitionDto definition) : base(definition)
        {
        }
    }
}
