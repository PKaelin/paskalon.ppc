namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Base class for different types of power factor (IEEE, IEC).
    /// </summary>
    public abstract class PowerFactorBase : IComparable, IComparable<PowerFactorBase>
    {
        /// <summary>
        /// Represents the maximum <see cref="PowerFactorBase"/> value.
        /// </summary>
        public const double MaxValue = 1;


        /// <summary>
        /// Represents the minimum <see cref="PowerFactorBase"/> value.
        /// </summary>
        public const double MinValue = -1;


        /// <summary>
        /// Value of the current <see cref="PowerFactorBase"/> structure expressed as ratio between 0 and 1."/>
        /// </summary>
        public double PowerFactor { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="PowerFactorBase"/> class with the specified power factor value.
        /// </summary>
        /// <param name="powerFactor">The power factor value, which must be in the range [-1, 1].</param>
        public PowerFactorBase(double powerFactor)
        {
            PowerFactor = powerFactor;
        }



        /// <summary>
        /// Validates that the provided power factor value is within the acceptable range of -1 to 1.
        /// </summary>
        /// <param name="powerFactor">The power factor value to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the power factor is outside the range [-1, 1].</exception>
        public static void ValidatePowerFactor(double powerFactor)
        {
            if (Math.Abs(powerFactor) > 1d)
            {
                throw new ArgumentOutOfRangeException("Power factor must be in the range [-1, 1].");
            }
        }


        /// <summary>
        /// Calculates the power factor magnitude given the active and reactive power.
        /// </summary>
        /// <param name="activePower">The active power (P) in the circuit.</param>
        /// <param name="reactivePower">The reactive power (Q) in the circuit.</param>
        /// <returns>
        /// The power factor magnitude as a decimal number between 0 and 1.</returns>
        /// <remarks>
        /// The power factor (PF) magnitude is the ratio of active power to apparent power, 
        /// expressed as a decimal number between 0 and 1 (or a percentage between 0% and 100%). 
        /// </remarks>
        public static double CalculateMagnitude(double activePower, double reactivePower)
        {
            // Power Factor (PF) = Active Power (P) / Apparent Power (S)
            // Apparent Power (S) = sqrt(P^2 + Q^2)
            double powerFactor;

            // When a circuit is completely reactive circuit.
            if (activePower == 0d)
            {
                powerFactor = 0d;
            }
            // When a circuit operates with 0 reactive power, the power factor is exactly 1.
            else if (reactivePower == 0d)
            {
                powerFactor = 1d;
            }
            else
            {
                double apparentPower = Math.Sqrt(Math.Pow(activePower, 2) + Math.Pow(reactivePower, 2));
                powerFactor = activePower / apparentPower;
            }

            return Math.Abs(powerFactor);
        }


        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="obj"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="obj"/>.
        /// Greater than zero: This instance follows <paramref name="obj"/> in the sort order.
        /// </returns>
        public int CompareTo(object? obj)
        {
            PowerFactorBase? other = obj as PowerFactorBase;

            if (other == null)
            {
                return -1;
            }

            return CompareTo(other);
        }


        /// <summary>
        /// Compares the current instance with another instance of <see cref="PowerFactorBase"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other instance.
        /// </summary>
        /// <param name="other">An instance of <see cref="PowerFactorBase"/> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="other"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="other"/>.
        /// Greater than zero: This instance follows <paramref name="other"/> in the sort order.
        /// </returns>
        public int CompareTo(PowerFactorBase? other)
        {
            if (other == null)
            {
                return 1;
            }

            return PowerFactor.CompareTo(other.PowerFactor);
        }
    }
}