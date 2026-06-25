using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public class GenericModbusInputRegisterMapEntryConfig : GenericModbusRegisterMapEntryBase
    {
        public int GenericModbusInputRegisterMapEntryConfigId { get; set; }


        public GenericModbusInputRegisterMapEntryConfig()
        {
            ModbusValueType = ModbusValueType.InputRegister;
        }
    }
}
