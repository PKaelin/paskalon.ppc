// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Luna.Application.Dependencies;
using paskalON.Luna.Dto.Systems;
using paskalON.Messaging.Health;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Provides the availability of the dependencies of the Luna service from the <see cref="IDependencyHealthMonitor"/>.
    /// </summary>
    public sealed class LunaDependencyManager : ILunaDependencyManager
    {
        /// <summary>
        /// Names of the dependencies, all must be registered in the monitor.
        /// </summary>
        private static readonly string[] DependencyNameList =
        [
            DependencyNames.PowerControl,
            DependencyNames.OperatingMode,
            DependencyNames.DeviceService,
            DependencyNames.Redis
        ];


        /// <summary>
        /// Monitor tracking the availability of the dependencies.
        /// </summary>
        private readonly IDependencyHealthMonitor _monitor;


        /// <summary>
        /// Constructor of <see cref="LunaDependencyManager"/>.
        /// </summary>
        /// <param name="monitor">Monitor tracking the availability of the dependencies.</param>
        public LunaDependencyManager(IDependencyHealthMonitor monitor)
        {
            ArgumentNullException.ThrowIfNull(monitor);

            _monitor = monitor;
        }


        /// <inheritdoc/>
        public IReadOnlyList<DependencyStatusDto> GetDependencyStatus()
        {
            List<DependencyStatusDto> statuses = new List<DependencyStatusDto>(DependencyNameList.Length);

            foreach (string name in DependencyNameList)
            {
                statuses.Add(new DependencyStatusDto
                {
                    Name = name,
                    IsAvailable = _monitor.GetState(name) == DependencyState.Available
                });
            }

            return statuses;
        }
    }
}
