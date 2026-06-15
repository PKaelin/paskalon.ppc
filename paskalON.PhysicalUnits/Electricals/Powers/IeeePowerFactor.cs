namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Represents the IEEE power factor, which is a measure of how effectively electrical power is being used in a circuit according to the IEEE standard.
    /// </summary>
    public class IeeePowerFactor : PowerFactorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IeeePowerFactor"/> class with the specified power factor value.
        /// </summary>
        /// <param name="powerFactor">The power factor value, which must be in the range [-1, 1].</param>
        public IeeePowerFactor(double powerFactor) : base(powerFactor)
        {
        }


        /// <summary>
        /// Gets a value indicating whether the power factor is leading (capacitive) or not.
        /// </summary>
        public bool IsLeading { get => PowerFactor > 0; }


        /// <summary>
        /// Gets a value indicating whether the power factor is lagging (inductive) or not.
        /// </summary>
        public bool IsLagging { get => PowerFactor < 0; }


        /// <summary>
        /// Sets the sign of a power factor value to comply with the IEEE sign convention.
        /// </summary>
        /// <param name="activePower">The active power component.</param>
        /// <param name="reactivePower">The reactive power component.</param>
        /// <returns>The sign of the associated power factor.</returns>
        /// <remarks>
        /// Uses IEEE power factor sign convention:
        /// P+ Q+ = PF- (lag)
        /// P+ Q- = PF+ (lead)
        /// P- Q+ = PF+ (lead)
        /// P- Q- = PF- (lag)
        /// </remarks>
        public static int IeeeSign(double activePower, double reactivePower)
        {
            if (activePower > 0d && reactivePower > 0d || activePower < 0d && reactivePower < 0d)
            {
                return -1;
            }

            return 1;
        }


        /// <summary>
        /// Calculates the IEEE power factor based on the active and reactive power components, applying the IEEE sign convention to determine the correct sign of the power factor.
        /// </summary>
        /// <param name="activePower">The active power component.</param>
        /// <param name="reactivePower">The reactive power component.</param>
        /// <returns>The calculated IEEE power factor</returns>
        /// <remarks>
        public static IeeePowerFactor Calculate(double activePower, double reactivePower)
        {
            return new IeeePowerFactor(CalculateMagnitude(activePower, reactivePower) * IeeeSign(activePower, reactivePower));
        }

    }
}
