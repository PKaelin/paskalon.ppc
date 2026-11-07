// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Actions;
using paskalON.Alarms.Application.UnitTest.SampleData;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Actions;

namespace paskalON.Alarms.Application.UnitTest.Actions
{
    /// <summary>
    /// Tests <see cref="AlarmActionDtoFactory"/>.
    /// </summary>
    [TestClass]
    public class AlarmActionDtoFactoryTest
    {
        /// <summary>
        /// Only the actions of the trigger are created, with the configured name.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmActionTrigger.Raise, "ReducePower")]
        [DataRow(AlarmActionTrigger.Normalize, "ResetPower")]
        public void CreateReturnsActionsOfTriggerTest(AlarmActionTrigger trigger, string expectedName)
        {
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(PcsAlarmConfigSampleData.CreatePcsFaultConfig(), 7, "FaultStates.OverTemperature");

            List<AlarmActionDto> actions = AlarmActionDtoFactory.Create(alarm, trigger);

            Assert.HasCount(1, actions);
            Assert.AreEqual(expectedName, actions[0].Name);
        }


        /// <summary>
        /// The placeholders of the parameter values are replaced with the values of the alarm, other text is not changed.
        /// </summary>
        [TestMethod]
        public void CreateReplacesPlaceholdersTest()
        {
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(PcsAlarmConfigSampleData.CreatePcsFaultConfig(), 7, "FaultStates.OverTemperature");

            List<AlarmActionDto> actions = AlarmActionDtoFactory.Create(alarm, AlarmActionTrigger.Raise);

            List<AlarmActionParameterDto> parameters = actions[0].AlarmActionParameters;
            Assert.HasCount(3, parameters);
            Assert.AreEqual("DeviceId", parameters[0].Key);
            Assert.AreEqual("7", parameters[0].Value);
            Assert.AreEqual("Percentage", parameters[1].Key);
            Assert.AreEqual("50", parameters[1].Value);
            Assert.AreEqual("Alarm FaultStates.OverTemperature of PCS 7", parameters[2].Value);
        }


        /// <summary>
        /// One action shared by two configurations creates the action with the values of each alarm and its configuration.
        /// </summary>
        [TestMethod]
        [DataRow(PpcAlarmType.PcsAlarm, "PCS-COMM", AlarmSeverity.Error, 7, "PCS-COMM|PcsAlarm|Error|7|CommunicationError")]
        [DataRow(PpcAlarmType.BatteryAlarm, "BB-COMM", AlarmSeverity.Critical, 11, "BB-COMM|BatteryAlarm|Critical|11|CommunicationError")]
        public void CreateSharedActionReplacesPlaceholdersOfConfigTest(PpcAlarmType ppcAlarmType, string alarmId, AlarmSeverity severityLevel,
            int deviceId, string expectedValue)
        {
            AlarmActionConfig showAlarm = new AlarmActionConfig
            {
                Id = 30,
                ChangedBy = "Homer",
                ChangedDate = new DateTimeOffset(2026, 11, 6, 8, 0, 0, TimeSpan.Zero),
                Name = "ShowAlarm",
                Trigger = AlarmActionTrigger.Raise
            };
            showAlarm.AlarmActionParameters.Add(new AlarmActionParameterConfig
            {
                Id = 31,
                ChangedBy = "Homer",
                ChangedDate = showAlarm.ChangedDate,
                AlarmActionConfig = showAlarm,
                Key = "Alarm",
                Value = "{{AlarmId}}|{{PpcAlarmType}}|{{SeverityLevel}}|{{DeviceId}}|{{PropertyName}}"
            });
            PpcAlarmConfig config = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            config.AlarmActions.Clear();
            config.AlarmActions.Add(showAlarm);
            config.AlarmId = alarmId;
            config.PpcAlarmType = ppcAlarmType;
            config.SeverityLevel = severityLevel;
            config.PropertyName = "CommunicationError";
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(config, deviceId, "CommunicationError");

            List<AlarmActionDto> actions = AlarmActionDtoFactory.Create(alarm, AlarmActionTrigger.Raise);

            Assert.AreEqual("ShowAlarm", actions.Single().Name);
            Assert.AreEqual(expectedValue, actions.Single().AlarmActionParameters.Single().Value);
        }


        /// <summary>
        /// The device id placeholder of an alarm without a device (system alarm) is replaced with an empty value.
        /// </summary>
        [TestMethod]
        public void CreateAlarmWithoutDeviceReplacesDeviceIdWithEmptyTest()
        {
            PpcAlarmConfig config = PcsAlarmConfigSampleData.CreatePcsFaultConfig();
            config.PpcAlarmType = PpcAlarmType.SystemAlarm;
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(config, null, "FaultStates.OverTemperature");

            List<AlarmActionDto> actions = AlarmActionDtoFactory.Create(alarm, AlarmActionTrigger.Raise);

            Assert.AreEqual(string.Empty, actions[0].AlarmActionParameters[0].Value);
        }


        /// <summary>
        /// Every created action gets a new unique message id, so that recipients can ignore duplicated deliveries.
        /// </summary>
        [TestMethod]
        public void CreateAssignsNewMessageIdsTest()
        {
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(PcsAlarmConfigSampleData.CreatePcsFaultConfig(), 7, "FaultStates.OverTemperature");

            Guid first = AlarmActionDtoFactory.Create(alarm, AlarmActionTrigger.Raise)[0].MessageId;
            Guid second = AlarmActionDtoFactory.Create(alarm, AlarmActionTrigger.Raise)[0].MessageId;

            Assert.AreNotEqual(Guid.Empty, first);
            Assert.AreNotEqual(first, second);
        }
    }
}
