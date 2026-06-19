using System.Globalization;

namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Struct for active power (Watts).
    /// </summary>
    /// <remarks>
    /// Active Power is measured in Watts (W). 
    /// Represents the actual energy consumed or dissipated by a load over time.
    /// </remarks>
    public struct ActivePower : IComparable, IComparable<ActivePower>, IEquatable<ActivePower>
    {
        /// <summary>
        /// Precision to round to the specific value.
        /// </summary>
        /// <remarks>
        /// Object initializer without setting precision will set it to 0.
        /// </remarks>
        public int Precision { get; private set; }


        /// <summary>
        /// Value of the current <see cref="ActivePower"/> structure. 
        /// </summary>
        /// <value>Total Watts (W)</value>
        public double Watts { get; set; }


        /// <summary>
        /// Value of the current <see cref="ActivePower"/> structure. 
        /// </summary>
        /// <value>Total Watts (W) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double WattsPrecision { get => Math.Round(Watts, Precision); }


        /// <summary>
        /// Value of the current <see cref="ActivePower"/> structure. 
        /// </summary>
        /// <value>Total Kilo-Watts (kW).</value>
        public readonly double KiloWatts { get => Watts / 1000; }


        /// <summary>
        /// Value of the current <see cref="ActivePower"/> structure. 
        /// </summary>
        /// <value>Total Kilo-Watts (kW) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double KiloWattsPrecision { get => Math.Round(KiloWatts, Precision); }


        /// <summary>
        /// Value of the current <see cref="ActivePower"/> structure. 
        /// </summary>
        /// <value>Total Mega-Watts (mW).</value>
        public readonly double MegaWatts { get => Watts / 1000000; }


        /// <summary>
        /// Value of the current <see cref="ActivePower"/> structure. 
        /// </summary>
        /// <value>Total Mega-Watts (mW) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double MegaWattsPrecision { get => Math.Round(MegaWatts, Precision); }


        /// <summary>
        /// Constructor for <see cref="ActivePower"/>
        /// </summary>
        /// <param name="watts">Number of watts (W).</param>
        public ActivePower(double watts) : this(watts, 5)
        {
        }


        /// <summary>
        /// Constructor for <see cref="ActivePower"/>
        /// </summary>
        /// <param name="watts">Number of watts (W).</param>
        /// <param name="precision">Precision for comparison.</param>
        public ActivePower(double watts, int precision)
        {
            Watts = watts;
            Precision = precision;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>Integer hash code.</returns>
        public override int GetHashCode() => Watts.GetHashCode();


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// True if obj represents the same active power value as this instance or otherwise false.
        /// </returns>
        public readonly override bool Equals(object? obj) => obj is ActivePower other && Equals(other);


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="other">An <see cref="ActivePower"/> to compare with this instance.</param>
        /// True if obj represents the same active power value as this instance or otherwise false.
        public readonly bool Equals(ActivePower other) => this == other;


        /// <summary>
        /// Indicates whether two <see cref="ActivePower"/> instances are equal.
        /// </summary>
        /// <param name="ap1">The first active power value to compare.</param>
        /// <param name="ap2">The second active power value to compare.</param>
        /// <returns>True if the values of ap1 and ap2 are equal or otherwise false.</returns>
        public static bool operator ==(ActivePower ap1, ActivePower ap2) => Math.Round(ap1.Watts, ap1.Precision) == Math.Round(ap2.Watts, ap2.Precision);


        /// <summary>
        /// Indicates whether two <see cref="ActivePower"/> instances are not equal.
        /// </summary>
        /// <param name="ap1">The first active power value to compare.</param>
        /// <param name="ap2">The second active power value to compare.</param>
        /// <returns>True if the values of ap1 and ap2 are not  equal or otherwise false.</returns>
        public static bool operator !=(ActivePower ap1, ActivePower ap2) => Math.Round(ap1.Watts, ap1.Precision) != Math.Round(ap2.Watts, ap2.Precision);


        /// <summary>
        /// Adds two <see cref="ActivePower" instances./>
        /// </summary>
        /// <param name="ap1">The first active power value to add to.</param>
        /// <param name="ap2">The second active power value to add to.</param>
        /// <returns>Sum of the two <see cref="ActivePower"./></returns>
        public static ActivePower operator +(ActivePower ap1, ActivePower ap2) => new(ap1.Watts + ap2.Watts);


        /// <summary>
        /// Subtracts second <see cref="ActivePower" instance from the first <see cref="ActivePower" instance./>
        /// </summary>
        /// <param name="ap1">The first active power value.</param>
        /// <param name="ap2">The second active power value to subtract.</param>
        /// <returns>Sum of the two <see cref="ActivePower"./></returns>
        public static ActivePower operator -(ActivePower ap1, ActivePower ap2) => new(ap1.Watts - ap2.Watts);


        /// <summary>
        /// Multiplies two <see cref="ActivePower" instances./>
        /// </summary>
        /// <param name="ap1">The first active power value.</param>
        /// <param name="ap2">The second active power value.</param>
        /// <returns>Sum of the two <see cref="ActivePower"./></returns>
        public static ActivePower operator *(ActivePower ap1, ActivePower ap2) => new(ap1.Watts * ap2.Watts);


        /// <summary>
        /// Divides first <see cref="ActivePower" instance with the second <see cref="ActivePower" instance./>
        /// </summary>
        /// <param name="ap1">The first active power value.</param>
        /// <param name="ap2">The second active power value to divide the first one with.</param>
        /// <returns>Sum of the two <see cref="ActivePower"./></returns>
        public static ActivePower operator /(ActivePower ap1, ActivePower ap2) => new(ap2.Watts != 0 ? ap1.Watts / ap2.Watts : double.NaN);


        /// <summary>
        /// Indicates whether a specified <see cref="ActivePower"/> is less than another specified <see cref="ActivePower"/>.
        /// </summary>
        /// <param name="ap1">The first active power value to compare.</param>
        /// <param name="ap2">The second active power value to compare.</param>
        /// <returns>
        /// True if the value of ap1 is less than the value of ap2 otherwise false.
        /// </returns>
        public static bool operator <(ActivePower ap1, ActivePower ap2)
        {
            return ap1.CompareTo(ap2) < 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ActivePower"/> is less than or equal another specified <see cref="ActivePower"/>.
        /// </summary>
        /// <param name="ap1">The first active power value to compare.</param>
        /// <param name="ap2">The second active power value to compare.</param>
        /// <returns>
        /// True if the value of ap1 is less than or equal the value of ap2 otherwise false.
        /// </returns>
        public static bool operator <=(ActivePower ap1, ActivePower ap2)
        {
            return ap1.CompareTo(ap2) <= 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ActivePower"/> is greater than another specified <see cref="ActivePower"/>.
        /// </summary>
        /// <param name="ap1">The first active power value to compare.</param>
        /// <param name="ap2">The second active power value to compare.</param>
        /// <returns>
        /// True if the value of ap1 is greater than the value of ap2 otherwise false.
        /// </returns>
        public static bool operator >(ActivePower ap1, ActivePower ap2)
        {
            return ap1.CompareTo(ap2) > 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="ActivePower"/> is greater than or equal another specified <see cref="ActivePower"/>.
        /// </summary>
        /// <param name="ap1">The first active power value to compare.</param>
        /// <param name="ap2">The second active power value to compare.</param>
        /// <returns>
        /// True if the value of ap1 is greather than or equal the value of ap2 otherwise false.
        /// </returns>
        public static bool operator >=(ActivePower ap1, ActivePower ap2)
        {
            return ap1.CompareTo(ap2) >= 0;
        }


        /// <summary>
        /// Compares this instance to a specified object and returns an integer that indicates 
        /// whether this instance is less than, equal to, or greater than the specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="value"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="value"/>.
        /// Greater than zero: This instance follows <paramref name="value"/> in the sort order.
        /// </returns>
        public int CompareTo(object? obj)
        {
            ActivePower? other = obj as ActivePower?;

            if (other == null)
            {
                return -1;
            }

            return CompareTo(other);
        }


        /// <summary>
        /// Compares this instance to a specified <see cref="ActivePower"/> and returns an integer that indicates
        /// whether this instance is less than, equal to, or greater than the specified <see cref="ActivePower"/>.
        /// </summary>
        /// <param name="other">An <see cref="ActivePower"/> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="other"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="other"/>.
        /// Greater than zero: This instance follows <paramref name="other"/> in the sort order.
        /// </returns>
        public int CompareTo(ActivePower other)
        {
            return Watts.CompareTo(other.Watts);
        }


        /// <summary>
        /// Checks whether <see cref="ActivePower"/> is NaN.
        /// </summary>
        /// <returns>True if <see cref="ActivePower"/> is NaN. Otherwise false.</returns>
        public bool IsNaN()
        {
            return double.IsNaN(Watts);
        }


        /// <summary>
        /// Current <see cref="ActivePower"/> object as string representation.
        /// </summary>
        /// <returns>Current Watts as a string with suffix VAR</returns>
        public override string ToString()
        {
            return $"{Watts.ToString(CultureInfo.CurrentCulture)} W";
        }
    }
}