// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;

namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// Solar DER unit configuration.
    /// </summary>
    public class DerSolarUnitConfig : DerUnitConfig
    {
        /// <summary>
        /// Child relationship to SolarPanelConfig Id. 
        /// At the moment we assume we dont control solar panels. Hence only one solar panel config is referenced.
        /// </summary>
        public int SolarPanelConfigId { get; set; }

        /// <summary>
        /// Child relationship to SolarPanelConfig. 
        /// At the moment we assume we dont control solar panels. Hence only one solar panel config is referenced.
        /// </summary>
        public SolarPanelConfig SolarPanelConfig { get; set; } = null!;

    }
}
