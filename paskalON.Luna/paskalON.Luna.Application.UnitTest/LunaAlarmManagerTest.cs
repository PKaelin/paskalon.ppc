// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Alarms.Client;
using paskalON.Alarms.Dto.Actions;
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
        /// Creates the manager.
        /// </summary>
        private LunaAlarmManager CreateManager()
        {
            return new LunaAlarmManager(NullLogger<LunaAlarmManager>.Instance, _alarmServer.Object);
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
                CriticalCount = 0
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
