// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.PowerConversionSystems
{
    /// <summary>
    /// Data Transfer Object for a Power Conversion System.
    /// </summary>
    public record PcsDto : DeviceBase<PcsDefinitionDto, PcsCoreDto, PcsDetailDto>
    {
        /// <summary>
        /// Constructor of <see cref="PcsDto"/>.
        /// </summary>
        /// <param name="definition">Power Conversion System definition DTO.</param>
        public PcsDto(PcsDefinitionDto definition) : base(definition)
        {
        }
    }
}
