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
        protected readonly MaintenanceModeMap _map;
        public DerUnit TargetDerUnit { get; init; }


        public MaintenanceMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, MaintenanceModeConfig config, DerUnit targetDerUnit,
            MaintenanceModeMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(targetDerUnit);

            TargetDerUnit = targetDerUnit;
            _config = config;
            _map = map;
        }


        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
