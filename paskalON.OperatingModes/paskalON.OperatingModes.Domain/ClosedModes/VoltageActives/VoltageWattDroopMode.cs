// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageActives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.ClosedModes.VoltageActives
{
    public class VoltageWattDroopMode : OperatingClosedModeBase
    {
        protected readonly VoltageWattDroopModeConfig _config;
        protected readonly VoltageWattDroopModeMap _map;

        public VoltageWattDroopMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, VoltageWattDroopModeConfig config,
            VoltageWattDroopModeMap map, IRampController rampController, ICurveController? curveController)
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
