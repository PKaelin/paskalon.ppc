// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.Simulations;

namespace paskalON.DeviceSimulator.Service.Workers
{
    /// <summary>
    /// Advances the simulated grid and all registered simulator models at a fixed interval.
    /// </summary>
    public sealed class SimulationWorker : BackgroundService
    {
        /// <summary>
        /// Simulation device registry.
        /// </summary>
        private readonly SimulationDeviceRegistry _devices;


        /// <summary>
        /// Shared simulated grid.
        /// </summary>
        private readonly SimulatedGrid _grid;


        /// <summary>
        /// Time based interval for simulation executions.
        /// </summary>
        private int _intervalMilliseconds;


        /// <summary>
        /// Constructor of <see cref="SimulationWorker"/>.
        /// </summary>
        /// <param name="devices">Simulation device registry.</param>
        /// <param name="grid">Shared simulated grid.</param>
        public SimulationWorker(SimulationDeviceRegistry devices, SimulatedGrid grid)
        {
            ArgumentNullException.ThrowIfNull(devices);
            ArgumentNullException.ThrowIfNull(grid);

            _devices = devices;
            _grid = grid;
        }


        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="intervalMilliseconds">Time based interval for data publisher.</param>
        public void Initialize(int intervalMilliseconds)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(intervalMilliseconds, 50);

            _intervalMilliseconds = intervalMilliseconds;
        }


        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan interval = TimeSpan.FromMilliseconds(_intervalMilliseconds);
            PeriodicTimer timer = new PeriodicTimer(interval);

            using (timer)
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    _grid.Tick();

                    foreach (ISimulatedDevice device in _devices.Devices)
                    {
                        await device.TickAsync(interval, stoppingToken);
                    }
                }
            }
        }
    }
}
