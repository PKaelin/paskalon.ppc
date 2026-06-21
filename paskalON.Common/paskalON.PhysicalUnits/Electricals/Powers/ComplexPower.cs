namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Complex power combines active and reactive power for an AC electrical circuit.
    /// </summary>
    public struct ComplexPower : IEquatable<ComplexPower>
    {
        /// <summary>
        /// Active power portion of complex power.
        /// </summary>
        public ActivePower ActivePower { get; set; }


        /// <summary>
        /// Reactive power portion of complex power.
        /// </summary>
        public ReactivePower ReactivePower { get; set; }


        /// <summary>
        /// Constructor of <see cref="ComplexPower"/>.
        /// </summary>
        /// <param name="activePower">Active part of this instance.</param>
        /// <param name="reactivePower">Reactive part of this instance.</param>
        public ComplexPower(ActivePower activePower, ReactivePower reactivePower)
        {
            ActivePower = activePower;
            ReactivePower = reactivePower;
        }


        /// <summary>
        /// Returns hash code for this instance.
        /// </summary>
        /// <returns>Integer hash code.</returns>
        public override int GetHashCode()
        {
            return ActivePower.GetHashCode() + ReactivePower.GetHashCode();
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if object represents the same complex power as this instance otherwise false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is ComplexPower && Equals((ComplexPower)obj);
        }


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="other">An <see cref="ComplexPower"/> to compare with this instance.</param>
        /// <returns>True if other represents the same complex power as this instance otherwise false.</returns>
        public bool Equals(ComplexPower other)
        {
            return this == other;
        }


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="cp1">The first complex power value to compare to.</param>
        /// <param name="cp2">The second complex power value to compare to.</param>
        /// <returns>Return true if the values of cp1 and cp2 are equal otherwise false.</returns>
        public static bool operator ==(ComplexPower cp1, ComplexPower cp2) => cp1.ActivePower == cp2.ActivePower && cp1.ReactivePower == cp2.ReactivePower;


        /// <summary>
        /// Returns a value indicating whether this instance is not equal.
        /// </summary>
        /// <param name="cp1">The first complex power value to compare to.</param>
        /// <param name="cp2">The second complex power value to compare to.</param>
        /// <returns>Return true if the values of cp1 and cp2 are not equal otherwise false.</returns>
        public static bool operator !=(ComplexPower cp1, ComplexPower cp2) => cp1.ActivePower != cp2.ActivePower || cp1.ReactivePower != cp2.ReactivePower;


        /// <summary>
        /// Adds to <see cref="ComplexPower"/> instances.
        /// </summary>
        /// <param name="cp1">The first complex power value to add to.</param>
        /// <param name="cp2">The second complex power value to add to.</param>
        /// <returns></returns>
        public static ComplexPower operator +(ComplexPower cp1, ComplexPower cp2) => new()
        {
            ActivePower = cp1.ActivePower + cp2.ActivePower,
            ReactivePower = cp1.ReactivePower + cp2.ReactivePower,
        };


        /// <summary>
        /// Subtracts second <see cref="ComplexPower" instance from the first <see cref="ComplexPower" instance./>
        /// </summary>
        /// <param name="cp1">The first apparent power value.</param>
        /// <param name="cp2">The second apparent power value to subtract.</param>
        /// <returns>Sum of the two <see cref="ComplexPower"./></returns>
        public static ComplexPower operator -(ComplexPower cp1, ComplexPower cp2) => new()
        {
            ActivePower = cp1.ActivePower - cp2.ActivePower,
            ReactivePower = cp1.ReactivePower - cp2.ReactivePower,
        };
    }
}
