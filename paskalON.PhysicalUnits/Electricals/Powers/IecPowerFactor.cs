namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Represents the IEC power factor, which is a measure of how effectively electrical power is being used in a circuit according to the IEC standard.
    /// </summary>
    public class IecPowerFactor : PowerFactorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IecPowerFactor"/> class with the specified power factor value.
        /// </summary>
        /// <param name="powerFactor">The power factor value, which must be in the range [-1, 1].</param>
        public IecPowerFactor(double powerFactor) : base(powerFactor)
        {
        }


        /// <summary>
        /// Sets the sign of a power factor value to comply with the IEC sign convention.
        /// </summary>
        /// <param name="activePower">The active power component.</param>
        /// <param name="reactivePower">The reactive power component.</param>
        /// <returns>The sign of the associated power factor.</returns>
        /// <remarks>
        /// Uses IEC power factor sign convention:
        /// P+ Q+ = PF+
        /// P+ Q- = PF+
        /// P- Q+ = PF-
        /// P- Q- = PF-.
        /// </remarks>
        public static int IecSign(double activePower, double reactivePower)
        {
            if (activePower > 0d && reactivePower > 0d || activePower > 0d && reactivePower < 0d)
            {
                return 1;
            }
            return -1;
        }


        /// <summary>
        /// Calculates the IEC power factor based on the active and reactive power components, applying the IEC sign convention to determine the correct sign of the power factor.
        /// </summary>
        /// <param name="activePower">The active power component.</param>
        /// <param name="reactivePower">The reactive power component.</param>
        /// <returns>The calculated IEC power factor</returns>
        public static IecPowerFactor Calculate(double activePower, double reactivePower)
        {
            return new IecPowerFactor(CalculateMagnitude(activePower, reactivePower) * IecSign(activePower, reactivePower));
        }

    }
}
