using System.Globalization;

namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Struct for current scalar using amperes (A).
    /// </summary>
    /// <remarks>
    /// CurrentScalar is the rate of electric charge per unit in time.
    /// Has magnitude only (e.g., 5 A).
    /// </remarks>
    public struct CurrentScalar : IComparable, IComparable<CurrentScalar>, IEquatable<CurrentScalar>
    {
        /// <summary>
        /// Precision to round to the specific value.
        /// </summary>
        /// <remarks>
        /// Object initializer without setting precision will set it to 0.
        /// </remarks>
        public int Precision { get; private set; }


        /// <summary>
        /// Value of the current <see cref="CurrentScalar"/> structure. 
        /// </summary>
        /// <value>Total Amperes (A)</value>
        public double Amperes { get; set; }


        /// <summary>
        /// Value of the current <see cref="CurrentScalar"/> structure. 
        /// </summary>
        /// <value>Total Amperes (A) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double AmperesPrecision { get => Math.Round(Amperes, Precision); }


        /// <summary>
        /// Value of the current <see cref="CurrentScalar"/> structure. 
        /// </summary>
        /// <value>Total Kilo-Amperes (kA).</value>
        public readonly double KiloAmperes { get => Amperes / 1000; }


        /// <summary>
        /// Value of the current <see cref="CurrentScalar"/> structure. 
        /// </summary>
        /// <value>Total Kilo-Amperes (kA) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double KiloAmperesPrecision { get => Math.Round(KiloAmperes, Precision); }


        /// <summary>
        /// Value of the current <see cref="CurrentScalar"/> structure. 
        /// </summary>
        /// <value>Total Mega-Amperes (mA).</value>
        public readonly double MegaAmperes { get => Amperes / 1000000; }


        /// <summary>
        /// Value of the current <see cref="CurrentScalar"/> structure. 
        /// </summary>
        /// <value>Total Mega-Amperes (mA) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double MegaAmperesPrecision { get => Math.Round(MegaAmperes, Precision); }


        /// <summary>
        /// Constructor for <see cref="CurrentScalar"/>
        /// </summary>
        /// <param name="amperes">Number of amperes (A).</param>
        public CurrentScalar(double amperes) : this(amperes, 5)
        {
        }


        /// <summary>
        /// Constructor for <see cref="CurrentScalar"/>
        /// </summary>
        /// <param name="amperes">Number of amperes (A).</param>
        /// <param name="precision">Precision for comparison.</param>
        public CurrentScalar(double amperes, int precision)
        {
            Amperes = amperes;
            Precision = precision;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>Integer hash code.</returns>
        public override int GetHashCode() => Amperes.GetHashCode();


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// True if obj represents the same amperes value as this instance or otherwise false.
        /// </returns>
        public readonly override bool Equals(object? obj) => obj is CurrentScalar other && Equals(other);


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="other">An <see cref="CurrentScalar"/> to compare with this instance.</param>
        /// True if obj represents the same amperes value as this instance or otherwise false.
        public readonly bool Equals(CurrentScalar other) => this == other;


        /// <summary>
        /// Indicates whether two <see cref="CurrentScalar"/> instances are equal.
        /// </summary>
        /// <param name="a1">The first amperes value to compare.</param>
        /// <param name="a2">The second amperes value to compare.</param>
        public static bool operator ==(CurrentScalar a1, CurrentScalar a2) => Math.Round(a1.Amperes, a1.Precision) == Math.Round(a2.Amperes, a2.Precision);


        /// <summary>
        /// Indicates whether two <see cref="CurrentScalar"/> instances are not equal.
        /// </summary>
        /// <param name="a1">The first amperes value to compare.</param>
        /// <param name="a2">The second amperes value to compare.</param>
        /// <returns>True if the values of a1 and a2 are not  equal or otherwise false.</returns>
        public static bool operator !=(CurrentScalar a1, CurrentScalar a2) => Math.Round(a1.Amperes, a1.Precision) != Math.Round(a2.Amperes, a2.Precision);


        /// <summary>
        /// Adds two <see cref="CurrentScalar" instances./>
        /// </summary>
        /// <param name="a1">The first amperes value to add to.</param>
        /// <param name="a2">The second amperes value to add to.</param>
        /// <returns>Sum of the two <see cref="CurrentScalar"./></returns>
        public static CurrentScalar operator +(CurrentScalar a1, CurrentScalar a2) => new(a1.Amperes + a2.Amperes);


        /// <summary>
        /// Subtracts second <see cref="CurrentScalar" instance from the first <see cref="CurrentScalar" instance./>
        /// </summary>
        /// <param name="a1">The first amperes value.</param>
        /// <param name="a2">The second amperes value to subtract.</param>
        /// <returns>Sum of the two <see cref="CurrentScalar"./></returns>
        public static CurrentScalar operator -(CurrentScalar a1, CurrentScalar a2) => new(a1.Amperes - a2.Amperes);


        /// <summary>
        /// Multiplies two <see cref="CurrentScalar" instances./>
        /// </summary>
        /// <param name="a1">The first amperes value.</param>
        /// <param name="a2">The second amperes value.</param>
        /// <returns>Sum of the two <see cref="CurrentScalar"./></returns>
        public static CurrentScalar operator *(CurrentScalar a1, CurrentScalar a2) => new(a1.Amperes * a2.Amperes);


        /// <summary>
        /// Divides first <see cref="CurrentScalar" instance with the second <see cref="CurrentScalar" instance./>
        /// </summary>
        /// <param name="a1">The first amperes value.</param>
        /// <param name="a2">The second amperes value to divide the first one with.</param>
        /// <returns>Sum of the two <see cref="CurrentScalar"./></returns>
        public static CurrentScalar operator /(CurrentScalar a1, CurrentScalar a2) => new(a2.Amperes != 0 ? a1.Amperes / a2.Amperes : double.NaN);


        /// <summary>
        /// Indicates whether a specified <see cref="CurrentScalar"/> is less than another specified <see cref="CurrentScalar"/>.
        /// </summary>
        /// <param name="a1">The first amperes value to compare.</param>
        /// <param name="a2">The second amperes value to compare.</param>
        /// <returns>
        /// True if the value of a1 is less than the value of a2 otherwise false.
        /// </returns>
        public static bool operator <(CurrentScalar a1, CurrentScalar a2)
        {
            return a1.CompareTo(a2) < 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="CurrentScalar"/> is less than or equal another specified <see cref="CurrentScalar"/>.
        /// </summary>
        /// <param name="a1">The first amperes value to compare.</param>
        /// <param name="a2">The second amperes value to compare.</param>
        /// <returns>
        /// True if the value of a1 is less than or equal the value of a2 otherwise false.
        /// </returns>
        public static bool operator <=(CurrentScalar a1, CurrentScalar a2)
        {
            return a1.CompareTo(a2) <= 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="CurrentScalar"/> is greater than another specified <see cref="CurrentScalar"/>.
        /// </summary>
        /// <param name="a1">The first amperes value to compare.</param>
        /// <param name="a2">The second amperes value to compare.</param>
        /// <returns>
        /// True if the value of a1 is greater than the value of a2 otherwise false.
        /// </returns>
        public static bool operator >(CurrentScalar a1, CurrentScalar a2)
        {
            return a1.CompareTo(a2) > 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="CurrentScalar"/> is greater than or equal another specified <see cref="CurrentScalar"/>.
        /// </summary>
        /// <param name="a1">The first amperes value to compare.</param>
        /// <param name="a2">The second amperes value to compare.</param>
        /// <returns>
        /// True if the value of a1 is greater than or equal the value of a2 otherwise false.
        /// </returns>
        public static bool operator >=(CurrentScalar a1, CurrentScalar a2)
        {
            return a1.CompareTo(a2) >= 0;
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
            CurrentScalar? other = obj as CurrentScalar?;

            if (other == null)
            {
                return -1;
            }

            return CompareTo(other);
        }


        /// <summary>
        /// Compares this instance to a specified <see cref="CurrentScalar"/> and returns an integer that indicates
        /// whether this instance is less than, equal to, or greater than the specified <see cref="CurrentScalar"/>.
        /// </summary>
        /// <param name="other">An <see cref="CurrentScalar"/> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="other"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="other"/>.
        /// Greater than zero: This instance follows <paramref name="other"/> in the sort order.
        /// </returns>
        public int CompareTo(CurrentScalar other)
        {
            return Amperes.CompareTo(other.Amperes);
        }


        /// <summary>
        /// Checks whether <see cref="CurrentScalar"/> is NaN.
        /// </summary>
        /// <returns>True if <see cref="CurrentScalar"/> is NaN. Otherwise false.</returns>
        public bool IsNaN()
        {
            return double.IsNaN(Amperes);
        }


        /// <summary>
        /// Current <see cref="CurrentScalar"/> object as string representation.
        /// </summary>
        /// <returns>Current Amperes as a string with suffix VAR</returns>
        public override string ToString()
        {
            return $"{Amperes.ToString(CultureInfo.CurrentCulture)} V";
        }
    }
}