// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs.Actions
{
    /// <summary>
    /// Defines when an alarm action is performed.
    /// </summary>
    public enum AlarmActionTrigger
    {
        /// <summary>
        /// The action is performed when the alarm is raised. E.g. Reduce power output of device by 50%.
        /// </summary>
        Raise = 0,


        /// <summary>
        /// The action is performed when the alarm is normalized. E.g. Reset power output of device to 100%.
        /// </summary>
        Normalize = 1
    }
}
