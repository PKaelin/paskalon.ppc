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
    public class MaintenanceOperatingMode : OperatingOpenModeBase, IOperatingOpenMode, IExclusiveMode
    {
        protected readonly MaintenanceOperatingModeConfig _config;
        public DerUnit TargetDerUnit { get; init; }


        public MaintenanceOperatingMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, MaintenanceOperatingModeConfig config, DerUnit targetDerUnit,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(targetDerUnit);

            TargetDerUnit = targetDerUnit;
            _config = config;
        }


    }
}
