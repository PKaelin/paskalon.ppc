// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Infrastructure.IntegrationTest.Storage.SampleData
{
    /// <summary>
    /// Sample alarm configurations of a site with PCS and system alarms.
    /// </summary>
    public static class PcsAlarmSampleData
    {
        /// <summary>
        /// Timestamp of the sample data.
        /// </summary>
        public static readonly DateTimeOffset ChangedOn = new DateTimeOffset(2026, 11, 5, 10, 45, 50, TimeSpan.Zero);


        /// <summary>
        /// Creates the fault states configuration of all PCS with a reduce power raise action and a reset power normalize action.
        /// </summary>
        public static PpcAlarmConfig CreatePcsFaultConfig()
        {
            PpcAlarmConfig config = CreateConfig("PCS-FAULT", "PCS.Fault", AlarmSeverity.Error, PpcAlarmType.PcsAlarm, null, "FaultStates");
            AlarmActionConfig reducePower = CreateAction("ReducePower", AlarmActionTrigger.Raise);
            reducePower.AlarmActionParameters.Add(CreateParameter(reducePower, "DeviceId", "{{DeviceId}}"));
            reducePower.AlarmActionParameters.Add(CreateParameter(reducePower, "Percentage", "50"));
            AlarmActionConfig resetPower = CreateAction("ResetPower", AlarmActionTrigger.Normalize);
            resetPower.AlarmActionParameters.Add(CreateParameter(resetPower, "DeviceId", "{{DeviceId}}"));
            config.AlarmActions.Add(reducePower);
            config.AlarmActions.Add(resetPower);

            return config;
        }


        /// <summary>
        /// Creates the warning states configuration of all PCS.
        /// </summary>
        public static PpcAlarmConfig CreatePcsWarningConfig()
        {
            return CreateConfig("PCS-WARNING", "PCS.Warning", AlarmSeverity.Warning, PpcAlarmType.PcsAlarm, null, "WarningStates");
        }


        /// <summary>
        /// Creates the critical grid loss override configuration of all PCS.
        /// </summary>
        public static PpcAlarmConfig CreatePcsGridLossConfig()
        {
            return CreateConfig("PCS-GRIDLOSS", "PCS.GridLoss", AlarmSeverity.Critical, PpcAlarmType.PcsAlarm, null, "FaultStates.GridLoss");
        }


        /// <summary>
        /// Creates the RTU communication lost system configuration.
        /// </summary>
        public static PpcAlarmConfig CreateSystemRtuConfig()
        {
            return CreateConfig("SYS-RTU", "System.RtuCommunication", AlarmSeverity.Critical, PpcAlarmType.SystemAlarm, null, "IsRtuCommunicationLost");
        }


        /// <summary>
        /// Creates a raised alarm of a configuration.
        /// </summary>
        public static PpcAlarm CreateAlarm(PpcAlarmConfig config, int? deviceId, string propertyName)
        {
            return new PpcAlarm
            {
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
        /// Creates an enabled configuration that raises the alarm when the property is true.
        /// </summary>
        private static PpcAlarmConfig CreateConfig(string alarmId, string stringId, AlarmSeverity severity, PpcAlarmType type, int? deviceId, string propertyName)
        {
            return new PpcAlarmConfig
            {
                ChangedBy = "Homer",
                ChangedDate = ChangedOn,
                IsEnabled = true,
                AlarmId = alarmId,
                Name = $"{{{{{stringId}.Name}}}}",
                SeverityLevel = severity,
                Description = $"{{{{{stringId}.Desc}}}}",
                PossibleCauses = $"{{{{{stringId}.Causes}}}}",
                Response = $"{{{{{stringId}.Response}}}}",
                Consequences = $"{{{{{stringId}.Consequences}}}}",
                PpcAlarmType = type,
                DeviceId = deviceId,
                PropertyName = propertyName,
                PropertyOperator = PropertyOperator.Equal,
                PropertyValue = 1
            };
        }


        /// <summary>
        /// Creates an action, it can be shared by several configurations.
        /// </summary>
        private static AlarmActionConfig CreateAction(string name, AlarmActionTrigger trigger)
        {
            return new AlarmActionConfig
            {
                ChangedBy = "Homer",
                ChangedDate = ChangedOn,
                Name = name,
                Trigger = trigger
            };
        }


        /// <summary>
        /// Creates a parameter of an action.
        /// </summary>
        private static AlarmActionParameterConfig CreateParameter(AlarmActionConfig action, string key, string value)
        {
            return new AlarmActionParameterConfig
            {
                ChangedBy = "Homer",
                ChangedDate = ChangedOn,
                Key = key,
                Value = value,
                AlarmActionConfig = action
            };
        }
    }
}
