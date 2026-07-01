namespace paskalON.Devices.Domain.Configs.EnergyStorages.Batteries
{
    /// <summary>
    /// Battery bank device configuration.
    /// </summary>
    public class BatteryBankDeviceConfig : NameBase
    {
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
        public double NameplateMaximumChargeRate { get; set; }


        /// <summary>
        /// NameplateMaximumDischargeRate in watts.
        /// </summary>
        public double NameplateMaximumDischargeRate { get; set; }


        /// <summary>
        /// Count of racks also know as segments of the battery.
        /// </summary>
        public int RackCount { get; set; }


        /// <summary>
        /// Count of strings per rack.
        /// </summary>
        public int StringsPerRackCount { get; set; }


        /// <summary>
        /// Which inverter BUS the battery is connected to.
        /// This is used to write the batteries maximum and minimum currents that are allowed to the PCS.
        /// </summary>
        public int InverterBusNumber { get; set; }


        /// <summary>
        /// A strict lower bound on how far the IC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity, not usable capacity.
        /// </summary>
        /// <remarks>
        /// This value is used in production configs for systems where the battery manufacturer
        /// defines SOC limits that need to be enforced independently of the customer's intended
        /// usage, such as for battery warranty compliance. (For customer-defined limits, use the 
        /// <see cref="PreferredMinimumStateOfCharge"/>.)
        /// 
        /// This value is also used in simulator configs to simulate the absolute minimum SOC reported 
        /// by the battery over Modbus, which is typically a hardware limit unrelated to the warranty.
        /// 
        /// The PPC combines both sets of absolute SOC limits (the values from the configuration 
        /// file and the values reported from the battery) into a single set of limits by taking
        /// the most restrictive values. These limits are then used by constraints to curtail power 
        /// commands.
        /// 
        /// When used in production configs, this value should ONLY be modified if it changes
        /// under the contractual agreement with the battery manufacturer. For example, some
        /// warranties allow the SOC limits to be adjusted each year as part of an annual
        /// recalibration process.
        /// </remarks>
        public double AbsoluteMinimumStateOfCharge { get; set; }


        /// <summary>
        /// A strict upper bound on how far the IC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity, not usable capacity.
        /// </summary>
        /// <remarks>
        /// This value is used in production configs for systems where the battery manufacturer
        /// defines SOC limits that need to be enforced independently of the customer's intended
        /// usage, such as for battery warranty compliance. (For customer-defined limits, use the 
        /// <see cref="PreferredMaximumStateOfCharge"/>.)
        /// 
        /// This value is also used in simulator configs to simulate the absolute minimum SOC reported 
        /// by the battery over Modbus, which is typically a hardware limit unrelated to the warranty.
        /// 
        /// The PPC combines both sets of absolute SOC limits (the values from the configuration 
        /// file and the values reported from the battery) into a single set of limits by taking
        /// the most restrictive values. These limits are then used by constraints to curtail power 
        /// commands.
        /// 
        /// When used in production configs, this value should ONLY be modified if it changes
        /// under the contractual agreement with the battery manufacturer. For example, some
        /// warranties allow the SOC limits to be adjusted each year as part of an annual
        /// recalibration process.
        /// </remarks>
        public double AbsoluteMaximumStateOfCharge { get; set; }


        /// <summary>
        /// The absolute minimum temperature the battery can operate at.
        /// </summary>
        public double AbsoluteMinimumTemperature { get; set; }


        /// <summary>
        /// The absolute maximum temperature the battery can operate at.
        /// </summary>
        public double AbsoluteMaximumTemperature { get; set; }


        /// <summary>
        /// The preferred minimum state of charge, as a percentage of usable capacity.
        /// </summary>
        public double PreferredMinimumStateOfCharge { get; set; }


        /// <summary>
        /// The preferred maximum state of charge, as a percentage of usable capacity.
        /// </summary>
        public double PreferredMaximumStateOfCharge { get; set; }


        /// <summary>
        /// The preferred minimum temperature the battery can operate at.
        /// </summary>
        public double PreferredMinimumTemperature { get; set; }


        /// <summary>
        /// The preferred maximum temperature the battery can operate at.
        /// </summary>
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
