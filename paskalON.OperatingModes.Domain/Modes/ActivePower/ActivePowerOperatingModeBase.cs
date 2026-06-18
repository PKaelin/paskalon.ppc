using Microsoft.Extensions.Logging;

namespace paskalON.OperatingModes.Domain.Modes.ActivePower
{

    public abstract class ActivePowerOperatingModeBase : OperatingModeBase
    {
        protected ActivePowerOperatingModeBase(ILogger logger, SystemConfig systemConfig) : base(logger, systemConfig)
        {
        }
    }
}
