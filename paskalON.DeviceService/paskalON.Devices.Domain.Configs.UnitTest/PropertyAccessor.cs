// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Reflection;

namespace paskalON.Devices.Domain.Configs.UnitTest
{
    /// <summary>
    /// Sets and gets properties by name for data-driven tests without wrapping setter exceptions.
    /// </summary>
    internal static class PropertyAccessor
    {
        /// <summary>
        /// Invokes the setter of the named property.
        /// </summary>
        /// <param name="target">The instance to modify.</param>
        /// <param name="propertyName">The public property name.</param>
        /// <param name="value">The value to assign. Converted to the property type.</param>
        internal static void Set(object target, string propertyName, object value)
        {
            PropertyInfo property = GetProperty(target, propertyName);
            object converted = Convert.ChangeType(value, property.PropertyType);

            property.SetMethod!.Invoke(target, BindingFlags.DoNotWrapExceptions, null, new object[] { converted }, null);
        }


        /// <summary>
        /// Reads the value of the named property.
        /// </summary>
        /// <param name="target">The instance to read.</param>
        /// <param name="propertyName">The public property name.</param>
        /// <returns>The property value.</returns>
        /// The property value.
        /// </returns>
        internal static object? Get(object target, string propertyName)
        {
            return GetProperty(target, propertyName).GetValue(target);
        }


        /// <summary>
        /// Gets the PropertyInfo for the named property.
        /// </summary>
        /// <param name="target">The instance to read.</param>
        /// <param name="propertyName">The public property name.</param>
        /// <returns>The PropertyInfo for the named property.</returns>
        private static PropertyInfo GetProperty(object target, string propertyName)
        {
            PropertyInfo? property = target.GetType().GetProperty(propertyName);
            ArgumentNullException.ThrowIfNull(property, propertyName);

            return property;
        }
    }
}
