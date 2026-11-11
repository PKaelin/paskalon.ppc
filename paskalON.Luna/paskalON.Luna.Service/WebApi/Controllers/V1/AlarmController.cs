// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Luna.Application;

namespace paskalON.Luna.Service.WebApi.Controllers.V1
{
    /// <summary>
    /// Controller of the alarm list and the alarm texts, forwarded to the alarm service API.
    /// </summary>
    /// <remarks>
    /// A request rejected by the alarm service is answered with its status code, an unreachable alarm service with 503.
    /// </remarks>
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class AlarmController : ControllerBase
    {
        /// <summary>
        /// Longest period of the alarm list in hours (30 days).
        /// </summary>
        public const int MaximumTriggeredWithinHours = 720;


        /// <summary>
        /// Logger for handling application logging and diagnostics.
        /// </summary>
        private readonly ILogger<AlarmController> _logger;


        /// <summary>
        /// Alarm manager reading the alarm list of the alarm service.
        /// </summary>
        private readonly ILunaAlarmManager _alarmManager;


        /// <summary>
        /// Constructor of <see cref="AlarmController"/>.
        /// </summary>
        /// <param name="logger">Logger for handling application logging and diagnostics.</param>
        /// <param name="alarmManager">Alarm manager reading the alarm list of the alarm service.</param>
        public AlarmController(ILogger<AlarmController> logger, ILunaAlarmManager alarmManager)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(alarmManager);

            _logger = logger;
            _alarmManager = alarmManager;
        }


        /// <summary>
        /// Gets the PPC alarms that were triggered within the last hours and match the other criteria.
        /// </summary>
        /// <param name="triggeredWithinHours">Period before now in hours in which the alarms were triggered (1 to 720).</param>
        /// <param name="severityLevel">Severity level of the alarms, empty for all severity levels.</param>
        /// <param name="alarmSequenceState">Sequence state of the alarms, empty for all sequence states.</param>
        /// <param name="deviceName">Part of the device name (case insensitive), empty for all alarms.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>
        /// 200 with the matching alarms (the latest triggered alarm first), 400 for an invalid period,
        /// the status code of the alarm service when it rejected the request, 503 when it is not reachable.
        /// </returns>
        [HttpGet]
        [ProducesResponseType<List<PpcAlarmDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<List<PpcAlarmDto>>> GetPpcAlarms([FromQuery, Range(1, MaximumTriggeredWithinHours)] int triggeredWithinHours,
            [FromQuery] AlarmSeverityDto? severityLevel, [FromQuery] AlarmSequenceStateDto? alarmSequenceState, [FromQuery] string? deviceName,
            CancellationToken cancellationToken)
        {
            return await ForwardAsync("alarm list", "Alarms not available", "The alarm service could not provide the alarms.",
                () => _alarmManager.GetPpcAlarmsAsync(TimeSpan.FromHours(triggeredWithinHours), severityLevel, alarmSequenceState, deviceName,
                    cancellationToken), cancellationToken);
        }


        /// <summary>
        /// Gets the texts of the localization keys of the alarm configurations (e.g. {{PCS.CommunicationError.Name}}) in the language of the user.
        /// A key without a text in the language has the English text.
        /// </summary>
        /// <param name="language">Language of the texts, e.g. en or de-CH.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>
        /// 200 with the texts by their localization key without the braces, 400 without a language,
        /// the status code of the alarm service when it rejected the request (e.g. 400 for an invalid language), 503 when it is not reachable.
        /// </returns>
        [HttpGet]
        [ProducesResponseType<LocalizedTextsDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<LocalizedTextsDto>> GetLocalizedTexts([FromQuery, Required] string language, CancellationToken cancellationToken)
        {
            return await ForwardAsync("alarm text", "Alarm texts not available", "The alarm service could not provide the alarm texts.",
                () => _alarmManager.GetLocalizedTextsAsync(language, cancellationToken), cancellationToken);
        }


        /// <summary>
        /// Forwards a request to the alarm service and translates its failures into problem responses.
        /// </summary>
        /// <typeparam name="T">Type of the response of the alarm service.</typeparam>
        /// <param name="requestName">Name of the request for the log, e.g. alarm list.</param>
        /// <param name="title">Title of the problem responses.</param>
        /// <param name="rejectedDetail">Detail of the problem response when the alarm service rejected the request.</param>
        /// <param name="request">Request to the alarm service.</param>
        /// <param name="cancellationToken">Token to cancel the request.</param>
        /// <returns>200 with the response, the status code of the alarm service when it rejected the request, 503 when it is not reachable, 504 on timeout.</returns>
        private async Task<ActionResult<T>> ForwardAsync<T>(string requestName, string title, string rejectedDetail, Func<Task<T>> request,
            CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await request());
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                _logger.LogWarning(ex, "Alarm service rejected the {RequestName} request with status code {StatusCode}.", requestName, ex.StatusCode);

                return Problem(detail: rejectedDetail, statusCode: (int)ex.StatusCode.Value, title: title);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Alarm service is not reachable for the {RequestName} request.", requestName);

                return Problem(detail: "The alarm service is not reachable.", statusCode: StatusCodes.Status503ServiceUnavailable, title: title);
            }
            catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested is false)
            {
                // The timeout of the HTTP client elapsed, the request of the browser was not cancelled
                _logger.LogWarning(ex, "Alarm service did not answer the {RequestName} request in time.", requestName);

                return Problem(detail: "The alarm service did not answer in time.", statusCode: StatusCodes.Status504GatewayTimeout, title: title);
            }
        }
    }
}
