// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.OperatingModes.Application.Factories;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Infrastructure.Storage.Repositories;
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
        /// Time provider to get the current time for operating mode operations.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Operating modes available for selection.
        /// </summary>
        private Dictionary<string, OperatingModeBase> _availableOperatingModes = new Dictionary<string, OperatingModeBase>(StringComparer.Ordinal);


        /// <summary>
        /// Stack of selected operating modes.
        /// </summary>
        private readonly OperatingModeStack _operatingModeStack = new OperatingModeStack();


        /// <inheritdoc/>
        public ICollection<IMetricsPublisher> MetricsPublishers { get; } = new List<IMetricsPublisher>();


        /// <inheritdoc/>
        public IReadOnlyCollection<OperatingModeBase> AvailableOperatingModes => _availableOperatingModes.Values;


        /// <inheritdoc/>
        public IReadOnlyCollection<OperatingModeBase> SelectedOperatingModes => _operatingModeStack.OperatingModes;


        public OperatingModeManager(ILogger<OperatingModeManager> logger, IDeviceClient deviceClient, IMetricsPublisherFactory metricsPublisherFactory,
            TimeProvider timeProvider, IOperatingModeFactory operatingModeFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);
            ArgumentNullException.ThrowIfNull(metricsPublisherFactory);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(operatingModeFactory);

            _logger = logger;
            _deviceClient = deviceClient;
            _metricsPublisherFactory = metricsPublisherFactory;
            _operatingModeFactory = operatingModeFactory;
            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        public void AddOperatingMode(string operatingModeName, int priority)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);

            if (_availableOperatingModes.TryGetValue(operatingModeName, out OperatingModeBase? operatingMode) is false)
            {
                throw new KeyNotFoundException($"Operating mode '{operatingModeName}' is not configured.");
            }

            _operatingModeStack.Add(operatingMode, priority);
        }


        /// <inheritdoc/>
        public void MoveOperatingMode(string operatingModeName, int priority)
        {
            _operatingModeStack.Move(operatingModeName, priority);
        }


        /// <inheritdoc/>
        public void RemoveOperatingMode(string operatingModeName)
        {
            _operatingModeStack.Remove(operatingModeName);
        }


        public virtual async Task Initialize(IOperatingModeRepository repository, SystemConfig systemConfig)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(systemConfig);

            _logger.LogInformation("Initialize operating mode during startup");
            List<OperatingModeBaseConfig> operatingModeConfigs = await repository.GetAllOperatingModes();

            List<OperatingModeBase> operatingModes = new List<OperatingModeBase>();

            foreach (OperatingModeBaseConfig config in operatingModeConfigs)
            {
                IMetricsPublisher operatingModeMetrics = _metricsPublisherFactory.Create();
                OperatingModeBase operatingMode = _operatingModeFactory.Create(_logger, _timeProvider, operatingModeMetrics, systemConfig, config, _deviceClient.Der);
                operatingModes.Add(operatingMode);
                MetricsPublishers.Add(operatingModeMetrics);
            }

            _availableOperatingModes = operatingModes.ToDictionary(operatingMode => operatingMode.Name, StringComparer.Ordinal);
            _operatingModeStack.Clear();
        }
    }
}
