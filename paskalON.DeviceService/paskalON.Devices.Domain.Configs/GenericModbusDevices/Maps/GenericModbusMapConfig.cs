using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public class GenericModbusMapConfig : NameBase
    {
        public const int MaxAnalogPointCount = 100;


        public const int MaxBinaryPointCount = 99;


        public const int MaxCounterPointCount = 100;


        public int GenericModbusMapConfigId { get; set; }


        public List<GenericModbusCoilMapEntryConfig>? Coils { get; set; }


        public List<GenericModbusDiscreteInputMapEntryConfig>? DiscreteInputs { get; set; }


        public List<GenericModbusInputRegisterMapEntryConfig>? InputRegisters { get; set; }


        public List<GenericModbusHoldingRegisterMapEntryConfig>? HoldingRegisters { get; set; }


        public ushort? SlaveHeartbeatRegister { get; set; }


        public ushort? MasterHeartbeatRegister { get; set; }


        public ModbusPollingClass MasterHeartbeatPollingClass { get; set; } = ModbusPollingClass.Default;

    }
}
