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
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


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
        private ActivePower _targetActivePower;


        /// <summary>
        /// Reactive power target for the power control.
        /// </summary>
        private ReactivePower _targetReactivePower;


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
        public ActivePower TargetActivePower
        {
            get { lock (dataLock) { return _targetActivePower; } }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ReactivePower TargetReactivePower
        {
            get { lock (dataLock) { return _targetReactivePower; } }
        }



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


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract void UpdatePower(ActivePower activePower, ReactivePower reactivePower);


        /// <summary>
        /// Update the power targets in a thread safe manner.
        /// </summary>
        /// <param name="activePower">Active power target for the power control.</param>
        /// <param name="reactivePower">Reactive power target for the power control.</param>
        public void SetTargetPower(ActivePower activePower, ReactivePower reactivePower)
        {
            lock (dataLock)
            {
                _targetActivePower = activePower;
                _targetReactivePower = reactivePower;
            }
        }


        /// <summary>
        /// Register metrics at the publisher.
        /// </summary>
        protected abstract void RegisterMetrics();
    }
}
