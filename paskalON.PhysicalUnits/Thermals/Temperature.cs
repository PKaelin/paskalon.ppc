using System.Globalization;

namespace paskalON.PhysicalUnits.Thermals
{
    /// <summary>
    /// Represents a temperature value and provides methods for converting between different temperature scales.
    /// </summary>
    public struct Temperature
    {
        /// <summary>
        /// Temperature value. The value is stored in the unit specified by the TemperatureUnit property.
        /// </summary>
        private double temperature;


        /// <summary>
        /// Precision for equal and not equal comparisons.
        /// </summary>
        /// <remarks>
        /// Object initializer without setting precision will set it to 0.
        /// </remarks>
        public int Precision { get; private set; }


        /// <summary>
        /// Temperature unit.
        /// </summary>
        public TemperatureUnit TemperatureUnit { get; private set; }


        /// <summary>
        /// Temperature in celsius. If the temperature unit is set to celsius, it will return the value directly.
        /// Otherwise, it will convert the value from fahrenheit to celsius before returning it.
        /// </summary>
        public double Celsius
        {
            get => TemperatureUnit == TemperatureUnit.Celsius ? temperature : ToCelsius(temperature);
            set => temperature = value;
        }


        /// <summary>
        /// Temperature in celsius. If the temperature unit is set to celsius, it will return the value directly.
        /// Otherwise, it will convert the value from fahrenheit to celsius before returning it.
        /// </summary>
        public double CelsiusPrecision => Math.Round(Celsius, Precision);


        /// <summary>
        /// Temperature in fahrenheit. If the temperature unit is set to fahrenheit, it will return the value directly. 
        /// Otherwise, it will convert the value from celsius to fahrenheit before returning it.
        /// </summary>
        public double Fahrenheit
        {
            get => TemperatureUnit == TemperatureUnit.Fahrenheit ? temperature : ToFahrenheit(temperature);
            set => temperature = value;
        }


        /// <summary>
        /// Temperature in fahrenheit. If the temperature unit is set to fahrenheit, it will return the value directly. 
        /// Otherwise, it will convert the value from celsius to fahrenheit before returning it.
        /// </summary>
        public double FahrenheitPrecision => Math.Round(Fahrenheit, Precision);


        /// <summary>
        /// Initializes a new instance of the <see cref="Temperature"/> struct with the specified temperature unit and value.
        /// </summary>
        /// <param name="unit">Temperature unit.</param>
        /// <param name="value">Temperature value.</param>
        public Temperature(TemperatureUnit unit, double value) : this(unit, value, 5)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Temperature"/> struct with the specified temperature unit and value.
        /// </summary>
        /// <param name="unit">Temperature unit.</param>
        /// <param name="value">Temperature value.</param>
        /// <param name="precision">Precision for comparison.</param>
        public Temperature(TemperatureUnit unit, double value, int precision)
        {
            Precision = precision;
            TemperatureUnit = unit;
            temperature = value;
        }


        /// <summary>
        /// Convert celsius to fahrenheit.
        /// </summary>
        /// <param name="celsius">The temperature in celsius.</param>
        /// <returns>Temperature in fahrenheit.</returns>
        public static double ToFahrenheit(double celsius)
        {
            return celsius * (9d / 5d) + 32d;
        }


