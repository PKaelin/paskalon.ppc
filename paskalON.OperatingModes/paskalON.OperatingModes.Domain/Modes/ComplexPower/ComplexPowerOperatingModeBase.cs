using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.Modes.ActivePower
{

    public abstract class ComplexPowerOperatingModeBase : OperatingModeBase
    {
        protected ComplexPowerOperatingModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeConfig operatingModeConfigBase,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, operatingModeConfigBase, rampController, curveController)
        {
        }
    }
}
