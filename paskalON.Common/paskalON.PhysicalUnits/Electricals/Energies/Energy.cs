namespace paskalON.PhysicalUnits.Electricals.Energies
{
    /// <summary>
    /// Struct for energy (Wh). The total amount of energy consumed over time (Power x Time)
    /// </summary>
    public struct Energy
    {
        /// <summary>
        /// Constant for WattSeconds in 1 WattHour.
        /// </summary>
        public const double WattSecondsPerWattHour = 3600d;


        /// <summary>
        /// Constant for WattHours in 1 KilowattHour.
        /// </summary>
        public const double WattHoursPerKiloWattHour = 1000d;


        /// <summary>
        /// Constant for WattHours in 1 MegawattHour.
        /// </summary>
        public const double WattHoursPerMegaWattHour = 1000000d;


        /// <summary>
        /// Value of the current <see cref="Energy"/> structure expressed in WattHours.
        /// </summary>
        /// <value>
        /// The total number of WattHours represented by this instance.
        /// </value>
        public double WattHours { get; private set; }


        /// <summary>
        /// Value of the current <see cref="Energy"/> structure expressed in KiloWattHours.
        /// </summary>
        /// <value>
        /// The total number of KiloWattHours represented by this instance.
        /// </value>
        public readonly double KiloWattHours { get => WattHours / WattHoursPerKiloWattHour; }


        /// <summary>
        /// Value of the current <see cref="Energy"/> structure expressed in MegaWattHours.
        /// </summary>
        /// <value>
        /// The total number of MegaWattHours represented by this instance.
        /// </value>
        public readonly double MegaWattHours { get => WattHours / WattHoursPerMegaWattHour; }


        /// <summary>
        /// Value of the current <see cref="Energy"/> structure expressed in WattSeconds.
        /// </summary>
        /// <value>
        /// The total number of WattSeconds represented by this instance.
        /// </value>
        public readonly double WattSeconds { get => WattHours * WattSecondsPerWattHour; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in WattHours.
        /// </summary>
        /// <param name="wattHours">Number of watt-hours</param>
        public Energy(double wattHours)
        {
            WattHours = wattHours;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in WattHours.
        /// </summary>
        /// <param name="wattHours">Number of watt-hours</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in WattHours.</returns>
        public static Energy FromWattHours(double wattHours)
        {
            return new Energy(wattHours);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in KiloWattHours.
        /// </summary>
        /// <param name="kiloWattHours">Number of kilo-watt-hours</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in KiloWattHours.</returns>
        public static Energy FromKiloWattHours(double kiloWattHours)
        {
            return new Energy(kiloWattHours * WattHoursPerKiloWattHour);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in MegaWattHours.
        /// </summary>
        /// <param name="megaWattHours">Number of mega-watt-hours</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in MegaWattHours.</returns>
        public static Energy FromMegaWattHours(double megaWattHours)
        {
            return new Energy(megaWattHours * WattHoursPerMegaWattHour);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Energy"/> structure with the specified amount of energy in WattSeconds.
        /// </summary>
        /// <param name="wattSeconds">Number of watt-seconds</param>
        /// <returns>New instance of the <see cref="Energy"/> structure with the specified amount of energy in WattSeconds.</returns>
        public static Energy FromWattSeconds(double wattSeconds)
        {
            return new Energy(wattSeconds / WattSecondsPerWattHour);
        }

    }
}
