// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Queries;
using paskalON.Alarms.Application.UnitTest.SampleData;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto;
using paskalON.Alarms.Dto.Ppcs;

namespace paskalON.Alarms.Application.UnitTest.Queries
{
    /// <summary>
    /// Tests <see cref="PpcAlarmDtoFactory"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmDtoFactoryTest
    {
        /// <summary>
        /// Earliest creation timestamp of the filters.
        /// </summary>
        private static readonly DateTimeOffset CreatedFrom = new DateTimeOffset(2026, 11, 9, 6, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// An acknowledged alarm is mapped with its configuration, the actions of the configuration are not mapped.
        /// </summary>
        [TestMethod]
        public void CreateMapsAlarmWithConfigWithoutActionsTest()
        {
            PpcAlarmConfig config = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            config.PossibleCauses = "{{PCS.Fault.Causes}}";
            config.Response = "{{PCS.Fault.Response}}";
            config.Consequences = "{{PCS.Fault.Consequences}}";
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(config, 7, "FaultStates.OverTemperature");
            alarm.DeviceName = "PCS 7";
            alarm.Acknowledge("Homer", PcsAlarmConfigSampleData.ChangedOn.AddMinutes(5));

            PpcAlarmDto dto = PpcAlarmDtoFactory.Create(alarm);

            Assert.AreEqual(100, dto.Id);
            Assert.AreEqual(PpcAlarmTypeDto.PcsAlarm, dto.PpcAlarmType);
            Assert.AreEqual(7, dto.DeviceId);
            Assert.AreEqual("PCS 7", dto.DeviceName);
            Assert.AreEqual("FaultStates.OverTemperature", dto.PropertyName);
            Assert.AreEqual(AlarmSequenceStateDto.Acknowledged, dto.AlarmSequenceState);
            Assert.AreEqual("Homer", dto.AcknowledgedBy);
            Assert.AreEqual(PcsAlarmConfigSampleData.ChangedOn.AddMinutes(5), dto.AcknowledgedOn);
            Assert.IsFalse(dto.CanBeAcknowledged);
            Assert.IsTrue(dto.PpcAlarmConfig.IsAcknowledgeable);
            Assert.AreEqual(PcsAlarmConfigSampleData.ChangedOn, dto.CreatedOn);
            Assert.AreEqual("Homer", dto.ChangedBy);
            Assert.AreEqual(PcsAlarmConfigSampleData.ChangedOn.AddMinutes(5), dto.ChangedDate);
            Assert.AreEqual("PCS-FAULT", dto.PpcAlarmConfig.AlarmId);
            Assert.AreEqual("{{PCS.Fault.Name}}", dto.PpcAlarmConfig.Name);
            Assert.AreEqual(AlarmSeverityDto.Error, dto.PpcAlarmConfig.SeverityLevel);
            Assert.AreEqual("{{PCS.Fault.Desc}}", dto.PpcAlarmConfig.Description);
            Assert.AreEqual("{{PCS.Fault.Causes}}", dto.PpcAlarmConfig.PossibleCauses);
            Assert.AreEqual("{{PCS.Fault.Response}}", dto.PpcAlarmConfig.Response);
            Assert.AreEqual("{{PCS.Fault.Consequences}}", dto.PpcAlarmConfig.Consequences);
            Assert.IsEmpty(dto.PpcAlarmConfig.AlarmActions);
        }


        /// <summary>
        /// A normalized, not yet acknowledged system alarm without device is mapped with the normal state, it can still be acknowledged.
        /// </summary>
        [TestMethod]
        public void CreateMapsNormalizedSystemAlarmTest()
        {
            PpcAlarmConfig config = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            config.PpcAlarmType = PpcAlarmType.SystemAlarm;
            config.SeverityLevel = AlarmSeverity.Critical;
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(config, null, "IsRtuCommunicationLost");
            alarm.Normalize("System", PcsAlarmConfigSampleData.ChangedOn.AddMinutes(1));

            PpcAlarmDto dto = PpcAlarmDtoFactory.Create(alarm);

            Assert.AreEqual(PpcAlarmTypeDto.SystemAlarm, dto.PpcAlarmType);
            Assert.IsNull(dto.DeviceId);
            Assert.IsNull(dto.DeviceName);
            Assert.AreEqual(AlarmSequenceStateDto.Normal, dto.AlarmSequenceState);
            Assert.IsNull(dto.AcknowledgedOn);
            Assert.IsTrue(dto.CanBeAcknowledged);
            Assert.AreEqual(AlarmSeverityDto.Critical, dto.PpcAlarmConfig.SeverityLevel);
        }


        /// <summary>
        /// Every criterion of the filter DTO is mapped to the domain filter with the same name.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSeverityDto.Info, AlarmSequenceStateDto.Alarm)]
        [DataRow(AlarmSeverityDto.Warning, AlarmSequenceStateDto.Acknowledged)]
        [DataRow(AlarmSeverityDto.Error, AlarmSequenceStateDto.Normal)]
        [DataRow(AlarmSeverityDto.Critical, AlarmSequenceStateDto.Normal)]
        public void CreateFilterMapsCriteriaTest(AlarmSeverityDto severityLevel, AlarmSequenceStateDto sequenceState)
        {
            PpcAlarmFilterDto filterDto = new PpcAlarmFilterDto
            {
                CreatedFrom = CreatedFrom,
                SeverityLevel = severityLevel,
                AlarmSequenceState = sequenceState,
                DeviceName = "PCS"
            };

            PpcAlarmFilter filter = PpcAlarmDtoFactory.CreateFilter(filterDto);

            Assert.AreEqual(CreatedFrom, filter.CreatedFrom);
            Assert.AreEqual(severityLevel.ToString(), filter.SeverityLevel.ToString());
            Assert.AreEqual(sequenceState.ToString(), filter.AlarmSequenceState.ToString());
            Assert.AreEqual("PCS", filter.DeviceName);
        }


        /// <summary>
        /// Criteria that are not set stay unset, they match every alarm.
        /// </summary>
        [TestMethod]
        public void CreateFilterWithoutCriteriaKeepsCriteriaUnsetTest()
        {
            PpcAlarmFilterDto filterDto = new PpcAlarmFilterDto { CreatedFrom = CreatedFrom };

            PpcAlarmFilter filter = PpcAlarmDtoFactory.CreateFilter(filterDto);

            PpcAlarmFilter expectedFilter = new PpcAlarmFilter { CreatedFrom = CreatedFrom };
            Assert.AreEqual(expectedFilter, filter);
        }
    }
}
