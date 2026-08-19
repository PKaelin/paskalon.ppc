// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.EnergyResources.Solars;

namespace paskalON.Devices.Dto.EnergyResources.Solars
{
    /// <summary>
    /// Data Transfer Object for a Photovoltaic (Solar) Core energy resource.
    /// </summary>
    /// <remarks>
    /// Used as high frequency DTO update.
    /// </remarks>
    public class PvCoreDto : IDevice
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// State of the solar panel.
        /// </summary>
        public SolarPanelState State { get; init; }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError { get; init; }
    }
}
