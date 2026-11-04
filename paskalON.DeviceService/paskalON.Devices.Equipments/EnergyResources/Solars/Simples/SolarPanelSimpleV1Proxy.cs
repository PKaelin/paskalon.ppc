using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.EnergyResources.Solars.Simples
{
    /// <summary>
    /// Solar Panel Simple is a basic implementation of the solar panel base class <see cref="SolarPanelBase"/>.
    /// </summary>
    public class SolarPanelSimpleV1Proxy : SolarPanelBase
    {
        /// <summary>
        /// Constructor of <see cref="SolarPanelSimpleV1Proxy"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The battery bank configuration.</param>
        /// <param name="derSolarUnit">Der solar unit configuration.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public SolarPanelSimpleV1Proxy(ILogger logger, SolarPanelConfig config, DerSolarUnit derSolarUnit, IMetricsPublisher publisher,
            IDataface dataface) : base(logger, config, derSolarUnit, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(dataface);
        }

        protected override void RegisterDataface()
        {
            // We dont communicate with solar panels yet so leave this implementation for now.
        }
    }
}
