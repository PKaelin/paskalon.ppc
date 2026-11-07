// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Application.UnitTest.SampleData
{
    /// <summary>
    /// Sample PCS alarm configurations with alarm actions.
    /// </summary>
    public static class PcsAlarmConfigSampleData
    {
        /// <summary>
        /// Timestamp of the sample data.
        /// </summary>
        public static readonly DateTimeOffset ChangedOn = new DateTimeOffset(2026, 11, 5, 10, 45, 50, TimeSpan.Zero);


        /// <summary>
        /// Creates the fault states configuration of all PCS with the raise action ReducePower and the normalize action ResetPower.
        /// </summary>
        public static PpcAlarmConfig CreatePcsFaultConfig()
        {
            PpcAlarmConfig config = new PpcAlarmConfig
            {
                Id = 1,
                ChangedBy = "Homer",
                ChangedDate = ChangedOn,
                IsEnabled = true,
                AlarmId = "PCS-FAULT",
                Name = "{{PCS.Fault.Name}}",
                SeverityLevel = AlarmSeverity.Error,
                Description = "{{PCS.Fault.Desc}}",
                PpcAlarmType = PpcAlarmType.PcsAlarm,
                PropertyName = "FaultStates",
                PropertyOperator = PropertyOperator.Equal,
                PropertyValue = 1
            };
            AlarmActionConfig reducePower = CreateAction(10, "ReducePower", AlarmActionTrigger.Raise);
            reducePower.AlarmActionParameters.Add(CreateParameter(reducePower, 11, "DeviceId", "{{DeviceId}}"));
            reducePower.AlarmActionParameters.Add(CreateParameter(reducePower, 12, "Percentage", "50"));
            reducePower.AlarmActionParameters.Add(CreateParameter(reducePower, 13, "Reason", "Alarm {{PropertyName}} of PCS {{DeviceId}}"));
            AlarmActionConfig resetPower = CreateAction(20, "ResetPower", AlarmActionTrigger.Normalize);
            resetPower.AlarmActionParameters.Add(CreateParameter(resetPower, 21, "DeviceId", "{{DeviceId}}"));
            config.AlarmActions.Add(reducePower);
            config.AlarmActions.Add(resetPower);

            return config;
        }


        /// <summary>
        /// Creates an active alarm of a configuration.
        /// </summary>
        public static PpcAlarm CreateActiveAlarm(PpcAlarmConfig config, int? deviceId, string propertyName)
        {
            return new PpcAlarm
            {
                Id = 100,
                ChangedBy = "System",
                ChangedDate = ChangedOn,
                CreatedOn = ChangedOn,
                DeviceId = deviceId,
                PropertyName = propertyName,
                PpcAlarmConfigId = config.Id,
                PpcAlarmConfig = config
            };
        }


        /// <summary>
        /// Creates an action, it can be shared by several configurations.
        /// </summary>
        private static AlarmActionConfig CreateAction(int id, string name, AlarmActionTrigger trigger)
        {
            return new AlarmActionConfig
            {
                Id = id,
                ChangedBy = "Homer",
                ChangedDate = ChangedOn,
                Name = name,
                Trigger = trigger
            };
        }


        /// <summary>
        /// Creates a parameter of an action.
        /// </summary>
        private static AlarmActionParameterConfig CreateParameter(AlarmActionConfig action, int id, string key, string value)
        {
            return new AlarmActionParameterConfig
            {
                Id = id,
                ChangedBy = "Homer",
                ChangedDate = ChangedOn,
                Key = key,
                Value = value,
                AlarmActionConfigId = action.Id,
                AlarmActionConfig = action
            };
        }
    }
}
