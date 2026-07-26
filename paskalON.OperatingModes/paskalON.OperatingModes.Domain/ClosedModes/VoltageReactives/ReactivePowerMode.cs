// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives
{
    public class ReactivePowerMode : OperatingClosedModeBase
    {
        protected readonly ReactivePowerModeConfig _config;
        protected readonly ReactivePowerModeMap _map;

        public ReactivePowerMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, ReactivePowerModeConfig config,
            ReactivePowerModeMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
        }


        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
