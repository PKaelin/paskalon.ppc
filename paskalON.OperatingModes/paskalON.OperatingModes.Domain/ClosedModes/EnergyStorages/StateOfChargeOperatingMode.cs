// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.ClosedModes.EnergyStorages
{
    public class StateOfChargeOperatingMode : OperatingModeBase
    {
        protected StateOfChargeOperatingMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeBaseConfig operatingModeConfigBase,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, operatingModeConfigBase, rampController, curveController)
        {
        }
    }
}
