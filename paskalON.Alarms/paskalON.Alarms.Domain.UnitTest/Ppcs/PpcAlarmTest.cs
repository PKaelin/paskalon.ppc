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
        /// A raised alarm is active, in the sequence state alarm and not acknowledged yet.
        /// </summary>
        [TestMethod]
        public void PpcAlarmRaisedIsActiveTest()
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Error);

            bool isActive = alarm.IsActive;

            Assert.IsTrue(isActive);
            Assert.AreEqual(AlarmSequenceState.Alarm, alarm.AlarmSequenceState);
            Assert.AreEqual(AlarmSeverity.Error, alarm.SeverityLevel);
            Assert.IsNull(alarm.AcknowledgedBy);
            Assert.IsNull(alarm.AcknowledgedOn);
            Assert.IsTrue(alarm.CanBeAcknowledged);
        }


        /// <summary>
        /// An active alarm of an acknowledgeable configuration gets acknowledged, whatever its severity (also critical).
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSeverity.Info)]
        [DataRow(AlarmSeverity.Warning)]
        [DataRow(AlarmSeverity.Error)]
        [DataRow(AlarmSeverity.Critical)]
        public void AcknowledgeActiveAlarmSetsAcknowledgedTest(AlarmSeverity severity)
        {
            PpcAlarm alarm = CreateAlarm(severity);

            alarm.Acknowledge("Homer", ChangedOn);

            Assert.AreEqual(AlarmSequenceState.Acknowledged, alarm.AlarmSequenceState);
            Assert.AreEqual("Homer", alarm.AcknowledgedBy);
            Assert.AreEqual(ChangedOn, alarm.AcknowledgedOn);
            Assert.AreEqual("Homer", alarm.ChangedBy);
            Assert.AreEqual(ChangedOn, alarm.ChangedDate);
            Assert.IsTrue(alarm.IsActive);
            Assert.IsFalse(alarm.CanBeAcknowledged);
        }


        /// <summary>
        /// An alarm that is already back to normal can still be acknowledged, it stays normal and records the acknowledgement.
        /// </summary>
        [TestMethod]
        public void AcknowledgeNormalAlarmRecordsAcknowledgementTest()
        {
            PpcAlarm alarm = CreateAlarmInState(AlarmSequenceState.Normal);
            bool canBeAcknowledged = alarm.CanBeAcknowledged;

            alarm.Acknowledge("Marge", ChangedOn);

            Assert.IsTrue(canBeAcknowledged);
            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
            Assert.AreEqual("Marge", alarm.AcknowledgedBy);
            Assert.AreEqual(ChangedOn, alarm.AcknowledgedOn);
            Assert.AreEqual("Marge", alarm.ChangedBy);
            Assert.IsFalse(alarm.CanBeAcknowledged);
        }


        /// <summary>
        /// Acknowledging an alarm again keeps the first acknowledgement, also after it was normalized.
        /// </summary>
        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void AcknowledgeAcknowledgedAlarmKeepsFirstAcknowledgementTest(bool isNormalizedAfterAcknowledgement)
        {
            PpcAlarm alarm = CreateAlarmInState(AlarmSequenceState.Acknowledged);
            AlarmSequenceState expectedState = AlarmSequenceState.Acknowledged;

            if (isNormalizedAfterAcknowledgement)
            {
                alarm.Normalize("System", RaisedOn.AddMinutes(1));
                expectedState = AlarmSequenceState.Normal;
            }

            string changedBy = alarm.ChangedBy;

            alarm.Acknowledge("Marge", ChangedOn);

            Assert.AreEqual(expectedState, alarm.AlarmSequenceState);
            Assert.AreEqual("Homer", alarm.AcknowledgedBy);
            Assert.AreEqual(RaisedOn, alarm.AcknowledgedOn);
            Assert.AreEqual(changedBy, alarm.ChangedBy);
        }


        /// <summary>
        /// An alarm of a configuration that is not acknowledgeable cannot be acknowledged, active or normal.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSequenceState.Alarm)]
        [DataRow(AlarmSequenceState.Normal)]
        public void AcknowledgeNotAcknowledgeableAlarmThrowsTest(AlarmSequenceState state)
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Warning, isAcknowledgeable: false);

            if (state == AlarmSequenceState.Normal)
            {
                alarm.Normalize("System", RaisedOn);
            }

            bool canBeAcknowledged = alarm.CanBeAcknowledged;

            Assert.IsFalse(canBeAcknowledged);
            Assert.ThrowsExactly<InvalidOperationException>(() => alarm.Acknowledge("Homer", ChangedOn));
            Assert.AreEqual(state, alarm.AlarmSequenceState);
            Assert.IsNull(alarm.AcknowledgedOn);
        }


        /// <summary>
        /// A system alarm is not a device alarm and has no device id.
        /// </summary>
        [TestMethod]
        public void PpcAlarmSystemAlarmHasNoDeviceTest()
        {
            PpcAlarmConfig config = CreateConfig(AlarmSeverity.Error, PpcAlarmType.SystemAlarm, null, "IsRtuCommunicationLost", true);

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
        /// An active alarm, acknowledged or not, gets normalized when its value is back to normal, an acknowledgement is kept.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSequenceState.Alarm)]
        [DataRow(AlarmSequenceState.Acknowledged)]
        public void NormalizeActiveAlarmSetsNormalTest(AlarmSequenceState state)
        {
            PpcAlarm alarm = CreateAlarmInState(state);

            alarm.Normalize("System", ChangedOn);

            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
            Assert.AreEqual("System", alarm.ChangedBy);
            Assert.AreEqual(ChangedOn, alarm.ChangedDate);
            Assert.IsFalse(alarm.IsActive);
            Assert.AreEqual(state == AlarmSequenceState.Acknowledged, alarm.AcknowledgedOn != null);
        }


        /// <summary>
        /// An alarm of a configuration that is not acknowledgeable gets normalized without an acknowledgement.
        /// </summary>
        [TestMethod]
        public void NormalizeNotAcknowledgeableAlarmSetsNormalTest()
        {
            PpcAlarm alarm = CreateAlarm(AlarmSeverity.Info, isAcknowledgeable: false);

            alarm.Normalize("System", ChangedOn);

            Assert.AreEqual(AlarmSequenceState.Normal, alarm.AlarmSequenceState);
        }


        /// <summary>
        /// A normalized alarm cannot be normalized again.
        /// </summary>
        [TestMethod]
        public void NormalizeNormalizedAlarmThrowsTest()
        {
            PpcAlarm alarm = CreateAlarmInState(AlarmSequenceState.Normal);

            Action normalize = () => alarm.Normalize("Homer", ChangedOn);

            Assert.ThrowsExactly<InvalidOperationException>(normalize);
        }


        /// <summary>
        /// The user who acknowledges the alarm is required.
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
        private PpcAlarm CreateAlarm(AlarmSeverity severity, bool isAcknowledgeable = true)
        {
            PpcAlarmConfig config = CreateConfig(severity, PpcAlarmType.PcsAlarm, 7, "FaultStates", isAcknowledgeable);

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
        private PpcAlarmConfig CreateConfig(AlarmSeverity severity, PpcAlarmType type, int? deviceId, string propertyName, bool isAcknowledgeable)
        {
            return new PpcAlarmConfig
            {
                Id = 1,
                ChangedBy = "Homer",
                IsEnabled = true,
                IsAcknowledgeable = isAcknowledgeable,
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
