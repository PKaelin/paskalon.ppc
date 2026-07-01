using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Coil map entry for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read/WriteData
    /// Type: 1-bit (Boolean: 1 or 0 / ON or OFF)
    /// Typical Use: Digital outputs used to trigger external actions (e.g. turning a motor on, opening a valve).
    /// </remarks>
    public class GenericModbusCoilPointConfig : GenericModbusPointBaseConfig
    {
        /// <inheritdoc/>
        public override bool ModbusWritable { get => true; }


        /// <inheritdoc/>
        public override ModbusValueType ModbusValueType { get => ModbusValueType.Coil; }

    }
}
