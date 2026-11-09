// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Alarms.Service.Controllers.V1;

namespace paskalON.Alarms.Service.UnitTest.Controllers.V1
{
    /// <summary>
    /// Tests <see cref="AlarmsController"/> with a mocked alarm handler.
    /// </summary>
    [TestClass]
    public class AlarmsControllerTest
    {
        /// <summary>
        /// Mocked alarm handler.
        /// </summary>
        private readonly Mock<IPpcAlarmHandler> _alarmHandler = new Mock<IPpcAlarmHandler>();


        /// <summary>
        /// The summary of the alarm handler is returned.
        /// </summary>
        [TestMethod]
        public async Task SummaryReturnsAlarmSummaryTest()
        {
            AlarmSummaryDto summary = new AlarmSummaryDto
            {
                CalculatedOn = new DateTimeOffset(2026, 11, 7, 11, 0, 0, TimeSpan.Zero),
                InfoCount = 2,
                WarningCount = 1,
                ErrorCount = 7,
                CriticalCount = 0,
                AcknowledgedInfoCount = 0,
                AcknowledgedWarningCount = 0,
                AcknowledgedErrorCount = 0,
                AcknowledgedCriticalCount = 0
            };
            _alarmHandler.Setup(handler => handler.GetSummaryAsync(It.IsAny<CancellationToken>())).ReturnsAsync(summary);
            AlarmsController controller = CreateController();

            ActionResult<AlarmSummaryDto> result = await controller.Summary(CancellationToken.None);

            OkObjectResult okResult = (OkObjectResult)result.Result!;
            Assert.AreSame(summary, okResult.Value);
        }


        /// <summary>
        /// When the database is not available to load invalidated alarms the summary returns 503.
        /// </summary>
        [TestMethod]
        public async Task SummaryDatabaseNotAvailableReturnsServiceUnavailableTest()
        {
            _alarmHandler.Setup(handler => handler.GetSummaryAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            AlarmsController controller = CreateController();

            ActionResult<AlarmSummaryDto> result = await controller.Summary(CancellationToken.None);

            ObjectResult problemResult = (ObjectResult)result.Result!;
            Assert.AreEqual(StatusCodes.Status503ServiceUnavailable, problemResult.StatusCode);
        }


        /// <summary>
        /// An unexpected error is not hidden as unavailable database.
        /// </summary>
        [TestMethod]
        public async Task SummaryUnexpectedErrorThrowsTest()
        {
            _alarmHandler.Setup(handler => handler.GetSummaryAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("The active alarms have not been loaded."));
            AlarmsController controller = CreateController();

            Func<Task> summary = () => controller.Summary(CancellationToken.None);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(summary);
        }


        /// <summary>
        /// Creates the controller with an HTTP context, like in a request.
        /// </summary>
        private AlarmsController CreateController()
        {
            return new AlarmsController(NullLogger<AlarmsController>.Instance, _alarmHandler.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }
    }
}
