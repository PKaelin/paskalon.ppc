// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.C37118;

namespace paskalON.Devices.Application.Factories
{
    /// <summary>
    /// C37 device factory interface definition.
    /// </summary>
    public interface IC37DeviceFactory
    {
        /// <summary>
        /// Create an IC37Dataface and IC37Client.
        /// </summary>
        /// <returns>The IC37Dataface and IC37Client implementation.</returns>
        (IC37Dataface Dataface, IC37Client Client) Create(C37Config config);
    }
}
