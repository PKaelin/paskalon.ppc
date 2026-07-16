// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    /// <summary>
    /// A mapping of named values in the C37 data stream.
    /// </summary>
    /// <remarks>
    /// Property name identifies the value and the value identifies the channel name.
    /// </remarks>
    public class PowerMeterMapC37Config : NameBase
    {
        /// <summary>
        /// Name of the C37 value that maps to <see cref="ApparentPower"/>.
        /// </summary>
        public string? ApparentPower { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentA"/>.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? CurrentA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentB"/>.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? CurrentB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentC"/>.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? CurrentC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="EnergyDelivered"/>.
        /// </summary>
        public string? EnergyDelivered { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="EnergyReceived"/>.
        /// </summary>
        public string? EnergyReceived { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="Frequency"/>.
        /// </summary>
        public string? Frequency { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="PowerFactor"/>.
        /// </summary>
        public string? PowerFactor { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ReactiveEnergyDelivered"/>.
        /// </summary>
        public string? ReactiveEnergyDelivered { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ReactiveEnergyReceived"/>.
        /// </summary>
        public string? ReactiveEnergyReceived { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ReactivePower"/>.
        /// </summary>
        public string? ReactivePower { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ReactivePowerA"/>.
        /// </summary>
        public string? ReactivePowerA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ReactivePowerB"/>.
        /// </summary>
        public string? ReactivePowerB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ReactivePowerC"/>.
        /// </summary>
        public string? ReactivePowerC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ActivePower"/>.
        /// </summary>
        public string? ActivePower { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ActivePowerA"/>.
        /// </summary>
        public string? ActivePowerA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ActivePowerB"/>.
        /// </summary>
        public string? ActivePowerB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="ActivePowerC"/>.
        /// </summary>
        public string? ActivePowerC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase positive sequence voltage average.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? VoltagePositiveSequence { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageA"/>.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? VoltageA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageB"/>.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? VoltageB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageC"/>.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? VoltageC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageLLAvg"/>.
        /// </summary>
        public string? VoltageLLAvg { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase A-B Line-to-Line voltage magnitude.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? VoltageAB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase B-C Line-to-Line voltage magnitude.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? VoltageBC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase C-A Line-to-Line voltage magnitude.
        /// </summary>
        /// <remarks>
        /// This is a phasor signal that has a magnitude and an angle.
        /// </remarks>
        public string? VoltageCA { get; set; }

    }
}
