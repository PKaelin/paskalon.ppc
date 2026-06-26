using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.PowerConversionSystems;

namespace paskalON.Devices.Domain.Ders
{
    public class DerSolarUnit : DerUnit
    {
        private DerSolarUnitConfig _config;


        public PowerConversionSystemBase? PowerConversionSystem { get; set; }


        /// <summary>
        /// In theory a list of solar panels but at this point we dont control the panels.
        /// We still use this class to calculate the solar configurations.
        /// </summary>
        public SolarPanelBase? Solar { get; set; }


        public DerSolarUnit(ILogger logger, DerSolarUnitConfig config, DerCircuit derCircuit) : base(logger, config, derCircuit)
        {
            _config = config;
        }
    }
}
