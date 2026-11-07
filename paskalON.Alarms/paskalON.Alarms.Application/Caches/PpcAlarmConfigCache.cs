// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.Caches
{
    /// <summary>
    /// In memory cache of the enabled PPC alarm configurations, so that the database is not queried for every received data.
    /// </summary>
    /// <remarks>
    /// - Singleton, loaded at startup and reloaded on request (reload API), the configurations change rarely.
    /// - A load builds a complete new set and replaces the cached set at once, so that an evaluation during a reload
    ///   uses either the old or the new configurations and never a mix. Reading is lock free, the cached set is never changed.
    /// - Loads run one after another, so that overlapping reloads cannot replace newer with older configurations.
    /// - Not loaded configurations throw instead of returning no configurations, because an evaluation without
    ///   configurations would normalize all active alarms (orphaned).
    /// - The configurations are not tracked by a database context, the alarm repository attaches them when saving an alarm.
    /// </remarks>
    public class PpcAlarmConfigCache : IPpcAlarmConfigCache
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<PpcAlarmConfigCache> _logger;


        /// <summary>
        /// Serializes the loads, so that the last started load is also the last replaced configurations.
        /// </summary>
        private readonly SemaphoreSlim _dataLock = new SemaphoreSlim(1, 1);


        /// <summary>
        /// Cached configurations per alarm type, null until loaded. Replaced at once by a load.
        /// </summary>
        private volatile IReadOnlyDictionary<PpcAlarmType, IReadOnlyList<PpcAlarmConfig>>? _configsByType;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmConfigCache"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public PpcAlarmConfigCache(ILogger<PpcAlarmConfigCache> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <inheritdoc/>
        public bool IsLoaded => _configsByType != null;


        /// <inheritdoc/>
        public async Task<int> LoadAsync(IPpcAlarmConfigRepository repository, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(repository);

            // Overlapping loads (e.g. reload requested twice) would otherwise replace newer with older configurations
            await _dataLock.WaitAsync(cancellationToken);

            try
            {
                List<PpcAlarmConfig> configs = await repository.GetAllEnabledAsync(cancellationToken);
                Dictionary<PpcAlarmType, IReadOnlyList<PpcAlarmConfig>> configsByType = configs
                    .GroupBy(config => config.PpcAlarmType)
                    .ToDictionary(group => group.Key, group => (IReadOnlyList<PpcAlarmConfig>)group.ToList().AsReadOnly());
                _configsByType = configsByType;
                _logger.LogInformation("Loaded {ConfigCount} enabled alarm configurations of {TypeCount} alarm types.", configs.Count, configsByType.Count);

                return configs.Count;
            }
            finally
            {
                _dataLock.Release();
            }
        }


        /// <inheritdoc/>
        public IReadOnlyList<PpcAlarmConfig> GetEnabled(PpcAlarmType ppcAlarmType)
        {
            IReadOnlyDictionary<PpcAlarmType, IReadOnlyList<PpcAlarmConfig>> configsByType = _configsByType
                ?? throw new InvalidOperationException("The alarm configurations have not been loaded.");

            if (configsByType.TryGetValue(ppcAlarmType, out IReadOnlyList<PpcAlarmConfig>? configs))
            {
                return configs;
            }

            return Array.Empty<PpcAlarmConfig>();
        }
    }
}
