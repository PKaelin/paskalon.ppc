// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Dto.Endpoints;

namespace paskalON.DeviceSimulator.Dto.EnergyResources.Solars
{
    public record PvDto : IDevice
    {
        /// <summary>
        /// Device ID of the Distributed Energy Resource (DER) Data Transfer Object (DTO.
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// Name of the Distributed Energy Resource (DER) Data Transfer Object (DTO.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// List of Modbus endpoints.
        /// </summary>
        public required List<ModbusEndpointDto> Endpoints { get; init; }
    }
}
