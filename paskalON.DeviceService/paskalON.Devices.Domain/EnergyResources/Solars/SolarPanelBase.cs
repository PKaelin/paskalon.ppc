

using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.EnergyResources.Solars
{
    public abstract class SolarPanelBase : DerBase
    {
        private readonly SolarPanelConfig _config;

        public DerSolarUnit SolarUnit { get; private set; }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode => SolarUnit.IsInMaintenanceMode;


        protected SolarPanelBase(ILogger logger, SolarPanelConfig config, DerSolarUnit derSolarUnit) : base(logger, config)
        {
            _config = config;
            SolarUnit = derSolarUnit;
        }
    }
}
