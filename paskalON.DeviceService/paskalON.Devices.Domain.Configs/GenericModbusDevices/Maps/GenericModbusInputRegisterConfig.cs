using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Input registers for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read-OnlyData
    /// Type: 16-bit word (Integer)
    /// Typical Use: Analog inputs measuring physical properties (e.g. real-time temperature, pressure, or flow rate readings from a sensor).
    /// </remarks>
    public class GenericModbusInputRegisterConfig : GenericModbusRegisterBase
    {
        /// <inheritdoc/>
        public override bool ModbusWritable { get => false; }


        /// <inheritdoc/>
        public override ModbusValueType ModbusValueType { get => ModbusValueType.InputRegister; }

    }
}
