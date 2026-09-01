// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Domains.Contracts
{
    /// <summary>
    /// Interface for registering properties with setter functions for a specific type T.
    /// </summary>
    /// <typeparam name="T">The type of the instance to register.</typeparam>
    public interface IValueSetter<T>
    {
        /// <summary>
        /// Registers a property with the specified name, setter function.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="setter">A function to set the value of the property on an instance of T.</param>
        /// <remarks>
        /// Syntax action: (x, value) => x.PropertyName = value;
        /// </remarks>
        void Register<TProperty>(string name, Action<T, TProperty> setter);
    }
}