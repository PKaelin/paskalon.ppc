// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Globalization;

namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Converts CSV values to the .NET property types.
    /// </summary>
    public static class PropertyValueConverter
    {
        /// <summary>
        /// Checks whether the type is converted from a value instead of being a reference to an entity.
        /// </summary>
        /// <param name="type">The property type.</param>
        /// <returns>True if the type is a scalar.</returns>
        public static bool IsScalar(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            return type.IsValueType || type == typeof(string);
        }


        /// <summary>
        /// Converts the value to the type.
        /// </summary>
        /// <param name="value">The CSV value, which must not be the null value.</param>
        /// <param name="type">The scalar property type, a nullable type is converted to its underlying type.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="FormatException">Thrown when the value cannot be converted.</exception>
        public static object Convert(string value, Type type)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(type);

            Type actualType = Nullable.GetUnderlyingType(type) ?? type;

            if (actualType == typeof(string))
            {
                return value;
            }

            if (actualType.IsEnum)
            {
                return ConvertEnum(value, actualType);
            }

            // Use invariant culture info
            CultureInfo culture = CultureInfo.InvariantCulture;
            bool isConverted;
            object result;

            switch (Type.GetTypeCode(actualType))
            {
                case TypeCode.Boolean:
                    isConverted = bool.TryParse(value, out bool boolean);
                    result = boolean;
                    break;
                case TypeCode.Byte:
                    isConverted = byte.TryParse(value, NumberStyles.Integer, culture, out byte unsignedByte);
                    result = unsignedByte;
                    break;
                case TypeCode.Int16:
                    isConverted = short.TryParse(value, NumberStyles.Integer, culture, out short shortInteger);
                    result = shortInteger;
                    break;
                case TypeCode.UInt16:
                    isConverted = ushort.TryParse(value, NumberStyles.Integer, culture, out ushort unsignedShort);
                    result = unsignedShort;
                    break;
                case TypeCode.Int32:
                    isConverted = int.TryParse(value, NumberStyles.Integer, culture, out int integer);
                    result = integer;
                    break;
                case TypeCode.Int64:
                    isConverted = long.TryParse(value, NumberStyles.Integer, culture, out long longInteger);
                    result = longInteger;
                    break;
                case TypeCode.Double:
                    isConverted = double.TryParse(value, NumberStyles.Float, culture, out double number);
                    result = number;
                    break;
                default:
                    if (actualType == typeof(DateTimeOffset))
                    {
                        isConverted = DateTimeOffset.TryParse(value, culture, DateTimeStyles.AssumeUniversal, out DateTimeOffset date);
                        result = date;
                    }
                    else if (actualType == typeof(Guid))
                    {
                        isConverted = Guid.TryParse(value, out Guid guid);
                        result = guid;
                    }
                    else
                    {
                        throw new FormatException($"Property type '{actualType.Name}' is not supported.");
                    }

                    break;
            }

            if (isConverted is false)
            {
                throw new FormatException($"Value '{value}' cannot be converted to '{actualType.Name}'.");
            }

            return result;
        }


        /// <summary>
        /// Converts the value to an enum value by its name, numeric values and flag combinations are rejected.
        /// </summary>
        /// <param name="value">The CSV value.</param>
        /// <param name="enumType">The enum type.</param>
        /// <returns>The enum value.</returns>
        private static object ConvertEnum(string value, Type enumType)
        {
            if (Enum.GetNames(enumType).Contains(value, StringComparer.Ordinal) is false)
            {
                throw new FormatException($"Value '{value}' is not a member of enum '{enumType.Name}'.");
            }

            return Enum.Parse(enumType, value);
        }
    }
}
