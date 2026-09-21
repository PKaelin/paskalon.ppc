// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs.EnergyStorages.Batteries
{
    /// <summary>
    /// Battery bank device configuration.
    /// </summary>
    public class BatteryBankDeviceConfig : NameBase
    {
        /// <summary>
        /// Child relationship to a list of custom configurations.
        /// </summary>
        public ICollection<BatteryBankDeviceCustomConfig> Customs { get; set; } = new List<BatteryBankDeviceCustomConfig>();


        /// <summary>
        /// The class name of the type to instantiate.
        /// This uniquely identifies the eventually used type of component. (e.g. a ManufacturerPcs, ManufacturerBattery etc.).
        /// </summary>
        public required string ClassName { get; set; }


        /// <summary>
        /// Battery type.
        /// </summary>
        public BatteryType BatteryType { get; set; }


        /// <summary>
        /// NameplateCapacity in watt hours.
        /// </summary>
        public double NameplateCapacity { get; set; }


        /// <summary>
        /// NameplateMaximumChargeRate in watts.
        /// </summary>
        /// <remarks>
        /// Maximum active power the battery bank can absorb while charging.
        /// </remarks>
        public double NameplateMaximumChargeRate { get; set; }


        /// <summary>
        /// NameplateMaximumDischargeRate in watts.
        /// </summary>
        /// <remarks>
        /// Maximum active power that the battery bank can deliver during discharge.
        /// </remarks>
        public double NameplateMaximumDischargeRate { get; set; }


        /// <summary>
        /// Count of racks also know as segments of the battery.
        /// </summary>
        public int RackCount { get; set; }


        /// <summary>
        /// Count of modules per rack.
        /// </summary>
        public int ModulesPerRackCount { get; set; }


        /// <summary>
        /// Which inverter BUS the battery is connected to.
        /// This is used to write the batteries maximum and minimum currents that are allowed to the PCS.
        /// </summary>
        public int InverterBusNumber { get; set; }


        //-----------------------------------------------------------------------------------------
        // State Of Charge parameters:
        // [ABSOLUTE SOC: 0%  =======================================  100% ] <- Physical Cells
        //                    \                                     /
        // [USABLE SOC:    0% (5% Abs) ================ 100% (95% Abs) ]      <- Manufacturer Window
        //                         \                 /
        // [PREFERRED SOC:         0% (20% Abs) == 100% (80% Abs) ]           <- Operator Window
        //
        // Most battery banks have a usable SOC endpoint.
        //-----------------------------------------------------------------------------------------


        /// <summary>
        /// A strict lower bound on how far the PPC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity.
        /// </summary>
        /// <remarks>
        /// This value is used in production configs for systems where the battery manufacturer
        /// defines SOC limits that need to be enforced independently of the customer's intended
        /// usage, such as for battery warranty compliance. (For customer-defined limits, use the 
        /// <see cref="PreferredMinimumStateOfCharge"/>.)
        /// 
        /// This value should ONLY be modified according to the contractual agreement with the battery
        /// manufacturer. For example, some warranties allow the SOC limits to be adjusted each year
        /// as part of an annual recalibration process.
        /// Running at these extremes causes severe degradation.
        /// </remarks>
        public double AbsoluteMinimumStateOfCharge { get; set; } = 0;


        /// <summary>
        /// A strict upper bound on how far the PPC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity.
        /// </summary>
        /// <remarks>
        /// This value is used in production configs for systems where the battery manufacturer
        /// defines SOC limits that need to be enforced independently of the customer's intended
        /// usage, such as for battery warranty compliance. (For customer-defined limits, use the 
        /// <see cref="PreferredMaximumStateOfCharge"/>.)
        /// 
        /// This value should ONLY be modified according to the contractual agreement with the battery
        /// manufacturer. For example, some warranties allow the SOC limits to be adjusted each year
        /// as part of an annual recalibration process.
        /// Running at these extremes causes severe degradation.
        /// </remarks>
        public double AbsoluteMaximumStateOfCharge { get; set; } = 100;


        /// <summary>
        /// The usable minimum state of charge in percent.
        /// The lower bound the PPC considers available during normal operations.
        /// </summary>
        /// <remarks>
        /// Most often this is also the SOC Modbus endpoint.
        /// </remarks>
        public double UsableMinimumStateOfCharge { get; set; } = 10;


        /// <summary>
        /// The usable maximum state of charge in percent.
        /// The upper bound the PPC considers available during normal operations.
        /// </summary>
        /// <remarks>
        /// Most often this is also the SOC Modbus endpoint.
        /// </remarks>
        public double UsableMaximumStateOfCharge { get; set; } = 90;


        /// <summary>
        /// The preferred minimum state of charge in percent.
        /// The lower bound the PPC maintains during normal operations.
        /// </summary>
        /// <remarks>
        /// The ideal window for daily use and long-term storage to maximize longevity.
        /// The SOC that is shown to the user/system.
        /// </remarks>
        public double PreferredMinimumStateOfCharge { get; set; } = 20;


        /// <summary>
        /// The preferred maximum state of charge in percentage.
        /// The upper bound the PPC maintains during normal operations.
        /// </summary>
        /// <remarks>
        /// The ideal window for daily use and long-term storage to maximize longevity.
        /// The SOC that is shown to the user/system.
        /// </remarks>
        public double PreferredMaximumStateOfCharge { get; set; } = 80;


        /// <summary>
        /// The absolute minimum temperature the battery can operate at.
        /// </summary>
        /// <remarks>
        /// Temperature is in Celsius. Usually around 0C.
        /// </remarks>
        public double AbsoluteMinimumTemperature { get; set; }


        /// <summary>
        /// The absolute maximum temperature the battery can operate at.
        /// </summary>
        /// <remarks>
        /// Temperature is in Celsius. Usually around 55C.
        /// </remarks>
        public double AbsoluteMaximumTemperature { get; set; }


        /// <summary>
        /// The preferred minimum temperature the battery can operate at.
        /// </summary>
        /// <remarks>
        /// Temperature is in Celsius.
        /// </remarks>
        public double PreferredMinimumTemperature { get; set; }


        /// <summary>
        /// The preferred maximum temperature the battery can operate at.
        /// </summary>
        /// <remarks>
        /// Temperature is in Celsius.
        /// </remarks>
        public double PreferredMaximumTemperature { get; set; }


        /// <summary>
        /// Expected maximum current (i.e. the absolute physical limit) the battery could produce.
        /// </summary>
        public double AbsoluteMaxDischargeCurrentAmps { get; set; }


        /// <summary>
        /// Expected minimum current (i.e. the absolute physical limit) the battery could produce.
        /// </summary>
        public double AbsoluteMaxChargeCurrentAmps { get; set; }


        /// <summary>
        /// The minimum DC voltage the battery can operate at.
        /// </summary>
        public double MinimumDcVoltage { get; set; }


        /// <summary>
        /// The maximum DC voltage the battery can operate at.
        /// </summary>
        public double MaximumDcVoltage { get; set; }


        /// <summary>
        /// Configured value determining whether the proxy should report 0 capability
        /// in the event of communication loss.
        /// </summary>
        /// <remarks>
        /// This setting is intended to keep loss compensation calculations accurate in situations where
        /// a battery bank communication error is experienced.
        /// </remarks>
        public bool ZeroCapacityOnCommLoss { get; set; } = true;

    }
}
