// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Dto.PowerConversionSystems
{
    /// <summary>
    /// Data Transfer Object for a Power Conversion System Core.
    /// </summary>
    /// <remarks>
    /// Used as high frequency DTO update.
    /// </remarks>
    public class PcsCoreDto : IDevice
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// State of the Power Conversion System (PCS).
        /// Specialized PCS has to map its states to the these states.
        /// </summary>
        public PcsState State { get; init; }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError { get; init; }


        /// <summary>
        /// Current active power output in Watts
        /// </summary>
        public ActivePower? ActivePower { get; init; }


        /// <summary>
        /// Current active available power output in Watts
        /// </summary>
        public ActivePower? ActiveAvailablePower { get; init; }


        /// <summary>
        /// Current reactive power output in Vars
        /// </summary>
        public ReactivePower? ReactivePower { get; init; }


        /// <summary>
        /// Current reactive available power output in Watts
        /// </summary>
        public ReactivePower? ReactiveAvailablePower { get; init; }


        /// <summary>
        /// Apparent power output.
        /// </summary>
        public ApparentPower? ApparentPower { get; init; }


        /// <summary>
        /// Frequency in hertz.
        /// </summary>
        public double? Frequency { get; init; }


        /// <summary>
        /// DC Current or calculated DC Current
        /// </summary>
        /// </remarks>
        public double? DCCurrent { get; init; }


        /// <summary>
        /// DC Voltage or calculated DC Voltage.
        /// </summary>      
        public double? DCVoltage { get; init; }


        /// <summary>
        /// AC Current or calculated AC Current
        /// </summary>
        public double? ACCurrent { get; init; }


        /// <summary>
        /// AC Voltage or calculated AC Voltage
        /// </summary>
        public double? ACVoltage { get; init; }
    }
}
