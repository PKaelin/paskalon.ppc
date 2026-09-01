// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Domains.Contracts
{
    /// <summary>
    /// Interface for registering properties with getter functions for a specific type T.
    /// </summary>
    /// <typeparam name="T">The type of the instance to register.</typeparam>
    public interface IValueGetter<T>
    {
        /// <summary>
        /// Registers a property with the specified name, getter function and optional interval.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getter">A function to get the value of the property from an instance of T.</param>
        /// /// <remarks>
        /// Syntax func: nameof(property/field), x => x.PropertyName/x.FieldName;
        /// </remarks>
        void Register<TProperty>(string name, Func<T, TProperty> getter);
    }
}
