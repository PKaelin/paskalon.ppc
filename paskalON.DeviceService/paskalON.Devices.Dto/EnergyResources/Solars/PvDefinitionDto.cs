// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyResources.Solars
{
    /// <summary>
    /// Data Transfer Object for a Photovoltaic (Solar) Definition energy resource.
    /// </summary>
    /// <remarks>
    /// Used to initialize the Photovoltaic (Solar) DTO in device client.
    /// </remarks>
    public record PvDefinitionDto : IDeviceDefinition
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// Name of the device.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Number of solar panels.
        /// </summary>
        public int NumberOfPanels { get; init; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MinimumVoltageSum.
        /// </summary>
        public double MinimumVoltageSum { get; init; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MaximumVoltageSum.
        /// </summary>
        public double MaximumVoltageSum { get; init; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MinimumCurrentSum.
        /// </summary>
        public double MinimumCurrentSum { get; init; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MaximumCurrentSum.
        /// </summary>
        public double MaximumCurrentSum { get; init; }
    }
}
