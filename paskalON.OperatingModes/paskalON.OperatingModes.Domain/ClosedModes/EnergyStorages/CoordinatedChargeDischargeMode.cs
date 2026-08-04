// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.EnergyStorages;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.ClosedModes.EnergyStorages
{
    public class CoordinatedChargeDischargeMode : OperatingClosedModeBase
    {
        protected readonly CoordinatedChargeDischargeModeConfig _config;
        protected readonly CoordinatedChargeDischargeModeMap _map;

        public CoordinatedChargeDischargeMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, CoordinatedChargeDischargeModeConfig config,
            CoordinatedChargeDischargeModeMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
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
