// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Alarms.Client;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Alarms.Dto.Summaries;

namespace paskalON.Luna.Application.UnitTest
{
    /// <summary>
    /// Tests <see cref="LunaAlarmManager"/> with a mocked alarm service API.
    /// </summary>
    [TestClass]
    public sealed class LunaAlarmManagerTest
    {
        /// <summary>
        /// Time of the first summary calculation of the alarm service.
        /// </summary>
        private static readonly DateTimeOffset CalculatedOn = new DateTimeOffset(2026, 11, 7, 13, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// Mocked alarm service API.
        /// </summary>
        private readonly Mock<IAlarmServer> _alarmServer = new Mock<IAlarmServer>();


        /// <summary>
        /// Clock of the service, the period of the alarm list ends now.
        /// </summary>
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 11, 9, 14, 0, 0, TimeSpan.Zero));


        /// <summary>
        /// Luna receives the show and clear alarm actions.
        /// </summary>
        [TestMethod]
        public void ActionNamesAreShowAndClearAlarmTest()
        {
            LunaAlarmManager manager = CreateManager();

            IReadOnlyCollection<string> actionNames = manager.ActionNames;

            CollectionAssert.AreEquivalent(new string[] { "RaiseAlarm", "ClearAlarm" }, actionNames.ToArray());
        }


        /// <summary>
        /// Before the first summary was received there is no summary.
        /// </summary>
        [TestMethod]
        public void GetSummaryBeforeFirstSummaryReturnsNullTest()
        {
            LunaAlarmManager manager = CreateManager();

            AlarmSummaryDto? summary = manager.GetSummary();

            Assert.IsNull(summary);
        }


        /// <summary>
        /// The summary of a received action is kept.
        /// </summary>
        [TestMethod]
        [DataRow("RaiseAlarm")]
        [DataRow("ClearAlarm")]
        public async Task HandleAsyncKeepsSummaryOfActionTest(string actionName)
        {
            LunaAlarmManager manager = CreateManager();
            AlarmSummaryDto summary = CreateSummary(CalculatedOn, 3);

            await manager.HandleAsync(CreateAction(actionName, summary));

            Assert.AreSame(summary, manager.GetSummary());
        }


        /// <summary>
        /// A summary calculated before the current summary (e.g. a delayed message) is ignored, a newer one replaces it.
        /// </summary>
        [TestMethod]
        [DataRow(-1, 3)]
        [DataRow(0, 3)]
        [DataRow(1, 4)]
        public async Task HandleAsyncKeepsLatestSummaryTest(int secondsAfterCurrent, int expectedErrorCount)
        {
            LunaAlarmManager manager = CreateManager();
            await manager.HandleAsync(CreateAction("RaiseAlarm", CreateSummary(CalculatedOn, 3)));

            await manager.HandleAsync(CreateAction("RaiseAlarm", CreateSummary(CalculatedOn.AddSeconds(secondsAfterCurrent), 4)));

            Assert.AreEqual(expectedErrorCount, manager.GetSummary()!.ErrorCount);
        }


        /// <summary>
        /// The change event is raised for a newer summary only, so that the UI clients get every change once.
        /// </summary>
        [TestMethod]
        public async Task HandleAsyncRaisesSummaryChangedForNewerSummaryOnlyTest()
        {
            LunaAlarmManager manager = CreateManager();
            List<AlarmSummaryDto> changedSummaries = new List<AlarmSummaryDto>();
            manager.SummaryChanged += (_, summary) => changedSummaries.Add(summary);
            AlarmSummaryDto first = CreateSummary(CalculatedOn, 1);
            AlarmSummaryDto newer = CreateSummary(CalculatedOn.AddSeconds(1), 2);

            await manager.HandleAsync(CreateAction("RaiseAlarm", first));
            await manager.HandleAsync(CreateAction("RaiseAlarm", first));
            await manager.HandleAsync(CreateAction("RaiseAlarm", newer));
            await manager.HandleAsync(CreateAction("ClearAlarm", CreateSummary(CalculatedOn, 0)));

            CollectionAssert.AreEqual(new List<AlarmSummaryDto> { first, newer }, changedSummaries);
        }


        /// <summary>
        /// The synchronization loads the summary from the alarm service API.
        /// </summary>
        [TestMethod]
        public async Task SynchronizeAsyncLoadsSummaryFromAlarmServiceTest()
        {
            AlarmSummaryDto summary = CreateSummary(CalculatedOn, 2);
            _alarmServer.Setup(server => server.GetAlarmSummary(It.IsAny<CancellationToken>())).ReturnsAsync(summary);
            LunaAlarmManager manager = CreateManager();

            await manager.SynchronizeAsync();

            Assert.AreSame(summary, manager.GetSummary());
        }


