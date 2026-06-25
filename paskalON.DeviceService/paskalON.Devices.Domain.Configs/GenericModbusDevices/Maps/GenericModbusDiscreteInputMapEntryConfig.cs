using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public class GenericModbusDiscreteInputMapEntryConfig : GenericModbusRegisterMapEntryBase
    {
        public int GenericModbusDiscreteInputMapEntryConfigId { get; set; }


        public GenericModbusDiscreteInputMapEntryConfig()
        {
            ModbusValueType = ModbusValueType.DiscreteInput;
        }
    }
}
