// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Hosting;

namespace paskalON.Alarms.Service.Controllers.V1
{
    /// <summary>
    /// Controller of the alarms.
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AlarmsController : ControllerBase
    {
        /// <summary>
        /// The logger instance for logging information and errors.
        /// </summary>
        private readonly ILogger<AlarmsController> _logger;


        /// <summary>
        /// Alarm handler that calculates the alarm summary.
        /// </summary>
        private readonly IPpcAlarmHandler _alarmHandler;


        /// <summary>
        /// Constructor of <see cref="AlarmsController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="alarmHandler">Alarm handler that calculates the alarm summary.</param>
        public AlarmsController(ILogger<AlarmsController> logger, IPpcAlarmHandler alarmHandler)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(alarmHandler);

            _logger = logger;
            _alarmHandler = alarmHandler;
        }


        /// <summary>
        /// Gets the number of active alarms per severity level, unacknowledged and acknowledged separately, e.g. for Luna at startup.
        /// The same summary is sent with every alarm action.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token of the request.</param>
        /// <returns>200 with the alarm summary, 503 when the database is not available.</returns>
        [HttpGet]
        [ProducesResponseType<AlarmSummaryDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<AlarmSummaryDto>> Summary(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _alarmHandler.GetSummaryAsync(cancellationToken));
            }
            catch (Exception exception) when (DatabaseTransientExceptionFilter.IsTransient(exception))
            {
                // Only the alarms of publishers whose save failed are loaded from the database
                _logger.LogWarning(exception, "Getting the alarm summary failed, the database is not available.");

                return Problem("The alarm summary could not be calculated, the database is not available.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
