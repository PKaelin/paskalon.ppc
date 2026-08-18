// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    public class BbDefinitionDto : IDeviceDefinition
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
