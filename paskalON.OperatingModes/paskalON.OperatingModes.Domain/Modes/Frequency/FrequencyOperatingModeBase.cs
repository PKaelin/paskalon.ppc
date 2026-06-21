using Microsoft.Extensions.Logging;

namespace paskalON.OperatingModes.Domain.Modes.Frequency
{
    public abstract class FrequencyOperatingModeBase : OperatingModeBase
    {
        protected FrequencyOperatingModeBase(ILogger logger, SystemConfig systemConfig) : base(logger, systemConfig)
        {
        }
    }
}
