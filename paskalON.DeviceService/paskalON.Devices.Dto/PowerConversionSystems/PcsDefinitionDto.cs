// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.PowerConversionSystems
{
    public class PcsDefinitionDto : IDeviceDefinition
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; set; }

        /// <summary>
        /// Name of the device.
        /// </summary>
        public required string Name { get; init; }
    }
}
