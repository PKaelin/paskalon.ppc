using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public class GenericModbusMapConfig : NameBase
    {
        public const int MaxAnalogPointCount = 100;


        public const int MaxBinaryPointCount = 99;


        public const int MaxCounterPointCount = 100;


        public int GenericModbusMapConfigId { get; set; }


        public ICollection<GenericModbusCoilMapEntryConfig>? Coils { get; set; }


        public ICollection<GenericModbusDiscreteInputMapEntryConfig>? DiscreteInputs { get; set; }


        public ICollection<GenericModbusInputRegisterMapEntryConfig>? InputRegisters { get; set; }


        public ICollection<GenericModbusHoldingRegisterMapEntryConfig>? HoldingRegisters { get; set; }


        public ushort? SlaveHeartbeatRegister { get; set; }


        public ushort? MasterHeartbeatRegister { get; set; }


        public ModbusPollingClass MasterHeartbeatPollingClass { get; set; } = ModbusPollingClass.Default;

    }
}
