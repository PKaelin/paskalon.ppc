using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Discrete input registers for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read-OnlyData
    /// Type: 1-bit (Boolean)
    /// Typical Use: Digital inputs originating from the field (e.g. a limit switch, door alarm, or emergency stop status).
    /// </remarks>
    public class GenericModbusDiscreteInputPointConfig : GenericModbusPointBase
    {
        /// <inheritdoc/>
        public override bool ModbusWritable { get => false; }


        /// <inheritdoc/>
        public override ModbusValueType ModbusValueType { get => ModbusValueType.DiscreteInput; }

    }
}
