// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.DummyModes
{
    /// <summary>
    /// Dummy additive active mode that adds a configurable active power delta to the base active power.
    /// </summary>
    /// <remarks>
    /// The delta goes through the power control like in a real additive mode, so the system operating mode control transitions
    /// it in and out with the ramp of this mode.
    /// </remarks>
    internal sealed class DummyAdditiveActiveMode : OperatingOpenModeBase, IAdditiveActiveMode
    {
        /// <summary>
        /// Input mapping class for signals.
        /// </summary>
        private readonly OperatingModeBaseMap _map;


        /// <summary>
        /// Gets or sets the active power delta added to the base active power.
        /// </summary>
        public ActivePower DeltaActivePower { get; set; }


        /// <summary>
        /// Constructor of <see cref="DummyAdditiveActiveMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <param name="publisher">The metrics publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="deltaActivePower">The active power delta added to the base active power.</param>
        public DummyAdditiveActiveMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig,
            DummyAdditiveActiveModeConfig config, OperatingModeBaseMap map, IRampController rampController, ActivePower deltaActivePower)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, null)
        {
            ArgumentNullException.ThrowIfNull(map);

            _map = map;
            DeltaActivePower = deltaActivePower;
        }


        /// <inheritdoc/>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            ArgumentNullException.ThrowIfNull(signals);

            ActivePower delta = ActiveControl!.Calculate(DeltaActivePower, _map.AvailableActivePower.Invoke());
            signals.AdditiveActivePowerDelta += delta;

            return signals;
        }
    }
}
