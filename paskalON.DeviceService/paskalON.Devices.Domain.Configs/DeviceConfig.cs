// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Root database context configuration class for the all devices and structure.
    /// </summary>
    public class DeviceConfig
    {
        /// <summary>
        /// Root instance of device configurations.
        /// </summary>
        public required DerConfig DerConfig { get; set; }
    }
}
