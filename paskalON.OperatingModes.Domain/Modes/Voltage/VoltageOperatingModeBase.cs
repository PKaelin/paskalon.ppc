using Microsoft.Extensions.Logging;

namespace paskalON.OperatingModes.Domain.Modes.Voltage
{
    public abstract class VoltageOperatingModeBase : OperatingModeBase
    {
        protected VoltageOperatingModeBase(ILogger logger, SystemConfig systemConfig) : base(logger, systemConfig)
        {
        }
    }
}
