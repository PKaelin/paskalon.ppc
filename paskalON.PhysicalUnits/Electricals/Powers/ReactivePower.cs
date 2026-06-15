using System.Globalization;

namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Struct for reactive power (volt amperes reactive).
    /// </summary>
    /// <remarks>
    /// Reactive Power is measured in Volt-Amperes Reactive (VAR). 
    /// Represents the power oscillating back and forth in the circuit, absorbed and returned by magnetic or electric fields (inductors and capacitors).
    /// </remarks>
    public struct ReactivePower : IComparable, IComparable<ReactivePower>, IEquatable<ReactivePower>
    {
        /// <summary>
        /// Precision to round to the specific value.
        /// </summary>
        /// <remarks>
        /// Object initializer without setting precision will set it to 0.
        /// </remarks>
        public int Precision { get; private set; }


        /// <summary>
        /// Value of the current <see cref="ReactivePower"/> structure. 
        /// </summary>
        /// <value>Total VoltAmperesReactive (VAR)</value>
        public double VoltAmperesReactive { get; set; }


        /// <summary>
        /// Value of the current <see cref="ReactivePower"/> structure. 
        /// </summary>
        /// <value>Total VoltAmperesReactive (VAR) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public double VoltAmperesReactivePercision { get => Math.Round(VoltAmperesReactive, Precision); }


        /// <summary>
        /// Value of the current <see cref="ReactivePower"/> structure. 
        /// </summary>
        /// <value>Total Kilo-VoltAmperesReactive (kVAR).</value>
        public double KiloVoltAmperesReactive { get { return VoltAmperesReactive / 1000; } }


        /// <summary>
        /// Value of the current <see cref="ReactivePower"/> structure. 
        /// </summary>
        /// <value>Total Kilo-VoltAmperesReactive (kVAR) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public double KiloVoltAmperesReactivePercision { get { return Math.Round(KiloVoltAmperesReactive, Precision); } }


        /// <summary>
        /// Value of the current <see cref="ReactivePower"/> structure. 
        /// </summary>
        /// <value>Total Mega-VoltAmperesReactive (mVAR).</value>
        public double MegaVoltAmperesReactive { get { return VoltAmperesReactive / 1000000; } }


        /// <summary>
        /// Value of the current <see cref="ReactivePower"/> structure. 
        /// </summary>
        /// <value>Total Mega-VoltAmperesReactive (mVAR) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public double MegaVoltAmperesReactivePercision { get { return Math.Round(MegaVoltAmperesReactive, Precision); } }


        /// <summary>
        /// Constructor for <see cref="ReactivePower"/>
        /// </summary>
        /// <param name="voltAmperesReactive">Number of volt ampere reactive (VAR).</param>
        public ReactivePower(double voltAmperesReactive) : this(voltAmperesReactive, 5)
        {
        }


        /// <summary>
        /// Constructor for <see cref="ReactivePower"/>
        /// </summary>
        /// <param name="voltAmperesReactive">Number of volt ampere reactive (VAR).</param>
        /// <param name="precision">Precision for comparison.</param>
        public ReactivePower(double voltAmperesReactive, int precision)
        {
            VoltAmperesReactive = voltAmperesReactive;
            Precision = precision;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// Integer hash code.
        /// </returns>
        public override int GetHashCode() => VoltAmperesReactive.GetHashCode();


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// True if obj represents the same reactive power value as this instance or otherwise false.
        /// </returns>
        public readonly override bool Equals(object? obj) => obj is ReactivePower other && Equals(other);


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="other">A ReactivePower to compare with this instance.</param>
        /// True if obj represents the same reactive power value as this instance or otherwise false.
        public readonly bool Equals(ReactivePower other) => this == other;


        /// <summary>
        /// Indicates whether two <see cref="ReactivePower"/> instances are equal.
        /// </summary>
        /// <param name="rp1">The first reactive power value to compare.</param>
        /// <param name="rp2">The second reactive power value to compare.</param>
        /// <returns>True if the values of rp1 and rp2 are equal or otherwise false.</returns>
        public static bool operator ==(ReactivePower rp1, ReactivePower rp2) => Math.Round(rp1.VoltAmperesReactive, rp1.Precision) == Math.Round(rp2.VoltAmperesReactive, rp2.Precision);


        /// <summary>
        /// Indicates whether two <see cref="ReactivePower"/> instances are not equal.
        /// </summary>
        /// <param name="rp1">The first reactive power value to compare.</param>
        /// <param name="rp2">The second reactive power value to compare.</param>
        /// <returns>True if the values of rp1 and rp2 are not  equal or otherwise false.</returns>
        public static bool operator !=(ReactivePower rp1, ReactivePower rp2) => Math.Round(rp1.VoltAmperesReactive, rp1.Precision) != Math.Round(rp2.VoltAmperesReactive, rp2.Precision);


        /// <summary>
        /// Adds two <see cref="ReactivePower" instances./>
        /// </summary>
        /// <param name="rp1">The first reactive power value to compare.</param>
        /// <param name="rp2">The second reactive power value to compare.</param>
        /// <returns>Sum of the two <see cref="ReactivePower"./></returns>
        public static ReactivePower operator +(ReactivePower rp1, ReactivePower rp2) => new(rp1.VoltAmperesReactive + rp2.VoltAmperesReactive);


        /// <summary>
        /// Substracts second <see cref="ReactivePower" instance from the first <see cref="ReactivePower" instance./>
        /// </summary>
        /// <param name="rp1">The first reactive power value.</param>
        /// <param name="rp2">The second reactive power value to substract.</param>
        /// <returns>Sum of the two <see cref="ReactivePower"./></returns>
        public static ReactivePower operator -(ReactivePower rp1, ReactivePower rp2) => new(rp1.VoltAmperesReactive - rp2.VoltAmperesReactive);


        /// <summary>
        /// Multiplies two <see cref="ReactivePower" instances./>
        /// </summary>
        /// <param name="rp1">The first reactive power value.</param>
        /// <param name="rp2">The second reactive power value.</param>
        /// <returns>Sum of the two <see cref="ReactivePower"./></returns>
        public static ReactivePower operator *(ReactivePower rp1, ReactivePower rp2) => new(rp1.VoltAmperesReactive * rp2.VoltAmperesReactive);


        /// <summary>
        /// Divides first <see cref="ReactivePower" instance with the second <see cref="ReactivePower" instance./>
        /// </summary>
        /// <param name="rp1">The first reactive power value.</param>
        /// <param name="rp2">The second reactive power value to divide the first one with.</param>
        /// <returns>Sum of the two <see cref="ReactivePower"./></returns>
        public static ReactivePower operator /(ReactivePower rp1, ReactivePower rp2) => new(rp2.VoltAmperesReactive != 0 ? rp1.VoltAmperesReactive / rp2.VoltAmperesReactive : double.NaN);


        /// <summary>
        /// Indicates whether a specified <see cref="ReactivePower"/> is less than another specified <see cref="ReactivePower"/>.
        /// </summary>
        /// <param name="rp1">The first reactive power value to compare.</param>
        /// <param name="rp2">The second reactive power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is less than the value of rp2 otherwise false.
        /// </returns>
        public static bool operator <(ReactivePower rp1, ReactivePower rp2)
        {
            return rp1.CompareTo(rp2) < 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ReactivePower"/> is less than or equal another specified <see cref="ReactivePower"/>.
        /// </summary>
        /// <param name="rp1">The first reactive power value to compare.</param>
        /// <param name="rp2">The second reactive power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is less than or equal the value of rp2 otherwise false.
        /// </returns>
        public static bool operator <=(ReactivePower rp1, ReactivePower rp2)
        {
            return rp1.CompareTo(rp2) <= 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ReactivePower"/> is greater than another specified <see cref="ReactivePower"/>.
        /// </summary>
        /// <param name="rp1">The first reactive power value to compare.</param>
        /// <param name="rp2">The second reactive power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is greater than the value of rp2 otherwise false.
        /// </returns>
        public static bool operator >(ReactivePower rp1, ReactivePower rp2)
        {
            return rp1.CompareTo(rp2) > 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ReactivePower"/> is greater than or equal another specified <see cref="ReactivePower"/>.
        /// </summary>
        /// <param name="rp1">The first reactive power value to compare.</param>
        /// <param name="rp2">The second reactive power value to compare.</param>
        /// <returns>
        /// True if the value of rp1 is greather than or equal the value of rp2 otherwise false.
        /// </returns>
        public static bool operator >=(ReactivePower rp1, ReactivePower rp2)
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
            ReactivePower? other = obj as ReactivePower?;

            if (other == null)
            {
                return -1;
            }

            return CompareTo(other);
        }


        /// <summary>
        /// Compares this instance to a specified <see cref="ReactivePower"/> and returns an integer that indicates
        /// whether this instance is less than, equal to, or greater than the specified <see cref="ReactivePower"/>.
        /// </summary>
        /// <param name="value">A <see cref="ReactivePower"/> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="value"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="value"/>.
        /// Greater than zero: This instance follows <paramref name="value"/> in the sort order.
        /// </returns>
        public int CompareTo(ReactivePower value)
        {
            return VoltAmperesReactive.CompareTo(value.VoltAmperesReactive);
        }


        /// <summary>
        /// Checks whether <see cref="ReactivePower"/> is NaN.
        /// </summary>
        /// <returns>True if <see cref="ReactivePower"/> is NaN. Otherwise false.</returns>
        public bool IsNaN()
        {
            return double.IsNaN(VoltAmperesReactive);
        }


        /// <summary>
        /// Current <see cref="ReactivePower"/> object as string representation.
        /// </summary>
        /// <returns>Current VoltAmperesReactive as a string with suffix VAR</returns>
        public override string ToString()
        {
            return $"{VoltAmperesReactive.ToString(CultureInfo.CurrentCulture)} VAR";
        }
    }
}