using Microsoft.Extensions.Logging;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Base class for all operating modes.
    /// </summary>
    public abstract class OperatingModeBase // TODO: IOperatingModeBase
    {

        protected readonly ILogger _logger;

        protected readonly SystemConfig _systemConfig;

        public OperatingModeBase(ILogger logger, SystemConfig systemConfig)
        {
            _logger = logger;
            _systemConfig = systemConfig;
        }
    }
}
