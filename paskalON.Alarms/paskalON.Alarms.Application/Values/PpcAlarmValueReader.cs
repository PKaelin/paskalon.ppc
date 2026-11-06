// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Collections.Concurrent;
using System.Reflection;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Messaging.Sequences;

namespace paskalON.Alarms.Application.Values
{
    /// <summary>
    /// Reads the alarm relevant values of published data generically, without references to the publisher's domain.
    /// </summary>
    /// <remarks>
    /// Only the public properties of the root object are read:
    /// - Boolean properties as true = 1 and false = 0.
    /// - Numeric properties converted to double.
    /// - Dictionaries of string/boolean, every entry as [Dictionary].[Key], e.g. FaultStates.OverTemperature.
    /// Null values and all other property types (strings, arrays, nested objects, etc.) are ignored.
    /// The sequence properties of a sequenced message (<see cref="ISequencedMessage"/>) are ignored.
    /// </remarks>
    public static class PpcAlarmValueReader
    {
        /// <summary>
        /// Readable public instance properties per type, so that the reflection is only done once per type.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesByType = new ConcurrentDictionary<Type, PropertyInfo[]>();


        /// <summary>
        /// Numeric types that are converted to double.
        /// </summary>
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
            typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)
        };


        /// <summary>
        /// Properties of a sequenced message that are not alarm relevant.
        /// </summary>
        private static readonly HashSet<string> SequencedMessageProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            nameof(ISequencedMessage.SourceInstance), nameof(ISequencedMessage.Sequence), nameof(ISequencedMessage.Timestamp)
        };


        /// <summary>
        /// Reads the alarm relevant values of the published data.
        /// </summary>
        /// <param name="data">Published data, e.g. a PcsDetailDto.</param>
        /// <returns>Values by property or dictionary entry name.</returns>
        public static Dictionary<string, double> Read(object data)
        {
            ArgumentNullException.ThrowIfNull(data);

            Dictionary<string, double> values = new Dictionary<string, double>(StringComparer.Ordinal);
            PropertyInfo[] properties = PropertiesByType.GetOrAdd(data.GetType(), GetReadableProperties);

            foreach (PropertyInfo property in properties)
            {
                object? propertyValue = property.GetValue(data);

                switch (propertyValue)
                {
                    case null:
                        break;
                    case bool booleanValue:
                        values[property.Name] = ToDouble(booleanValue);
                        break;
                    case IEnumerable<KeyValuePair<string, bool>> dictionary:
                        AddDictionaryEntries(values, property.Name, dictionary);
                        break;
                    default:
                        if (NumericTypes.Contains(propertyValue.GetType()))
                        {
                            values[property.Name] = Convert.ToDouble(propertyValue);
                        }

                        break;
                }
            }

            return values;
        }


        /// <summary>
        /// Adds the entries of a dictionary as [Dictionary].[Key].
        /// </summary>
        private static void AddDictionaryEntries(Dictionary<string, double> values, string dictionaryName, IEnumerable<KeyValuePair<string, bool>> dictionary)
        {
            foreach (KeyValuePair<string, bool> entry in dictionary)
            {
                values[PpcAlarmKeyValue.CreateDictionaryEntryName(dictionaryName, entry.Key)] = ToDouble(entry.Value);
            }
        }


        /// <summary>
        /// Converts a boolean to a double, true = 1 and false = 0.
        /// </summary>
        private static double ToDouble(bool value)
        {
            return value ? 1 : 0;
        }


        /// <summary>
        /// Gets the readable public instance properties of a type, without indexers and without the message sequence.
        /// </summary>
        /// <remarks>
        /// The sequence of a sequenced message changes with every message, it would evaluate every message again.
        /// </remarks>
        private static PropertyInfo[] GetReadableProperties(Type type)
        {
            bool isSequencedMessage = typeof(ISequencedMessage).IsAssignableFrom(type);

            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
                .Where(property => isSequencedMessage is false || SequencedMessageProperties.Contains(property.Name) is false)
                .ToArray();
        }
    }
}
