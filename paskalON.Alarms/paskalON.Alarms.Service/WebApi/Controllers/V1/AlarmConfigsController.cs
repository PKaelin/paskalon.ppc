// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Alarms.Application.Caches;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Alarms.Service.Dto.V1.Responses;
using paskalON.Hosting;

namespace paskalON.Alarms.Service.Controllers.V1
{
    /// <summary>
    /// Controller for managing the alarm configurations.
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AlarmConfigsController : ControllerBase
    {
        /// <summary>
        /// The logger instance for logging information and errors.
        /// </summary>
        private readonly ILogger<AlarmConfigsController> _logger;


        /// <summary>
        /// Cache of the enabled alarm configurations.
        /// </summary>
        private readonly IPpcAlarmConfigCache _configCache;


        /// <summary>
        /// Repository of the alarm configurations.
        /// </summary>
        private readonly IPpcAlarmConfigRepository _configRepository;


        /// <summary>
        /// Alarm handler that normalizes the active alarms of removed or disabled configurations.
        /// </summary>
        private readonly IPpcAlarmHandler _alarmHandler;


        /// <summary>
        /// Constructor of <see cref="AlarmConfigsController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="configCache">Cache of the enabled alarm configurations.</param>
        /// <param name="configRepository">Repository of the alarm configurations.</param>
        /// <param name="alarmHandler">Alarm handler that normalizes the active alarms of removed or disabled configurations.</param>
        public AlarmConfigsController(ILogger<AlarmConfigsController> logger, IPpcAlarmConfigCache configCache,
            IPpcAlarmConfigRepository configRepository, IPpcAlarmHandler alarmHandler)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configCache);
            ArgumentNullException.ThrowIfNull(configRepository);
            ArgumentNullException.ThrowIfNull(alarmHandler);

            _logger = logger;
            _configCache = configCache;
            _configRepository = configRepository;
            _alarmHandler = alarmHandler;
        }


        /// <summary>
        /// Reloads the enabled alarm configurations from the database, e.g. after the configurations were changed,
        /// and normalizes the active alarms of removed or disabled configurations.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token of the request.</param>
        /// <returns>
        /// 200 with the number of loaded configurations and normalized alarms, 503 when the database is not available.
        /// When the reload fails the previously loaded configurations stay in use.
        /// </returns>
        [HttpPost]
        [ProducesResponseType<ReloadAlarmConfigsResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ReloadAlarmConfigsResponse>> Reload(CancellationToken cancellationToken)
        {
            int configCount;

            try
            {
                configCount = await _configCache.LoadAsync(_configRepository, cancellationToken);
            }
            catch (Exception exception) when (DatabaseTransientExceptionFilter.IsTransient(exception))
            {
                _logger.LogWarning(exception, "Reloading the alarm configurations failed, the previously loaded configurations stay in use.");

                return Problem("The alarm configurations could not be reloaded, the database is not available.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            try
            {
                int normalizedAlarmCount = await _alarmHandler.NormalizeOrphanedAsync(cancellationToken);

                return Ok(new ReloadAlarmConfigsResponse { ConfigCount = configCount, NormalizedAlarmCount = normalizedAlarmCount });
            }
            catch (Exception exception) when (DatabaseTransientExceptionFilter.IsTransient(exception))
            {
                _logger.LogWarning(exception, "Normalizing the active alarms of removed or disabled configurations failed.");

                return Problem("The alarm configurations were reloaded, but the active alarms of removed or disabled configurations " +
                    "could not be normalized, the database is not available. Reload again when the database is available.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
