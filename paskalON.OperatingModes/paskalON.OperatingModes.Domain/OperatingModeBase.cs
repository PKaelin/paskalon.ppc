// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Base class for all operating modes that defines the specific behavior and control strategy
    /// the system uses to interact with the power grid.
    /// </summary>
    /// <remarks>
    /// Concerns are separated as follows:
    /// Loop type (open/closed) -> inheritance (<see cref="OperatingOpenModeBase"/>, <see cref="OperatingClosedModeBase"/>).
    /// Power axis (active/reactive) -> composition (<see cref="ActiveControl"/>, <see cref="ReactiveControl"/>), created from the classification.
    /// Classification -> interfaces (<see cref="IBaseActiveMode"/>, <see cref="IAdditiveActiveMode"/>, etc.).
    /// </remarks>
    public abstract class OperatingModeBase : IOperatingMode
    {
        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// Time provider for system time abstraction.
        /// </summary>
        protected readonly TimeProvider _timeProvider;


        /// <summary>
        /// Metrics publisher of this operating mode.
        /// </summary>
        /// <remarks>
        /// The publisher should only publish metrics if this operating mode is not disabled.
        /// </remarks>
        protected readonly IMetricsPublisher _metricsPublisher;


        /// <summary>
        /// System configuration.
        /// </summary>
        protected readonly SystemConfig _systemConfig;


        /// <summary>
        /// Operating mode base configuration.
        /// </summary>
        private readonly OperatingModeBaseConfig _config;


        /// <inheritdoc/>
        public string Name { get => _config.Name; }


        /// <inheritdoc/>
        public OperatingModeClassification Classification { get; }


        /// <inheritdoc/>
        public bool IsActivated { get; private set; }


        /// <inheritdoc/>
        public DateTimeOffset LastActivated { get; private set; } = DateTimeOffset.MinValue;


        /// <inheritdoc/>
        public OperatingModeState State
        {
            get
            {
                OperatingModeState? active = ActiveControl?.State;
                OperatingModeState? reactive = ReactiveControl?.State;

                if (Matches(active, reactive, OperatingModeState.Enabled))
                {
                    return OperatingModeState.Enabled;
                }

                if (Matches(active, reactive, OperatingModeState.Enabling))
                {
                    return OperatingModeState.Enabling;
                }

                if (Matches(active, reactive, OperatingModeState.RampingToEnabled))
                {
                    return OperatingModeState.RampingToEnabled;
                }

                if (Matches(active, reactive, OperatingModeState.RampingToDisabled))
                {
                    return OperatingModeState.RampingToDisabled;
                }

                return OperatingModeState.Disabled;
            }
        }


        /// <inheritdoc/>
        public PowerControl<ActivePower>? ActiveControl { get; }


        /// <inheritdoc/>
        public PowerControl<ReactivePower>? ReactiveControl { get; }


        /// <inheritdoc/>
        public ActivePower ActivePowerContribution { get; private set; } = new ActivePower(0);


        /// <inheritdoc/>
        public ReactivePower ReactivePowerContribution { get; private set; } = new ReactivePower(0);


        /// <inheritdoc/>
        public ICurveController? CurveController { get; }



        /// <summary>
        /// Constructor of <see cref="OperatingModeBase"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode base configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        protected OperatingModeBase(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, OperatingModeBaseConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController = null)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(systemConfig);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(rampController);

            _logger = logger;
            _timeProvider = timeProvider;
            _metricsPublisher = publisher;
            _systemConfig = systemConfig;
            _config = config;
            CurveController = curveController;
            Classification = OperatingModeClassificationExtensions.Resolve(this);

            if (Classification.HasActive())
            {
                ActiveControl = new PowerControl<ActivePower>(logger, Name, rampController, PowerControlLimits.CreateActive(config, systemConfig), PowerConverters.Active);
            }

            if (Classification.HasReactive())
            {
                // Each axis needs its own ramp, only copy the ramp controller if it is already used by the active axis
                IRampController reactiveRampController = ActiveControl is null ? rampController : rampController.ShallowCopy();
                ReactiveControl = new PowerControl<ReactivePower>(logger, Name, reactiveRampController, PowerControlLimits.CreateReactive(config, systemConfig), PowerConverters.Reactive);
            }

            _logger.LogInformation("{Name} operating mode created. Classification: {Classification}", Name, Classification);
        }


        /// <inheritdoc/>
        public abstract OperatingModeSignals Calculate(OperatingModeSignals signals);


        /// <summary>
        /// Activates the operating mode when it is committed to the stack or the override slot.
        /// </summary>
        /// <remarks>
        /// Called by the system operating mode control. Stack operating modes establish their target immediately because the
        /// system operating mode control transitions the applied target. Override operating modes ramp from their current target.
        /// </remarks>
        internal virtual void Activate()
        {
            PowerControlStart start = this is IStackableMode ? PowerControlStart.AtTargetSetpoint : PowerControlStart.RampFromCurrentTarget;
            IsActivated = true;
            LastActivated = _timeProvider.GetUtcNow();
            SetContribution(new ActivePower(0), new ReactivePower(0));
            ActiveControl?.Activate(start);
            ReactiveControl?.Activate(start);

            _logger.LogInformation("{Name} operating mode activated.", Name);
        }


        /// <summary>
        /// Deactivates the operating mode immediately when it is removed from the stack or the override slot.
        /// </summary>
        /// <remarks>
        /// Called by the system operating mode control which transitions the applied target.
        /// </remarks>
        internal virtual void Deactivate()
        {
            IsActivated = false;
            SetContribution(new ActivePower(0), new ReactivePower(0));
            ActiveControl?.Deactivate();
            ReactiveControl?.Deactivate();
            _logger.LogInformation("{Name} operating mode deactivated.", Name);
        }


        /// <summary>
        /// Sets the change of the final power caused by this operating mode in the last calculation cycle.
        /// </summary>
        /// <param name="activePower">The active power contribution.</param>
        /// <param name="reactivePower">The reactive power contribution.</param>
        internal void SetContribution(ActivePower activePower, ReactivePower reactivePower)
        {
            ActivePowerContribution = activePower;
            ReactivePowerContribution = reactivePower;
        }


        /// <summary>
        /// Register metrics at the publisher.
        /// </summary>
        /// <remarks>
        /// Axis metrics are only registered for the power axes this operating mode has.
        /// </remarks>
        protected virtual void RegisterMetrics()
        {
            Dictionary<string, object?> tags = new Dictionary<string, object?>
            {
                { "Name", Name },
                { "Classification", Classification.ToString() }
            };

            if (this is IOverrideMode overrideMode && overrideMode.TargetDerUnit is not null)
            {
                tags.Add("DerUnit", overrideMode.TargetDerUnit);
            }

            // Initialize metrics
            _metricsPublisher.Initialize("OperatingMode", tags);
            // MetricsFactorClass2
            _metricsPublisher.Register<OperatingModeBase, int>(this, nameof(State), MetricType.Gauge, x => (int)x.State, _config.MetricsFactorClass2);
            _metricsPublisher.Register<OperatingModeBase, int>(this, nameof(IsActivated), MetricType.Gauge, x => x.IsActivated ? 1 : 0, _config.MetricsFactorClass2);
            // Power axes
            ActiveControl?.RegisterMetrics(_metricsPublisher, _config);
            ReactiveControl?.RegisterMetrics(_metricsPublisher, _config);

            // Contributions only exist for stack modes, override modes do not change the plant signals
            if (this is IStackableMode)
            {
                RegisterContributionMetrics();
            }
        }


        /// <summary>
        /// Register the contribution metrics at the publisher.
        /// </summary>
        private void RegisterContributionMetrics()
        {
            if (ActiveControl is not null)
            {
                _metricsPublisher.Register<OperatingModeBase, double>(this, nameof(ActivePowerContribution), MetricType.Gauge, x => x.ActivePowerContribution.Watts, _config.MetricsFactorClass1);
            }

            if (ReactiveControl is not null)
            {
                _metricsPublisher.Register<OperatingModeBase, double>(this, nameof(ReactivePowerContribution), MetricType.Gauge, x => x.ReactivePowerContribution.VoltAmperesReactive, _config.MetricsFactorClass1);
            }

            if ((Classification & OperatingModeClassification.LimitingActive) != OperatingModeClassification.None)
            {
                _metricsPublisher.Register<OperatingModeBase, int>(this, "ActivePowerIsLimiting", MetricType.Gauge, x => x.ActivePowerContribution.Watts != 0 ? 1 : 0, _config.MetricsFactorClass2);
            }

            if ((Classification & OperatingModeClassification.LimitingReactive) != OperatingModeClassification.None)
            {
                _metricsPublisher.Register<OperatingModeBase, int>(this, "ReactivePowerIsLimiting", MetricType.Gauge, x => x.ReactivePowerContribution.VoltAmperesReactive != 0 ? 1 : 0, _config.MetricsFactorClass2);
            }
        }


        /// <summary>
        /// Matches the active and reactive operating state to a common state.
        /// </summary>
        /// <param name="active">The active operating state or null if the axis does not exist.</param>
        /// <param name="reactive">The reactive operating state or null if the axis does not exist.</param>
        /// <param name="state">The operating state to match.</param>
        /// <returns>True if the states match the common state.</returns>
        private bool Matches(OperatingModeState? active, OperatingModeState? reactive, OperatingModeState state)
        {
            return (active == state && reactive == state) ||
                   (active == state && (reactive == null || reactive == OperatingModeState.Disabled)) ||
                   (reactive == state && (active == null || active == OperatingModeState.Disabled));
        }
    }
}
