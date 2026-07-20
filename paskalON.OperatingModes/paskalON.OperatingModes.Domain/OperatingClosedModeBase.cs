// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain
{
    public abstract class OperatingClosedModeBase : OperatingModeBase, IOperatingClosedMode
    {
        public OperatingClosedModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeBaseConfig config,
            IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, rampController, curveController)
        {
        }


        /// <summary>
        /// Error adjustment calculated from input and used to make adjustments in real time.
        /// </summary>
        public ComplexPower ErrorAdjustment { get; protected set; }


        /// <summary>
        /// Gets the adjusted complex power target for the operating mode.
        /// </summary>
        public ComplexPower TargetAdjusted
        {
            get => new ComplexPower(Target.ActivePower + ErrorAdjustment.ActivePower, Target.ReactivePower + ErrorAdjustment.ReactivePower);
        }


        /// <summary>
        /// Calculates the operating modes power target.
        /// </summary>
        public Task CalculateAsync(CancellationToken cancellationToken, string todo_actual_meter)
        {
            // TODO: implement actual meter
            throw new NotImplementedException();
        }
    }
}
