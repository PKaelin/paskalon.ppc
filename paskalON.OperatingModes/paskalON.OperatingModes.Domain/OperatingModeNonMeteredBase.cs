using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain
{
    public abstract class OperatingModeNonMeteredBase : OperatingModeBase, IOperatingModeNonMetered
    {
        public OperatingModeNonMeteredBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeConfig operatingModeConfigBase,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, operatingModeConfigBase, rampController, curveController)
        {
        }

        public Task CalculateAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
