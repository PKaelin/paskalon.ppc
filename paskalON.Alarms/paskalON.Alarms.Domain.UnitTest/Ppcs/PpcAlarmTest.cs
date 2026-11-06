// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Domain.UnitTest.Ppcs
{
    /// <summary>
    /// Tests the alarm sequence of <see cref="PpcAlarm"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmTest
    {
        /// <summary>
        /// Timestamp when the alarm was raised.
        /// </summary>
        private static readonly DateTimeOffset RaisedOn = new DateTimeOffset(2026, 11, 5, 10, 45, 50, TimeSpan.Zero);


        /// <summary>
        /// Timestamp when the alarm was changed.
        /// </summary>
        private static readonly DateTimeOffset ChangedOn = new DateTimeOffset(2026, 11, 5, 10, 47, 12, TimeSpan.Zero);


        /// <summary>
        /// A raised alarm is active, abnormal and in the sequence state alarm.
        /// </summary>
        [TestMethod]
        public void PpcAlarmRaisedIsActiveAndAbnormalTest()
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Error);

            bool isActive = alarm.IsActive;

            Assert.IsTrue(isActive);
            Assert.AreEqual(AlarmSequenceState.Alarm, alarm.AlarmSequenceState);
            Assert.AreEqual(AlarmProcessCondition.Abnormal, alarm.AlarmProcessCondition);
            Assert.AreEqual(AlarmSeverity.Error, alarm.SeverityLevel);
        }


        /// <summary>
        /// A non critical alarm gets acknowledged and stays abnormal.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSeverity.Info)]
        [DataRow(AlarmSeverity.Warning)]
        [DataRow(AlarmSeverity.Error)]
        public void AcknowledgeNonCriticalAlarmSetsAcknowledgedTest(AlarmSeverity severity)
        {
            PpcAlarm alarm = CreateAlarm(severity);

            alarm.Acknowledge("Homer", ChangedOn);

            Assert.AreEqual(AlarmSequenceState.Acknowledged, alarm.AlarmSequenceState);
            Assert.AreEqual(AlarmProcessCondition.Abnormal, alarm.AlarmProcessCondition);
            Assert.AreEqual("Homer", alarm.ChangedBy);
            Assert.AreEqual(ChangedOn, alarm.ChangedDate);
            Assert.IsTrue(alarm.IsActive);
        }


        /// <summary>
        /// A critical alarm cannot be acknowledged and stays in the sequence state alarm.
        /// </summary>
        [TestMethod]
        public void AcknowledgeCriticalAlarmThrowsTest()
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Critical);

            bool canBeAcknowledged = alarm.CanBeAcknowledged;

            Assert.IsFalse(canBeAcknowledged);
            Assert.ThrowsExactly<InvalidOperationException>(() => alarm.Acknowledge("Homer", ChangedOn));
            Assert.AreEqual(AlarmSequenceState.Alarm, alarm.AlarmSequenceState);
        }


        /// <summary>
        /// Acknowledging an acknowledged or normalized alarm does nothing.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSequenceState.Acknowledged)]
        [DataRow(AlarmSequenceState.Normal)]
        public void AcknowledgeNotInAlarmStateDoesNothingTest(AlarmSequenceState state)
        {
            PpcAlarm alarm = CreateAlarmInState(state);
            string changedBy = alarm.ChangedBy;

            alarm.Acknowledge("Marge", ChangedOn);

            Assert.AreEqual(state, alarm.AlarmSequenceState);
            Assert.AreEqual(changedBy, alarm.ChangedBy);
            Assert.AreEqual(RaisedOn, alarm.ChangedDate);
            Assert.IsFalse(alarm.CanBeAcknowledged);
        }


        /// <summary>
        /// Acknowledging a normalized critical alarm does nothing instead of throwing.
        /// </summary>
        [TestMethod]
        public void AcknowledgeNormalizedCriticalAlarmDoesNothingTest()
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Critical);
            alarm.Normalize("System", RaisedOn);

            alarm.Acknowledge("Marge", ChangedOn);

            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
            Assert.AreEqual("System", alarm.ChangedBy);
        }


        /// <summary>
        /// A system alarm is not a device alarm and has no device id.
        /// </summary>
        [TestMethod]
        public void PpcAlarmSystemAlarmHasNoDeviceTest()
        {
            PpcAlarmConfig config = CreateConfig(AlarmSeverity.Error, PpcAlarmType.SystemAlarm, null, "IsRtuCommunicationLost");

            PpcAlarm alarm = new PpcAlarm
            {
                ChangedBy = "Homer",
                CreatedOn = RaisedOn,
                PropertyName = "IsRtuCommunicationLost",
                PpcAlarmConfig = config
            };

            Assert.IsNull(config.DeviceId);
            Assert.IsNull(alarm.DeviceId);
            Assert.IsTrue(alarm.IsActive);
        }


        /// <summary>
        /// An active alarm, acknowledged or not, gets normalized when the process condition reverts to normal.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSequenceState.Alarm)]
        [DataRow(AlarmSequenceState.Acknowledged)]
        public void NormalizeActiveAlarmSetsNormalTest(AlarmSequenceState state)
        {
            PpcAlarm alarm = CreateAlarmInState(state);

            alarm.Normalize("Homer", ChangedOn);

            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
            Assert.AreEqual(AlarmProcessCondition.Normal, alarm.AlarmProcessCondition);
            Assert.AreEqual("Homer", alarm.ChangedBy);
            Assert.AreEqual(ChangedOn, alarm.ChangedDate);
            Assert.IsFalse(alarm.IsActive);
        }


        /// <summary>
        /// A critical alarm gets normalized without an acknowledgement.
        /// </summary>
        [TestMethod]
        public void NormalizeCriticalAlarmSetsNormalTest()
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Critical);

            alarm.Normalize("Homer", ChangedOn);

            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
        }


        /// <summary>
        /// A normalized alarm is final and cannot be normalized again.
        /// </summary>
        [TestMethod]
        public void NormalizeNormalizedAlarmThrowsTest()
        {
            PpcAlarm alarm = CreateAlarmInState(AlarmSequenceState.Normal);

            Action normalize = () => alarm.Normalize("Homer", ChangedOn);

            Assert.ThrowsExactly<InvalidOperationException>(normalize);
        }


        /// <summary>
        /// The user or system that changes the alarm is required.
        /// </summary>
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void AcknowledgeWithoutChangedByThrowsTest(string changedBy)
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Error);

            Action acknowledge = () => alarm.Acknowledge(changedBy, ChangedOn);

            Assert.ThrowsExactly<ArgumentException>(acknowledge);
            Assert.AreEqual(AlarmSequenceState.Alarm, alarm.AlarmSequenceState);
        }


        /// <summary>
        /// Creates a raised PCS fault state alarm of a dictionary configuration.
        /// </summary>
        private PpcAlarm CreateAlarm(AlarmSeverity severity)
        {
            PpcAlarmConfig config = CreateConfig(severity, PpcAlarmType.PcsAlarm, 7, "FaultStates");

            return new PpcAlarm
            {
                Id = 42,
                ChangedBy = "Homer",
                ChangedDate = RaisedOn,
                CreatedOn = RaisedOn,
                DeviceId = 7,
                DeviceName = "PCS 1",
                PropertyName = "FaultStates.OverTemperature",
                PpcAlarmConfigId = config.Id,
                PpcAlarmConfig = config
            };
        }


        /// <summary>
        /// Creates an enabled alarm configuration that raises the alarm when the property is true.
        /// </summary>
        private PpcAlarmConfig CreateConfig(AlarmSeverity severity, PpcAlarmType type, int? deviceId, string propertyName)
        {
            return new PpcAlarmConfig
            {
                Id = 1,
                ChangedBy = "Homer",
                IsEnabled = true,
                AlarmId = $"{type}-{propertyName}",
                Name = $"{{{{{type}.{propertyName}.Name}}}}",
                SeverityLevel = severity,
                Description = $"{{{{{type}.{propertyName}.Desc}}}}",
                PpcAlarmType = type,
                DeviceId = deviceId,
                PropertyName = propertyName,
                PropertyOperator = PropertyOperator.Equal,
                PropertyValue = 1
            };
        }


        /// <summary>
        /// Creates a raised error alarm and moves it into the given sequence state.
        /// </summary>
        private PpcAlarm CreateAlarmInState(AlarmSequenceState state)
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Error);

            if (state == AlarmSequenceState.Acknowledged)
            {
                alarm.Acknowledge("Homer", RaisedOn);
            }

            if (state == AlarmSequenceState.Normal)
            {
                alarm.Normalize("System", RaisedOn);
            }

            return alarm;
        }
    }
}
