using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
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
        public CircuitPowerMeter(ILogger logger, CircuitPowerMeterConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
