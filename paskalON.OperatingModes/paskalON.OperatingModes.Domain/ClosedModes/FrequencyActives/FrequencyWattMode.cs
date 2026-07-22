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
    public class FrequencyWattMode : OperatingClosedModeBase
    {
        protected readonly FrequencyWattModeConfig _config;


        public FrequencyWattMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, FrequencyWattModeConfig config,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }


        public override Task CalculateAsync<TInput>(TInput input, CancellationToken cancellationToken = default)
        {
            if (input is not FrequencyWattModeMap map)
            {
                throw new ArgumentException(nameof(input));
            }

            throw new NotImplementedException();
        }
    }
}
