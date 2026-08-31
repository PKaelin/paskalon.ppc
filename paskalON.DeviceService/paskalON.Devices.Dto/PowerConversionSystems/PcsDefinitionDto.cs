// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Dto.PowerConversionSystems
{
    /// <summary>
    /// Data Transfer Object for a Power Conversion System Definition.
    /// </summary>
    /// <remarks>
    /// Used to initialize the PCS DTO in device client.
    /// </remarks>
    public record PcsDefinitionDto : IDeviceDefinition
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
        /// Nameplate maximum active power rating.
        /// </summary>
        public ActivePower NameplateMaximumActivePower { get; init; }


        /// <summary>
        /// Nameplate maximum reactive power rating.
        /// </summary>
        public ReactivePower NameplateMaximumReactivePower { get; init; }


        /// <summary>
        /// Nameplate maximum apparent power rating.
        /// </summary>
        public ApparentPower NameplateMaximumApparentPower { get; init; }


        /// <summary>
        /// Theoretical maximum AC current output.
        /// </summary>
        public double NameplateMaximumACCurrent { get; init; }


        /// <summary>
        /// Theoretical minimum DC voltage output.
        /// </summary>
        public double MinimumDCVoltage { get; init; }


        /// <summary>
        /// Theoretical maximum DC voltage output.
        /// </summary>
        public double MaximumDCVoltage { get; init; }


        /// <summary>
        /// Configured value determining whether the proxy should report 0 real and reactive power in the event of communication loss.
        /// </summary>        
        public bool ZeroOutputOnCommLoss { get; init; }


        /// <summary>
        /// Configured minimum active power that the PCS should output when in standby mode.
        /// </summary>
        public double StandbyActivePowerKiloWatts { get; init; }
    }
}
