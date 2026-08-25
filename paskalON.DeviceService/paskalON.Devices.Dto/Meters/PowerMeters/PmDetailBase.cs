// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Energies;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Base record for Data Transfer Object for a Power Meter Detail.
    /// </summary>
    /// <remarks>
    /// Used as low frequency DTO update.
    /// </remarks>
    public abstract record PmDetailBase : IDevice
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int DeviceId { get; init; }


        /// <summary>
        /// Voltage ll average value.
        /// </summary>
        public double? VoltageLLAvg { get; init; }


        /// <summary>
        /// Active power A.
        /// </summary>
        public ActivePower? ActivePowerA { get; init; }


        /// <summary>
        /// Active power B.
        /// </summary>
        public ActivePower? ActivePowerB { get; init; }


        /// <summary>
        /// Active power C.
        /// </summary>
        public ActivePower? ActivePowerC { get; init; }


        /// <summary>
        /// Reactive power A.
        /// </summary>
        public ReactivePower? ReactivePowerA { get; init; }


        /// <summary>
        /// Reactive power B.
        /// </summary>
        public ReactivePower? ReactivePowerB { get; init; }


        /// <summary>
        /// Reactive power C.
        /// </summary>
        public ReactivePower? ReactivePowerC { get; init; }


        /// <summary>
        /// Energy delivered.
        /// </summary>
        public Energy? EnergyDelivered { get; init; }


        /// <summary>
        /// Energy received.
        /// </summary>
        public Energy? EnergyReceived { get; init; }


        /// <summary>
        /// Reactive energy delivered.
        /// </summary>
        public ReactiveEnergy? ReactiveEnergyDelivered { get; init; }


        /// <summary>
        /// Reactive energy received.
        /// </summary>
        public ReactiveEnergy? ReactiveEnergyReceived { get; init; }
    }
}
