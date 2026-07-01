using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Circuit power meter measures the electrical power output just for a specific circuit.
    /// </summary>
    /// <remarks>
    /// It is sometimes called feeder meter.
    /// </remarks>
    public class CircuitPowerMeter : PowerMeterBase
    {
        /// <summary>
        /// The circuit power meter configuration.
        /// </summary>
        private readonly CircuitPowerMeterConfig _config;


        /// <summary>
        /// Constructor of <see cref="CircuitPowerMeter"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The </param>
        public CircuitPowerMeter(ILogger logger, CircuitPowerMeterConfig config, IMetricsPublisher<AuxiliaryPowerMeter> metricsPublisher)
            : base(logger, config, (IMetricsPublisher<PowerMeterBase>)metricsPublisher)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}
