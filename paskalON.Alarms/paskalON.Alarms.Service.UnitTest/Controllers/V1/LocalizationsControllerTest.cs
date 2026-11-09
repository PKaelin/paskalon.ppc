// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Application.Localizations;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Service.Controllers.V1;

namespace paskalON.Alarms.Service.UnitTest.Controllers.V1
{
    /// <summary>
    /// Tests <see cref="LocalizationsController"/> with a mocked text query handler.
    /// </summary>
    [TestClass]
    public class LocalizationsControllerTest
    {
        /// <summary>
        /// Mocked text query handler.
        /// </summary>
        private readonly Mock<ILocalizedTextQueryHandler> _queryHandler = new Mock<ILocalizedTextQueryHandler>();


        /// <summary>
        /// The constructor validates its dependencies.
        /// </summary>
        [TestMethod]
        public void LocalizationsControllerConstructorInvalidArgumentsThrowsTest()
        {
            Action withoutLogger = () => new LocalizationsController(null!, _queryHandler.Object);
            Action withoutQueryHandler = () => new LocalizationsController(NullLogger<LocalizationsController>.Instance, null!);

            Assert.ThrowsExactly<ArgumentNullException>(withoutLogger);
            Assert.ThrowsExactly<ArgumentNullException>(withoutQueryHandler);
        }


        /// <summary>
        /// The texts of the query handler in the requested language are returned.
        /// </summary>
        [TestMethod]
        public async Task TextsReturnsTextsOfLanguageTest()
        {
            LocalizedTextsDto texts = new LocalizedTextsDto
            {
                Language = "en",
                Texts = new Dictionary<string, string> { ["PCS.CommunicationError.Name"] = "PCS communication error" }
            };
            _queryHandler.Setup(handler => handler.GetTextsAsync("en", It.IsAny<CancellationToken>())).ReturnsAsync(texts);
            LocalizationsController controller = CreateController();

            ActionResult<LocalizedTextsDto> result = await controller.Texts("en", CancellationToken.None);

            OkObjectResult okResult = (OkObjectResult)result.Result!;
            Assert.AreSame(texts, okResult.Value);
        }


        /// <summary>
        /// A language with an invalid format returns 400 without reading the texts.
        /// </summary>
        /// <param name="language">Invalid language.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow("english")]
        public async Task TextsInvalidLanguageReturnsBadRequestTest(string language)
        {
            LocalizationsController controller = CreateController();

            ActionResult<LocalizedTextsDto> result = await controller.Texts(language, CancellationToken.None);

            ObjectResult problemResult = (ObjectResult)result.Result!;
            Assert.AreEqual(StatusCodes.Status400BadRequest, problemResult.StatusCode);
            _queryHandler.Verify(handler => handler.GetTextsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// When the database is not available the texts return 503.
        /// </summary>
        [TestMethod]
        public async Task TextsDatabaseNotAvailableReturnsServiceUnavailableTest()
        {
            _queryHandler.Setup(handler => handler.GetTextsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            LocalizationsController controller = CreateController();

            ActionResult<LocalizedTextsDto> result = await controller.Texts("en", CancellationToken.None);

            ObjectResult problemResult = (ObjectResult)result.Result!;
            Assert.AreEqual(StatusCodes.Status503ServiceUnavailable, problemResult.StatusCode);
        }


        /// <summary>
        /// Creates the controller with the mocked query handler.
        /// </summary>
        /// <returns>The controller.</returns>
        private LocalizationsController CreateController()
        {
            return new LocalizationsController(NullLogger<LocalizationsController>.Instance, _queryHandler.Object);
        }
    }
}
