// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Queries;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Ppcs;
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
        /// Query handler that reads the alarms.
        /// </summary>
        private readonly IPpcAlarmQueryHandler _queryHandler;


        /// <summary>
        /// Constructor of <see cref="AlarmsController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="alarmHandler">Alarm handler that calculates the alarm summary.</param>
        /// <param name="queryHandler">Query handler that reads the alarms.</param>
        public AlarmsController(ILogger<AlarmsController> logger, IPpcAlarmHandler alarmHandler, IPpcAlarmQueryHandler queryHandler)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(alarmHandler);
            ArgumentNullException.ThrowIfNull(queryHandler);

            _logger = logger;
            _alarmHandler = alarmHandler;
            _queryHandler = queryHandler;
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


        /// <summary>
        /// Gets the active and normalized PPC alarms that match the filter, e.g. for the alarm list of Luna.
        /// </summary>
        /// <param name="createdFrom">Earliest creation timestamp of the alarms (inclusive).</param>
        /// <param name="severityLevel">Severity level of the alarm configuration, empty for all severity levels.</param>
        /// <param name="alarmProcessCondition">Process condition of the alarms, empty for all process conditions.</param>
        /// <param name="alarmSequenceState">Sequence state of the alarms, empty for all sequence states.</param>
        /// <param name="deviceName">Part of the device name (case insensitive), empty for all alarms.</param>
        /// <param name="cancellationToken">Cancellation token of the request.</param>
        /// <returns>
        /// 200 with the matching alarms (the latest created alarm first), 400 without a creation timestamp,
        /// 503 when the database is not available.
        /// </returns>
        [HttpGet]
        [ProducesResponseType<List<PpcAlarmDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<List<PpcAlarmDto>>> PpcAlarms([FromQuery, BindRequired] DateTimeOffset createdFrom,
            [FromQuery] AlarmSeverityDto? severityLevel, [FromQuery] AlarmProcessConditionDto? alarmProcessCondition,
            [FromQuery] AlarmSequenceStateDto? alarmSequenceState, [FromQuery] string? deviceName, CancellationToken cancellationToken)
        {
            PpcAlarmFilterDto filter = new PpcAlarmFilterDto
            {
                CreatedFrom = createdFrom,
                SeverityLevel = severityLevel,
                AlarmProcessCondition = alarmProcessCondition,
                AlarmSequenceState = alarmSequenceState,
                DeviceName = deviceName
            };

            try
            {
                return Ok(await _queryHandler.GetAlarmsAsync(filter, cancellationToken));
            }
            catch (Exception exception) when (DatabaseTransientExceptionFilter.IsTransient(exception))
            {
                _logger.LogWarning(exception, "Getting the PPC alarms failed, the database is not available.");

                return Problem("The alarms could not be read, the database is not available.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
