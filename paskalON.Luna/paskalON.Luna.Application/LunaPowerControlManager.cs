// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Client.Subscribers;
using paskalON.PowerControls.Dto.Powers;
using System.Collections.Concurrent;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna power control manager holding the latest power targets received from the power control publisher.
    /// </summary>
    /// <remarks>
    /// The power targets are written by the <see cref="IPowerTargetSubscriber"/> and read concurrently by the web API.
    /// </remarks>
    public class LunaPowerControlManager : ILunaPowerControlManager, IPowerTargetHandler
    {
        /// <summary>
        /// Latest power targets per power conversion system (PCS), keyed by device ID.
        /// </summary>
        /// <remarks>
        /// Keeps only the latest power targets per PCS, so the size is bound by the number of PCS.
        /// </remarks>
        private readonly ConcurrentDictionary<int, CurrentTargetsDto> _targets = new ConcurrentDictionary<int, CurrentTargetsDto>();


        /// <inheritdoc/>
        public IReadOnlyCollection<CurrentTargetsDto> GetTargets()
        {
            return _targets.Values.ToArray();
        }


        /// <inheritdoc/>
        public CurrentTargetsDto? GetTarget(int deviceId)
        {
            _targets.TryGetValue(deviceId, out CurrentTargetsDto? targets);

            return targets;
        }


        /// <inheritdoc/>
        public Task HandleAsync(CurrentTargetsDto targets)
        {
            ArgumentNullException.ThrowIfNull(targets);

            _targets[targets.DeviceId] = targets;

            return Task.CompletedTask;
        }
    }
}
