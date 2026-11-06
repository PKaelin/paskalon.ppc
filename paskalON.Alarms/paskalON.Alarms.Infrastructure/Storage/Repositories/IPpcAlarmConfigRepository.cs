// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Ppcs;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// PPC alarm configuration repository interface definition.
    /// </summary>
    public interface IPpcAlarmConfigRepository
    {
        /// <summary>
        /// Gets the enabled configurations of an alarm type including their actions and action parameters.
        /// </summary>
        /// <param name="ppcAlarmType">Alarm type of the publisher.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Enabled configurations of the alarm type, of all devices.</returns>
        Task<List<PpcAlarmConfig>> GetEnabledAsync(PpcAlarmType ppcAlarmType, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the enabled configurations of all alarm types including their actions and action parameters, not tracked.
        /// </summary>
        /// <remarks>
        /// Used to cache the configurations, they are not tracked so that they can be attached to other contexts.
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Enabled configurations of all alarm types.</returns>
        Task<List<PpcAlarmConfig>> GetAllEnabledAsync(CancellationToken cancellationToken = default);
    }
}
