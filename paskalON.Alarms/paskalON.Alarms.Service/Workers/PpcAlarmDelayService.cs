// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Domain.Evaluations;

namespace paskalON.Alarms.Service.Workers
{
    /// <summary>
    /// Raises and normalizes the delayed alarms (TimeBeforeRaisingAlarmMilliseconds and TimeBeforeNormalizingAlarmMilliseconds)
    /// once their delay elapsed.
    /// </summary>
    /// <remarks>
    /// The evaluation of received data only runs when a value changes, a value that stays abnormal (or normal) after
    /// it changed is not evaluated again. Therefore the pending transitions are checked every interval.
    /// A scope with the alarm handler is only created when a transition is due.
    /// </remarks>
    public class PpcAlarmDelayService : BackgroundService
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<PpcAlarmDelayService> _logger;


        /// <summary>
        /// Factory of the scopes of the alarm handler.
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;


        /// <summary>
        /// Delayed raises and normalizations of the alarms.
        /// </summary>
        private readonly PpcAlarmPendingTransitions _pendingTransitions;


        /// <summary>
        /// Time provider of the check interval and the due times.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Interval between two checks, zero until initialized.
        /// </summary>
        private TimeSpan _checkInterval;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmDelayService"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="scopeFactory">Factory of the scopes of the alarm handler.</param>
        /// <param name="pendingTransitions">Delayed raises and normalizations of the alarms.</param>
        /// <param name="timeProvider">Time provider of the check interval and the due times.</param>
        public PpcAlarmDelayService(ILogger<PpcAlarmDelayService> logger, IServiceScopeFactory scopeFactory,
            PpcAlarmPendingTransitions pendingTransitions, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(scopeFactory);
            ArgumentNullException.ThrowIfNull(pendingTransitions);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _scopeFactory = scopeFactory;
            _pendingTransitions = pendingTransitions;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// Sets the check interval of the system configuration, must be called before the service starts.
        /// </summary>
        /// <param name="checkIntervalMilliseconds">Interval between two checks (SystemConfig.AlarmDelayCheckIntervalMilliseconds).</param>
        public void Initialize(int checkIntervalMilliseconds)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(checkIntervalMilliseconds, 0);

            _checkInterval = TimeSpan.FromMilliseconds(checkIntervalMilliseconds);
        }


        /// <summary>
        /// Raises and normalizes the delayed alarms whose delay elapsed.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if transitions were due and handled, false if none was due or the handling failed.</returns>
        public async Task<bool> CheckDueAsync(CancellationToken cancellationToken = default)
        {
            if (_pendingTransitions.GetDueKeys(_timeProvider.GetUtcNow()).Count == 0)
            {
                return false;
            }

            try
            {
                await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
                IPpcAlarmHandler handler = scope.ServiceProvider.GetRequiredService<IPpcAlarmHandler>();
                await handler.HandleDueAsync(cancellationToken);

                return true;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Raising or normalizing the delayed alarms failed.");

                return false;
            }
        }


        /// <summary>
        /// Checks the delayed alarms every interval until the service stops.
        /// </summary>
        /// <param name="stoppingToken">Cancelled when the service stops.</param>
        /// <returns>Task.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the service was not initialized.</exception>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_checkInterval == TimeSpan.Zero)
            {
                throw new InvalidOperationException($"{nameof(PpcAlarmDelayService)} must be initialized before it starts.");
            }

            try
            {
                while (stoppingToken.IsCancellationRequested is false)
                {
                    await Task.Delay(_checkInterval, _timeProvider, stoppingToken);
                    await CheckDueAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected when normal shutdown.
            }
        }
    }
}
