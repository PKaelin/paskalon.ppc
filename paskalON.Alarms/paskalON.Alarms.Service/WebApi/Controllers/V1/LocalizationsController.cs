// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using paskalON.Alarms.Application.Localizations;
using paskalON.Alarms.Domain.Configs.Localizations;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Hosting;

namespace paskalON.Alarms.Service.Controllers.V1
{
    /// <summary>
    /// Controller of the texts of the localization keys used by the alarm configurations.
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class LocalizationsController : ControllerBase
    {
        /// <summary>
        /// The logger instance for logging information and errors.
        /// </summary>
        private readonly ILogger<LocalizationsController> _logger;


        /// <summary>
        /// Query handler that reads the localized texts.
        /// </summary>
        private readonly ILocalizedTextQueryHandler _queryHandler;


        /// <summary>
        /// Constructor of <see cref="LocalizationsController"/>.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        /// <param name="queryHandler">Query handler that reads the localized texts.</param>
        public LocalizationsController(ILogger<LocalizationsController> logger, ILocalizedTextQueryHandler queryHandler)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queryHandler);

            _logger = logger;
            _queryHandler = queryHandler;
        }


        /// <summary>
        /// Gets the texts of all localization keys in the language, e.g. for Luna to show the alarm names and descriptions.
        /// A key without a text in the language has the text of the default language (en).
        /// </summary>
        /// <param name="language">Language of the texts, e.g. en or de-CH.</param>
        /// <param name="cancellationToken">Cancellation token of the request.</param>
        /// <returns>200 with the texts by their localization key, 400 for an invalid language, 503 when the database is not available.</returns>
        [HttpGet]
        [ProducesResponseType<LocalizedTextsDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<LocalizedTextsDto>> Texts([FromQuery, BindRequired] string language, CancellationToken cancellationToken)
        {
            if (LocalizedText.IsValidLanguage(language) is false)
            {
                return Problem($"The language '{language}' is not valid, expected e.g. en or de-CH.", statusCode: StatusCodes.Status400BadRequest);
            }

            try
            {
                return Ok(await _queryHandler.GetTextsAsync(language, cancellationToken));
            }
            catch (Exception exception) when (DatabaseTransientExceptionFilter.IsTransient(exception))
            {
                _logger.LogWarning(exception, "Getting the localized texts failed, the database is not available.");

                return Problem("The localized texts could not be read, the database is not available.",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
