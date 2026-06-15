using System.Globalization;

namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Struct for apparent power (volt amperes).
    /// </summary>
    /// <remarks>
    /// Apparent Power is measured in Volt-Amperes (VA). 
    /// Represents the total magnitude of power flowing in the circuit, which is the vector sum of active and reactive power.    
    /// </remarks>
    public struct ApparentPower : IComparable, IComparable<ApparentPower>, IEquatable<ApparentPower>
    {
        /// <summary>
        /// Precision to round to the specific value.
        /// </summary>
        /// <remarks>
        /// Object initializer without setting precision will set it to 0.
        /// </remarks>
        public int Precision { get; private set; }


        /// <summary>
        /// Value of the current <see cref="ApparentPower"/> structure. 
        /// </summary>
        /// <value>Total VoltAmperes (VA)</value>
        public double VoltAmperes { get; set; }


        /// <summary>
        /// Value of the current <see cref="ApparentPower"/> structure. 
        /// </summary>
        /// <value>Total VoltAmperes (VA) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public double VoltAmperesPrecision { get => Math.Round(VoltAmperes, Precision); }


        /// <summary>
        /// Value of the current <see cref="ApparentPower"/> structure. 
        /// </summary>
        /// <value>Total Kilo-VoltAmperes (kVA).</value>
        public double KiloVoltAmperes { get { return VoltAmperes / 1000; } }


        /// <summary>
        /// Value of the current <see cref="ApparentPower"/> structure. 
        /// </summary>
        /// <value>Total Kilo-VoltAmperes (kVA) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public double KiloVoltAmperesPrecision { get { return Math.Round(KiloVoltAmperes, Precision); } }


        /// <summary>
        /// Value of the current <see cref="ApparentPower"/> structure. 
        /// </summary>
        /// <value>Total Mega-VoltAmperes (MVA).</value>
        public double MegaVoltAmperes { get { return VoltAmperes / 1000000; } }


        /// <summary>
        /// Value of the current <see cref="ApparentPower"/> structure. 
        /// </summary>
        /// <value>Total Mega-VoltAmperes (MVA) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public double MegaVoltAmperesPrecision { get { return Math.Round(MegaVoltAmperes, Precision); } }


        /// <summary>
        /// Constructor for <see cref="ApparentPower"/>
        /// </summary>
        /// <param name="voltAmperes">Number of volt amperes (VA).</param>
        public ApparentPower(double voltAmperes) : this(voltAmperes, 5)
        {
        }


        /// <summary>
        /// Constructor for <see cref="ApparentPower"/>
        /// </summary>
        /// <param name="voltAmperes">Number of volt amperes (VA).</param>
        /// <param name="precision">Precision for comparison.</param>
        public ApparentPower(double voltAmperes, int precision)
        {
            VoltAmperes = voltAmperes;
            Precision = precision;
        }


        /// <summary>
        /// Constructor for <see cref="ApparentPower"/> that calculates apparent power from active and reactive power.
        /// </summary>
        /// <param name="activePower">The active power component.</param>
        /// <param name="reactivePower">The reactive power component.</param>
        public ApparentPower(ActivePower activePower, ReactivePower reactivePower)
        {
            Precision = 5;
            VoltAmperes = Math.Sqrt(activePower.Watts * activePower.Watts + reactivePower.VoltAmperesReactive * reactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// Integer hash code.
        /// </returns>
        public override int GetHashCode() => VoltAmperes.GetHashCode();


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// True if obj represents the same apparent power value as this instance or otherwise false.
        /// </returns>
        public readonly override bool Equals(object? obj) => obj is ApparentPower other && Equals(other);


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="other">A ApparentPower to compare with this instance.</param>
        /// True if obj represents the same apparent power value as this instance or otherwise false.
        public readonly bool Equals(ApparentPower other) => this == other;


        /// <summary>
        /// Indicates whether two <see cref="ApparentPower"/> instances are equal.
        /// </summary>
        /// <param name="rp1">The first apparent power value to compare.</param>
        /// <param name="rp2">The second apparent power value to compare.</param>
        /// <returns>True if the values of rp1 and rp2 are equal or otherwise false.</returns>
        public static bool operator ==(ApparentPower rp1, ApparentPower rp2) => Math.Round(rp1.VoltAmperes, rp1.Precision) == Math.Round(rp2.VoltAmperes, rp2.Precision);


        /// <summary>
        /// Indicates whether two <see cref="ApparentPower"/> instances are not equal.
        /// </summary>
        /// <param name="rp1">The first apparent power value to compare.</param>
        /// <param name="rp2">The second apparent power value to compare.</param>
        /// <returns>True if the values of rp1 and rp2 are not  equal or otherwise false.</returns>
        public static bool operator !=(ApparentPower rp1, ApparentPower rp2) => Math.Round(rp1.VoltAmperes, rp1.Precision) != Math.Round(rp2.VoltAmperes, rp2.Precision);


        /// <summary>
        /// Adds two <see cref="ApparentPower" instances./>
        /// </summary>
        /// <param name="rp1">The first apparent power value to compare.</param>
        /// <param name="rp2">The second apparent power value to compare.</param>
        /// <returns>Sum of the two <see cref="ApparentPower"./></returns>
        public static ApparentPower operator +(ApparentPower rp1, ApparentPower rp2) => new(rp1.VoltAmperes + rp2.VoltAmperes);


        /// <summary>
        /// Substracts second <see cref="ApparentPower" instance from the first <see cref="ApparentPower" instance./>
        /// </summary>
        /// <param name="rp1">The first apparent power value.</param>
        /// <param name="rp2">The second apparent power value to substract.</param>
        /// <returns>Sum of the two <see cref="ApparentPower"./></returns>
        public static ApparentPower operator -(ApparentPower rp1, ApparentPower rp2) => new(rp1.VoltAmperes - rp2.VoltAmperes);


        /// <summary>
        /// Multiplies two <see cref="ApparentPower" instances./>
        /// </summary>
        /// <param name="rp1">The first apparent power value.</param>
        /// <param name="rp2">The second apparent power value.</param>
        /// <returns>Sum of the two <see cref="ApparentPower"./></returns>
        public static ApparentPower operator *(ApparentPower rp1, ApparentPower rp2) => new(rp1.VoltAmperes * rp2.VoltAmperes);


        /// <summary>
        /// Divides first <see cref="ApparentPower" instance with the second <see cref="ApparentPower" instance./>
        /// </summary>
        /// <param name="rp1">The first apparent power value.</param>
        /// <param name="rp2">The second apparent power value to divide the first one with.</param>
        /// <returns>Sum of the two <see cref="ApparentPower"./></returns>
        public static ApparentPower operator /(ApparentPower rp1, ApparentPower rp2) => new(rp2.VoltAmperes != 0 ? rp1.VoltAmperes / rp2.VoltAmperes : double.NaN);


        /// <summary>
        /// Indicates whether a specified <see cref="ApparentPower"/> is less than another specified <see cref="ApparentPower"/>.
        /// </summary>
        /// <param name="rp1">The first apparent power value to compare.</param>
        /// <param name="rp2">The second apparent power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is less than the value of rp2 otherwise false.
        /// </returns>
        public static bool operator <(ApparentPower rp1, ApparentPower rp2)
        {
            return rp1.CompareTo(rp2) < 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ApparentPower"/> is less than or equal another specified <see cref="ApparentPower"/>.
        /// </summary>
        /// <param name="rp1">The first apparent power value to compare.</param>
        /// <param name="rp2">The second apparent power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is less than or equal the value of rp2 otherwise false.
        /// </returns>
        public static bool operator <=(ApparentPower rp1, ApparentPower rp2)
        {
            return rp1.CompareTo(rp2) <= 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ApparentPower"/> is greater than another specified <see cref="ApparentPower"/>.
        /// </summary>
        /// <param name="rp1">The first apparent power value to compare.</param>
        /// <param name="rp2">The second apparent power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is greater than the value of rp2 otherwise false.
        /// </returns>
        public static bool operator >(ApparentPower rp1, ApparentPower rp2)
        {
            return rp1.CompareTo(rp2) > 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ApparentPower"/> is greater than or equal another specified <see cref="ApparentPower"/>.
        /// </summary>
        /// <param name="rp1">The first apparent power value to compare.</param>
        /// <param name="rp2">The second apparent power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is greather than or equal the value of rp2 otherwise false.
        /// </returns>
        public static bool operator >=(ApparentPower rp1, ApparentPower rp2)
        {
            return rp1.CompareTo(rp2) >= 0;
        }


        /// <summary>
        /// Compares this instance to a specified object and returns an integer that indicates 
        /// whether this instance is less than, equal to, or greater than the specified object.
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
            ApparentPower? other = obj as ApparentPower?;

            if (other == null)
            {
                return -1;
            }

            return CompareTo(other);
        }


        /// <summary>
        /// Compares this instance to a specified <see cref="ApparentPower"/> and returns an integer that indicates
        /// whether this instance is less than, equal to, or greater than the specified <see cref="ApparentPower"/>.
        /// </summary>
        /// <param name="other">An <see cref="ApparentPower"/> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="other"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="other"/>.
        /// Greater than zero: This instance follows <paramref name="other"/> in the sort order.
        /// </returns>
        public int CompareTo(ApparentPower other)
        {
            return VoltAmperes.CompareTo(other.VoltAmperes);
        }


        /// <summary>
        /// Checks whether <see cref="ApparentPower"/> is NaN.
        /// </summary>
        /// <returns>True if <see cref="ApparentPower"/> is NaN. Otherwise false.</returns>
        public bool IsNaN()
        {
            return double.IsNaN(VoltAmperes);
        }


        /// <summary>
        /// Current <see cref="ApparentPower"/> object as string representation.
        /// </summary>
        /// <returns>Current VoltAmperes as a string with suffix VAR</returns>
        public override string ToString()
        {
            return $"{VoltAmperes.ToString(CultureInfo.CurrentCulture)} VA";
        }
    }
}