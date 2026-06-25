using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public class GenericModbusCoilMapEntryConfig : GenericModbusPointMapEntryBase
    {
        public int GenericModbusCoilMapEntryConfigId { get; set; }


        public GenericModbusCoilMapEntryConfig()
        {
            ModbusValueType = ModbusValueType.Coil;
        }
    }
}
