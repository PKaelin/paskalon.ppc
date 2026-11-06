// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs.Ppcs
{
    /// <summary>
    /// Alarm types specific for PPC alarms.
    /// </summary>
    public enum PpcAlarmType
    {
        /// <summary>
        /// Power plant controller alarms. E.g. Site controller is unable to communicate with the RTU.
        /// </summary>
        SystemAlarm = 0,


        /// <summary>
        /// Power conversion system alarms. E.g. PCS is above its maximum operating temperature.
        /// </summary>
        PcsAlarm = 1,


        /// <summary>
        /// Battery management system alarms. E.g. Battery cell is above its maximum operating voltage.
        /// </summary>
        BatteryAlarm = 2,


        /// <summary>
        /// Power meter alarms. E.g. PPC is unable to communicate with the power meter.
        /// </summary>
        MeterAlarm = 3,


        /// <summary>
        /// Battery container alarms. E.g. Door is open.
        /// </summary>
        BatteryContainerAlarm = 4,


        /// <summary>
        /// Generic modbus device alarms. E.g. PPC is unable to communicate with the GMD.
        /// </summary>
        GenericModbusDeviceAlarm = 5
    }
}
