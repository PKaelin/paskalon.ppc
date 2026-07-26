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
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, map, rampController, curveController)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower ErrorAdjustmentActive { get; protected set; } = new ActivePower(0);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower TargetAdjustedActive
        {
            get => new ActivePower(TargetActivePower.Watts + ErrorAdjustmentActive.Watts);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower ErrorAdjustmentReactive { get; protected set; } = new ReactivePower(0);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower TargetAdjustedReactive
        {
            get => new ReactivePower(TargetReactivePower.VoltAmperesReactive + ErrorAdjustmentReactive.VoltAmperesReactive);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract Task CalculateAsync(CancellationToken cancellationToken = default);
    }
}
