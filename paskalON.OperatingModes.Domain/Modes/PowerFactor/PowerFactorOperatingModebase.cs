using Microsoft.Extensions.Logging;

namespace paskalON.OperatingModes.Domain.Modes.PowerFactor
{
    public abstract class PowerFactorOperatingModebase : OperatingModeBase
    {
        protected PowerFactorOperatingModebase(ILogger logger, SystemConfig systemConfig) : base(logger, systemConfig)
        {
        }
    }
}
