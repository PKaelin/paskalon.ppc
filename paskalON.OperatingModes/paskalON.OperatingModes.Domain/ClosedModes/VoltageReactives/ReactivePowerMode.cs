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


        public ReactivePowerMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, ReactivePowerModeConfig config,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }


        public override Task CalculateAsync<TInput>(TInput input, CancellationToken cancellationToken = default)
        {
            if (input is not ReactivePowerModeMap map)
            {
                throw new ArgumentException(nameof(input));
            }

            throw new NotImplementedException();
        }
    }
}
