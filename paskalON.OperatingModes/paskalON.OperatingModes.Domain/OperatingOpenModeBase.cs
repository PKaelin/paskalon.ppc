// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain
{
    public abstract class OperatingOpenModeBase : OperatingModeBase, IOperatingOpenMode
    {
        public OperatingOpenModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeBaseConfig config,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
        }


        /// <summary>
        /// Calculates the operating modes power target.
        /// </summary>
        public abstract Task CalculateAsync(CancellationToken cancellationToken);
    }
}
