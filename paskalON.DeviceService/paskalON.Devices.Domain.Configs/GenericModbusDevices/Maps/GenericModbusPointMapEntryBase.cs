using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    public abstract class GenericModbusPointMapEntryBase : NameBase
    {
        public ushort ModbusNumber { get; set; }


        public bool ModbusWritable { get; set; }


        public ModbusValueType ModbusValueType { get; set; } = ModbusValueType.HoldingRegister;


        public bool IsAlarm { get; set; } = true;


        public bool IsAlarmReset { get; set; } = false;


        private short? _bitIndex;
        public short BitIndex
        {
            get
            {
                if (_bitIndex.HasValue)
                {
                    return _bitIndex.Value;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (value > 31 || value < 0)
                {
                    _bitIndex = -1;
                }
                else
                {
                    _bitIndex = value;
                }
            }
        }


        public ushort? AltModbusNumber { get; set; }


        public ModbusPollingClass PollingClass { get; set; } = ModbusPollingClass.Default;

    }
}
