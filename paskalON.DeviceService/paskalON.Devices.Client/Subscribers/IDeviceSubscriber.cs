// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Client.Subscribers
{
    /// <summary>
    /// Device subscriber definition.
    /// </summary>
    public interface IDeviceSubscriber
    {
        /// <summary>
        /// Gets a json string, deserializes the string into a defined type and
        /// calls the definition update method on that registered type.
        /// </summary>
        /// <param name="json">The json string representing the defined type.</param>
        void UpdateDefinition(string json);


        /// <summary>
        /// Gets a json string, deserializes the string into a defined type and
        /// calls the core update method on that registered type.
        /// </summary>
        /// <param name="json">The json string representing the defined type.</param>
        void UpdateCore(string json);


        /// <summary>
        /// Gets a json string, deserializes the string into a defined type and
        /// calls the detail update method on that registered type.
        /// </summary>
        /// <param name="json">The json string representing the defined type.</param>
        void UpdateDetail(string json);
    }
}
