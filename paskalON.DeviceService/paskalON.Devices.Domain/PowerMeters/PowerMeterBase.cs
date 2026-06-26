

using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Ders;

namespace paskalON.Devices.Domain.PowerMeters
{
    public class PowerMeterBase : DerBase
    {
        private readonly PowerMeter _powerMeterConfig;

        public PowerMeterBase(ILogger logger, PowerMeter powerMeterConfig) : base(logger, powerMeterConfig)
        {
            _powerMeterConfig = powerMeterConfig;
        }
    }
}
