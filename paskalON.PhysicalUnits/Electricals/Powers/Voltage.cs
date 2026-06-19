using System.Globalization;

namespace paskalON.PhysicalUnits.Electricals.Powers
{
    /// <summary>
    /// Struct for voltage V.
    /// </summary>
    /// <remarks>
    /// Voltage is the pressure from an electrical circuits power source that pushes charged electrons through a conducting loop.
    /// </remarks>
    public struct Voltage : IComparable, IComparable<Voltage>, IEquatable<Voltage>
    {
        /// <summary>
        /// Precision to round to the specific value.
        /// </summary>
        /// <remarks>
        /// Object initializer without setting precision will set it to 0.
        /// </remarks>
        public int Precision { get; private set; }


        /// <summary>
        /// Value of the current <see cref="Voltage"/> structure. 
        /// </summary>
        /// <value>Total Volts (V)</value>
        public double Volts { get; set; }


        /// <summary>
        /// Value of the current <see cref="Voltage"/> structure. 
        /// </summary>
        /// <value>Total Volts (V) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double VoltsPrecision { get => Math.Round(Volts, Precision); }


        /// <summary>
        /// Value of the current <see cref="Voltage"/> structure. 
        /// </summary>
        /// <value>Total Kilo-Volts (kV).</value>
        public readonly double KiloVolts { get => Volts / 1000; }


        /// <summary>
        /// Value of the current <see cref="Voltage"/> structure. 
        /// </summary>
        /// <value>Total Kilo-Volts (kV) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double KiloVoltsPrecision { get => Math.Round(KiloVolts, Precision); }


        /// <summary>
        /// Value of the current <see cref="Voltage"/> structure. 
        /// </summary>
        /// <value>Total Mega-Volts (mV).</value>
        public readonly double MegaVolts { get => Volts / 1000000; }


        /// <summary>
        /// Value of the current <see cref="Voltage"/> structure. 
        /// </summary>
        /// <value>Total Mega-Volts (mW) rounded to the specified precision.</value>
        /// <remarks>
        /// Use wisely as precision is less performant in calculations then full floating number.
        /// </remarks>
        public readonly double MegaVoltsPrecision { get => Math.Round(MegaVolts, Precision); }


        /// <summary>
        /// Constructor for <see cref="Voltage"/>
        /// </summary>
        /// <param name="volts">Number of volts (V).</param>
        public Voltage(double volts) : this(volts, 5)
        {
        }


