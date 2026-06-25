using paskalON.Communication.Protocols.Modbus.Configurations;

namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    public class PowerMeterMapModbusConfig : NameBase
    {
        /// <summary>
        /// Start of holding register.
        /// </summary>
        public ushort StartingHoldingRegister { get; set; } = 40001;


        /// <summary>
        /// Start of input register.
        /// </summary>
        public ushort StartingInputRegister { get; set; } = 30001;


        /// <summary>
        /// Start of discrete register.
        /// </summary>
        public ushort StartingDiscreteInput { get; set; } = 10001;


        /// <summary>
        /// Start of coil register.
        /// </summary>
        public ushort StartingCoil { get; set; } = 1;


        /// <summary>
        /// Modbus polling range.
        /// </summary>
        public List<ModbusRegisterMapPollingRangeConfig> PollingRange { get; set; } = new List<ModbusRegisterMapPollingRangeConfig>();


        /// <summary>
        /// Gets/Sets the map entry for ActivePower lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ActivePowerMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ReactivePower lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ReactivePowerMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ApparentPower lookup.
        /// </summary>        
        public ModbusRegisterMapEntryConfig? ApparentPowerMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for PowerFactor lookup.
        /// </summary>        
        public ModbusRegisterMapEntryConfig? PowerFactorMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for Frequency lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? FrequencyMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for VoltageA lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? VoltageAMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for VoltageB lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? VoltageBMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for VoltageC lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? VoltageCMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for VoltageLLAvg lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? VoltageLLAvgMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for CurrentA lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? CurrentAMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for CurrentB lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? CurrentBMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for CurrentC lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? CurrentCMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ActivePowerA lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ActivePowerAMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ActivePowerB lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ActivePowerBMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ActivePowerC lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ActivePowerCMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ReactivePowerA lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ReactivePowerAMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ReactivePowerB lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ReactivePowerBMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ReactivePowerC lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ReactivePowerCMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for EnergyDelivered lookup.
        /// </summary>        
        public ModbusRegisterMapEntryConfig? EnergyDeliveredMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for EnergyReceived lookup.
        /// </summary>        
        public ModbusRegisterMapEntryConfig? EnergyReceivedMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ReactiveEnergyDelivered lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ReactiveEnergyDeliveredMap { get; set; }


        /// <summary>
        /// Gets/Sets the map entry for ReactiveEnergyReceived lookup.
        /// </summary>
        public ModbusRegisterMapEntryConfig? ReactiveEnergyReceivedMap { get; set; }

    }
}
