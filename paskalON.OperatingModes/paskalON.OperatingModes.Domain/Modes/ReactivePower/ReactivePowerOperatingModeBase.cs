using Microsoft.Extensions.Logging;

namespace paskalON.OperatingModes.Domain.Modes.ReactivePower
{
    public abstract class ReactivePowerOperatingModeBase : OperatingModeBase
    {
        protected ReactivePowerOperatingModeBase(ILogger logger, SystemConfig systemConfig) : base(logger, systemConfig)
        {
        }
    }
}
