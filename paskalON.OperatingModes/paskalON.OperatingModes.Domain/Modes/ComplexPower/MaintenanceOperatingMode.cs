using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.Modes.ComplexPower
{
    public class MaintenanceOperatingMode : OperatingModeNonMeteredBase, IOperatingModeNonMetered, IExclusiveMode
    {
        public MaintenanceOperatingMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeConfig operatingModeConfigBase,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, operatingModeConfigBase, rampController, curveController)
        {
        }


    }
}
