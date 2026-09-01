// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs;
using paskalON.Telemetry;

namespace paskalON.PowerControls.Domain
{
    public abstract class PowerControlBase : IPowerControl
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// Power control base configuration.
        /// </summary>
        private readonly PowerControlBaseConfig _config;


        /// <summary>
        /// Power control base map.
        /// </summary>
        private readonly PowerControlBaseMap _map;


        /// <summary>
        /// Active power target for the power control.
        /// </summary>
        /// <remarks>
        /// For performance this is a class variable.
        /// </remarks>
        protected ActivePower _targetActivePower = new ActivePower(0);


        /// <summary>
        /// Reactive power target for the power control.
        /// </summary>
        /// <remarks>
        /// For performance this is a class variable.
        /// </remarks>
        protected ReactivePower _targetReactivePower = new ReactivePower(0);


        /// <summary>
        /// Interface for registering and publishing metrics for a given type T.
        /// </summary>
        public IMetricsPublisher MetricsPublisher { get; init; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsActive { get => _config.IsActive; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsEnabled { get => _config.IsEnabled; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ref ActivePower TargetActivePower { get => ref _targetActivePower; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ref ReactivePower TargetReactivePower { get => ref _targetReactivePower; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract void UpdatePower(ActivePower activePower, ReactivePower reactivePower);


        public PowerControlBase(ILogger logger, PowerControlBaseConfig config, PowerControlBaseMap map, IMetricsPublisher publisher)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(publisher);

            _logger = logger;
            _config = config;
            _map = map;
            MetricsPublisher = publisher;
        }
    }
}
