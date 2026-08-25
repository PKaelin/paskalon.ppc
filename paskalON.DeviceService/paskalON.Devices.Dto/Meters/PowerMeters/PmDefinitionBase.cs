// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Data Transfer Object for a Power Meter Definition.
    /// </summary>
    /// <remarks>
    /// Used to initialize the Power Meter DTO in device client.
    /// </remarks>
    public abstract record PmDefinitionBase : IDeviceDefinition
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int DeviceId { get; init; }


        /// <summary>
        /// Name of the device.
        /// </summary>
        public string Name { get; init; } = string.Empty;


        /// <summary>
        /// Is reverse power flow from configuration.
        /// </summary>        
        public bool IsReversePowerFlow { get; init; }


        /// <summary>
        /// Is current signed from configuration.
        /// </summary>
        public bool IsCurrentSigned { get; init; }


        /// <summary>
        /// Power factor standard used for this meter.
        /// </summary>
        public PowerFactorStandard PowerFactorStandard { get; init; }
    }
}
