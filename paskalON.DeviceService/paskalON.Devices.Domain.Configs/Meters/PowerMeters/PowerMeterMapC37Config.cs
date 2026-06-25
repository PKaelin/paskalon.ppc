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
        public string? CurrentA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentAngleA"/>.
        /// </summary>
        public string? CurrentAngleA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentB"/>.
        /// </summary>
        public string? CurrentB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentAngleB"/>.
        /// </summary>
        public string? CurrentAngleB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentC"/>.
        /// </summary>
        public string? CurrentC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="CurrentAngleC"/>.
        /// </summary>
        public string? CurrentAngleC { get; set; }


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
        public string? VoltagePositiveSequence { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase positive sequence voltage angle average.
        /// </summary>
        public string? VoltagePositiveSequenceAngle { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageA"/>.
        /// </summary>
        public string? VoltageA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageAngleA"/>.
        /// </summary>
        public string? VoltageAngleA { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageB"/>.
        /// </summary>
        public string? VoltageB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageAngleB"/>.
        /// </summary>
        public string? VoltageAngleB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageC"/>.
        /// </summary>
        public string? VoltageC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageAngleC"/>.
        /// </summary>
        public string? VoltageAngleC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to <see cref="VoltageLLAvg"/>.
        /// </summary>
        public string? VoltageLLAvg { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase A-B Line-to-Line voltage magnitude.
        /// </summary>
        public string? VoltageAB { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase B-C Line-to-Line voltage magnitude.
        /// </summary>
        public string? VoltageBC { get; set; }


        /// <summary>
        /// Name of the C37 value that maps to 3-phase C-A Line-to-Line voltage magnitude.
        /// </summary>
        public string? VoltageCA { get; set; }

    }
}
