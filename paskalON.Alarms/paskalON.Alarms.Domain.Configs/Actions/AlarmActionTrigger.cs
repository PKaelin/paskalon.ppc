// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs.Actions
{
    /// <summary>
    /// Defines at which change of the alarm the action is sent.
    /// </summary>
    /// <remarks>
    /// The alarm configuration defines the abnormal condition with PropertyName, PropertyOperator and PropertyValue
    /// (e.g. CommunicationError Equal 1). Each published value change of a device is evaluated against it:
    /// - Condition becomes true and the device has no active alarm of the configuration: the alarm is raised.
    /// - Condition becomes false and the device has an active alarm of the configuration: the alarm is normalized.
    ///   An alarm is also normalized when its configuration is disabled or removed (configuration reload).
    /// - An unchanged value or condition sends nothing, so every alarm occurrence sends its raise actions once and
    ///   its normalize actions once.
    /// All actions of the configuration with the trigger of the change are sent, each as its own message.
    /// </remarks>
    public enum AlarmActionTrigger
    {
        /// <summary>
        /// The action is sent when the alarm is raised (condition became true).
        /// E.g. RaiseAlarm to show the alarm in Luna, send an email or reduce the power output of the device by 50%.
        /// </summary>
        Raise = 0,


        /// <summary>
        /// The action is sent when the alarm is normalized (condition became false, or its configuration was disabled or removed).
        /// E.g. ClearAlarm to clear the alarm in Luna or reset the power output of the device to 100%.
        /// </summary>
        Normalize = 1
    }
}
