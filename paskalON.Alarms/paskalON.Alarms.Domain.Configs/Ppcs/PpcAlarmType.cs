// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Configs.Ppcs
{
    /// <summary>
    /// Alarm types specific for PPC alarms, the source of the evaluated data.
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
        /// External power meter alarms. E.g. PPC is unable to communicate with the external power meter.
        /// </summary>
        ExternalPowerMeterAlarm = 3,


        /// <summary>
        /// Battery container alarms. E.g. Door is open.
        /// </summary>
        BatteryContainerAlarm = 4,


        /// <summary>
        /// Generic modbus device alarms. E.g. PPC is unable to communicate with the GMD.
        /// </summary>
        GenericModbusDeviceAlarm = 5,


        /// <summary>
        /// Auxiliary power meter alarms. E.g. PPC is unable to communicate with the auxiliary power meter.
        /// </summary>
        AuxiliaryPowerMeterAlarm = 6,


        /// <summary>
        /// Circuit power meter alarms. E.g. PPC is unable to communicate with the circuit power meter.
        /// </summary>
        CircuitPowerMeterAlarm = 7,


        /// <summary>
        /// System power meter alarms. E.g. PPC is unable to communicate with the system power meter.
        /// </summary>
        SystemPowerMeterAlarm = 8,


        /// <summary>
        /// Solar panel alarms. E.g. Solar panel reports a communication error.
        /// </summary>
        SolarPanelAlarm = 9,


        /// <summary>
        /// Power control alarms of the power targets per PCS. E.g. Power target is above the PCS rating.
        /// </summary>
        PowerControlAlarm = 10,


        /// <summary>
        /// Operating mode alarms of the system setpoint. E.g. Active power setpoint is above the plant limit.
        /// </summary>
        OperatingModeAlarm = 11
    }
}
