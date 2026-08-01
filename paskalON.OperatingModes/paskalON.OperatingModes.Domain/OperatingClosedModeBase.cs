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
    /// <summary>
    /// Base class for all closed operating modes that defines the specific behavior and control strategy
    /// the system uses to interact with the power grid.
    /// </summary>
    public abstract class OperatingClosedModeBase : OperatingModeBase, IOperatingClosedMode
    {
        /// <summary>
        /// Error adjustment for active power, used to correct any discrepancies between the target and actual active power.
        /// </summary>
        protected ActivePower _errorAdjustmentActive = new ActivePower(0);


        /// <summary>
        /// Error adjustment for reactive power, used to correct any discrepancies between the target and actual reactive power.
        /// </summary>
        protected ReactivePower _errorAdjustmentReactive = new ReactivePower(0);


        /// <summary>
        /// Constructor of <see cref="OperatingClosedModeBase"/>
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="targetDerUnit"></param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public OperatingClosedModeBase(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, OperatingModeBaseConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, map, rampController, curveController)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ActivePower ErrorAdjustmentActive { get => _errorAdjustmentActive; }


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
        public ReactivePower ErrorAdjustmentReactive { get => _errorAdjustmentReactive; }


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
