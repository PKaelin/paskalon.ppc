namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Generic Modbus device interface for the instances that communicate with the device.
    /// </summary>
    /// <typeparam name="T">The type of the generic Modbus device.</typeparam>
    public interface IGenericModbusDevice<T> : IDevice<T>
    {
    }
}