        /// <summary>
        /// A loaded summary that is older than an action received meanwhile does not replace the summary of the action.
        /// </summary>
        [TestMethod]
        public async Task SynchronizeAsyncOlderSummaryThanActionIsIgnoredTest()
        {
            _alarmServer.Setup(server => server.GetAlarmSummary(It.IsAny<CancellationToken>())).ReturnsAsync(CreateSummary(CalculatedOn, 2));
            LunaAlarmManager manager = CreateManager();
            await manager.HandleAsync(CreateAction("ClearAlarm", CreateSummary(CalculatedOn.AddSeconds(1), 1)));

            await manager.SynchronizeAsync();

            Assert.AreEqual(1, manager.GetSummary()!.ErrorCount);
        }


        /// <summary>
        /// When the alarm service is not reachable the synchronization throws (it is retried) and the summary stays unchanged.
        /// </summary>
        [TestMethod]
        public async Task SynchronizeAsyncAlarmServiceNotReachableThrowsTest()
        {
            _alarmServer.Setup(server => server.GetAlarmSummary(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Alarm service not reachable."));
            LunaAlarmManager manager = CreateManager();

            Func<Task> synchronize = () => manager.SynchronizeAsync();

            await Assert.ThrowsExactlyAsync<HttpRequestException>(synchronize);
            Assert.IsNull(manager.GetSummary());
        }


        /// <summary>
        /// The period before now is sent as creation timestamp with the criteria, the device name trimmed, and the alarms are returned.
        /// </summary>
        [TestMethod]
        public async Task GetPpcAlarmsAsyncSendsFilterOfPeriodAndReturnsAlarmsTest()
        {
            List<PpcAlarmDto> alarms = new List<PpcAlarmDto> { CreateAlarm() };
            PpcAlarmFilterDto? sentFilter = null;
            _alarmServer.Setup(server => server.GetPpcAlarms(It.IsAny<PpcAlarmFilterDto>(), It.IsAny<CancellationToken>()))
                .Callback<PpcAlarmFilterDto, CancellationToken>((filter, _) => sentFilter = filter)
                .ReturnsAsync(alarms);
            LunaAlarmManager manager = CreateManager();

            List<PpcAlarmDto> result = await manager.GetPpcAlarmsAsync(TimeSpan.FromHours(8), AlarmSeverityDto.Error, AlarmProcessConditionDto.Abnormal,
                AlarmSequenceStateDto.Alarm, " PCS 7 ");

            Assert.AreSame(alarms, result);
            PpcAlarmFilterDto expectedFilter = new PpcAlarmFilterDto
            {
                CreatedFrom = new DateTimeOffset(2026, 11, 9, 6, 0, 0, TimeSpan.Zero),
                SeverityLevel = AlarmSeverityDto.Error,
                AlarmProcessCondition = AlarmProcessConditionDto.Abnormal,
                AlarmSequenceState = AlarmSequenceStateDto.Alarm,
                DeviceName = "PCS 7"
            };
            Assert.AreEqual(expectedFilter, sentFilter);
        }


        /// <summary>
        /// Without criteria only the creation timestamp is set, an empty device name is not sent.
        /// </summary>
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task GetPpcAlarmsAsyncWithoutCriteriaSendsCreatedFromOnlyTest(string? deviceName)
        {
            PpcAlarmFilterDto? sentFilter = null;
            _alarmServer.Setup(server => server.GetPpcAlarms(It.IsAny<PpcAlarmFilterDto>(), It.IsAny<CancellationToken>()))
                .Callback<PpcAlarmFilterDto, CancellationToken>((filter, _) => sentFilter = filter)
                .ReturnsAsync(new List<PpcAlarmDto>());
            LunaAlarmManager manager = CreateManager();

            await manager.GetPpcAlarmsAsync(TimeSpan.FromDays(30), null, null, null, deviceName);

            PpcAlarmFilterDto expectedFilter = new PpcAlarmFilterDto { CreatedFrom = new DateTimeOffset(2026, 10, 10, 14, 0, 0, TimeSpan.Zero) };
            Assert.AreEqual(expectedFilter, sentFilter);
        }


        /// <summary>
        /// A period that is not positive is rejected without calling the alarm service.
        /// </summary>
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task GetPpcAlarmsAsyncNotPositivePeriodThrowsTest(int hours)
        {
            LunaAlarmManager manager = CreateManager();

            Func<Task> getAlarms = () => manager.GetPpcAlarmsAsync(TimeSpan.FromHours(hours), null, null, null, null);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(getAlarms);
            _alarmServer.Verify(server => server.GetPpcAlarms(It.IsAny<PpcAlarmFilterDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// When the alarm service is not reachable the exception is passed on, so that the controller answers with the error.
        /// </summary>
        [TestMethod]
        public async Task GetPpcAlarmsAsyncAlarmServiceNotReachableThrowsTest()
        {
            _alarmServer.Setup(server => server.GetPpcAlarms(It.IsAny<PpcAlarmFilterDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Alarm service not reachable."));
            LunaAlarmManager manager = CreateManager();

            Func<Task> getAlarms = () => manager.GetPpcAlarmsAsync(TimeSpan.FromHours(8), null, null, null, null);

            await Assert.ThrowsExactlyAsync<HttpRequestException>(getAlarms);
        }


        /// <summary>
        /// The texts of the trimmed language are read from the alarm service and returned.
        /// </summary>
        [TestMethod]
        public async Task GetLocalizedTextsAsyncReturnsTextsOfAlarmServiceTest()
        {
            LocalizedTextsDto texts = new LocalizedTextsDto
            {
                Language = "en",
                Texts = new Dictionary<string, string>
                {
                    ["PCS.CommunicationError.Name"] = "PCS communication error",
                    ["PCS.CommunicationError.Desc"] = "The PPC cannot communicate with the power conversion system."
                }
            };
            _alarmServer.Setup(server => server.GetLocalizedTexts("en", It.IsAny<CancellationToken>())).ReturnsAsync(texts);
            LunaAlarmManager manager = CreateManager();

            LocalizedTextsDto result = await manager.GetLocalizedTextsAsync(" en ");

            Assert.AreSame(texts, result);
        }


        /// <summary>
        /// An empty language is rejected without a request to the alarm service.
        /// </summary>
        /// <param name="language">Empty language.</param>
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetLocalizedTextsAsyncEmptyLanguageThrowsTest(string language)
        {
            LunaAlarmManager manager = CreateManager();

            Func<Task> getTexts = () => manager.GetLocalizedTextsAsync(language);

            await Assert.ThrowsExactlyAsync<ArgumentException>(getTexts);
            _alarmServer.Verify(server => server.GetLocalizedTexts(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// When the alarm service is not reachable the exception is passed on, so that the controller answers with the error.
        /// </summary>
        [TestMethod]
        public async Task GetLocalizedTextsAsyncAlarmServiceNotReachableThrowsTest()
        {
            _alarmServer.Setup(server => server.GetLocalizedTexts(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Alarm service not reachable."));
            LunaAlarmManager manager = CreateManager();

            Func<Task> getTexts = () => manager.GetLocalizedTextsAsync("en");

            await Assert.ThrowsExactlyAsync<HttpRequestException>(getTexts);
        }


        /// <summary>
        /// Creates the manager.
        /// </summary>
        private LunaAlarmManager CreateManager()
        {
            return new LunaAlarmManager(NullLogger<LunaAlarmManager>.Instance, _alarmServer.Object, _timeProvider);
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
                AlarmProcessCondition = AlarmProcessConditionDto.Abnormal,
                CreatedOn = new DateTimeOffset(2026, 11, 9, 13, 0, 0, TimeSpan.Zero),
                ChangedBy = "System",
                ChangedDate = new DateTimeOffset(2026, 11, 9, 13, 0, 0, TimeSpan.Zero),
                PpcAlarmConfig = new PpcAlarmConfigDto
                {
                    AlarmId = "PCS-FAULT",
                    Name = "{{PCS.Fault.Name}}",
                    SeverityLevel = AlarmSeverityDto.Error,
                    Description = "{{PCS.Fault.Desc}}",
                    PossibleCauses = "{{PCS.Fault.Causes}}"
                }
            };
        }


        /// <summary>
        /// Creates a summary with error alarms only.
        /// </summary>
        private static AlarmSummaryDto CreateSummary(DateTimeOffset calculatedOn, int errorCount)
        {
            return new AlarmSummaryDto
            {
                CalculatedOn = calculatedOn,
                InfoCount = 0,
                WarningCount = 0,
                ErrorCount = errorCount,
                CriticalCount = 0,
                AcknowledgedInfoCount = 0,
                AcknowledgedWarningCount = 0,
                AcknowledgedErrorCount = 0,
                AcknowledgedCriticalCount = 0
            };
        }


        /// <summary>
        /// Creates an action of a PCS communication error like sent by the alarm service.
        /// </summary>
        private static AlarmActionDto CreateAction(string actionName, AlarmSummaryDto summary)
        {
            return new AlarmActionDto
            {
                MessageId = Guid.CreateVersion7(),
                Name = actionName,
                AlarmActionParameters = new List<AlarmActionParameterDto>
                {
                    new AlarmActionParameterDto { Key = "AlarmId", Value = "PCS-COMM" },
                    new AlarmActionParameterDto { Key = "SeverityLevel", Value = "Error" },
                    new AlarmActionParameterDto { Key = "DeviceId", Value = "1" }
                },
                AlarmSummary = summary
            };
        }
    }
}
