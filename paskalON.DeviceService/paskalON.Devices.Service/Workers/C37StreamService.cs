// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.C37;

namespace paskalON.Devices.Service.Workers
{
    public class C37StreamService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<C37StreamService> _logger;


        /// <summary>
        /// List of C37 transmission engines.
        /// </summary>
        private IEnumerable<IC37TransmissionEngine> _engines = new List<IC37TransmissionEngine>();


        /// <summary>
        /// Time based interval to try to reconnect if engine is disconnected.
        /// </summary>
        private int _intervalReconnectMilliseconds;


        /// <summary>
        /// Constructor of <see cref="C37StreamService"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public C37StreamService(ILogger<C37StreamService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <summary>
        /// /// Initializes the service.
        /// </summary>
        /// <param name="engines"></param>
        /// <param name="intervalReconnectMilliseconds">Reconnect timer in case connections was lost.</param>
        /// <remarks>
        /// intervalReconnectMilliseconds has to consider the connect timeouts and the retry connect attempt's.
        /// </remarks>
        public void Initialize(IEnumerable<IC37TransmissionEngine> engines, int intervalReconnectMilliseconds = 60000)
        {
            ArgumentNullException.ThrowIfNull(engines);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalReconnectMilliseconds);

            _engines = engines;
            _intervalReconnectMilliseconds = intervalReconnectMilliseconds;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task[] tasksReconnect = _engines.Select(engine => ReconnectEngineAsync(engine, stoppingToken)).ToArray();

            await Task.WhenAll(tasksReconnect);
        }


        /// <summary>
        /// Reconnects the engine in case a disconnect happened.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="stoppingToken">The cancellation token.</param>
        /// <returns>Task</returns>
        private async Task ReconnectEngineAsync(IC37TransmissionEngine engine, CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(90), stoppingToken);
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_intervalReconnectMilliseconds));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await engine.StartStreaming(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Could not reconnect C37 engine {Engine}. Error: {Error}", $"{engine.DestinationAddress}:{engine.DestinationPort}", ex);
                }
            }
        }
    }
}
