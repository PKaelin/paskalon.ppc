// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.Modbus;
using System.Diagnostics;

namespace paskalON.DeviceSimulator.Service.Workers
{
    public class ModbusPollService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<ModbusPollService> _logger;


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
        /// <param name="logger">Logger for application logging and diagnostics.</param>
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
            _logger.LogInformation("Initializing ModbusPollService with {Interval}ms interval", intervalMilliseconds);
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
            Stopwatch stopwatch = Stopwatch.StartNew();
            long nextRun = 0;
            int interval = 0;

            while (stoppingToken.IsCancellationRequested == false)
            {
                long now = stopwatch.ElapsedMilliseconds;
                long delay = nextRun - now;

                if (delay > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(delay), stoppingToken);
                }

                nextRun += _intervalMilliseconds;

                if (++interval == int.MaxValue)
                {
                    interval = 1;
                }


                foreach (ModbusPollingEngine engine in _modbusEngines)
                {
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
                        _logger.LogError("Error polling Modbus engine {Engine}. Error: {Error}", $"{engine.DestinationAddress}:{engine.DestinationPort}", ex);
                    }
                }
            }
        }
    }
}
