using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Domain.Configs.Ders
{
    /// <summary>
    /// Solar DER unit configuration.
    /// </summary>
    public class DerSolarUnitConfig : DerUnitConfig
    {
        /// <summary>
        /// Child relationship to PowerConversionSystemConfigId.
        /// </summary>
        public PowerConversionSystemConfig? PowerConversionSystemConfig { get; set; }


        /// <summary>
        /// Child relationship to SolarPanelConfig. 
        /// At the moment we assume we dont control solar panels. Hence only one solar panel config is referenced.
        /// </summary>
        public SolarPanelConfig? SolarPanelConfig { get; set; }

    }
}
