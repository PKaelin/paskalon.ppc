namespace paskalON.PhysicalUnits.Electricals.Energies
{
    /// <summary>
    /// Struct for reactive energy (VArh). The total amount of reactive energy consumed over time (Volt Ampere Reactive x Time)
    /// </summary>
    public struct ReactiveEnergy
    {
        /// <summary>
        /// Constant for VoltAmperesReactiveSeconds in 1 VoltAmperesReactiveHour.
        /// </summary>
        public const double VoltAmperesReactiveSecondsPerVarHour = 3600d;


        /// <summary>
        /// Constant for VoltAmperesReactiveHours in 1 KilovoltAmperesReactiveHour.
        /// </summary>
        public const double VoltAmperesReactiveHoursPerKiloVarHour = 1000d;


        /// <summary>
        /// Constant for VoltAmperesReactiveHours in 1 MegavoltAmperesReactiveHour.
        /// </summary>
        public const double VoltAmperesReactiveHoursPerMegaVarHour = 1000000d;


        /// <summary>
        /// Gets the value of the current <see cref="Energy"/> structure expressed in VoltAmperesReactiveHours.
        /// </summary>
        /// <value>
        /// The total number of VoltAmperesReactiveHours represented by this instance.
        /// </value>
        public double VoltAmperesReactiveHours { get; private set; }


        /// <summary>
        /// Gets the value of the current <see cref="Energy"/> structure expressed in KiloVoltAmperesReactiveHours.
        /// </summary>
        /// <value>
        /// The total number of KiloVoltAmperesReactiveHours represented by this instance.
        /// </value>
        public readonly double KiloVoltAmperesReactiveHours { get => VoltAmperesReactiveHours / VoltAmperesReactiveHoursPerKiloVarHour; }


        /// <summary>
        /// Gets the value of the current <see cref="Energy"/> structure expressed in MegaVoltAmperesReactiveHours.
        /// </summary>
        /// <value>
        /// The total number of MegaVoltAmperesReactiveHours represented by this instance.
        /// </value>
        public readonly double MegaVoltAmperesReactiveHours { get => VoltAmperesReactiveHours / VoltAmperesReactiveHoursPerMegaVarHour; }


        /// <summary>
        /// Gets the value of the current <see cref="Energy"/> structure expressed in VoltAmperesReactiveSeconds.
        /// </summary>
        /// <value>
        /// The total number of VoltAmperesReactiveSeconds represented by this instance.
        /// </value>
        public readonly double VoltAmperesReactiveSeconds { get => VoltAmperesReactiveHours * VoltAmperesReactiveSecondsPerVarHour; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in VoltAmperesReactiveHours.
        /// </summary>
        /// <param name="voltAmperesReactiveHours">Number of voltAmperesReactive-hours</param>
        public ReactiveEnergy(double voltAmperesReactiveHours)
        {
            VoltAmperesReactiveHours = voltAmperesReactiveHours;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in VoltAmperesReactiveHours.
        /// </summary>
        /// <param name="voltAmperesReactiveHours">Number of voltAmperesReactive-hours</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in VoltAmperesReactiveHours.</returns>
        public static ReactiveEnergy FromVoltAmperesReactiveHours(double voltAmperesReactiveHours)
        {
            return new ReactiveEnergy(voltAmperesReactiveHours);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in KiloVoltAmperesReactiveHours.
        /// </summary>
        /// <param name="kiloVoltAmperesReactiveHours">Number of kilo-voltAmperesReactive-hours</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in KiloVoltAmperesReactiveHours.</returns>
        public static ReactiveEnergy FromKiloVoltAmperesReactiveHours(double kiloVoltAmperesReactiveHours)
        {
            return new ReactiveEnergy(kiloVoltAmperesReactiveHours * VoltAmperesReactiveHoursPerKiloVarHour);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in MegaVoltAmperesReactiveHours.
        /// </summary>
        /// <param name="megaVoltAmperesReactiveHours">Number of mega-voltAmperesReactive-hours</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in MegaVoltAmperesReactiveHours.</returns>
        public static ReactiveEnergy FromMegaVoltAmperesReactiveHours(double megaVoltAmperesReactiveHours)
        {
            return new ReactiveEnergy(megaVoltAmperesReactiveHours * VoltAmperesReactiveHoursPerMegaVarHour);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in VoltAmperesReactiveSeconds.
        /// </summary>
        /// <param name="voltAmperesReactiveSeconds">Number of VoltAmperesReactive-seconds</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in VoltAmperesReactiveSeconds.</returns>
        public static ReactiveEnergy FromVoltAmperesReactiveSeconds(double voltAmperesReactiveSeconds)
        {
            return new ReactiveEnergy(voltAmperesReactiveSeconds / VoltAmperesReactiveSecondsPerVarHour);
        }

    }
}
