// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Protocols.Modbus;
using System.Diagnostics;

namespace paskalON.Devices.Service.Workers
{
    public class ModbusPollService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// List of Modbus polling engines.
        /// </summary>
        private IEnumerable<IModbusPollingEngine> _modbusEngines = new List<IModbusPollingEngine>();


        /// <summary>
        /// Time based interval for Modbus polls.
        /// </summary>
        /// <remarks>
        /// Time based interval means that the poll is called periodically with this time interval.
        /// </remarks>
        private int _intervalMilliseconds;


        /// <summary>
        /// Constructor of <see cref="ModbusPollService"/>.
        /// </summary>
        /// <param name="logger"></param>
        public ModbusPollService(ILogger<ModbusPollService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="modbusEngines">List of Modbus polling engines.</param>
        /// <param name="intervalMilliseconds">Time based interval for Modbus polls.</param>
        public void Initialize(IEnumerable<IModbusPollingEngine> modbusEngines, int intervalMilliseconds)
        {
            ArgumentNullException.ThrowIfNull(modbusEngines);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMilliseconds);

            _modbusEngines = modbusEngines;
            _intervalMilliseconds = intervalMilliseconds;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task[] tasks = _modbusEngines.Select(engine => RunEngineAsync(engine, stoppingToken)).ToArray();

            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Run all the engines in a separate threat.
        /// </summary>
        /// <param name="engine">The engine to call the poll on.</param>
        /// <param name="stoppingToken">The cancellation token.</param>
        /// <remarks>
        /// 0ms       500       1000      1500
        /// │──────────│─────────│─────────│
        /// START     due       due       due
        /// │
        /// │  PollAsync takes 1200ms
        /// │
        /// └───────────────────> 1200ms FINISH
        /// │
        /// └──> START IMMEDIATELY when past due
        /// </remarks>
        private async Task RunEngineAsync(IModbusPollingEngine engine, CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long nextRun = 0;
            int interval = 0;

            try
            {
                while (stoppingToken.IsCancellationRequested == false)
                {
                    // Wait until the scheduled time, unless we are already overdue
                    long now = stopwatch.ElapsedMilliseconds;
                    long delay = nextRun - now;

                    if (delay > 0)
                    {
                        // If there wasn't a delay wait the remaining time span
                        await Task.Delay(TimeSpan.FromMilliseconds(delay), stoppingToken);
                    }

                    // This execution is scheduled.
                    // If the task is always slow it will never catch up but that's ok
                    // If the task is sometimes slower and sometimes faster it the loop is trying to maintain the interval
                    nextRun += _intervalMilliseconds;

                    if (++interval == int.MaxValue)
                    {
                        interval = 1;
                    }

                    try
                    {
                        await engine.PollAsync(interval, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error polling Modbus engine {Engine}. Error: {Error}", engine.ModbusPollingDestination, ex);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected when normal shutdown
            }
        }
    }
}
