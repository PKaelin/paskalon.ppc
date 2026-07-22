// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.OpenModes.EnergyResources
{
    public class MaximumPowerPointTrackingMode : OperatingOpenModeBase
    {
        protected readonly MaximumPowerPointTrackingModeConfig _config;


        public MaximumPowerPointTrackingMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, MaximumPowerPointTrackingModeConfig config,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }


        public override Task CalculateAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
