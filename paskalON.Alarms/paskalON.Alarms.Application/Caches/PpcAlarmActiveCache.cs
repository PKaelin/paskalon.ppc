// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.Caches
{
    /// <summary>
    /// In memory cache of the active alarms per publisher, so that the database is not queried for every received data.
    /// </summary>
    /// <remarks>
    /// - Singleton, loaded at startup. The alarm service is the only writer of the alarms, so the cache stays consistent
    ///   with the database when every saved change is applied.
    /// - The cache is updated only after the evaluation has been saved (<see cref="Apply"/>).
    /// - The evaluator changes the cached alarm instances (normalize) before they are saved. When saving fails the
    ///   publisher is invalidated (<see cref="Invalidate"/>) and its active alarms are loaded again from the database
    ///   with the next evaluation, also when the database is still not available at that time.
    /// - The cached alarms are shared instances and must be changed and saved by one writer at a time (alarm handler).
    /// </remarks>
    public class PpcAlarmActiveCache : IPpcAlarmActiveCache
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<PpcAlarmActiveCache> _logger;


        /// <summary>
        /// Lock of the cached alarms.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Cached active alarms per publisher, a publisher without active alarms has no entry.
        /// </summary>
        private Dictionary<PpcAlarmKey, List<PpcAlarm>> _alarmsByKey = new Dictionary<PpcAlarmKey, List<PpcAlarm>>();


        /// <summary>
        /// Publishers whose cached active alarms are not valid and are loaded again from the database.
        /// </summary>
        private readonly HashSet<PpcAlarmKey> _invalidKeys = new HashSet<PpcAlarmKey>();


        /// <summary>
        /// Indicates whether the active alarms have been loaded.
        /// </summary>
        private bool _isLoaded;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmActiveCache"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public PpcAlarmActiveCache(ILogger<PpcAlarmActiveCache> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <inheritdoc/>
        public bool IsLoaded
        {
            get
            {
                lock (_dataLock)
                {
                    return _isLoaded;
                }
            }
        }


        /// <inheritdoc/>
        public async Task<int> LoadAsync(IPpcAlarmRepository repository, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(repository);

            List<PpcAlarm> alarms = await repository.GetAllActiveAsync(cancellationToken);
            Dictionary<PpcAlarmKey, List<PpcAlarm>> alarmsByKey = alarms
                .GroupBy(CreateKey)
                .ToDictionary(group => group.Key, group => group.ToList());

            lock (_dataLock)
            {
                _alarmsByKey = alarmsByKey;
                _invalidKeys.Clear();
                _isLoaded = true;
            }

            _logger.LogInformation("Loaded {AlarmCount} active alarms of {PublisherCount} publishers.", alarms.Count, alarmsByKey.Count);

            return alarms.Count;
        }


        /// <inheritdoc/>
        public async Task<List<PpcAlarm>> GetActiveAsync(PpcAlarmKey key, IPpcAlarmRepository repository, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(repository);

            lock (_dataLock)
            {
                if (_isLoaded is false)
                {
                    throw new InvalidOperationException("The active alarms have not been loaded.");
                }

                if (_invalidKeys.Contains(key) is false)
                {
                    return GetCachedAlarms(key);
                }
            }

            // Loaded outside the lock, the alarm handler is the only writer of the publisher's alarms
            List<PpcAlarm> alarms = await repository.GetActiveAsync(key, cancellationToken);

            lock (_dataLock)
            {
                SetCachedAlarms(key, alarms);
                _invalidKeys.Remove(key);
                _logger.LogInformation("Reloaded {AlarmCount} active alarms of {PpcAlarmType} device {DeviceId}.", alarms.Count, key.PpcAlarmType, key.DeviceId);

                return GetCachedAlarms(key);
            }
        }


        /// <inheritdoc/>
        public IReadOnlyList<PpcAlarmKey> GetKeys()
        {
            lock (_dataLock)
            {
                if (_isLoaded is false)
                {
                    throw new InvalidOperationException("The active alarms have not been loaded.");
                }

                return _alarmsByKey.Keys.Union(_invalidKeys).ToList();
            }
        }


        /// <inheritdoc/>
        public void Apply(PpcAlarmKey key, PpcAlarmEvaluation evaluation)
        {
            ArgumentNullException.ThrowIfNull(evaluation);

            lock (_dataLock)
            {
                // The evaluator normalizes the cached instances returned by GetActiveAsync
                List<PpcAlarm> alarms = GetCachedAlarms(key);
                alarms.RemoveAll(alarm => evaluation.Normalized.Any(normalized => ReferenceEquals(normalized, alarm)));
                alarms.AddRange(evaluation.Raised);
                SetCachedAlarms(key, alarms);
            }
        }


        /// <inheritdoc/>
        public void Invalidate(PpcAlarmKey key)
        {
            lock (_dataLock)
            {
                _invalidKeys.Add(key);
            }
        }


        /// <summary>
        /// Gets a new list of the cached active alarms of a publisher, must be called within the lock.
        /// </summary>
        private List<PpcAlarm> GetCachedAlarms(PpcAlarmKey key)
        {
            if (_alarmsByKey.TryGetValue(key, out List<PpcAlarm>? alarms))
            {
                return new List<PpcAlarm>(alarms);
            }

            return new List<PpcAlarm>();
        }


        /// <summary>
        /// Sets the cached active alarms of a publisher, a publisher without active alarms is removed. Must be called within the lock.
        /// </summary>
        private void SetCachedAlarms(PpcAlarmKey key, List<PpcAlarm> alarms)
        {
            if (alarms.Count == 0)
            {
                _alarmsByKey.Remove(key);

                return;
            }

            _alarmsByKey[key] = alarms;
        }


        /// <summary>
        /// Creates the key of the publisher of an alarm.
        /// </summary>
        private static PpcAlarmKey CreateKey(PpcAlarm alarm)
        {
            return new PpcAlarmKey { PpcAlarmType = alarm.PpcAlarmConfig.PpcAlarmType, DeviceId = alarm.DeviceId };
        }
    }
}
