// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Ders;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.OpenModes
{
    public class MaintenanceMode : OperatingOpenModeBase, IOperatingOpenMode, IExclusiveMode
    {
        protected readonly MaintenanceModeConfig _config;

        public DerUnit TargetDerUnit { get; init; }


        public MaintenanceMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, MaintenanceModeConfig config, DerUnit targetDerUnit,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(targetDerUnit);

            TargetDerUnit = targetDerUnit;
            _config = config;
        }


        public override Task CalculateAsync<TInput>(TInput input, CancellationToken cancellationToken = default)
        {
            if (input is not MaintenanceModeMap map)
            {
                throw new ArgumentException(nameof(input));
            }

            throw new NotImplementedException();
        }
    }
}
