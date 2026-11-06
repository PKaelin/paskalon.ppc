// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Domain.UnitTest.Evaluations
{
    /// <summary>
    /// Tests <see cref="PpcAlarmEvaluator"/>.
    /// </summary>
    [TestClass]
    public class PpcAlarmEvaluatorTest
    {
        /// <summary>
        /// Device id of the evaluated PCS.
        /// </summary>
        private const int PcsDeviceId = 7;


        /// <summary>
        /// Timestamp when the values changed.
        /// </summary>
        private static readonly DateTimeOffset ChangedOn = new DateTimeOffset(2026, 11, 5, 10, 45, 50, TimeSpan.Zero);


        /// <summary>
        /// Key of the evaluated PCS.
        /// </summary>
        private static readonly PpcAlarmKey PcsKey = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.PcsAlarm, DeviceId = PcsDeviceId };


        /// <summary>
        /// The evaluator under test.
        /// </summary>
        private readonly PpcAlarmEvaluator _evaluator = new PpcAlarmEvaluator();


        /// <summary>
        /// A fault state that turns true raises an alarm with the dictionary configuration for all devices.
        /// </summary>
        [TestMethod]
        public void EvaluateTrueFaultStateRaisesAlarmTest()
        {
            PpcAlarmConfig faultConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.OverTemperature", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { faultConfig }, new List<PpcAlarm>());

            Assert.HasCount(1, evaluation.Raised);
            Assert.IsEmpty(evaluation.Normalized);
            PpcAlarm alarm = evaluation.Raised[0];
            Assert.AreEqual(PcsDeviceId, alarm.DeviceId);
            Assert.AreEqual("FaultStates.OverTemperature", alarm.PropertyName);
            Assert.AreSame(faultConfig, alarm.PpcAlarmConfig);
            Assert.AreEqual(faultConfig.Id, alarm.PpcAlarmConfigId);
            Assert.AreEqual(AlarmSeverity.Error, alarm.SeverityLevel);
            Assert.AreEqual(AlarmSequenceState.Alarm, alarm.AlarmSequenceState);
            Assert.AreEqual(ChangedOn, alarm.CreatedOn);
            Assert.AreEqual(PpcAlarmEvaluator.SystemUser, alarm.ChangedBy);
        }


        /// <summary>
        /// A fault state that is false without an active alarm does nothing.
        /// </summary>
        [TestMethod]
        public void EvaluateFalseFaultStateWithoutAlarmDoesNothingTest()
        {
            PpcAlarmConfig faultConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.OverTemperature", 0) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { faultConfig }, new List<PpcAlarm>());

            Assert.IsFalse(evaluation.HasChanges);
        }


        /// <summary>
        /// A fault state that is still true with an active alarm does not raise a second alarm.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSequenceState.Alarm)]
        [DataRow(AlarmSequenceState.Acknowledged)]
        public void EvaluateTrueFaultStateWithActiveAlarmDoesNothingTest(AlarmSequenceState state)
        {
            PpcAlarmConfig faultConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            PpcAlarm activeAlarm = CreateActiveAlarm(faultConfig, "FaultStates.OverTemperature", state);
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.OverTemperature", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { faultConfig }, new List<PpcAlarm> { activeAlarm });

            Assert.IsFalse(evaluation.HasChanges);
            Assert.AreEqual(state, activeAlarm.AlarmSequenceState);
        }


        /// <summary>
        /// A fault state that turns false normalizes the active alarm, acknowledged or not.
        /// </summary>
        [TestMethod]
        [DataRow(AlarmSequenceState.Alarm)]
        [DataRow(AlarmSequenceState.Acknowledged)]
        public void EvaluateFalseFaultStateNormalizesActiveAlarmTest(AlarmSequenceState state)
        {
            PpcAlarmConfig faultConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            PpcAlarm activeAlarm = CreateActiveAlarm(faultConfig, "FaultStates.OverTemperature", state);
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.OverTemperature", 0) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { faultConfig }, new List<PpcAlarm> { activeAlarm });

            Assert.IsEmpty(evaluation.Raised);
            Assert.HasCount(1, evaluation.Normalized);
            Assert.AreSame(activeAlarm, evaluation.Normalized[0]);
            Assert.AreEqual(AlarmSequenceState.Normal, activeAlarm.AlarmSequenceState);
            Assert.AreEqual(ChangedOn, activeAlarm.ChangedDate);
        }


        /// <summary>
        /// The configuration of a dictionary entry overrides the configuration of the dictionary.
        /// </summary>
        [TestMethod]
        public void EvaluateEntryConfigOverridesDictionaryConfigTest()
        {
            PpcAlarmConfig faultConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            PpcAlarmConfig gridLossConfig = CreateConfig(2, AlarmSeverity.Critical, null, "FaultStates.GridLoss");
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue>
            {
                CreateValue("FaultStates.OverTemperature", 1),
                CreateValue("FaultStates.GridLoss", 1)
            };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { faultConfig, gridLossConfig }, new List<PpcAlarm>());

            Assert.HasCount(2, evaluation.Raised);
            PpcAlarm overTemperature = evaluation.Raised.Single(alarm => alarm.PropertyName == "FaultStates.OverTemperature");
            PpcAlarm gridLoss = evaluation.Raised.Single(alarm => alarm.PropertyName == "FaultStates.GridLoss");
            Assert.AreSame(faultConfig, overTemperature.PpcAlarmConfig);
            Assert.AreSame(gridLossConfig, gridLoss.PpcAlarmConfig);
            Assert.AreEqual(AlarmSeverity.Critical, gridLoss.SeverityLevel);
        }


        /// <summary>
        /// The configuration of the device overrides the configuration of all devices on the same property level.
        /// </summary>
        [TestMethod]
        public void EvaluateDeviceConfigOverridesAllDevicesConfigTest()
        {
            PpcAlarmConfig allDevicesConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            PpcAlarmConfig deviceConfig = CreateConfig(2, AlarmSeverity.Warning, PcsDeviceId, "FaultStates");
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.OverTemperature", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { allDevicesConfig, deviceConfig }, new List<PpcAlarm>());

            Assert.HasCount(1, evaluation.Raised);
            Assert.AreSame(deviceConfig, evaluation.Raised[0].PpcAlarmConfig);
            Assert.AreEqual(AlarmSeverity.Warning, evaluation.Raised[0].SeverityLevel);
        }


        /// <summary>
        /// The dictionary entry level wins over the device level, an entry configuration for all devices overrides a dictionary configuration of the device.
        /// </summary>
        [TestMethod]
        public void EvaluateEntryConfigOverridesDeviceDictionaryConfigTest()
        {
            PpcAlarmConfig deviceFaultConfig = CreateConfig(1, AlarmSeverity.Error, PcsDeviceId, "FaultStates");
            PpcAlarmConfig gridLossConfig = CreateConfig(2, AlarmSeverity.Critical, null, "FaultStates.GridLoss");
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.GridLoss", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { deviceFaultConfig, gridLossConfig }, new List<PpcAlarm>());

            Assert.HasCount(1, evaluation.Raised);
            Assert.AreSame(gridLossConfig, evaluation.Raised[0].PpcAlarmConfig);
            Assert.AreEqual(AlarmSeverity.Critical, evaluation.Raised[0].SeverityLevel);
        }


        /// <summary>
        /// An active alarm of a configuration that got overridden is normalized and the overriding configuration raises its own alarm.
        /// </summary>
        [TestMethod]
        public void EvaluateOverriddenConfigNormalizesActiveAlarmTest()
        {
            PpcAlarmConfig faultConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            PpcAlarmConfig gridLossConfig = CreateConfig(2, AlarmSeverity.Critical, null, "FaultStates.GridLoss");
            PpcAlarm activeAlarm = CreateActiveAlarm(faultConfig, "FaultStates.GridLoss", AlarmSequenceState.Alarm);
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.GridLoss", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { faultConfig, gridLossConfig }, new List<PpcAlarm> { activeAlarm });

            Assert.HasCount(1, evaluation.Normalized);
            Assert.AreSame(activeAlarm, evaluation.Normalized[0]);
            Assert.HasCount(1, evaluation.Raised);
            Assert.AreSame(gridLossConfig, evaluation.Raised[0].PpcAlarmConfig);
        }


        /// <summary>
        /// An active alarm of a disabled configuration is normalized and no new alarm is raised.
        /// </summary>
        [TestMethod]
        public void EvaluateDisabledConfigNormalizesActiveAlarmTest()
        {
            PpcAlarmConfig faultConfig = CreateConfig(1, AlarmSeverity.Error, null, "FaultStates");
            PpcAlarm activeAlarm = CreateActiveAlarm(faultConfig, "FaultStates.OverTemperature", AlarmSequenceState.Alarm);
            faultConfig.IsEnabled = false;
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.OverTemperature", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { faultConfig }, new List<PpcAlarm> { activeAlarm });

            Assert.IsEmpty(evaluation.Raised);
            Assert.HasCount(1, evaluation.Normalized);
        }


        /// <summary>
        /// Configurations of another device, another alarm type or another property are not evaluated.
        /// </summary>
        [TestMethod]
        public void EvaluateNotMatchingConfigsDoNothingTest()
        {
            List<PpcAlarmConfig> configs = new List<PpcAlarmConfig>
            {
                CreateConfig(1, AlarmSeverity.Error, 8, "FaultStates"),
                CreateConfig(2, AlarmSeverity.Error, null, "FaultStates", PpcAlarmType.BatteryAlarm),
                CreateConfig(3, AlarmSeverity.Warning, null, "WarningStates"),
                CreateConfig(4, AlarmSeverity.Error, null, "Fault")
            };

            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("FaultStates.OverTemperature", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, configs, new List<PpcAlarm>());

            Assert.IsFalse(evaluation.HasChanges);
        }


        /// <summary>
        /// Multiple configurations of the same level each raise their own alarm, e.g. warning and error thresholds.
        /// </summary>
        [TestMethod]
        [DataRow(75.0, 0)]
        [DataRow(85.0, 1)]
        [DataRow(95.0, 2)]
        public void EvaluateThresholdConfigsRaiseOwnAlarmsTest(double temperature, int expectedAlarms)
        {
            PpcAlarmConfig warningConfig = CreateConfig(1, AlarmSeverity.Warning, null, "Temperature", operation: PropertyOperator.GreaterThan, propertyValue: 80);
            PpcAlarmConfig errorConfig = CreateConfig(2, AlarmSeverity.Error, null, "Temperature", operation: PropertyOperator.GreaterThan, propertyValue: 90);
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("Temperature", temperature) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { warningConfig, errorConfig }, new List<PpcAlarm>());

            Assert.HasCount(expectedAlarms, evaluation.Raised);
        }


        /// <summary>
        /// The property operator conditions the value against the property value of the configuration.
        /// </summary>
        [TestMethod]
        [DataRow(PropertyOperator.Equal, 50.0, true)]
        [DataRow(PropertyOperator.Equal, 50.1, false)]
        [DataRow(PropertyOperator.NotEqual, 50.1, true)]
        [DataRow(PropertyOperator.NotEqual, 50.0, false)]
        [DataRow(PropertyOperator.GreaterThan, 50.1, true)]
        [DataRow(PropertyOperator.GreaterThan, 50.0, false)]
        [DataRow(PropertyOperator.SmallerThan, 49.9, true)]
        [DataRow(PropertyOperator.SmallerThan, 50.0, false)]
        [DataRow(PropertyOperator.GreaterThanOrEqual, 50.0, true)]
        [DataRow(PropertyOperator.GreaterThanOrEqual, 49.9, false)]
        [DataRow(PropertyOperator.SmallerThanOrEqual, 50.0, true)]
        [DataRow(PropertyOperator.SmallerThanOrEqual, 50.1, false)]
        public void EvaluatePropertyOperatorTest(PropertyOperator operation, double frequency, bool isRaised)
        {
            PpcAlarmConfig config = CreateConfig(1, AlarmSeverity.Warning, null, "Frequency", operation: operation, propertyValue: 50);
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("Frequency", frequency) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(PcsKey, values, new List<PpcAlarmConfig> { config }, new List<PpcAlarm>());

            Assert.AreEqual(isRaised, evaluation.Raised.Count == 1);
        }


        /// <summary>
        /// A system alarm is evaluated without a device and raises an alarm without a device id.
        /// </summary>
        [TestMethod]
        public void EvaluateSystemAlarmWithoutDeviceTest()
        {
            PpcAlarmKey systemKey = new PpcAlarmKey { PpcAlarmType = PpcAlarmType.SystemAlarm };
            PpcAlarmConfig systemConfig = CreateConfig(1, AlarmSeverity.Critical, null, "IsRtuCommunicationLost", PpcAlarmType.SystemAlarm);
            List<PpcAlarmKeyValue> values = new List<PpcAlarmKeyValue> { CreateValue("IsRtuCommunicationLost", 1) };

            PpcAlarmEvaluation evaluation = _evaluator.Evaluate(systemKey, values, new List<PpcAlarmConfig> { systemConfig }, new List<PpcAlarm>());

            Assert.HasCount(1, evaluation.Raised);
            Assert.IsNull(evaluation.Raised[0].DeviceId);
        }


        /// <summary>
        /// Creates an enabled configuration that raises the alarm when the property is true by default.
        /// </summary>
        private PpcAlarmConfig CreateConfig(int id, AlarmSeverity severity, int? deviceId, string propertyName,
            PpcAlarmType type = PpcAlarmType.PcsAlarm, PropertyOperator operation = PropertyOperator.Equal, double propertyValue = 1)
        {
            return new PpcAlarmConfig
            {
                Id = id,
                ChangedBy = "Homer",
                IsEnabled = true,
                AlarmId = $"{type}-{propertyName}",
                Name = $"{{{{{type}.{propertyName}.Name}}}}",
                SeverityLevel = severity,
                Description = $"{{{{{type}.{propertyName}.Desc}}}}",
                PpcAlarmType = type,
                DeviceId = deviceId,
                PropertyName = propertyName,
                PropertyOperator = operation,
                PropertyValue = propertyValue
            };
        }


        /// <summary>
        /// Creates a changed value.
        /// </summary>
        private PpcAlarmKeyValue CreateValue(string name, double value)
        {
            return new PpcAlarmKeyValue
            {
                Name = name,
                Value = value,
                ValueChanged = true,
                ValueChangedOn = ChangedOn
            };
        }


        /// <summary>
        /// Creates an active alarm of the evaluated PCS in the given sequence state.
        /// </summary>
        private PpcAlarm CreateActiveAlarm(PpcAlarmConfig config, string propertyName, AlarmSequenceState state)
        {
            PpcAlarm alarm = new PpcAlarm
            {
                Id = 42,
                ChangedBy = PpcAlarmEvaluator.SystemUser,
                ChangedDate = ChangedOn.AddMinutes(-5),
                CreatedOn = ChangedOn.AddMinutes(-5),
                DeviceId = PcsDeviceId,
                PropertyName = propertyName,
                PpcAlarmConfigId = config.Id,
                PpcAlarmConfig = config
            };

            if (state == AlarmSequenceState.Acknowledged)
            {
                alarm.Acknowledge("Homer", ChangedOn.AddMinutes(-1));
            }

            return alarm;
        }
    }
}
