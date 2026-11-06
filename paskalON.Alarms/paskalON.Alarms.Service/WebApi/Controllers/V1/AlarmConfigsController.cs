// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Alarms.Application.Caches;
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
        /// Constructor of <see cref="AlarmConfigsController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="configCache">Cache of the enabled alarm configurations.</param>
        /// <param name="configRepository">Repository of the alarm configurations.</param>
        public AlarmConfigsController(ILogger<AlarmConfigsController> logger, IPpcAlarmConfigCache configCache, IPpcAlarmConfigRepository configRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configCache);
            ArgumentNullException.ThrowIfNull(configRepository);

            _logger = logger;
            _configCache = configCache;
            _configRepository = configRepository;
        }


        /// <summary>
        /// Reloads the enabled alarm configurations from the database, e.g. after the configurations were changed.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token of the request.</param>
        /// <returns>
        /// 200 with the number of loaded configurations, 503 when the database is not available.
        /// When the reload fails the previously loaded configurations stay in use.
        /// </returns>
        [HttpPost]
        [ProducesResponseType<ReloadAlarmConfigsResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ReloadAlarmConfigsResponse>> Reload(CancellationToken cancellationToken)
        {
            try
            {
                int configCount = await _configCache.LoadAsync(_configRepository, cancellationToken);

                return Ok(new ReloadAlarmConfigsResponse { ConfigCount = configCount });
            }
            catch (Exception exception) when (DatabaseTransientExceptionFilter.IsTransient(exception))
            {
                _logger.LogWarning(exception, "Reloading the alarm configurations failed, the previously loaded configurations stay in use.");

                return Problem("The alarm configurations could not be reloaded, the database is not available.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
