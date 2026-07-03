using paskalON.Domains.Contracts;

namespace paskalON.Devices.Domain.Contracts
{
    /// <summary>
    /// Interface to register data points for a Modbus register.
    /// </summary>
    /// <typeparam name="T">The type of of instance to register.</typeparam>
    public interface IKnownModbusRegister<T>
    {
        /// <summary>
        /// Registers a property with the specified name, getter function, scale, word order and optional offset.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to register.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="setter">A function to set the value of the property on an instance of T.</param>
        /// <param name="register">Unique register number.</param>
        /// <param name="scale">The scale of the endpoint.</param>
        /// <param name="wordOrder">The word order of the endpoint (Little/Big Endian.</param>
        /// <param name="offset">The offset applied.</param>
        void Register<TProperty>(string name, Action<T, TProperty> setter, int register, double scale, WordOrder wordOrder, int offset = 0);
    }
}
