// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.Modes.StateOfCharge
{
    public abstract class StateOfChargeOperatingModeBase : OperatingModeBase
    {
        protected StateOfChargeOperatingModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeConfig operatingModeConfigBase,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, operatingModeConfigBase, rampController, curveController)
        {
        }
    }
}
