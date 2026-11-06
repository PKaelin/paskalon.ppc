// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Dto.Ppcs
{
    /// <summary>
    /// Alarm types specific for PPC alarms, analog to the PPC alarm type of the domain.
    /// </summary>
    public enum PpcAlarmTypeDto
    {
        /// <summary>
        /// Power plant controller alarms.
        /// </summary>
        SystemAlarm = 0,


        /// <summary>
        /// Power conversion system alarms.
        /// </summary>
        PcsAlarm = 1,


        /// <summary>
        /// Battery management system alarms.
        /// </summary>
        BatteryAlarm = 2,


        /// <summary>
        /// Power meter alarms.
        /// </summary>
        MeterAlarm = 3,


        /// <summary>
        /// Battery container alarms.
        /// </summary>
        BatteryContainerAlarm = 4,


        /// <summary>
        /// Generic modbus device alarms.
        /// </summary>
        GenericModbusDeviceAlarm = 5
    }
}
