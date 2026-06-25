using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public class GenericModbusHoldingRegisterMapEntryConfig : GenericModbusRegisterMapEntryBase
    {
        public int GenericModbusHoldingRegisterMapEntryConfigId { get; set; }


        public GenericModbusHoldingRegisterMapEntryConfig()
        {
            ModbusValueType = ModbusValueType.HoldingRegister;
        }
    }
}
