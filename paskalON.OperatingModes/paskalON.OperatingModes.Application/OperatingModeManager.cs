// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.OperatingModes.Application.Factories;
using paskalON.OperatingModes.Application.Overrides;
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.OperatingModes.Infrastructure.Storage.Repositories;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;
using paskalON.Telemetry.Factories;

namespace paskalON.OperatingModes.Application
{
    /// <summary>
    /// Operating mode manager responsible for managing systems operating modes and configurations.
    /// </summary>
    public class OperatingModeManager : IOperatingModeManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<OperatingModeManager> _logger;


        /// <summary>
        /// Metrics publisher factory to create metrics publishers for telemetry.
        /// </summary>
        private readonly IMetricsPublisherFactory _metricsPublisherFactory;


        /// <summary>
        /// Device client to receive device DTOs from pub/sub.
        /// </summary>
        private readonly IDeviceClient _deviceClient;


        /// <summary>
        /// Operating mode factory to create operating mode instances based on configurations.
        /// </summary>
        private readonly IOperatingModeFactory _operatingModeFactory;


        /// <summary>
        /// Operating mode map factory to create the input map of the system operating mode control.
        /// </summary>
        private readonly IOperatingModeMapFactory _operatingModeMapFactory;


        /// <summary>
        /// Time provider to get the current time for operating mode operations.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <inheritdoc/>
        public ICollection<IMetricsPublisher> MetricsPublishers { get; } = new List<IMetricsPublisher>();


        /// <inheritdoc/>
        public IStackManager StackManager { get; } = new StackManager();


        /// <inheritdoc/>
        public IOverrideManager OverrideManager { get; } = new OverrideManager(new OperatingModeOverrides());


        /// <inheritdoc/>
        public ISystemOperatingModeControl SystemOperatingModeControl { get; private set; } = null!;


        /// <summary>
        /// Constructor of <see cref="OperatingModeManager"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="deviceClient">Device client to receive device DTOs.</param>
        /// <param name="metricsPublisherFactory">Factory to create metrics publishers.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <param name="operatingModeFactory">Factory to create operating modes.</param>
        /// <param name="operatingModeMapFactory">Factory to create operating mode maps.</param>
        public OperatingModeManager(ILogger<OperatingModeManager> logger, IDeviceClient deviceClient, IMetricsPublisherFactory metricsPublisherFactory,
            TimeProvider timeProvider, IOperatingModeFactory operatingModeFactory, IOperatingModeMapFactory operatingModeMapFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);
            ArgumentNullException.ThrowIfNull(metricsPublisherFactory);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(operatingModeFactory);
            ArgumentNullException.ThrowIfNull(operatingModeMapFactory);

            _logger = logger;
            _deviceClient = deviceClient;
            _metricsPublisherFactory = metricsPublisherFactory;
            _operatingModeFactory = operatingModeFactory;
            _operatingModeMapFactory = operatingModeMapFactory;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The override slot exists before the operating modes are created, so that the plant-level map functions can exclude overridden DER units.
        /// </remarks>
        public virtual async Task Initialize(IOperatingModeRepository repository, SystemConfig systemConfig, ISetpointsPublisher setpointsPublisher)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(systemConfig);
            ArgumentNullException.ThrowIfNull(setpointsPublisher);

            _logger.LogInformation("Initialize operating mode during startup");
            List<OperatingModeBaseConfig> operatingModeConfigs = await repository.GetAllOperatingModes();
            Dictionary<string, OperatingModeBase> operatingModes = new Dictionary<string, OperatingModeBase>(StringComparer.Ordinal);

            foreach (OperatingModeBaseConfig config in operatingModeConfigs)
            {
                IMetricsPublisher operatingModeMetrics = _metricsPublisherFactory.Create();
                OperatingModeBase operatingMode = _operatingModeFactory.Create(_logger, _timeProvider, operatingModeMetrics, systemConfig, config,
                    _deviceClient.Der, OverrideManager.Overrides);
                operatingModes.Add(operatingMode.Name, operatingMode);
                MetricsPublishers.Add(operatingModeMetrics);
            }

            StackManager.InitializeStack(operatingModes);
            OverrideManager.InitializeOverrides(operatingModes);
            IMetricsPublisher systemModeMetrics = _metricsPublisherFactory.Create();
            SystemOperatingModeControl = new SystemOperatingModeControl(_logger, systemConfig, StackManager.Stack, OverrideManager.Overrides,
                _operatingModeMapFactory.CreateSystemMap(_deviceClient.Der), systemModeMetrics, setpointsPublisher);
            MetricsPublishers.Add(systemModeMetrics);
            SystemOperatingModeControl.State = SystemState.Ready;
        }


        /// <inheritdoc/>
        public async Task SetSystemPowerSetpoints(ActivePower activePower, ReactivePower reactivePower)
        {
            SystemOperatingModeControl.UpdateSetpoints(activePower, reactivePower);
        }
    }
}
