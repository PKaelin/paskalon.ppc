using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// External power meter measures the electrical power output outside the POI.
    /// </summary>
    public class ExternalPowerMeter : PowerMeterBase
    {
        /// <summary>
        /// External power meter configuration.
        /// </summary>
        private readonly ExternalPowerMeterConfig _config;


        /// <summary>
        /// Constructor of <see cref="ExternalPowerMeter"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The external power meter configuration.</param>
        public ExternalPowerMeter(ILogger logger, ExternalPowerMeterConfig config, IMetricsPublisher<AuxiliaryPowerMeter> metricsPublisher)
            : base(logger, config, (IMetricsPublisher<PowerMeterBase>)metricsPublisher)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}
