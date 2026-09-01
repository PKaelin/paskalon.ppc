// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyStorages.Batteries
{
    /// <summary>
    /// Data Transfer Object for a Battery Bank Definition.
    /// </summary>
    /// <remarks>
    /// Used to initialize the Battery Bank DTO in device client.
    /// </remarks>
    public record BbDefinitionDto : IDeviceDefinition
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public required int DeviceId { get; init; }


        /// <summary>
        /// Name of the device.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Indicates whether the battery bank is initially connected or not.
        /// </summary>
        public bool InitiallyConnected { get; init; }


        /// <summary>
        /// NameplateCapacity in watt hours.
        /// </summary>
        public double NameplateCapacity { get; init; }


        /// <summary>
        /// NameplateMaximumChargeRate in watts.
        /// </summary>
        public double NameplateMaximumChargeRate { get; init; }


        /// <summary>
        /// NameplateMaximumDischargeRate in watts.
        /// </summary>
        public double NameplateMaximumDischargeRate { get; init; }


        /// <summary>
        /// Count of racks also know as segments of the battery.
        /// </summary>
        public int RackCount { get; init; }


        /// <summary>
        /// Count of modules per rack.
        /// </summary>
        public int ModulesPerRackCount { get; init; }


        /// <summary>
        /// Which inverter BUS the battery is connected to.
        /// This is used to write the batteries maximum and minimum currents that are allowed to the PCS.
        /// </summary>
        public int InverterBusNumber { get; init; }


        /// <summary>
        /// A strict lower bound on how far the IC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity, not usable capacity.
        /// </summary>
        public double AbsoluteMinimumStateOfCharge { get; init; }



        /// <summary>
        /// A strict upper bound on how far the IC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity, not usable capacity.
        /// </summary>
        public double AbsoluteMaximumStateOfCharge { get; init; }


        /// <summary>
        /// The absolute minimum temperature the battery can operate at.
        /// </summary>
        public double AbsoluteMinimumTemperature { get; init; }


        /// <summary>
        /// The absolute maximum temperature the battery can operate at.
        /// </summary>
        public double AbsoluteMaximumTemperature { get; init; }


        /// <summary>
        /// The preferred minimum state of charge, as a percentage of usable capacity.
        /// </summary>
        public double PreferredMinimumStateOfCharge { get; init; }


        /// <summary>
        /// The preferred maximum state of charge, as a percentage of usable capacity.
        /// </summary>
        public double PreferredMaximumStateOfCharge { get; init; }


        /// <summary>
        /// The preferred minimum temperature the battery can operate at.
        /// </summary>
        public double PreferredMinimumTemperature { get; init; }


        /// <summary>
        /// The preferred maximum temperature the battery can operate at.
        /// </summary>
        public double PreferredMaximumTemperature { get; init; }


        /// <summary>
        /// Expected maximum current (i.e. the absolute physical limit) the battery could produce.
        /// </summary>
        public double AbsoluteMaxDischargeCurrentAmps { get; init; }


        /// <summary>
        /// Expected minimum current (i.e. the absolute physical limit) the battery could produce.
        /// </summary>
        public double AbsoluteMaxChargeCurrentAmps { get; init; }


        /// <summary>
        /// The minimum DC voltage the battery can operate at.
        /// </summary>
        public double MinimumDcVoltage { get; init; }


        /// <summary>
        /// The maximum DC voltage the battery can operate at.
        /// </summary>
        public double MaximumDcVoltage { get; init; }


        /// <summary>
        /// Configured value determining whether the proxy should report 0 capability in the event of communication loss.
        /// </summary>
        public bool ZeroCapacityOnCommLoss { get; init; }
    }
}
