// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Base record for Data Transfer Object for a Power Meter Core.
    /// </summary>
    /// <remarks>
    /// Used as high frequency DTO update.
    /// </remarks>
    public abstract record PmCoreBase : IDevice
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// State of the Power Meter.
        /// </summary>
        public PowerMeterState State { get; init; }


        /// <summary>
        /// Communication error.
        /// </summary>
        public bool CommunicationError { get; init; }


        /// <summary>
        /// Active power.
        /// </summary>
        public ActivePower? ActivePower { get; init; }


        /// <summary>
        /// Reactive power.
        /// </summary>
        public ReactivePower? ReactivePower { get; init; }


        /// <summary>
        /// Apparent power.
        /// </summary>
        public ApparentPower? ApparentPower { get; init; }


        /// <summary>
        /// Calculated power factor according to power factor standard configuration.
        /// </summary>
        public double? PowerFactor { get; init; }


        /// <summary>
        /// Frequency. 
        /// </summary>
        public double? Frequency { get; init; }


        /// <summary>
        /// Voltage phasor value, phase A.
        /// </summary>
        public ulong? VoltageA { get; init; }


        /// <summary>
        /// Voltage magnitude, phase A.
        /// </summary>
        public double? VoltageAMagnitude { get; init; }


        /// <summary>
        /// Voltage angle, phase A.
        /// </summary>
        public double? VoltageAAngle { get; init; }


        /// <summary>
        /// Voltage phasor value, phase B.
        /// </summary>
        public ulong? VoltageB { get; init; }


        /// <summary>
        /// Voltage magnitude, phase B.
        /// </summary>
        public double? VoltageBMagnitude { get; init; }


        /// <summary>
        /// Voltage angle, phase B.
        /// </summary>
        public double? VoltageBAngle { get; init; }


        /// <summary>
        /// Voltage phasor value, phase C.
        /// </summary>
        public ulong? VoltageC { get; init; }


        /// <summary>
        /// Voltage magnitude, phase C.
        /// </summary>
        public double? VoltageCMagnitude { get; init; }


        /// <summary>
        /// Voltage angle, phase C.
        /// </summary>
        public double? VoltageCAngle { get; init; }


        /// <summary>
        /// Voltage phasor value, line-to-line AB.
        /// </summary>
        public ulong? VoltageAB { get; init; }


        /// <summary>
        /// Voltage magnitude, line-to-line AB.
        /// </summary>
        public double? VoltageABMagnitude { get; init; }


        /// <summary>
        /// Voltage angle, line-to-line AB.
        /// </summary>
        public double? VoltageABAngle { get; init; }


        /// <summary>
        /// Voltage phasor value, line-to-line BC.
        /// </summary>
        public ulong? VoltageBC { get; init; }


        /// <summary>
        /// Voltage magnitude, line-to-line BC.
        /// </summary>
        public double? VoltageBCMagnitude { get; init; }


        /// <summary>
        /// Voltage angle, line-to-line BC.
        /// </summary>
        public double? VoltageBCAngle { get; init; }


        /// <summary>
        /// Voltage phasor value, line-to-line CA.
        /// </summary>
        public ulong? VoltageCA { get; init; }


        /// <summary>
        /// Voltage magnitude, line-to-line CA.
        /// </summary>
        public double? VoltageCAMagnitude { get; init; }


        /// <summary>
        /// Voltage angle, line-to-line BC.
        /// </summary>
        public double? VoltageCAAngle { get; init; }


        /// <summary>
        /// Voltage positive sequence phasor value.
        /// </summary>
        public ulong? VoltagePositiveSequence { get; init; }


        /// <summary>
        /// Voltage positive sequence magnitude.
        /// </summary>
        public double? VoltagePositiveSequenceMagnitude { get; init; }


        /// <summary>
        /// Voltage positive sequence angle.
        /// </summary>
        public double? VoltagePositiveSequenceAngle { get; init; }


        /// <summary>
        /// Current phasor value, phase A.
        /// </summary>
        public ulong? CurrentA { get; init; }


        /// <summary>
        /// Current magnitude, phase A.
        /// </summary>
        public double? CurrentAMagnitude { get; init; }


        /// <summary>
        /// Current angle, phase A.
        /// </summary>
        public double? CurrentAAngle { get; init; }


        /// <summary>
        /// Current phasor value, phase B.
        /// </summary>
        public ulong? CurrentB { get; init; }


        /// <summary>
        /// Current magnitude, phase B.
        /// </summary>
        public double? CurrentBMagnitude { get; init; }


        /// <summary>
        /// Current angle, phase B.
        /// </summary>
        public double? CurrentBAngle { get; init; }


        /// <summary>
        /// Current phasor value, phase C.
        /// </summary>
        public ulong? CurrentC { get; init; }


        /// <summary>
        /// Current magnitude, phase C.
        /// </summary>
        public double? CurrentCMagnitude { get; init; }


        /// <summary>
        /// Current angle, phase C.
        /// </summary>
        public double? CurrentCAngle { get; init; }

    }
}
