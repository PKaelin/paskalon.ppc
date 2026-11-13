// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyResources.Solars
{
    /// <summary>
    /// Data Transfer Object for a Photovoltaic (Solar) Core energy resource.
    /// </summary>
    /// <remarks>
    /// Used as high frequency DTO update.
    /// </remarks>
    public record PvCoreDto : IDevice
    {
        /// <inheritdoc/>
        public required int DeviceId { get; init; }


        /// <summary>
        /// State of the solar panel.
        /// </summary>
        public SolarPanelStateDto State { get; init; }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError { get; init; }
    }
}
