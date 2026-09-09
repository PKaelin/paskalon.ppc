// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Workers
{
    public class ServerService : BackgroundService
    {
        private readonly ILogger<ServerService> _logger;

        public ServerService(ILogger<ServerService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        public void Initialize()
        {
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
