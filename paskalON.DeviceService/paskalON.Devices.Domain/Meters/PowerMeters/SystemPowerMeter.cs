using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    /// <summary>
    /// System power meter measures the electrical power output at the point of interconnection (POI).
    /// </summary>
    /// <remarks>
    /// POI is where the plant connects to to the local electrical grid.
    /// </remarks>
    public class SystemPowerMeter : PowerMeterBase
    {
        /// <summary>
        /// System power meter configuration.
        /// </summary>
        private readonly SystemPowerMeterConfig _config;


        /// <summary>
        /// Constructor of <see cref="SystemPowerMeter"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The system power meter config.</param>
        public SystemPowerMeter(ILogger logger, SystemPowerMeterConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
