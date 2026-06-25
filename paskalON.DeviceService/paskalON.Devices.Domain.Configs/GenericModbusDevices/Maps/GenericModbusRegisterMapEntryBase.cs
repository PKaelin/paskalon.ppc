using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public abstract class GenericModbusRegisterMapEntryBase : GenericModbusPointMapEntryBase
    {
        public double ModbusScale { get; set; } = 1;


        public int IndividualOffset { get; set; } = 0;


        public ModbusRegisterFormat? ModbusRegisterFormat { get; set; }


        public bool ReverseSign { get; set; } = false;

    }
}
