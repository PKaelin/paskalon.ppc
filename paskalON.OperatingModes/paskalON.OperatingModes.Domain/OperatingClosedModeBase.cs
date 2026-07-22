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
        /// <inheritdoc/>
        /// </summary>
        public ComplexPower ErrorAdjustment { get; protected set; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ComplexPower TargetAdjusted
        {
            get => new ComplexPower(Target.ActivePower + ErrorAdjustment.ActivePower, Target.ReactivePower + ErrorAdjustment.ReactivePower);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract Task CalculateAsync<TInput>(TInput input, CancellationToken cancellationToken = default) where TInput : class;
    }
}
