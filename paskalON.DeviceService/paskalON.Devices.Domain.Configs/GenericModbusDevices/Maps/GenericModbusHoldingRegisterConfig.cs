using paskalON.Communication.Protocols.Modbus.Enums;

namespace paskalON.Devices.Domain.Configs.GenericModbusDevices.Maps
{
    /// <summary>
    /// Holding registers for Modbus devices.
    /// </summary>
    /// <remarks>
    /// Access: Read/WriteData
    /// Type: 16-bit word (Integer)
    /// Typical Use: General storage and system configurations. Often stores setpoints, scaling factors, and calibration parameters.
    /// </remarks>
    public class GenericModbusHoldingRegisterConfig : GenericModbusRegisterBase
    {
        /// <inheritdoc/>
        public override bool ModbusWritable { get => false; }


        /// <inheritdoc/>
        public override ModbusValueType ModbusValueType { get => ModbusValueType.HoldingRegister; }

    }
}
