using Microsoft.Extensions.Logging;

namespace paskalON.OperatingModes.Domain.Modes.StateOfCharge
{
    public abstract class StateOfChargeOperatingModeBase : OperatingModeBase
    {
        protected StateOfChargeOperatingModeBase(ILogger logger, SystemConfig systemConfig) : base(logger, systemConfig)
        {
        }
    }
}
