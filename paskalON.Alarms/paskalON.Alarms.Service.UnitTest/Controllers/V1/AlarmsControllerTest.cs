// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Queries;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Ppcs;
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
        /// Mocked alarm query handler.
        /// </summary>
        private readonly Mock<IPpcAlarmQueryHandler> _queryHandler = new Mock<IPpcAlarmQueryHandler>();


        /// <summary>
        /// Earliest creation timestamp of the requested alarms.
        /// </summary>
        private static readonly DateTimeOffset CreatedFrom = new DateTimeOffset(2026, 11, 9, 6, 0, 0, TimeSpan.Zero);


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
        /// The filter of the query parameters is passed to the query handler and its alarms are returned.
        /// </summary>
        [TestMethod]
        public async Task PpcAlarmsPassesFilterAndReturnsAlarmsTest()
        {
            List<PpcAlarmDto> alarms = new List<PpcAlarmDto> { CreateAlarm() };
            PpcAlarmFilterDto? passedFilter = null;
            _queryHandler.Setup(handler => handler.GetAlarmsAsync(It.IsAny<PpcAlarmFilterDto>(), It.IsAny<CancellationToken>()))
                .Callback<PpcAlarmFilterDto, CancellationToken>((filter, _) => passedFilter = filter)
                .ReturnsAsync(alarms);
            AlarmsController controller = CreateController();

            ActionResult<List<PpcAlarmDto>> result = await controller.PpcAlarms(CreatedFrom, AlarmSeverityDto.Error, AlarmSequenceStateDto.Alarm, "PCS", CancellationToken.None);

            OkObjectResult okResult = (OkObjectResult)result.Result!;
            Assert.AreSame(alarms, okResult.Value);
            PpcAlarmFilterDto expectedFilter = new PpcAlarmFilterDto
            {
                CreatedFrom = CreatedFrom,
                SeverityLevel = AlarmSeverityDto.Error,
                AlarmSequenceState = AlarmSequenceStateDto.Alarm,
                DeviceName = "PCS"
            };
            Assert.AreEqual(expectedFilter, passedFilter);
        }


        /// <summary>
        /// When the database is not available the alarms return 503.
        /// </summary>
        [TestMethod]
        public async Task PpcAlarmsDatabaseNotAvailableReturnsServiceUnavailableTest()
        {
            _queryHandler.Setup(handler => handler.GetAlarmsAsync(It.IsAny<PpcAlarmFilterDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Database not available."));
            AlarmsController controller = CreateController();

            ActionResult<List<PpcAlarmDto>> result = await controller.PpcAlarms(CreatedFrom, null, null, null, CancellationToken.None);

            ObjectResult problemResult = (ObjectResult)result.Result!;
            Assert.AreEqual(StatusCodes.Status503ServiceUnavailable, problemResult.StatusCode);
        }


        /// <summary>
        /// Creates an active error alarm of the over temperature fault of PCS 7.
        /// </summary>
        private static PpcAlarmDto CreateAlarm()
        {
            return new PpcAlarmDto
            {
                Id = 100,
                PpcAlarmType = PpcAlarmTypeDto.PcsAlarm,
                DeviceId = 7,
                DeviceName = "PCS 7",
                PropertyName = "FaultStates.OverTemperature",
                AlarmSequenceState = AlarmSequenceStateDto.Alarm,
                CanBeAcknowledged = true,
                CreatedOn = CreatedFrom.AddHours(1),
                ChangedBy = "System",
                ChangedDate = CreatedFrom.AddHours(1),
                PpcAlarmConfig = new PpcAlarmConfigDto
                {
                    AlarmId = "PCS-FAULT",
                    Name = "{{PCS.Fault.Name}}",
                    SeverityLevel = AlarmSeverityDto.Error,
                    IsAcknowledgeable = true,
                    Description = "{{PCS.Fault.Desc}}",
                    PossibleCauses = "{{PCS.Fault.Causes}}"
                }
            };
        }


        /// <summary>
        /// Creates the controller with an HTTP context, like in a request.
        /// </summary>
        private AlarmsController CreateController()
        {
            return new AlarmsController(NullLogger<AlarmsController>.Instance, _alarmHandler.Object, _queryHandler.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }
    }
}
