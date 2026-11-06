// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.Caches
{
    /// <summary>
    /// In memory cache of the enabled PPC alarm configurations, so that the database is not queried for every received data.
    /// </summary>
    public interface IPpcAlarmConfigCache
    {
        /// <summary>
        /// Indicates whether the configurations have been loaded.
        /// </summary>
        bool IsLoaded { get; }


        /// <summary>
        /// Loads (or reloads) the enabled configurations and replaces the cached configurations at once.
        /// </summary>
        /// <param name="repository">Repository of the configurations.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of loaded configurations.</returns>
        /// <remarks>
        /// When loading fails the previously cached configurations stay in use.
        /// </remarks>
        Task<int> LoadAsync(IPpcAlarmConfigRepository repository, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the cached enabled configurations of an alarm type.
        /// </summary>
        /// <param name="ppcAlarmType">Alarm type of the publisher.</param>
        /// <returns>Enabled configurations of the alarm type, of all devices.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the configurations have not been loaded.</exception>
        IReadOnlyList<PpcAlarmConfig> GetEnabled(PpcAlarmType ppcAlarmType);
    }
}
