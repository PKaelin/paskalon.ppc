// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.C37s
{
    /// <summary>
    /// Interface for C37 register entries.
    /// </summary>
    public interface IC37RegisterEntry
    {
        /// <summary>
        /// Instance to update the value for.
        /// </summary>
        object Instance { get; }


        /// <summary>
        /// Register entry name.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// C37 signal type.
        /// </summary>
        C37SignalType SignalType { get; }


        /// <summary>
        /// Update the property on the instance.
        /// </summary>
        /// <param name="value">Value for update.</param>
        void Update(object value);
    }
}
