// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives
{
    public class PowerFactorMode : OperatingClosedModeBase, IExclusiveMode, IStackable
    {
        protected readonly PowerFactorModeConfig _config;
        protected readonly PowerFactorModeMap _map;
        public double PowerFactor { get; set; }

        public PowerFactorMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, PowerFactorModeConfig config,
            PowerFactorModeMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            RegisterMetrics();
        }


        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
