using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    /// <summary>
    /// Auxiliary power meter measures the electrical energy consumed by the plant's own supporting infrastructure.
    /// </summary>
    /// <remarks>
    /// Pumps & Fans: Boiler feed pumps, cooling water pumps, and forced-draft fans.
    /// Facility Utilities: Control room HVAC, lighting, and fire protection systems.
    /// Handling Systems: Coal conveyors, ash removal, and scrubbers.
    /// </remarks>
    public class AuxiliaryPowerMeter : PowerMeterBase
    {
        /// <summary>
        /// Auxiliary power meter configuration.
        /// </summary>
        private readonly AuxiliaryPowerMeterConfig _config;


        /// <summary>
        /// Constructor of <see cref="AuxiliaryPowerMeter"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The auxiliary power meter configuration.</param>
        public AuxiliaryPowerMeter(ILogger logger, AuxiliaryPowerMeterConfig config) : base(logger, config)
        {
            _config = config;
        }
    }
}
