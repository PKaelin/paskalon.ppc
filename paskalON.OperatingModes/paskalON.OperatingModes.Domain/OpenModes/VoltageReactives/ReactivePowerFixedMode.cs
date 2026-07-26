// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.OpenModes.VoltageReactives
{
    public class ReactivePowerFixedMode : OperatingOpenModeBase
    {
        protected readonly ReactivePowerFixedModeConfig _config;
        protected readonly ReactivePowerFixedModeMap _map;

        public ReactivePowerFixedMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, ReactivePowerFixedModeConfig config,
            ReactivePowerFixedModeMap map, IRampController rampController, ICurveController? curveController)
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