        /// <summary>
        /// Convert fahrenheit to celsius.
        /// </summary>
        /// <param name="fahrenheit">The temperature in fahrenheit.</param>
        /// <returns>Temperature in celsius.</returns>
        public static double ToCelsius(double fahrenheit)
        {
            return (fahrenheit - 32d) * (5d / 9d);
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>Integer hash code.</returns>
        public override int GetHashCode() => temperature.GetHashCode();


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// True if obj represents the same temperature value as this instance or otherwise false.
        /// </returns>
        public readonly override bool Equals(object? obj) => obj is Temperature other && Equals(other);


        /// <summary>
        /// Returns a value indicating whether this instance is equal.
        /// </summary>
        /// <param name="other">A Temperature to compare with this instance.</param>
        /// True if obj represents the same temperature value as this instance or otherwise false.
        public readonly bool Equals(Temperature other) => this == other;


        /// <summary>
        /// Indicates whether two <see cref="Temperature"/> instances are equal.
        /// </summary>
        /// <param name="temp1">The first temperature value to compare.</param>
        /// <param name="temp2">The second temperature value to compare.</param>
        /// <returns>True if the values of temp1 and temp2 are equal or otherwise false.</returns>
        public static bool operator ==(Temperature temp1, Temperature temp2) => Math.Round(temp1.Celsius, temp1.Precision) == Math.Round(temp2.Celsius, temp2.Precision);


        /// <summary>
        /// Indicates whether two <see cref="Temperature"/> instances are not equal.
        /// </summary>
        /// <param name="temp1">The first temperature value to compare.</param>
        /// <param name="temp2">The second temperature value to compare.</param>
        /// <returns>True if the values of temp1 and temp2 are not  equal or otherwise false.</returns>
        public static bool operator !=(Temperature temp1, Temperature temp2) => Math.Round(temp1.Celsius, temp1.Precision) != Math.Round(temp2.Celsius, temp2.Precision);


        /// <summary>
        /// Adds two <see cref="Temperature"/> instances.
        /// </summary>
        /// <param name="temp1">The first temperature value to add to.</param>
        /// <param name="temp2">The second temperature value to add to.</param>
        /// <returns>Sum of the two <see cref="Temperature"./></returns>
        public static Temperature operator +(Temperature temp1, Temperature temp2) =>
            new(temp1.TemperatureUnit, temp1.TemperatureUnit == TemperatureUnit.Celsius ? temp1.Celsius + temp2.Celsius : temp1.Fahrenheit + temp2.Fahrenheit);


        /// <summary>
        /// Subtracts second <see cref="Temperature" instance from the first <see cref="Temperature" instance./>
        /// </summary>
        /// <param name="temp1">The first temperature value.</param>
        /// <param name="temp2">The second temperature value to subtract.</param>
        /// <returns>Sum of the two <see cref="Temperature"./></returns>
        public static Temperature operator -(Temperature temp1, Temperature temp2) =>
            new(temp1.TemperatureUnit, temp1.TemperatureUnit == TemperatureUnit.Celsius ? temp1.Celsius - temp2.Celsius : temp1.Fahrenheit - temp2.Fahrenheit);


        /// <summary>
        /// Multiplies two <see cref="Temperature" instances./>
        /// </summary>
        /// <param name="temp1">The first temperature value.</param>
        /// <param name="temp2">The second temperature value.</param>
        /// <returns>Sum of the two <see cref="Temperature"./></returns>
        public static Temperature operator *(Temperature temp1, Temperature temp2) =>
            new(temp1.TemperatureUnit, temp1.TemperatureUnit == TemperatureUnit.Celsius ? temp1.Celsius * temp2.Celsius : temp1.Fahrenheit * temp2.Fahrenheit);


        /// <summary>
        /// Divides first <see cref="Temperature" instance with the second <see cref="Temperature" instance./>
        /// </summary>
        /// <param name="temp1">The first temperature value.</param>
        /// <param name="temp2">The second temperature value to divide the first one with.</param>
        /// <returns>Sum of the two <see cref="Temperature"./></returns>
        public static Temperature operator /(Temperature temp1, Temperature temp2) =>
            new(temp1.TemperatureUnit, temp1.TemperatureUnit == TemperatureUnit.Celsius ?
                (temp2.Celsius != 0 ? temp1.Celsius / temp2.Celsius : double.NaN) :
                (temp2.Fahrenheit != 0 ? temp1.Fahrenheit / temp2.Fahrenheit : double.NaN));


        /// <summary>
        /// Indicates whether a specified <see cref="Temperature"/> is less than another specified <see cref="Temperature"/>.
        /// </summary>
        /// <param name="temp1">The first temperature value to compare.</param>
        /// <param name="temp2">The second temperature value to compare.</param>
        /// <returns>
        /// True if the value of temp1 is less than the value of temp2 otherwise false.
        /// </returns>
        public static bool operator <(Temperature temp1, Temperature temp2)
        {
            return temp1.CompareTo(temp2) < 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="Temperature"/> is less than or equal another specified <see cref="Temperature"/>.
        /// </summary>
        /// <param name="temp1">The first temperature value to compare.</param>
        /// <param name="temp2">The second temperature value to compare.</param>
        /// <returns>
        /// True if the value of temp1 is less than or equal the value of temp2 otherwise false.
        /// </returns>
        public static bool operator <=(Temperature temp1, Temperature temp2)
        {
            return temp1.CompareTo(temp2) <= 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="Temperature"/> is greater than another specified <see cref="Temperature"/>.
        /// </summary>
        /// <param name="temp1">The first temperature value to compare.</param>
        /// <param name="temp2">The second temperature value to compare.</param>
        /// <returns>
        /// True if the value of temp1 is greater than the value of temp2 otherwise false.
        /// </returns>
        public static bool operator >(Temperature temp1, Temperature temp2)
        {
            return temp1.CompareTo(temp2) > 0;
        }


        /// <summary>
        /// Indicates whether a specified <see cref="Temperature"/> is greater than or equal another specified <see cref="Temperature"/>.
        /// </summary>
        /// <param name="temp1">The first temperature value to compare.</param>
        /// <param name="temp2">The second temperature value to compare.</param>
        /// <returns>
        /// True if the value of temp1 is greather than or equal the value of temp2 otherwise false.
        /// </returns>
        public static bool operator >=(Temperature temp1, Temperature temp2)
        {
            return temp1.CompareTo(temp2) >= 0;
        }


        /// <summary>
        /// Compares this instance to a specified <see cref="Temperature"/> object and returns an integer that indicates 
        /// whether this instance is less than, equal to, or greater than the <see cref="Temperature"/> object.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value:
        /// - A negative integer if this instance is less than value.
        /// - Zero if this instance is equal to value. 
        /// - A positive integer if this instance is greater than value.
        /// </returns>
        public int CompareTo(Temperature value)
        {
            return Celsius.CompareTo(value.Celsius);
        }


        /// <summary>
        /// Checks whether <see cref="Temperature"/> is NaN.
        /// </summary>
        /// <returns>True if <see cref="Temperature"/> is NaN. Otherwise false.</returns>
        public bool IsNaN()
        {
            return double.IsNaN(Celsius);
        }


        /// <summary>
        /// Current <see cref="Temperature"/> object as string representation.
        /// </summary>
        /// <returns>Current Celsius as a string with suffix VAR</returns>
        public override string ToString()
        {
            if (TemperatureUnit == TemperatureUnit.Fahrenheit)
            {
                return $"{Fahrenheit.ToString(CultureInfo.CurrentCulture)} Fahrenheit";
            }

            return $"{Celsius.ToString(CultureInfo.CurrentCulture)} Celsius";
        }
    }
}