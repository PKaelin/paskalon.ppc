// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.DemoSimple.Devices.Data
{
    /// <summary>
    /// Create domain data for the service here.
    /// </summary>
    static class ServiceData
    {
        /// <summary>
        /// Initial changed by user.
        /// </summary>
        private const string _changedBy = "System Init";


        /// <summary>
        /// Main method to create the service data.
        /// </summary>
        /// <param name="context">DB context interface.</param>
        public static void Create(IDeviceServiceContext context)
        {
            // TODO: Add service data
        }
    }
}
