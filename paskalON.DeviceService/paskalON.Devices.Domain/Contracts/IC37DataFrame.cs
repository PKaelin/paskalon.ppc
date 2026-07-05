// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Contracts
{
    /// <summary>
    /// Interface to register data points for a C37 data frame.
    /// </summary>
    /// <typeparam name="T">The type of of instance to register.</typeparam>
    public interface IC37DataFrame<T>
    {
        /// <summary>
        /// Registers a property with the specified name, getter function and optional interval.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="setter">A function to set the value of the property on an instance of T.</param>
        /// /// <remarks>
        /// Syntax func: nameof(property/field), x => x.PropertyName/x.FieldName;
        /// </remarks>
        void Register<TProperty>(string? name, Action<T, TProperty> setter);
    }
}