        /// <summary>
        /// Constructor for <see cref="Voltage"/>
        /// </summary>
        /// <param name="volts">Number of volts (V).</param>
        /// <param name="precision">Precision for comparison.</param>
        public Voltage(double volts, int precision)
        {
            Volts = volts;
            Precision = precision;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>Integer hash code.</returns>
        public override int GetHashCode() => Volts.GetHashCode();


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// True if obj represents the same voltage value as this instance or otherwise false.
        /// </returns>
        public readonly override bool Equals(object? obj) => obj is Voltage other && Equals(other);


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="other">An <see cref="Voltage"/> to compare with this instance.</param>
        /// True if obj represents the same voltage value as this instance or otherwise false.
        public readonly bool Equals(Voltage other) => this == other;


        /// <summary>
        /// Indicates whether two <see cref="Voltage"/> instances are equal.
        /// </summary>
        /// <param name="v1">The first voltage value to compare.</param>
        /// <param name="v2">The second voltage value to compare.</param>
        public static bool operator ==(Voltage v1, Voltage v2) => Math.Round(v1.Volts, v1.Precision) == Math.Round(v2.Volts, v2.Precision);


        /// <summary>
        /// Indicates whether two <see cref="Voltage"/> instances are not equal.
        /// </summary>
        /// <param name="v1">The first voltage value to compare.</param>
        /// <param name="v2">The second voltage value to compare.</param>
        /// <returns>True if the values of v1 and v2 are not  equal or otherwise false.</returns>
        public static bool operator !=(Voltage v1, Voltage v2) => Math.Round(v1.Volts, v1.Precision) != Math.Round(v2.Volts, v2.Precision);


        /// <summary>
        /// Adds two <see cref="Voltage" instances./>
        /// </summary>
        /// <param name="v1">The first voltage value to add to.</param>
        /// <param name="v2">The second voltage value to add to.</param>
        /// <returns>Sum of the two <see cref="Voltage"./></returns>
        public static Voltage operator +(Voltage v1, Voltage v2) => new(v1.Volts + v2.Volts);


        /// <summary>
        /// Subtracts second <see cref="Voltage" instance from the first <see cref="Voltage" instance./>
        /// </summary>
        /// <param name="v1">The first voltage value.</param>
        /// <param name="v2">The second voltage value to subtract.</param>
        /// <returns>Sum of the two <see cref="Voltage"./></returns>
        public static Voltage operator -(Voltage v1, Voltage v2) => new(v1.Volts - v2.Volts);


        /// <summary>
        /// Multiplies two <see cref="Voltage" instances./>
        /// </summary>
        /// <param name="v1">The first voltage value.</param>
        /// <param name="v2">The second voltage value.</param>
        /// <returns>Sum of the two <see cref="Voltage"./></returns>
        public static Voltage operator *(Voltage v1, Voltage v2) => new(v1.Volts * v2.Volts);


        /// <summary>
        /// Divides first <see cref="Voltage" instance with the second <see cref="Voltage" instance./>
        /// </summary>
        /// <param name="v1">The first voltage value.</param>
        /// <param name="v2">The second voltage value to divide the first one with.</param>
        /// <returns>Sum of the two <see cref="Voltage"./></returns>
        public static Voltage operator /(Voltage v1, Voltage v2) => new(v2.Volts != 0 ? v1.Volts / v2.Volts : double.NaN);


        /// <summary>
        /// Indicates whether a specified <see cref="Voltage"/> is less than another specified <see cref="Voltage"/>.
        /// </summary>
        /// <param name="v1">The first voltage value to compare.</param>
        /// <param name="v2">The second voltage value to compare.</param>
        /// <returns>
        /// True if the value of v1 is less than the value of v2 otherwise false.
        /// </returns>
        public static bool operator <(Voltage v1, Voltage v2)
        {
            return v1.CompareTo(v2) < 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="Voltage"/> is less than or equal another specified <see cref="Voltage"/>.
        /// </summary>
        /// <param name="v1">The first voltage value to compare.</param>
        /// <param name="v2">The second voltage value to compare.</param>
        /// <returns>
        /// True if the value of v1 is less than or equal the value of v2 otherwise false.
        /// </returns>
        public static bool operator <=(Voltage v1, Voltage v2)
        {
            return v1.CompareTo(v2) <= 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="Voltage"/> is greater than another specified <see cref="Voltage"/>.
        /// </summary>
        /// <param name="v1">The first voltage value to compare.</param>
        /// <param name="v2">The second voltage value to compare.</param>
        /// <returns>
        /// True if the value of v1 is greater than the value of v2 otherwise false.
        /// </returns>
        public static bool operator >(Voltage v1, Voltage v2)
        {
            return v1.CompareTo(v2) > 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="Voltage"/> is greater than or equal another specified <see cref="Voltage"/>.
        /// </summary>
        /// <param name="v1">The first voltage value to compare.</param>
        /// <param name="v2">The second voltage value to compare.</param>
        /// <returns>
        /// True if the value of v1 is greater than or equal the value of v2 otherwise false.
        /// </returns>
        public static bool operator >=(Voltage v1, Voltage v2)
        {
            return v1.CompareTo(v2) >= 0;
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
            Voltage? other = obj as Voltage?;

            if (other == null)
            {
                return -1;
            }

            return CompareTo(other);
        }


        /// <summary>
        /// Compares this instance to a specified <see cref="Voltage"/> and returns an integer that indicates
        /// whether this instance is less than, equal to, or greater than the specified <see cref="Voltage"/>.
        /// </summary>
        /// <param name="other">An <see cref="Voltage"/> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Less than zero: This instance precedes <paramref name="other"/> in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as <paramref name="other"/>.
        /// Greater than zero: This instance follows <paramref name="other"/> in the sort order.
        /// </returns>
        public int CompareTo(Voltage other)
        {
            return Volts.CompareTo(other.Volts);
        }


        /// <summary>
        /// Checks whether <see cref="Voltage"/> is NaN.
        /// </summary>
        /// <returns>True if <see cref="Voltage"/> is NaN. Otherwise false.</returns>
        public bool IsNaN()
        {
            return double.IsNaN(Volts);
        }


        /// <summary>
        /// Current <see cref="Voltage"/> object as string representation.
        /// </summary>
        /// <returns>Current Volts as a string with suffix VAR</returns>
        public override string ToString()
        {
            return $"{Volts.ToString(CultureInfo.CurrentCulture)} V";
        }
    }
}