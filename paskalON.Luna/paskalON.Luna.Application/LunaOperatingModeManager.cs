// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Client.Subscribers;
using paskalON.OperatingModes.Dto.Powers;
using System.Collections.Concurrent;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna operating mode manager holding the latest power setpoints received from the operating mode publisher.
    /// </summary>
    /// <remarks>
    /// The power setpoints are written by the <see cref="IPowerSetpointSubscriber"/> and read concurrently by the web API.
    /// </remarks>
    public class LunaOperatingModeManager : ILunaOperatingModeManager, IPowerSetpointHandler
    {
        /// <summary>
        /// Latest system power setpoints.
        /// </summary>
        /// <remarks>
        /// Replaced as a whole on every update, so readers always see a complete setpoint without locking.
        /// </remarks>
        private volatile CurrentSetpointsDto _currentSetpoints = new CurrentSetpointsDto { SetpointActivePowerWatts = 0, SetpointReactivePowerVars = 0 };


        /// <summary>
        /// Latest power setpoints per DER unit, keyed by DER unit name.
        /// </summary>
        /// <remarks>
        /// Keeps only the latest setpoints per DER unit, so the size is bound by the number of DER units.
        /// </remarks>
        private readonly ConcurrentDictionary<string, CurrentDerUnitSetpointsDto> _currentDerUnitSetpoints =
            new ConcurrentDictionary<string, CurrentDerUnitSetpointsDto>();


        /// <inheritdoc/>
        public CurrentSetpointsDto GetSetpoints()
        {
            return _currentSetpoints;
        }


        /// <inheritdoc/>
        public IReadOnlyCollection<CurrentDerUnitSetpointsDto> GetDerUnitSetpoints()
        {
            return _currentDerUnitSetpoints.Values.ToArray();
        }


        /// <inheritdoc/>
        public CurrentDerUnitSetpointsDto? GetDerUnitSetpoints(string derUnitName)
        {
            ArgumentNullException.ThrowIfNull(derUnitName);

            _currentDerUnitSetpoints.TryGetValue(derUnitName, out CurrentDerUnitSetpointsDto? setpoints);

            return setpoints;
        }


        /// <inheritdoc/>
        public Task HandleSetpointsAsync(CurrentSetpointsDto setpoints)
        {
            ArgumentNullException.ThrowIfNull(setpoints);

            _currentSetpoints = setpoints;

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task HandleDerUnitSetpointsAsync(CurrentDerUnitSetpointsDto setpoints)
        {
            ArgumentNullException.ThrowIfNull(setpoints);
            ArgumentException.ThrowIfNullOrWhiteSpace(setpoints.DerUnitName);

            _currentDerUnitSetpoints[setpoints.DerUnitName] = setpoints;

            return Task.CompletedTask;
        }
    }
}
