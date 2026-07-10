// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// Interface for registering C37 registers.
    /// </summary>
    public interface IC37Register
    {
        /// <summary>
        /// Register a C37 entry.
        /// </summary>
        /// <typeparam name="TDevice">The device type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="instance">Instance the property is member of.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="setter">Setter action.</param>
        void Register<TDevice, TProperty>(TDevice instance, string name, Action<TDevice, TProperty> setter);
    }
}
