
namespace paskalON.Communication.Protocols.Modbus.Enums
{
    public enum ModbusFunctionCode : byte
    {       
        ReadCoils = 1,
        ReadDiscreteInputs = 2,
        ReadHoldingRegisters = 3,
        ReadInputRegisters = 4,
        WriteSingleCoil = 5,
        WriteSingleRegister = 6,
        WriteMultipleCoils = 15,
        WriteMultipleRegisters = 16,
        ReadFileRecord = 20,
        WriteFileRecord = 21,
        MaskWriteRegister = 22,
        ReadWriteMultipleRegisters = 23,
        ReadFifoQueue = 24,
        ReadDeviceIdentification = 43,
        // 0x80 
        // bitwise or this constant with the function code to produce a Modbus exception code, 
        // bitwise and with this constant can be used to determine if an exception has happened
        ModbusExceptionIndicator = 128, 
        // exception function codes
        WriteSingleRegisterException = WriteSingleRegister | ModbusExceptionIndicator,
        WriteSingleCoilException = WriteSingleCoil | ModbusExceptionIndicator,
        WriteMultipleRegistersException = WriteMultipleRegisters | ModbusExceptionIndicator,
        WriteMultipleCoilsException = WriteMultipleCoils | ModbusExceptionIndicator
    };
}