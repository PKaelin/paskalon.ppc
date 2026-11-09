// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Alarms.Application.Queries;
using paskalON.Alarms.Application.UnitTest.SampleData;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.UnitTest.Queries
{
    /// <summary>
    /// Tests <see cref="PpcAlarmQueryHandler"/> with a mocked alarm repository.
    /// </summary>
    [TestClass]
    public class PpcAlarmQueryHandlerTest
    {
        /// <summary>
        /// Earliest creation timestamp of the filter.
        /// </summary>
        private static readonly DateTimeOffset CreatedFrom = new DateTimeOffset(2026, 11, 5, 6, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// Mocked alarm repository.
        /// </summary>
        private readonly Mock<IPpcAlarmRepository> _alarmRepository = new Mock<IPpcAlarmRepository>();


        /// <summary>
        /// The constructor validates the repository.
        /// </summary>
        [TestMethod]
        public void PpcAlarmQueryHandlerConstructorWithoutRepositoryThrowsTest()
        {
            Action create = () => new PpcAlarmQueryHandler(null!);

            Assert.ThrowsExactly<ArgumentNullException>(create);
        }


        /// <summary>
        /// The filter is passed to the repository as domain filter and the alarms are returned as DTOs in the order of the repository.
        /// </summary>
        [TestMethod]
        public async Task GetAlarmsAsyncPassesFilterAndMapsAlarmsInOrderTest()
        {
            PpcAlarmConfig config = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            PpcAlarm newerAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(config, 8, "FaultStates.OverTemperature");
            newerAlarm.Id = 101;
            newerAlarm.DeviceName = "PCS 8";
            newerAlarm.CreatedOn = PcsAlarmConfigSampleData.ChangedOn.AddMinutes(3);
            PpcAlarm olderAlarm = PcsAlarmConfigSampleData.CreateActiveAlarm(config, 7, "FaultStates.OverTemperature");
            olderAlarm.DeviceName = "PCS 7";
            PpcAlarmFilter? passedFilter = null;
            _alarmRepository.Setup(repository => repository.GetAsync(It.IsAny<PpcAlarmFilter>(), It.IsAny<CancellationToken>()))
                .Callback<PpcAlarmFilter, CancellationToken>((filter, _) => passedFilter = filter)
                .ReturnsAsync(new List<PpcAlarm> { newerAlarm, olderAlarm });
            PpcAlarmQueryHandler handler = new PpcAlarmQueryHandler(_alarmRepository.Object);
            PpcAlarmFilterDto filterDto = new PpcAlarmFilterDto
            {
                CreatedFrom = CreatedFrom,
                SeverityLevel = AlarmSeverityDto.Error,
                AlarmSequenceState = AlarmSequenceStateDto.Alarm,
                DeviceName = "pcs"
            };

            List<PpcAlarmDto> alarms = await handler.GetAlarmsAsync(filterDto);

            PpcAlarmFilter expectedFilter = new PpcAlarmFilter
            {
                CreatedFrom = CreatedFrom,
                SeverityLevel = AlarmSeverity.Error,
                AlarmSequenceState = AlarmSequenceState.Alarm,
                DeviceName = "pcs"
            };
            Assert.AreEqual(expectedFilter, passedFilter);
            CollectionAssert.AreEqual(new List<int> { 101, 100 }, alarms.Select(alarm => alarm.Id).ToList());
            CollectionAssert.AreEqual(new List<string?> { "PCS 8", "PCS 7" }, alarms.Select(alarm => alarm.DeviceName).ToList());
        }


        /// <summary>
        /// Without matching alarms an empty list is returned.
        /// </summary>
        [TestMethod]
        public async Task GetAlarmsAsyncWithoutMatchingAlarmsReturnsEmptyListTest()
        {
            _alarmRepository.Setup(repository => repository.GetAsync(It.IsAny<PpcAlarmFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PpcAlarm>());
            PpcAlarmQueryHandler handler = new PpcAlarmQueryHandler(_alarmRepository.Object);

            List<PpcAlarmDto> alarms = await handler.GetAlarmsAsync(new PpcAlarmFilterDto { CreatedFrom = CreatedFrom });

            Assert.IsEmpty(alarms);
        }
    }
}
