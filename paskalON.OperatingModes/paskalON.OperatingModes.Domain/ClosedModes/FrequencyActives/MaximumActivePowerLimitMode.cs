// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives
{
    public class MaximumActivePowerLimitMode : OperatingClosedModeBase
    {
        protected readonly MaximumActivePowerLimitModeConfig _config;


        public MaximumActivePowerLimitMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, MaximumActivePowerLimitModeConfig config,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }


        public override async Task CalculateAsync<TInput>(TInput input, CancellationToken cancellationToken) where TInput : class
        {
            if (input is not MaximumActivePowerLimitModeMap map)
            {
                throw new ArgumentException(nameof(input));
            }

            throw new NotImplementedException();
        }
    }
}
