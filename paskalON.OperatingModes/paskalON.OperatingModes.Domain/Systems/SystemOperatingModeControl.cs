// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.Systems
{
    public class SystemOperatingModeControl
    {
        /// <summary>
        /// The current selected operating modes of the system operating mode control.
        /// </summary>
        private IReadOnlyCollection<OperatingModeBase> _operatingModes;


        /// <summary>
        /// Metrics publisher for publishing telemetry data.
        /// </summary>
        private readonly IMetricsPublisher _publisher;


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
        /// Gets or sets the current state of the system operating mode control.
        /// </summary>
        public SystemState State
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Setpoint active power is the setpoint received from the supervisory control.
        /// </summary>
        public ActivePower SetpointActivePower
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Setpoint reactive power is the setpoint received from the supervisory control.
        /// </summary>
        public ReactivePower SetpointReactivePower
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Setpoint active power actual is the setpoint after applying operating mode controls.
        /// </summary>
        public ActivePower SetpointActivePowerActual
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Setpoint reactive power actual is the setpoint after applying operating mode controls.
        /// </summary>
        public ReactivePower SetpointReactivePowerActual
        {
            get { lock (dataLock) { return field; } }
            private set { lock (dataLock) { field = value; } }
        }



        public SystemOperatingModeControl(ILogger logger, IReadOnlyCollection<OperatingModeBase> operatingModes, IMetricsPublisher publisher)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(operatingModes);
            ArgumentNullException.ThrowIfNull(publisher);

            _logger = logger;
            _operatingModes = operatingModes;
            _publisher = publisher;
            RegisterMetrics();
        }


        public async Task Calculate(CancellationToken cancellationToken)
        {
            List<OperatingModeBase> operatingModes;

            lock (dataLock)
            {
                operatingModes = _operatingModes.ToList();
            }

            foreach (OperatingModeBase operatingMode in operatingModes)
            {
                await operatingMode.CalculateAsync(cancellationToken);
            }
        }


        protected void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>()
            {
            };

            // Initialize metrics
            _publisher.Initialize(nameof(SystemOperatingModeControl), tags);
            // MetricsFactorClass1
            _publisher.Register<SystemOperatingModeControl, int>(this, nameof(State), MetricType.Gauge, x => (int)x.State, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointActivePower), MetricType.Gauge, x => x.SetpointActivePower.Watts, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointReactivePower), MetricType.Gauge, x => x.SetpointReactivePower.VoltAmperesReactive, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointActivePowerActual), MetricType.Gauge, x => x.SetpointActivePowerActual.Watts, 1);
            _publisher.Register<SystemOperatingModeControl, double>(this, nameof(SetpointReactivePowerActual), MetricType.Gauge, x => x.SetpointReactivePowerActual.VoltAmperesReactive, 1);
        }
    }
}
