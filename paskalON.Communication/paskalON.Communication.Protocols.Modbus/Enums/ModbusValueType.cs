

namespace paskalON.Communication.Protocols.Modbus.Enums
{
    /// <summary>
    /// Enumeration for the types of Modbus values.
    /// </summary>
    /// <remarks>
    /// NOTE: There is a "file" type in Modbus but we have no current plans to support it so it is omitted.
    /// </remarks>
    public enum ModbusValueType
    {
        Coil,
        DiscreteInput,
        InputRegister,
        HoldingRegister
    }
}
