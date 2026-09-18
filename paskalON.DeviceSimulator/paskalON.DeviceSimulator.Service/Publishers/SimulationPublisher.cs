// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Publishers
{
    public class SimulationPublisher : BackgroundService
    {
        private readonly ILogger<SimulationPublisher> _logger;

        private int _intervalTimerMilliseconds;

        public SimulationPublisher(ILogger<SimulationPublisher> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }

        public void Initialize(int intervalTimerMilliseconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalTimerMilliseconds);

            _intervalTimerMilliseconds = intervalTimerMilliseconds;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(6000), stoppingToken);
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_intervalTimerMilliseconds));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {

            }
        }
    }
}
